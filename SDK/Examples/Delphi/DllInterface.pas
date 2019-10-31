unit DllInterface;

interface

uses windows, graphics, DataTypes;

type

  PImageControl = ^TImageControl;
  TImageControl = packed record
    Revision : integer;
    // Camera Device Ino
    DeviceType : eDEVICETYPE;
    Reserved1: Array[0..7] of Byte;
    // Row/Column are also reserved.
    Reserved2: integer;
    Reserved3: integer;
    // For Image Capture
    WorkMode : eWORKMODE;
    Resolution : integer;
    ExposureTime : integer;
    Bin    : integer;
    XStart : integer;
    YStart : integer;
    RedGain: integer;
    GreenGain : integer;
    BlueGain : integer;
    BufferCnt : integer;
    BufferOption : integer;
    // GPIO Control
    GpioConfigByte: Byte; // Config for Input/Output for each pin.
    GpioCurrentSet: Byte; // For output Pins only.
  end;

  PProcessedDataProperty = ^TProcessedDataProperty;
  TProcessedDataProperty = record
    CameraID     : integer;
    WorkMode     : integer;
    SensorClock  : integer;
    Row          : integer;
    Column       : integer;
    Bin          : integer;
    BinMode      : integer;
    CameraBit    : integer;
    XStart       : integer;
    YStart       : integer;
    ExposureTime : integer;
    RedGain      : integer;
    GreenGain    : integer;
    BlueGain     : integer;
    TimeStamp    : integer;
    SensorMode   : integer;
    TriggerOccurred  : integer;
    TriggerEventCount: integer;
    FrameSequenceNo  : integer;
    IsFrameBad       : integer;

    FrameProcessType : integer;
    FilterAcceptForFile : integer;
  end;

  TGetFrameCallBack = procedure( ImageProperty : PProcessedDataProperty ; FramePtr : PByte ); cdecl;
  TUSBDeviceCallBack = procedure( DeviceID: integer; DeviceType: integer ); cdecl;

// General functions
function SSCLASSICUSBInitDevice() : integer; cdecl;
function SSCLASSICUSBUnInitDevice() : integer; cdecl;
function SSCLASSICUSBGetModuleNoSerialNo( deviceID : integer; ModuleNo : PAnsiChar; SerialNo : PAnsiChar ): integer; cdecl;
function SSCLASSICUSBAddDeviceToWorkingSet( deviceID : integer ) : integer; cdecl;
function SSCLASSICUSBRemoveDeviceFromWorkingSet( deviceID : integer ) : integer; cdecl;
function SSCLASSICUSBStartCameraEngine( ParentFormHandle : THandle ; CameraBitOption : integer ; ProcessThreads : integer; IsCallbackInThread : integer ) : integer ; cdecl;
function SSCLASSICUSBStopCameraEngine() : integer ; cdecl;
function SSCLASSICUSBSetBayerFilterType( DeviceID: integer; filterType : integer ) : integer ; cdecl;
function SSCLASSICUSBSetCameraWorkMode( DeviceID : integer ;
                                  WorkMode : Integer ) : integer; cdecl;
function SSCLASSICUSBStartFrameGrab( deviceID : integer; TotalFrames : Integer ) : integer ; cdecl;
function SSCLASSICUSBStopFrameGrab(  deviceID : integer ) : integer ; cdecl;
function SSCLASSICUSBShowFactoryControlPanel( deviceID : integer ; passWord: PAnsiChar ) : integer; cdecl;
function SSCLASSICUSBHideFactoryControlPanel() : integer; cdecl;
function SSCLASSICUSBGetFrameSetting( deviceID : integer ; SettingPtr : PImageControl ) : integer; cdecl;
function SSCLASSICUSBSetFrameSetting( deviceID : THandle ; SettingPtr : PImageControl ) : integer; cdecl;
function SSCLASSICUSBSetCustomizedResolution( deviceID : integer ;
                                         RowSize : integer; ColSize : integer; Bin : integer ; BinMode : integer ):integer; cdecl;
function SSCLASSICUSBSetExposureTime( deviceID : integer ; exposureTime : integer ) : integer; cdecl;
function SSCLASSICUSBSetXYStart( deviceID : integer ;  XStart : integer; YStart : integer ):integer; cdecl;
function SSCLASSICUSBSetGains(deviceID : integer ;
                         RedGain : integer ; GreenGain : integer ; BlueGain : integer ):integer; cdecl;
