using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace USBCam
{
    public partial class FUSBCam : Form
    {
        //private ArrayList _myLights;
        enum IMAGEEXT { JPEG = 1, BMP, PNG, TIFF };  //corresponds to file filters in Save dialog box
        private string _camConfError = "Camera Configuration Error";
        private CCameraUSB myUSBCam; // = new CCameraUSB( this );
        private int _numCams;
        private bool _camEngineStarted = false;
        private bool _startedContFG = false;
        private int _callbackCount;

        public ImageProperty CurrentImageProperty;
        public uint PixelAverage;


        public FUSBCam()
        {
            InitializeComponent();

            myUSBCam = new CCameraUSB( this );
            _callbackCount = 0;
            
            _numCams = myUSBCam.InitDevice();
            //Populate camera combo box
            if (_numCams > 0)
            {
                string cam = "";
                for (int i = 0; i < _numCams; i++)
                {
                    cam = String.Format("Camera {0}", i + 1);
                    CBCameraSelect.Items.Add(cam);
                }
                //CBCameraSelect.SelectedIndex = 0;
                CBResolution.SelectedIndex = (int)CCameraUSB.RESOLUTION.VGA;
                NUpDownExposureTime.Value = myUSBCam.GetExpTime()/1000;

                //ROI
                NUpDownX.Value = myUSBCam.GetXstart();
                NUpDownY.Value = myUSBCam.GetYstart();

                //Gains
                NUpDownRed.Value = myUSBCam.GetRedGain();
                NUpDownGreen.Value = myUSBCam.GetGreenGain();
                NUpDownBlue.Value = myUSBCam.GetBlueGain();

                //Set light controls
                //_myLights = lights;
                //double intensity1 = ((CLight)_myLights[0]).IntensityLevel;
                //((CLight)_myLights[0]).SetIntensity(intensity1);
                //SBLightIntensity1.Value = System.Convert.ToInt32(intensity1);
                //TBLightIntensity1.Text = intensity1.ToString("F1");
            
            }
        }

        private void BCancel_Click(object sender, EventArgs e)
        {
            //Cleanup Resources called by FUSBCam_FormClosed
            Close();
        }

        private void BOK_Click(object sender, EventArgs e)
        {
            //Cleanup Resources called by FUSBCam_FormClosed
            Close();
        }

        private void CleanUpResources()
        {
            if (this._camEngineStarted)
            {
                //Free IntPtr memory in myUSBCam object
                myUSBCam.FreeImageMem();
                myUSBCam.StopCameraEngine();
            }

            //release camera resources
            this.myUSBCam.UnInitDevice();
        }

        private void CBCameraSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Open device and get module and serial numbers
            myUSBCam.AddCameraToWorkingSet( CBCameraSelect.SelectedIndex+1 );
            TBModuleNo.Text = myUSBCam.GetModuleNo();
            TBSerialNo.Text = myUSBCam.GetSerialNo();
            
            //Start the camera engine
            myUSBCam.StartCameraEngine(this.Handle);
            _camEngineStarted = true;
            System.Threading.Thread.Sleep(200);

            //Set default camera settings
            Trace.Flush();
            Trace.WriteLine("Calling SetFrameSetting() from FUSBCam Form....");
            Trace.Indent();

            myUSBCam.SetResolution(CCameraUSB.RESOLUTION.VGA, 0, 0);
            myUSBCam.SetGain(12, 12, 12);
            myUSBCam.SetStartPosition(0, 0);
            myUSBCam.SetExposureTime(5);

            Trace.Unindent();
            Trace.WriteLine("Returned from SetFrameSetting(), end trace.");

            //Black and White and Orientation
            bool blkWht = myUSBCam.GetBlkWht();
            bool horzMirr = myUSBCam.GetHorzMirr();
            bool flipVert = myUSBCam.GetFlipVert();
            ////Set Orientation & B&W
            //myUSBCam.SetShowMode( false, true, true );

            //if (blkWht)
            //    CBBlackWhite.CheckState = CheckState.Checked;
            //if (horzMirr)
            //    CBHorzMirr.CheckState = CheckState.Checked;
            //if (flipVert)
            //    CBFlipVert.CheckState = CheckState.Checked;
       }

         private void BContFG_Click(object sender, EventArgs e)
        {
            if (!_startedContFG)
            {
                //myUSBCam.SetShowMode(false, true, true);
                myUSBCam.StartFrameGrab(CCameraUSB.INFINITE_FRAMES);
                _startedContFG = true;
                BContFG.Text = "Stop Cont FG";
                //myUSBCam.SetResolution((CCameraUSB.RESOLUTION)CBResolution.SelectedIndex, 1, false);
                //timerContFG.Enabled = true;
                //System.Threading.Thread.Sleep(200);
                myUSBCam.GetCurrentFrame();
            }
            else
            {
                myUSBCam.StopFrameGrab();
                _startedContFG = false;
                BContFG.Text = "Start Cont FG";
            }
        }

        private void CBResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_startedContFG)
            {
                myUSBCam.SetResolution((CCameraUSB.RESOLUTION)CBResolution.SelectedIndex, 0, 0);
            }
        }

        private void CBBlackWhite_CheckedChanged(object sender, EventArgs e)
        {
            //bool blkWht = myUSBCam.GetBlkWht();
            bool horzMirr = myUSBCam.GetHorzMirr();
            bool flipVert = myUSBCam.GetFlipVert();

            if (CBBlackWhite.Checked)
            {
                myUSBCam.SetShowMode(true, horzMirr, flipVert);
            }
            else
            {
                myUSBCam.SetShowMode(false, horzMirr, flipVert);
                //myUSBCam.SetShowMode(false, false, false);
            }
        }

        private void NUpDownExposureTime_ValueChanged(object sender, EventArgs e)
        {
            int newExpTime = System.Convert.ToInt32(NUpDownExposureTime.Value);
            if (_camEngineStarted)
            {
                myUSBCam.SetExposureTime(newExpTime);
            }
        }

        private void NUpDownRed_ValueChanged(object sender, EventArgs e)
        {
            int newRedGain = System.Convert.ToInt32(NUpDownRed.Value);
            if (_camEngineStarted)
            {
                myUSBCam.SetGain(newRedGain, myUSBCam.GetGreenGain(), myUSBCam.GetBlueGain());
            }
        }

        private void NUpDownGreen_ValueChanged(object sender, EventArgs e)
        {
            int newGreenGain = System.Convert.ToInt32(NUpDownGreen.Value);
            if (_camEngineStarted)
            {
                myUSBCam.SetGain(myUSBCam.GetRedGain(), newGreenGain, myUSBCam.GetBlueGain());
            }
        }

        private void NUpDownBlue_ValueChanged(object sender, EventArgs e)
        {
            int newBlueGain = System.Convert.ToInt32(NUpDownBlue.Value);
            if(_camEngineStarted)
            {
                myUSBCam.SetGain(myUSBCam.GetRedGain(), myUSBCam.GetGreenGain(), newBlueGain);
            }
        }

         private void NUpDownX_ValueChanged(object sender, EventArgs e)
        {
            int xPos = System.Convert.ToInt32(NUpDownX.Value);
            int yPos = System.Convert.ToInt32(NUpDownY.Value);

            //Set max x and y based on resolution
            NUpDownX.Maximum = myUSBCam.MaxX;
            NUpDownY.Maximum = myUSBCam.MaxY;

            if (_camEngineStarted)
            {
                myUSBCam.SetStartPosition(xPos, yPos);
            }
        }

        private void NUpDownY_ValueChanged(object sender, EventArgs e)
        {
            int xPos = System.Convert.ToInt32(NUpDownX.Value);
            int yPos = System.Convert.ToInt32(NUpDownY.Value);

            if (_camEngineStarted)
            {
                myUSBCam.SetStartPosition(xPos, yPos);
            }
        }


        private void FUSBCam_FormClosed(object sender, FormClosedEventArgs e)
        {
            CleanUpResources();
        }

        public void SetCallBackMessage( ref ImageProperty imageProp, uint pixelAvg )
        {
            CurrentImageProperty  = imageProp;
            PixelAverage = pixelAvg;
            _callbackCount++;
            CallBackMessageLabel1.Text = "Frame Count:" + _callbackCount + "Exposure Time: " + (CurrentImageProperty.ExposureTime/20) + "(ms) Pixel Average:" + PixelAverage; // ShowMessage
            CallBackMessageLabel2.Text = "Row:" + CurrentImageProperty.Row + " Column:" + CurrentImageProperty.Column + " Bin:" + CurrentImageProperty.Bin + " AT(" +
                                         CurrentImageProperty.XStart + "," + CurrentImageProperty.YStart + ")";
            CallBackMessageLabel3.Text = "Gain: (" + CurrentImageProperty.RedGain + "," + CurrentImageProperty.GreenGain + "," + CurrentImageProperty.BlueGain + ")";
        }

        private void CBAutoExposure_CheckedChanged(object sender, EventArgs e)
        {
            // Usually, user rarely use Auto exposure.
        }

        private void CBShowAutoExpRegion_CheckedChanged(object sender, EventArgs e)
        {
            // Do nothing now.
        }

     }
}