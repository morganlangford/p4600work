Imports System
Imports System.Text
Imports System.Runtime.InteropServices

Module modMightexCamera

    'Delegate Sub MyFrameHookerDelegate(ByRef lpImageAttribute As ImageProperty, ByRef Buffer As Byte)
    Delegate Sub MyFrameHookerDelegate(ByRef lpImageAttribute As ImageProperty, ByVal Buffer As IntPtr)

    Public FrameCount As Integer
    Public Average As Integer
    Public BrightestPixel As Byte
    'Public ImageBuf() As Byte
    Public _pImage As IntPtr
    Dim CurrentDevice As Integer
    Dim WindowHandle As IntPtr

    Dim FrameHooker1 As MyFrameHookerDelegate
    Dim FrameHooker2 As MyFrameHookerDelegate

    Public Const RAWDATA_IMAGE = 0
    Public Const BMPDATA_IMAGE = 1
    Public Const NORMAL_MODE = 0
    Public Const TRIGGER_MODE = 1

    <StructLayout(LayoutKind.Explicit)> _
    Public Structure ImageProperty
        <FieldOffset(0)> Public CameraID As Integer
        <FieldOffset(4)> Public WorkMode As Integer    ' 0 - NORMAL mode, 1 - TRIGGER mode
        <FieldOffset(8)> Public SensorClock As Integer ' 24, 48, 96 for 24MHz, 48MHz and 96MHz
        <FieldOffset(12)> Public Row As Integer    ' It's ColSize, in 1280x1024, it's 1024
        <FieldOffset(16)> Public Column As Integer ' It's RowSize, in 1280x1024, it's 1280
        <FieldOffset(20)> Public Bin As Integer    ' 0, 1, 2 for no-decimation, 1:2 and 1:4 decimation
        <FieldOffset(24)> Public BinMode As Integer '0 - Skip, 1 - Bin
        <FieldOffset(28)> Public CameraBit As Integer '8 or 16.
        <FieldOffset(32)> Public XStart As Integer
        <FieldOffset(36)> Public YStart As Integer
        <FieldOffset(40)> Public ExposureTime As Integer ' in 50us unit, e.g. 100 means 5000us(5ms)
        <FieldOffset(44)> Public RedGain As Integer
        <FieldOffset(48)> Public GreenGain As Integer
        <FieldOffset(52)> Public BlueGain As Integer
        <FieldOffset(56)> Public TimeStamp As Integer ' in 1ms unit, 0 - 0xFFFFFFFF and round back
        <FieldOffset(60)> Public SensorMode As Integer ' Bit0 is used for GRR mode, Bit1 is used for Strobe out enable
        <FieldOffset(64)> Public TriggerOccurred As Integer ' Used for NORMAL mode only, set when the external trigger occurred during the grabbing of last frame.
        <FieldOffset(68)> Public TriggerEventCount As Integer ' Reserved.
        <FieldOffset(72)> Public FrameSequenceNo As Integer ' Reserved.
        <FieldOffset(76)> Public IsFrameBad As Integer ' Is the current frame a bad one.

        <FieldOffset(80)> Public FrameProcessType As Integer ' 0 - RAW, 1 - BMP
        <FieldOffset(84)> Public FilterAcceptForFile As Integer ' Reserved
    End Structure

    <DllImport("SSClassic_USBCamera_SDK_Stdcall.dll", CallingConvention:=CallingConvention.StdCall)> _
    Function SSClassicUSB_GetModuleNoSerialNo(ByVal DeviceID As Integer, ByVal ModuleNo As StringBuilder, ByVal SerialNo As StringBuilder) As Integer
    End Function


    'Function MTUSB_GetModuleNo(ByVal DevHandle As Integer, ByVal ModuleNo As StringBuilder) As Integer
    Declare Function SSClassicUSB_InitDevice Lib "SSClassic_USBCamera_SDK_Stdcall" () As Integer
    Declare Function SSClassicUSB_UnInitDevice Lib "SSClassic_USBCamera_SDK_Stdcall" () As Integer
    Declare Function SSClassicUSB_AddDeviceToWorkingSet Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal DeviceID As Integer) As Integer
    Declare Function SSClassicUSB_RemoveDeviceFromWorkingSet Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal DeviceID As Integer) As Integer

    Declare Function SSClassicUSB_StartCameraEngine Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal ParentHandle As Integer, ByVal BitMode As Integer, _
                                                                                           ByVal ProcessThreads As Integer, ByVal IsCallBackFromThread As Integer) As Integer
    Declare Function SSClassicUSB_StopCameraEngine Lib "SSClassic_USBCamera_SDK_Stdcall" () As Integer

    Declare Function SSClassicUSB_ShowFactoryControlPanel Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal DeviceID As Integer, ByVal Title As String) As Integer
    Declare Function SSClassicUSB_HideFactoryControlPanel Lib "SSClassic_USBCamera_SDK_Stdcall" () As Integer
    Declare Function SSClassicUSB_StartFrameGrab Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal DeviceID As Integer, ByVal TotalFrames As Integer) As Integer
    Declare Function SSClassicUSB_StopFrameGrab Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal DeviceID As Integer) As Integer

    'Declare Function MTUSB_SetFrameSetting Lib " MT_USBCamera_SDK_Stdcall" ( DEV_HANDLE DevHandle, PImageCtl SettingPtr);
    Declare Function SSClassicUSB_SetCameraWorkMode Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal DeviceID As Integer, ByVal WorkMode As Integer) As Integer
    Declare Function SSClassicUSB_SetCustomizedResolution Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal deviceID As Integer, ByVal rowSize As Integer, _
                                                                                           ByVal columnSize As Integer, ByVal bin As Integer, ByVal BinMode As Integer) As Integer
    Declare Function SSClassicUSB_SetXYStart Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal DeviceID As Integer, ByVal XStart As Integer, ByVal YStart As Integer) As Integer
    Declare Function SSClassicUSB_SetGains Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal DeviceID As Integer, ByVal RedGain As Integer, ByVal GreenGain As Integer, _
                                                                            ByVal BlueGain As Integer) As Integer
    Declare Function SSClassicUSB_SetExposureTime Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal DeviceID As Integer, ByVal ExposureTime As Integer) As Integer
    Declare Function SSClassicUSB_SetGamma Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal DeviceID As Integer, ByVal Gamma As Integer, ByVal Contrast As Integer, ByVal Brightness As Integer, _
                                                                           ByVal SharpLevel As Integer) As Integer
    Declare Function SSClassicUSB_SetBWMode Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal DeviceID As Integer, ByVal BWMode As Integer, ByVal hMirror As Integer, ByVal vFlip As Integer) As Integer

    Declare Function SSClassicUSB_InstallFrameHooker Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal FrameType As Integer, ByVal FrameHooker As MyFrameHookerDelegate) As Integer

    Declare Function SSClassicUSB_SetGPIOConfig Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal DeviceID As Integer, ByVal ConfigByte As Byte) As Integer
    Declare Function SSClassicUSB_SetGPIOInOut Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal DeviceID As Integer, ByVal OutputByte As Byte, ByRef InputBytePtr As Byte) As Integer



    'Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (ByRef Destination As Byte, ByRef Source As Byte, ByVal Length As Integer)
    'Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (ByVal Destination As IntPtr, ByRef Source As Byte, ByVal Length As Integer)
    Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (ByVal Destination As IntPtr, ByVal Source As IntPtr, ByVal Length As Integer)
    Declare Function PostMessage Lib "user32" Alias "PostMessageA" (ByVal hwnd As IntPtr, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As IntPtr

    '
    'Declare Function GetWindowsDirectory Lib "kernel32" Alias "GetWindowsDirectoryA" (ByVal lpBuffer As String, ByVal nSize As Integer) As Integer
    'Declare Sub GetSystemInfo Lib "kernel32" (lpSystemInfo As SystemInfo)
    'Declare Sub GlobalMemoryStatus Lib "kernel32" (lpBuffer As MEMORYSTATUS)
    'Declare Function GetVersionEx Lib "kernel32" Alias "GetVersionExA" (ByRef lpVersionInformation As OSVERSIONINFO) As Integer
    'Declare Function GetSystemMetrics Lib "User32" (ByVal nIndex As Integer) As Integer
    'Declare Function GetDeviceCaps Lib "GDI32" (ByVal hDC As Integer, ByVal nIndex As Integer) As Integer
    'Declare Function TrackPopupMenu Lib "User32" (ByVal hMenu As Integer, ByVal wFlags As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal nReserved As Integer, ByVal hWnd As Integer, lpReserved As Any) As Integer
    'Declare Function GetMenu Lib "User32" (ByVal hWnd As Integer) As Integer
    'Declare Function GetSubMenu Lib "User32" (ByVal hMenu As Integer, ByVal nPos As Integer) As Integer
    'Declare Function GetDesktopWindow Lib "User32" () As Integer
    'Declare Function GetDC Lib "User32" (ByVal hWnd As Integer) As Integer
    'Declare Function ReleaseDC Lib "User32" (ByVal hWnd As Integer, ByVal hDC As Integer) As Integer
    'Declare Function BitBlt Lib "GDI32" (ByVal hDestDC As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hSrcDC As Integer, ByVal XSrc As Integer, ByVal YSrc As Integer, ByVal dwRop As Integer) As Integer
    'Declare Sub SetWindowPos Lib "User32" (ByVal hWnd As Integer, ByVal hWndInsertAfter As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal wFlags As Integer)
    'Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, lpKeyName As Any, ByVal lpDefault As String, ByVal lpRetunedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
    'Declare Function GetProfileString Lib "kernel32" Alias "GetProfileStringA" (ByVal lpAppName As String, lpKeyName As Any, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Integer) As Integer
    'Declare Function waveOutGetNumDevs Lib "winmm" () As Integer
    'Declare Function GetSystemDirectory Lib "kernel32" Alias "GetSystemDirectoryA" (ByVal lpBuffer As String, ByVal nSize As Integer) As Integer
    'Declare Function sndPlaySound Lib "winmm" Alias "sndPlaySoundA" (ByVal lpszSoundName As String, ByVal uFlags As Integer) As Integer


    Sub NoFrameHooker(ByRef lpImageAttribute As ImageProperty, ByVal Buffer As IntPtr)
        'Nothing
    End Sub

    Sub MyFrameHooker(ByRef lpImageAttribute As ImageProperty, ByVal Buffer As IntPtr)
        'Dim MyStr As String
        Dim i As Integer
        Dim TotalPixel As Integer
        Dim tmpBrightestPixel As Byte
        Dim M As New Message

        tmpBrightestPixel = 0

        FrameCount = FrameCount + 1
        'ReDim ImageBuf(lpImageAttribute.Column * lpImageAttribute.Row)

        ' We copy the image data to ImageBuf()...Note that this callback is invoked in a higher
        ' priority thread... so this callback should not be blocked or do any GUI operations
        ' (Although this example does a simple GUI showing for demo purpose)...we recommend user to put the
        ' Image data and attributes in buffer and post a message to main thread for further
        ' processing.
        CopyMemory(_pImage, Buffer, (lpImageAttribute.Column * lpImageAttribute.Row))
        'CopyMemory(_pImage, Buffer, (lpImageAttribute.Column * lpImageAttribute.Row * 3))
        TotalPixel = 0
        'For i = 0 To (lpImageAttribute.Column * lpImageAttribute.Row) - 1
        'TotalPixel = TotalPixel + ImageBuf(i)
        'If ImageBuf(i) > tmpBrightestPixel Then tmpBrightestPixel = ImageBuf(i)
        'Next
        Average = TotalPixel / (lpImageAttribute.Column * lpImageAttribute.Row)

        BrightestPixel = tmpBrightestPixel
        'MyStr = "Frames: " & FrameCount & "   Resolution: " & Column & "x" & row & " Gain (" & lpImageAttribute.RedGain _
        '& "," & lpImageAttribute.GreenGain & "," & lpImageAttribute.BlueGain & ") PixelValue: " & Average
        'Form1.StatusLabel.Text = MyStr

        'frmMain.FrameInfoLabel.Text = "Brightest pixel: " & BrightestPixel & vbCr & "Average pixel: " & Average & vbCr & "Frames: " & FrameCount
        M.Msg = &H401   'User defined message
        M.HWnd = WindowHandle
        M.WParam = BrightestPixel
        M.LParam = FrameCount
        PostMessage(M.HWnd, M.Msg, M.WParam, M.LParam)
        'DoEvents
    End Sub



    '----- FUNCTIONS FOR STARTING AND STOPPING CAMERA -----'

    Public Sub CameraSTART()
        InitializeDevices()
        OpenDevice()
        GetModuleNo()
        GetSerialNo()
        StartCameraEngine()
        SetCameraSettings()
        InstallFrameHooker()
        StartGrab()
    End Sub

    Public Sub CameraSTOP()
        'Stops the camera
        UninstallFrameHooker()
        StopGrab()
        StopCameraEngine()
        UninitDevices()
        'MsgBox("Camera Stopped")
    End Sub



    Private Sub InitializeDevices()
        Dim TotalDevices As Integer
        Dim MyStr As String
        Dim device As Integer

        _pImage = Marshal.AllocHGlobal(3 * 2560 * 1920) 'Maximum space for all resolutions.

        WindowHandle = frmMain.WinHwnd  'It's not changed...we keep it here.
        TotalDevices = SSClassicUSB_InitDevice
        ' Important: We should have two delegate to reference the callbacks...otherwise
        ' GC will collect the callbacks back.
        FrameHooker1 = AddressOf MyFrameHooker
        FrameHooker2 = AddressOf NoFrameHooker
        If TotalDevices > 0 Then
            For device = 1 To TotalDevices
                SSClassicUSB_AddDeviceToWorkingSet(device)
            Next
        End If
        If TotalDevices <> 1 Then

            MyStr = ("There are " & TotalDevices & " devices.")
            MsgBox(MyStr)
        End If
    End Sub

    Private Sub OpenDevice()
        CurrentDevice = 1
    End Sub


    Private Sub GetModuleNo()
        Dim ModuleNo As New StringBuilder(32)
        Dim SerialNo As New StringBuilder(32)
        Dim Result As Integer
        'Dim block As IntPtr
        'ModuleName = Space(32)
        Result = SSClassicUSB_GetModuleNoSerialNo(CurrentDevice, ModuleNo, SerialNo)
        'block = Marshal.AllocHGlobal(32)
        'Result = MTUSB_GetModuleNo(DeviceHandle, block.ToInt32)
        'Result = MTUSB_GetModuleNo(DeviceHandle, &H12345678)
        frmMain.ModuleNoLabel.Text = "ModuleNo.: " & ModuleNo.ToString
        'Marshal.FreeHGlobal(block)
    End Sub

    Private Sub GetSerialNo()
        Dim ModuleNo As New StringBuilder(32)
        Dim SerialNo As New StringBuilder(32)
        Dim Result As Integer
        'Dim block As IntPtr
        'ModuleName = Space(32)
        Result = SSClassicUSB_GetModuleNoSerialNo(CurrentDevice, ModuleNo, SerialNo)
        'block = Marshal.AllocHGlobal(32)
        frmMain.SerialNoLabel.Text = "SerialNo.: " & SerialNo.ToString
    End Sub

    Private Sub StartCameraEngine()
        SSClassicUSB_StartCameraEngine(frmMain.Handle, 8, 2, 0) 'frmMain.hWnd????
    End Sub

    Private Sub SetCameraSettings()
        'Dim Result As Integer
        SSClassicUSB_SetCameraWorkMode(CurrentDevice, NORMAL_MODE)
        SSClassicUSB_SetCustomizedResolution(CurrentDevice, 640, 480, 0, 0)  ' 2048x1536 resolution
        'SSClassicUSB_SetGains(CurrentDevice, 14, 14, 14)
        SSClassicUSB_SetExposureTime(CurrentDevice, 100)  '100x50= 5ms
    End Sub


    Private Sub InstallFrameHooker()
        SSClassicUSB_InstallFrameHooker(BMPDATA_IMAGE, FrameHooker1)
    End Sub

    Private Sub StartGrab()
        SSClassicUSB_StartFrameGrab(&H88, &H8888)
    End Sub

    Private Sub UninstallFrameHooker()
        SSClassicUSB_InstallFrameHooker(BMPDATA_IMAGE, Nothing)
    End Sub

    Private Sub StopGrab()
        SSClassicUSB_StopFrameGrab(&H88)
    End Sub

    Private Sub StopCameraEngine()
        SSClassicUSB_StopCameraEngine()
    End Sub

    Private Sub UninitDevices()
        If _pImage <> IntPtr.Zero Then
            Marshal.FreeHGlobal(_pImage)
        End If
        SSClassicUSB_UnInitDevice()
    End Sub
End Module
