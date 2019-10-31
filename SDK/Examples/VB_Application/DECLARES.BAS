Attribute VB_Name = "Module1"
Option Explicit
'Dim WinVersion As Integer, SoundAvailable As Integer
'Public VisibleFrame As Frame
Dim FrameCount As Long
Dim ImageBuf() As Byte
 
Public Const RAWDATA_IMAGE = 0
Public Const BMPDATA_IMAGE = 1
Public Const NORMAL_MODE = 0
Public Const TRIGGER_MODE = 1
Public Const GRAB_FRAME_FOREVER = &H8888


Public Type TPROCESSEDDATAPROPERTY
        CameraID As Long
        WorkMode As Long    ' 0 - NORMAL mode, 1 - TRIGGER mode
        SensorClock As Long ' 24, 48, 96 for 24MHz, 48MHz and 96MHz
        Row As Long    ' It's ColSize, in 1280x1024, it's 1024
        Column As Long ' It's RowSize, in 1280x1024, it's 1280
        Bin As Long    ' 0, 1, 2 for no-decimation, 1:2 and 1:4 decimation
        BinMode As Long '0 - Skip, 1 - Bin
        CameraBit As Long '8 or 16.
        XStart As Long
        YStart As Long
        ExposureTime As Long ' in 50us unit, e.g. 100 means 5000us(5ms)
        RedGain As Long
        GreenGain As Long
        BlueGain As Long
        TimeStamp As Long ' in 1ms unit, 0 - 0xFFFFFFFF and round back
        SensorMode As Long ' Bit0 is used for GRR mode, Bit1 is used for Strobe out enable
        TriggerOccurred As Long ' Used for NORMAL mode only, set when the external trigger occurred during the grabbing of last frame.
        TriggerEventCount As Long ' Reserved.
        FrameSequenceNo As Long ' Reserved.
        IsFrameBad As Long ' Is the current frame a bad one.

        FrameProcessType As Long ' 0 - RAW, 1 - BMP
        FilterAcceptForFile As Long ' Reserved
End Type


Declare Function SSClassicUSB_InitDevice Lib "SSClassic_USBCamera_SDK_Stdcall" () As Long
Declare Function SSClassicUSB_UnInitDevice Lib "SSClassic_USBCamera_SDK_Stdcall" () As Long
Declare Function SSClassicUSB_GetModuleNoSerialNo Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal deviceID As Long, ByVal ModuleNo As String, ByVal SerialNo As String) As Long
Declare Function SSClassicUSB_AddDeviceToWorkingSet Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal deviceID As Long) As Long
Declare Function SSClassicUSB_RemoveDeviceFromWorkingSet Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal deviceID As Long) As Long
Declare Function SSClassicUSB_StartCameraEngine Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal ParentHandle As Long, ByVal CameraBitOption As Long, ByVal ProcessThreads As Long, ByVal IsCallFromThread As Long) As Long
Declare Function SSClassicUSB_StopCameraEngine Lib "SSClassic_USBCamera_SDK_Stdcall" () As Long
Declare Function SSClassicUSB_SetCameraWorkMode Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal deviceID As Long, ByVal WorkMode As Long) As Long

Declare Function SSClassicUSB_ShowFactoryControlPanel Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal deviceID As Long, ByVal passWord As String) As Long
Declare Function SSClassicUSB_HideFactoryControlPanel Lib "SSClassic_USBCamera_SDK_Stdcall" () As Long
Declare Function SSClassicUSB_StartFrameGrab Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal deviceID As Long, ByVal TotalFrames As Long) As Long
Declare Function SSClassicUSB_StopFrameGrab Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal deviceID As Long) As Long
Declare Function SSClassicUSB_SetCustomizedResolution Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal deviceID As Long, ByVal RowSize As Long, _
                                                                                  ByVal ColSize As Long, ByVal Bin As Long, ByVal BinMode As Long) As Long
Declare Function SSClassicUSB_SetExposureTime Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal deviceID As Long, ByVal ExposureTime As Long) As Long
Declare Function SSClassicUSB_SetXYStart Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal deviceID As Long, ByVal XStart As Long, ByVal YStart As Long) As Long
Declare Function SSClassicUSB_SetGains Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal deviceID As Long, ByVal RedGain As Long, ByVal GreenGain As Long, _
                                                                   ByVal BlueGain As Long) As Long
Declare Function SSClassicUSB_SetGamma Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal deviceID As Long, ByVal Gamma As Long, ByVal Contrast As Long, ByVal Bright As Long, ByVal Sharp As Long) As Long
Declare Function SSClassicUSB_SetBWMode Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal deviceID As Long, ByVal BWMode As Long, ByVal H_Mirror As Long, ByVal V_Flip As Long) As Long
Declare Function SSClassicUSB_InstallFrameHooker Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal FrameType As Long, ByVal FrameHooker As Long) As Long
Declare Function SSClassicUSB_InstallUSBDeviceHooker Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal USBDeviceHooker As Long) As Long
Declare Function SSClassicUSB_SetGPIOConifg Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal deviceID As Long, ByVal ConfigByte As Byte) As Long
Declare Function SSClassicUSB_SetGPIOInOut Lib "SSClassic_USBCamera_SDK_Stdcall" (ByVal deviceID As Long, ByVal OutputByte As Byte, ByRef InputByteState As Byte) As Long
                                                                                  
Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (Destination As Any, Source As Any, ByVal Length As Long)
'

Sub MyFrameHooker(ByRef lpImageAttribute As TPROCESSEDDATAPROPERTY, ByRef Buffer As Byte)
    Dim MyStr As String
    Dim i As Long
    Dim TotalPixel As Long
    Dim Average As Long, Row As Long, Column As Long, Bin As Long
    
    If lpImageAttribute.Bin = 2 Then
       lpImageAttribute.Bin = 3
    End If

    Row = lpImageAttribute.Row / (lpImageAttribute.Bin + 1)
    Column = lpImageAttribute.Column / (lpImageAttribute.Bin + 1)
    FrameCount = FrameCount + 1
    ReDim ImageBuf(Column * Row)
       
    ' We copy the image data to ImageBuf()...Note that this callback is invoked in a higher
    ' priority thread... so this callback should not be blocked or do any GUI operations
    ' (Although this example does a simple GUI showing for demo purpose)...we recommend user to put the
    ' Image data and attributes in buffer and post a message to main thread for further
    ' processing.
    CopyMemory ImageBuf(0), Buffer, (Column * Row)
    TotalPixel = 0
    For i = 0 To (Column * Row - 1)
        TotalPixel = TotalPixel + ImageBuf(i)
    Next
    Average = TotalPixel / (Column * Row)
        
    MyStr = "Frames: " & FrameCount & "   Resolution: " & Column & "x" & Row & " Gain (" & lpImageAttribute.RedGain _
     & "," & lpImageAttribute.GreenGain & "," & lpImageAttribute.BlueGain & ") PixelValue: " & Average
    Form1.StatusLabel.Caption = MyStr
    
End Sub