function SSCLASSICUSBSetGainRatios(deviceID : integer ;
                             RedGainRatio : integer ; BlueGainRatio : integer ):integer; cdecl;
function SSCLASSICUSBSetGamma(deviceID : integer ; Gamma : integer ; Contrast : integer ;
                         Bright: integer ; Sharp : integer ):integer;cdecl;
function SSCLASSICUSBSetBWMode(deviceID : integer ; BWMode : integer ; H_Mirror : integer ; V_Flip : integer):integer;cdecl;
function SSCLASSUSBSoftTrigger( deviceID : integer ) : integer; cdecl;
function SSCLASSICUSBSetSensorMode( deviceID: integer ; sensorMode : integer ) : integer; cdecl;
function SSCLASSICUSBInstallFrameHooker( FrameType : integer ; FrameHooker : TGetFrameCallBack  ) : integer; cdecl;
function SSCLASSICUSBInstallUSBDeviceHooker( USBDeviceHooker : TUSBDeviceCallBack ) : integer; cdecl;
function SSCLASSICUSBGetCurrentFrame( FrameType : integer ; deviceID : integer; var FramePtr : PByte ) : PByte ; cdecl;
function SSCLASSICUSBSetGPIOConfig( deviceID : THandle ; ConfigByte : Byte ) : integer; cdecl;
function SSCLASSICUSBSetGPIOInOut( deviceID : integer ; OutputByte : Byte;
                             InputBytePtr : PByte ) : integer; cdecl;
function SSCLASSICUSBApplicationActivate( IsActive : Boolean ) : integer; cdecl;


implementation

function SSCLASSICUSBInitDevice; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_InitDevice';
function SSCLASSICUSBUnInitDevice; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_UnInitDevice';
function SSCLASSICUSBGetModuleNoSerialNo; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_GetModuleNoSerialNo';
function SSCLASSICUSBAddDeviceToWorkingSet; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_AddDeviceToWorkingSet';
function SSCLASSICUSBRemoveDeviceFromWorkingSet; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_RemoveDeviceFromWorkingSet';
function SSCLASSICUSBStartCameraEngine; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_StartCameraEngine';
function SSCLASSICUSBStopCameraEngine; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_StopCameraEngine';
function SSCLASSICUSBSetBayerFilterType; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_SetBayerFilterType';
function SSCLASSICUSBSetCameraWorkMode; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_SetCameraWorkMode';
function SSCLASSICUSBStartFrameGrab; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_StartFrameGrab';
function SSCLASSICUSBStopFrameGrab; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_StopFrameGrab';
function SSCLASSICUSBShowFactoryControlPanel; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_ShowFactoryControlPanel';
function SSCLASSICUSBHideFactoryControlPanel; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_HideFactoryControlPanel';
function SSCLASSICUSBGetFrameSetting; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_GetFrameSetting';
function SSCLASSICUSBSetFrameSetting; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_SetFrameSetting';
function SSCLASSICUSBSetCustomizedResolution; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_SetCustomizedResolution';
function SSCLASSICUSBSetExposureTime; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_SetExposureTime';
function SSCLASSICUSBSetXYStart; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_SetXYStart';
function SSCLASSICUSBSetGains; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_SetGains';
function SSCLASSICUSBSetGainRatios; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_SetGainRatios';
function SSCLASSICUSBSetGamma; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_SetGamma';
function SSCLASSICUSBSetBWMode; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_SetBWMode';
function SSCLASSUSBSoftTrigger; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_SoftTrigger';
function SSCLASSICUSBSetSensorMode; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_SetSensorMode';
function SSCLASSICUSBInstallFrameHooker; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_InstallFrameHooker';
function SSCLASSICUSBInstallUSBDeviceHooker; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_InstallUSBDeviceHooker';
function SSCLASSICUSBGetCurrentFrame; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_GetCurrentFrame';
function SSCLASSICUSBSetGPIOConfig; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_SetGPIOConfig';
function SSCLASSICUSBSetGPIOInOut; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_SetGPIOInOut';
function SSCLASSICUSBApplicationActivate; external 'SSClassic_USBCamera_SDK.dll' name 'SSClassicUSB_ApplicationActivate';

end.

