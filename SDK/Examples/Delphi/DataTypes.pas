unit DataTypes;

interface

uses Windows, graphics;

const
  VID = $04B4;
  PID = $0228;  // For Bufferred Camera

  RAW_DATA_IMAGE = 0;
  BMP_DATA_IMAGE = 1;

  NORMAL_FRAMES = 4;
  TRIGGER_FRAMES= 24;

  ALL_DEVICES = $88;

  PRESET_SHOW_IMAGE_WIDTH = 480;
  PRESET_SHOW_IMAGE_HEIGHT= 360;

  FOREVER_TARGET_FRAMES = $8888;
  AUTO_EXPOSURE_ROI_WIDTH = 64;

  MAX_RESOLUTIONS = 11;
  MAX_RESOLUTIONS_V032 = 5;  
  MAX_RESOLUTIONS_M001 = 8;
  MAX_RESOLUTIONS_T001 = 10;

  MAX_CAMERA_WORK_SET  = 8;
  MAX_EXPOSURE_TIME = 15000;   // (750*20); In 50us unit
  MIN_EXPOSURE_TIME = 1;

  MAX_EXPOSURE_TIME_LEVEL = 3;
  MAX_EXPOSURETIME : Array[0..MAX_EXPOSURE_TIME_LEVEL] of integer = ( 5, 10, 100, 750 );


  IMAGE_RESOLUTION : Array[1..MAX_RESOLUTIONS] of String =
        ( ' 64 x 64 ', ' 192 x 128 ', ' 320 x 240 ', ' 640 x 480 ', ' 768 x 480',
          ' 1024 x 768 ', ' 1280 x 960 ', ' 1280 x 1024 ', ' 1536 x 1216 ', ' 2048 x 1536 ', ' 2560 x 1920 ' );
  IMAGE_SIZES : Array[1..MAX_RESOLUTIONS] of integer =
        ( (64*64), (192*128), (320*240), (640*480), (768*480), (1024*768), (1280*960),
          (1280*1024), (1536*1216), (2048*1536), (2560*1920) );
  IMAGE_ROWS : Array[1..MAX_RESOLUTIONS] of integer =
        ( 64, 128, 240, 480, 480, 768,  960,  1024, 1216, 1536, 1920 );
  IMAGE_COLS : Array[1..MAX_RESOLUTIONS] of integer =
        ( 64, 192, 320, 640, 768, 1024, 1280, 1280, 1536, 2048, 2560 );
  IMAGE_BIN_ALLOW : Array[1..MAX_RESOLUTIONS] of integer =
        ( 0,   0,   0,   1,   1,   2,    2,    2,    2,    2,    2   );        
  BIN_DATA_FACTOR : Array[0..2] of integer = ( 1, 2, 4);

type
  eDEVICETYPE = ( MT9M001_M, MT9M001_C, MT9V032_M, MT9V032_C, MT9T001,MT9P031_M, MT9P031_C );
  eWORKMODE = ( CONTINUE_MODE , EXT_TRIGGER_MODE );
  eAutoExposureState = ( FIRST_TIME_SET, FOLLOWING_SETS );
  //eEXTTRIGGERSTAT = ( EXT_SUCCESS, EXT_INPROCESS, EXT_ABORT );

  DWord = LongInt;
  PByte = ^Byte;
  PWord = ^Word;
  PDword = ^DWord;
  PLong = ^Integer;
  PInteger = ^Integer;
  pBitMap = ^TBitmap;

  LogPal = record
    lpal : TLogPalette;
    dummy:Array[0..255] of TPaletteEntry;
  end;  

  TImageSize = record
    Row : integer;
    Column : integer;
  end;

  TBUFCamera = record
    DeviceType : eDEVICETYPE;
    FWVersion  : integer;
    APVersion  : integer;
    BLVersion  : integer;
  end;

  TGrabFrameToFileCtl = record
    SavetoFileNeeded: Boolean;
    SaveAsJPEG : Boolean;
    AppendDataTime : Boolean;
    SwitchSkipModeNeeded : Boolean;
    SaveFileCount : integer;
    SavedCount: integer;
  end;

  TCameraControl = record
    CurrentMode  : eWORKMODE;
    RowSize    : integer;
    ColSize    : integer;
    Resolution : integer;
    Bin    : integer;  // 0: No decimation, 1: 1:2, 2: 1:4.
    BinMode: integer;  // 0 : Skip mode, 1: Bin mode
    ExposureTime : integer;
    XStart : integer;
    YStart : integer;
    RedGain: integer;
    GreenGain : integer;
    BlueGain : integer;
    RedRatio : integer;
    BlueRatio: integer;

    AutoExposureEnable : integer;

    SaveFileCtl: TGrabFrameToFileCtl;
    GrabFrameCount : integer;
    GrabFrameDirectory : String;
    GrabFrameFile : String;

    IgnoreSkipMode : Boolean;
    SaveAsJpeg : Boolean;
    AppendDateTime : Boolean;
  end;

  TParameterSetCtl = record
    HaveCommand : Boolean;
    CommandValue: integer;
    ExtraValue1 : integer;
    ExtraValue2 : integer;
    ExtraValue3 : integer;
    ExtraValue4 : integer;
    Tag : integer;
    Command : ( SET_EXPOSURE, SET_XYSTART, SET_GAINS,
                SET_GAMMA, SET_GPIOCONFIG, SET_GPIOSET );
  end;

  TDebugLog = record
    PixelAverage : integer;
    XStart       : integer;
    YStart       : integer;
    ExposureTime : integer;
    RedGain      : integer;
    GreenGain    : integer;
    BlueGain     : integer;
    TimeStamp    : integer;
  end;

var
  OneInstaceMutex : THandle;
  ContinueRun : Boolean;
  CurrentDevices : integer;
  CurrentSelectedDevices : integer;
  SelectedDevice : integer;
  CurrentDeviceIndex : integer;
  GrabFrameType : integer;

  //ParaSetCtl : TParameterSetCtl;
  //SaveFileCtl: TGrabFrameToFileCtl;

  CameraModuleNo: Array[1..MAX_CAMERA_WORK_SET] of String;
  CameraSerialNo: Array[1..MAX_CAMERA_WORK_SET] of String;
  //CameraParaControl : Array[1..MAX_CAMERA_WORK_SET] of TCameraControl;
  CameraSelected : Array[1..MAX_CAMERA_WORK_SET] of Boolean;
  SelectableDevices : Array[0..MAX_CAMERA_WORK_SET-1] of integer;
  
implementation

end.
