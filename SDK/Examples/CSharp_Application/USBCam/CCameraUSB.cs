using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;

namespace USBCam
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ImageProperty
    {
        [FieldOffset(0)] public int CameraID;
        [FieldOffset(4)] public int WorkMode;	 // 0 - NORMAL mode, 1 - TRIGGER mode
        [FieldOffset(8)] public int SensorClock; // 24, 48, 96 for 24MHz, 48MHz and 96MHz
        [FieldOffset(12)] public int Row;	// It's ColSize, in 1280x1024, it's 1024
        [FieldOffset(16)] public int Column;	// It's RowSize, in 1280x1024, it's 1280
        [FieldOffset(20)] public int Bin;	// 0, 1, 2 for no-decimation, 1:2 and 1:4 decimation
        [FieldOffset(24)] public int BinMode;// 0 - Skip, 1 - Bin
        [FieldOffset(28)] public int CameraBit; // 8 or 16.
        [FieldOffset(32)] public int XStart;
        [FieldOffset(36)] public int YStart;
        [FieldOffset(40)] public int ExposureTime; // in 50us unit, e.g. 100 means 5000us(5ms)
        [FieldOffset(44)] public int RedGain;
        [FieldOffset(48)] public int GreenGain;
        [FieldOffset(52)] public int BlueGain;
        [FieldOffset(56)] public int TimeStamp; // in 1ms unit, 0 - 0xFFFFFFFF and round back
        [FieldOffset(60)] public int SensorMode;// Bit0 is used for GRR mode, Bit1 is used for Strobe out enable
        [FieldOffset(64)] public int TriggerOccurred; // Used for NORMAL mode only, set when the external trigger occurred during the grabbing of last frame.
        [FieldOffset(68)] public int TriggerEventCount; // Reserved.
        [FieldOffset(72)] public int FrameSequenceNo; // Reserved.
        [FieldOffset(76)] public int IsFrameBad; // Is the current frame a bad one.

        [FieldOffset(80)] public int FrameProcessType; // 0 - RAW, 1 - BMP
        [FieldOffset(84)] public int FilterAcceptForFile; // Reserved
    }

    public delegate void FrameCallbackDelegate( ref ImageProperty frameProperty, IntPtr SSClassicferPtr );
    public delegate void DeviceCallbackDelegate( int deviceID, int ErrorType );

    public class CCameraUSB
    {
        //Members...
        public enum IMAGES {RAW, BMP, JPG};
        public enum CAM_WORKMODE {VIDEO, EXT_TRIG};
        public enum FRAME_TYPE { RAW, DIB };
        public enum SHARPNESS_LEV { NORMAL, SHARP, SHARPER, SHARPEST };
        public enum RESOLUTION { TINY, LITTLE, SMALL, MOBILE, VGA, SVGA, UVGA, VGALARGE, RES_3M, RES_5M }
        public enum CAMERA_BIT { BIT_8_CAMERA = 8, BIT_16_CAMERA = 16 };
        public const int INFINITE_FRAMES = 0x8888;
        public const int ALL_DEVICES = 0x88;
        public FrameCallbackDelegate frameDelegate;
        public DeviceCallbackDelegate DeviceDelegate;
        private FUSBCam MightexCam; // The GUI class
        private int _deviceID = 1;
        private string _camError = "USB Camera Error";
        private IntPtr _pImage = new IntPtr();  //image pointer

        private int _maxX;
        public int MaxX
        {
            get { return _maxX; }
        }
        private int _maxY;
        public int MaxY
        {
            get { return _maxY; }
        }

        private int pixelDepthBits = 24/8;  //24 bit color camera div by 8 bits for 3 bytes total
        public int PixelDepth
        {
            get { return pixelDepthBits; }
        }

        public struct ImageControl
        {
            [XmlElement("Revision")]
            public RESOLUTION _resolution;
            public int _rowSize;
            public int _columnSize;
            public int _bin;           // 0: no decimation, 1: 1:2, 2: 1:4.
            public int _binMode;       // 0: Skip, 1: Bin
            public int _xStart;        //Upper left hand corner, positive right 
            public int _yStart;        //Upper left hand corner, positive down
            public int _greenGain;     //Green Gain Value: 0 ?128, the actual gain is GreenGain/8
            public int _blueGain;      //Blue Gain Value: 0 ?128, the actual gain is BlueGain/8
            public int _redGain;       //Red Gain Value: 0 ?128, the actual gain is RedGain/8
            public int _exposureTime;  //current exposure time in microseconds

            public int _gamma;         //Gamma value: 0 ?20, means 0.0 ?2.0
            public int _contrast;      //Contrast value: 0 ?100, means 0% -- 100%
            public int _bright;        //Brightness : 0 ?100, means 0% -- 100%

            public int  _sharpLevel;   //SharpLevel index: 0=Normal, 1=Sharp, 2=Sharper, 3=Sharpest
            public int _blkWhtMode;   //true for B&W
            public int _horzMirror;   //true for Horiz
            public int _vertFlip;     //true for vertical flip
        }

        private ImageControl _imgControl = new ImageControl();

        //default constructor for testing
        public CCameraUSB( FUSBCam mightexCamera )
        {
            _imgControl._resolution = RESOLUTION.VGA;
            _imgControl._rowSize = 640;
            _imgControl._columnSize = 480;
            _imgControl._bin = 0;
            _imgControl._binMode = 1;
            _imgControl._xStart = 0;
            _imgControl._yStart = 0;
            _imgControl._greenGain = 12;
            _imgControl._blueGain = 12;
            _imgControl._redGain = 12;
            _imgControl._exposureTime = 5000; // 5ms.

            _imgControl._gamma = 10;
            _imgControl._contrast = 50;
            _imgControl._bright = 50;

            _imgControl._sharpLevel = 0;
            _imgControl._blkWhtMode = 0;
            _imgControl._horzMirror = 1;
            _imgControl._vertFlip = 1;

            MightexCam = mightexCamera;
            frameDelegate = new FrameCallbackDelegate(GrabbingFrameCallback);
            DeviceDelegate = new DeviceCallbackDelegate(DeviceCallback);
            _maxX = 1280; // Assume we're using 1.3M camera.
            _maxY = 1024;

            AllocImageMem();
        }

        public void DeviceCallback(int DeviceID, int ErrorNo)
        {
            MessageBox.Show("Camera error!");
        }

        // JTZ: The frame callback.
        public void GrabbingFrameCallback( ref ImageProperty frameProperty, IntPtr SSClassicferPtr)
        {
            uint i, pixelAvg;

            pixelAvg = 0;
            MightexCam.SetCallBackMessage(ref frameProperty, pixelAvg);

           // System.Drawing.Imaging.PixelFormat imageFormat = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
            System.Drawing.Imaging.PixelFormat imageFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;

            int imageWidth = frameProperty.Column / (frameProperty.Bin + 1);
            int imageHeight = frameProperty.Row / (frameProperty.Bin + 1);
            int imageStride;

            imageStride = imageWidth * 1;
            imageStride += (imageStride % 4);
            imageStride *= 3;

            unsafe
            {
                byte* dptr = (byte*)_pImage;
                byte* sptr = (byte*)SSClassicferPtr;
                // For Mono camera, the returned image data is 1 byte for each pixel.
                for (i = 0; i < (imageWidth * imageHeight); i++)
                {
                    *dptr++ = *sptr;
                    *dptr++ = *sptr;
                    *dptr++ = *sptr++;
                }
            }
            //Bitmap imageBitmap = new Bitmap(imageWidth, imageHeight, imageStride, imageFormat, new IntPtr(SSClassicferPtr));
            Bitmap imageBitmap = new Bitmap(imageWidth, imageHeight, imageStride, imageFormat, _pImage);
            //imageBitmap.RotateFlip(RotateFlipType.Rotate180FlipY);


            MightexCam.pictureBox1.Image = imageBitmap;
        }

        public int GetCurrentFrame()
        {
            unsafe
            {
                byte* rptr;
                int tmp;
                rptr = (byte*)SSClassicGetCurrentFrame(1, 1, (uint)&tmp);
            }
            return 1;
        }

        public int GetExpTime()
        {
            return _imgControl._exposureTime;
        }

        public int GetXstart()
        {
            return _imgControl._xStart;
        }
        public int GetYstart()
        {
            return _imgControl._yStart;
        }

        public int GetRedGain()
        {
            return _imgControl._redGain;
        }
        public int GetGreenGain()
        {
            return _imgControl._greenGain;
        }
        public int GetBlueGain()
        {
            return _imgControl._blueGain;
        }

        public bool GetBlkWht()
        {
            return (_imgControl._blkWhtMode == 1 ? true : false);
        }
        public bool GetHorzMirr()
        {
            return (_imgControl._horzMirror == 1 ? true : false);
        }
        public bool GetFlipVert()
        {
            return (_imgControl._vertFlip == 1 ? true : false);
        }
        //Thin wrapper for calling into SSClassic_USBCamera_SDK_Stdcall.dll
        //Interops are in the USBCam.Designer.cs (partial class) file

        /// <summary>
        /// Call this function first, this function communicates with device driver to reserve resources
        /// </summary>
        /// <returns>number of cameras on USB 2.0 chan</returns>
        public int InitDevice()
        {
            int numCam = SSClassicInitDevice();

            if ( numCam < 0 )
            {
                MessageBox.Show("Error trying to initialize camera resources. No cameras found on USB 2.0 bus.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return numCam;
        }
        
        //Call this function before the app terminates, it releases all resources
        public void UnInitDevice()
        {
            if (SSClassicUnInitDevice() < 0)
            {
                MessageBox.Show("Error trying to uninitialize camera.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Function used to get the module number on a particular USB channel
        /// </summary>
        /// <returns></returns>
        public string GetModuleNo()
        {
            string moduleNumber = "Unknown";
            //char moduleNo = 'X';
            StringBuilder rtnModuleNo = new StringBuilder();
            StringBuilder rtnSerialNo = new StringBuilder();

            if (SSClassicGetModuleNoSerialNo( _deviceID, rtnModuleNo, rtnSerialNo) < 0)
            {
                MessageBox.Show("Error trying to retrieve camera module number.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                moduleNumber = rtnModuleNo.ToString();
            }
            return moduleNumber;
        }

        /// <summary>
        /// Function used to get the serial number on a particular USB channel
        /// </summary>
        /// <returns></returns>
        public string GetSerialNo()
        {
            string serialNumber = "Unknown2";
            StringBuilder rtnModuleNo = new StringBuilder();
            StringBuilder rtnSerialNo = new StringBuilder();

            if (SSClassicGetModuleNoSerialNo(_deviceID, rtnModuleNo, rtnSerialNo) < 0)
            {
                MessageBox.Show("Error trying to retrieve camera serial number.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                serialNumber = rtnSerialNo.ToString();
            }
            return serialNumber;
        }

        public void AddCameraToWorkingSet(int cameraID)
        {
            _deviceID = cameraID;
            if (SSClassicAddDeviceToWorkingSet( cameraID ) < 0)
            {
                MessageBox.Show("Error adding camera to working set", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void RemoveCameraFromWorkingSet(int cameraID)
        {
            if (SSClassicRemoveDeviceFromWorkingSet(cameraID) < 0)
            {
                MessageBox.Show("Error removing camera from working set", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        //Camera has multithread engine internally, which is responsible for all the frame grabbing, raw data to RGB data conversion…etc. functions. 
        //User MUST start this engine for all the following camera related operations
        //ParentHandle ?The window handle of the main form of user’s application, as the engine relies on
        //Windows Message Queue, it needs a parent window handle
        public void StartCameraEngine(IntPtr parentHandle)
        {
            if (SSClassicStartCameraEngine(parentHandle, (uint)CAMERA_BIT.BIT_8_CAMERA, 2, 0) < 0)
            {
                MessageBox.Show("Error trying to start camera engine.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Stops the started camera engine.
        /// </summary>
        public void StopCameraEngine()
        {
            if ( SSClassicStopCameraEngine() < 0 )
            {
                MessageBox.Show("Error trying to stop camera engine.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sets camera to either "video" mode - continuously deliver frames to PC or
        /// "external trigger" mode - camera waits for external trigger to capture 1 frame
        /// </summary>
        /// <param name="mode"></param>
        public void SetCameraWorkMode(CAM_WORKMODE mode)
        {
            int WorkMode = (int)mode;
            if (SSClassicSetCameraWorkMode(_deviceID, WorkMode) < 0) // We take the first camera as example here.
            {
                MessageBox.Show("Error trying to set camera work mode.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Hides (makes invisible) the control panel which will be displayed once the camera engine starts
        /// </summary>
        ///
        /* JTZ: I add this method for user's debug purpose...use might show the control panel...so all the
         * settings (e.g. exposure time...etc.) are visible on this panel...user might hide it in his  formal
         * application.
         */
        public void ShowFactoryControlPanel( uint Left, uint Top)
        {
            String passWord = "123456";

            if (SSClassicShowFactoryControlPanel(_deviceID, passWord) < 0)
            {
                MessageBox.Show("Error trying to show camera control panel.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Hides (makes invisible) the control panel which will be displayed once the camera engine starts
        /// </summary>
        public void HideFactoryControlPanel()
        {
            if ( SSClassicHideFrameControlPanel() < 0)
            {
                MessageBox.Show("Error trying to hide camera control panel.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Starts frame grabbing after camera resources prepared. After call, user should see images in video window
        /// </summary>
        /// <param name="totalFrames"></param>
        public void StartFrameGrab(int totalFrames)
        {
            // Install frame call back.
            SSClassicInstallFrameHooker( 1, frameDelegate); 
            SSClassicInstallUSBDeviceHooker(DeviceDelegate);
            if ( SSClassicStartFrameGrab( ALL_DEVICES, totalFrames) < 0)
            {
                MessageBox.Show("Error trying to start frame grabbing images.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Stops frame grabbing, call if totalFrames set to INFINITE_FRAMES
        /// </summary>
        public void StopFrameGrab()
        {
            // Install frame call back.
            SSClassicInstallFrameHooker( 0, null ); // Unhooker the callback.
            if (SSClassicStopFrameGrab(ALL_DEVICES) < 0)
            {
                MessageBox.Show("Error trying to stop frame grabbing images.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Set the resolution (including capture and render)
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="binMode"></param>
        public void SetResolution( RESOLUTION resolution, int bin, int binMode )
        {
            this.SetMaxXY(resolution);
            _imgControl._bin = bin;
            _imgControl._binMode = binMode;

            if (SSClassicSetCustomizedResolution(_deviceID, _imgControl._rowSize, _imgControl._columnSize, _imgControl._bin, _imgControl._binMode) < 0)
            {
                MessageBox.Show("Error trying to set camera resolution, bin mode, and image rendor fitting.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetMaxXY(RESOLUTION res)
        {
            //Set maximum X and Y start positions based on resolution
            switch (res)
            {
                case RESOLUTION.TINY:
                    _maxX = 32;
                    _maxY = 32;
                    break;
                case RESOLUTION.LITTLE:
                    _maxX = 64;
                    _maxY = 64;
                    break;
                case RESOLUTION.SMALL:
                    _maxX = 160;
                    _maxY = 120;
                    break;
                case RESOLUTION.MOBILE:
                    _maxX = 320;
                    _maxY = 240;
                    break;
                case RESOLUTION.VGA:
                    _maxX = 640;
                    _maxY = 480;
                    break;
                case RESOLUTION.SVGA:
                    _maxX = 800;
                    _maxY = 600;
                    break;
                case RESOLUTION.UVGA:
                    _maxX = 1024;
                    _maxY = 768;
                    break;
                case RESOLUTION.VGALARGE:
                    _maxX = 1280;
                    _maxY = 1024;
                    break;
                case RESOLUTION.RES_3M:
                    _maxX = 2048;
                    _maxY = 1536;
                    break;
                case RESOLUTION.RES_5M:
                    _maxX = 2560;
                    _maxY = 1920;
                    break;
                default:
                    MessageBox.Show("Resolution not defined, unable to set maximum X and Y start positions.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
            _imgControl._rowSize = _maxX;
            _imgControl._columnSize = _maxY;
        }

        //Note: only two elements _xStart and _yStart are used by this function, all others are ignored.
        /// <summary>
        /// Set the start position of ROI
        /// </summary>
        /// <param name="Xstart"></param>
        /// <param name="Ystart"></param>
        public void SetStartPosition(int Xstart, int Ystart)
        {
            _imgControl._xStart = Xstart;
            _imgControl._yStart = Ystart;

            if (SSClassicSetStartPosition(_deviceID, Xstart, Ystart) < 0)
            {
                MessageBox.Show("Error trying to set camera X and Y start positions.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Note: only three elements _greenGain, _blueGain and _redGain are used by this function, all others are ignored.
        /// <summary>
        /// Set RGB Gain parameters
        /// </summary>
        /// <param name="redGain"></param>
        /// <param name="greenGain"></param>
        /// <param name="blueGain"></param>
        public void SetGain(int redGain, int greenGain, int blueGain)
        {
            _imgControl._redGain = redGain;
            _imgControl._greenGain = greenGain;
            _imgControl._blueGain = blueGain;

            if (SSClassicSetGains(_deviceID, redGain, greenGain, blueGain) < 0)
            {
                MessageBox.Show("Error trying to set camera RGB gains.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Note: only two elements _maxExposureTimeIndex and _exposureTime are used by this function, all others are ignored.
        /// <summary>
        /// Set camera exposure parameters
        /// </summary>
        /// <param name="expTime"></param>
        public void SetExposureTime( int expTime)
        {
            _imgControl._exposureTime = expTime*1000; //convert milli to microseconds

            if (SSClassicSetExposureTime(_deviceID, (_imgControl._exposureTime / 50)) < 0)
            {
                MessageBox.Show("Error trying to set camera exposure settings.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Set the Gamma, Contrast and Brightness parameters
        //Note: only four elements _gamma, _contrast, _bright and _sharpLevel are used by this function, all others are ignored.
        public void SetGammaValue(int gamma, int contrast, int brightness, SHARPNESS_LEV sharpLev)
        {
            _imgControl._gamma = gamma;
            _imgControl._contrast = contrast;
            _imgControl._bright = brightness;
            _imgControl._sharpLevel = (int) sharpLev;

            if (SSClassicSetGamma(_deviceID, gamma, contrast, brightness, (int)sharpLev) < 0)
            {
                MessageBox.Show("Error trying to set camera gamma, contrast, brightness, and sharpness levels.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        //Note: only three elements _blkWhtmode, _horzMirror and _vertFlip are used by this function, all others are ignored.
        /// <summary>
        /// Set the BWMode, HorizontalMirror and VerticalFlip parameters
        /// </summary>
        /// <param name="bwMode"></param>
        /// <param name="horzMirror"></param>
        /// <param name="vertFlip"></param>
        public void SetShowMode(bool bwMode, bool horzMirror, bool vertFlip)
        {
            _imgControl._blkWhtMode = ( bwMode ? 1 : 0 );
            _imgControl._horzMirror = (horzMirror ? 1 : 0);
            _imgControl._vertFlip = (vertFlip ? 1 : 0);

            if (SSClassicSetBWMode(_deviceID, _imgControl._blkWhtMode, _imgControl._horzMirror, _imgControl._vertFlip) < 0)
            {
                MessageBox.Show("Error trying to set camera B&W moode, horizontal mirror, and vertical flip settings.", _camError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        public void AllocImageMem()
        {
            _pImage = Marshal.AllocHGlobal(pixelDepthBits * 2560 * 1920);  // 5M as maximum
        }

        public void FreeImageMem()
        {
            if (_pImage != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_pImage);
            }
        }
        
        #region Mightex interop functions for accessing SSClassic_USBCamera_SDK_Stdcall.dll and SSClassicferCameraUsblib.dll files
  
        //Call this function first, this function communicates with device driver to reserve resources
        //When the system uses NTFS use WINNT, for FAT32 use WINDOWS
        /*
         * JTZ: Note that I assume we put SSClassic_USBCamera_SDK_Stdcall.dll and MtUsblib.dll in windows\system32.
         */

        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_InitDevice", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicInitDevice();


        //Call this function before the app terminates, it releases all resources
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_UnInitDevice", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicUnInitDevice();

        //The module number and serial number are what appear if one calls the
        //SDK_HANDLE_API SSClassicUSB_ShowOpenDeviceDialog() method.
        //returns:  -1 If the function fails (e.g. invalid device handle), 1 if the call succeeds.
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_GetModuleNoSerialNo", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicGetModuleNoSerialNo(int deviceID, StringBuilder moduleNo, StringBuilder serialNo);

        //Add device to working set, deviceID is a one base index, so if InitDevice returns 2 for example, there devices at 1 and 2.
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_AddDeviceToWorkingSet", CallingConvention = CallingConvention.StdCall)]
        private static extern uint SSClassicAddDeviceToWorkingSet(int deviceID);

        //Remove device from working set. 
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_RemoveDeviceFromWorkingSet", CallingConvention = CallingConvention.StdCall)]
        private static extern uint SSClassicRemoveDeviceFromWorkingSet(int deviceID);

        //Camera has multithread engine internally, which is responsible for all the frame grabbing, raw data to RGB data conversion etc. functions. 
        //User MUST start this engine for all the following camera related operations
        //ParentHandle ?The window handle of the main form of user’s application, as the engine relies on
        //Windows Message Queue, it needs a parent window handle
        //returns:  -1 If the function fails (e.g. invalid device handle), 1 if the call succeeds.
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_StartCameraEngine", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicStartCameraEngine(IntPtr parentHandle, uint cameraBitOption, uint processThreads, uint IsCallbackFromThread);

        //Stops the started camera engine.
        //returns:  -1 If the function fails (e.g. invalid device handle or if the engine is NOT started), 1 if the call succeeds.
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_StopCameraEngine", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicStopCameraEngine();

        //Sets camera to either "video" mode - continuously deliver frames to PC or
        //"external trigger" mode - camera waits for external trigger to capture 1 frame
        //returns:  -1 If the function fails (e.g. invalid device handle), 1 if the call succeeds.
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_SetCameraWorkMode", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicSetCameraWorkMode( int deviceID, int WorkMode);

        //Showes (makes visible) the factory control panel which will be displayed once the camera engine starts
        //returns:  -1 If the function fails (e.g. invalid device handle or if the engine is NOT started yet), 1 if the call succeeds.
        /*
         * JTZ: I add this API for user debug purpose. 
         */
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_ShowFactoryControlPanel", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicShowFactoryControlPanel( int deviceID, String password );

        //Hides (makes invisible) the control panel which will be displayed once the camera engine starts
        //returns:  -1 If the function fails (e.g. invalid device handle or if the engine is NOT started yet), 1 if the call succeeds.
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_HideFactoryControlPanel", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicHideFrameControlPanel();

            //Starts frame grabbing after camera resources prepared.
        //After call, user should see images in video window
        //returns:  -1 If the function fails (e.g. invalid device handle or if the engine is NOT started yet), 1 if the call succeeds.
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_StartFrameGrab", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicStartFrameGrab(int deviceID, int totalFrames);

        //Stops frame grabbing, call if totalFrames set to INFINITE_FRAMES
        //returns:  -1 If the function fails (e.g. invalid device handle or if the engine is NOT started yet), 1 if the call succeeds.
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_StopFrameGrab", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicStopFrameGrab( int deviceID );

        //returns:  -1 If the function fails (e.g. invalid device handle), 1 if the call succeeds.
        //Note: only three elements _resolution, _binMode and _imageRendorFitWindow are used by this function, all others are ignored.
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_SetCustomizedResolution", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicSetCustomizedResolution(int deviceID, int rowSize, int columnSize, int bin, int binMode);


        //Set the start position of ROI
        //returns:  -1 If the function fails (e.g. invalid device handle), 1 if the call succeeds.
        //Note: only two elements _xStart and _yStart are used by this function, all others are ignored.
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_SetXYStart", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicSetStartPosition( int deviceID, int xStart, int yStart);

        //Set RGB Gains parameters
        //returns:  -1 If the function fails (e.g. invalid device handle), 1 if the call succeeds.
        //Note: only three elements _greenGain, _blueGain and _redGain are used by this function, all others are ignored.
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_SetGains", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicSetGains( int deviceID, int redGain, int greenGain, int blueGain);

        //Set exposure parameters
        //returns:  -1 If the function fails (e.g. invalid device handle), 1 if the call succeeds.
        //Note: only two elements _maxExposureTimeIndex and _exposureTime are used by this function, all others are ignored.
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_SetExposureTime", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicSetExposureTime( int deviceID, int exposureTime);

        //Set the Gamma, Contrast and Brightness parameters
        //returns:  -1 If the function fails (e.g. invalid device handle), 1 if the call succeeds.
        //Note: only four elements _gamma? _contrast? _bright and _sharpLevel are used by this function, all others are ignored.
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_SetGamma", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicSetGamma( int deviceID, int gamma, int contrast, int brightness, int sharpLevel);

        //Automatically set white balance, set proper exposure time and put white paper in front of camera
        //returns:  -1 If the function fails (e.g. invalid device handle), 1 if the call succeeds.
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_SetBWMode", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicSetBWMode( int deviceID, int bWMode, int hMirror, int vFlip);


        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_GetCurrentFrame", CallingConvention = CallingConvention.StdCall)]
        private static extern uint SSClassicGetCurrentFrame(int FrameType, int deviceID, uint framePtr);

        // JTZ: we allow user to install a callback for each grabbed frame.
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_InstallFrameHooker", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicInstallFrameHooker( int FrameType, Delegate FrameCallBack);

        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_InstallUSBDeviceHooker", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicInstallUSBDeviceHooker( Delegate USBDeviceCallBack);
        
        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_SetGPIOConfig", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicSetGPIOConfig( int deviceID, byte configByte);

        [DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", EntryPoint = "SSClassicUSB_SetGPIOInOut", CallingConvention = CallingConvention.StdCall)]
        private static extern int SSClassicSetGPIOInOut( int deviceID, byte outputByte, out byte InByte );

        #endregion
    }

}
