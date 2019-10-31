unit SSCamera_Example;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ExtCtrls,
  TeEngine, Series, TeeProcs, Chart, DataTypes, DllInterface,
  Mask, ImgList, inifiles,
  Buttons, ComCtrls;

const
  WM_USER_NOTIFY = WM_USER + 100;

  MAX_BUF_POINTS = 3648;
  BUF_POINT_START = 32;

type
  TMainForm = class(TForm)
    RzPanel1: TPanel;
    RzPanel2: TPanel;
    DevicesComboBox: TComboBox;
    ReSelectDeviceBitBtn: TBitBtn;
    ShowFactoryPanelBitBtn: TBitBtn;
    StartStopGrabbingButton: TBitBtn;
    StatusBar1: TStatusBar;
    SetToNormalModeButton: TButton;
    SetToTriggerModeButton: TButton;
    SetExposureTimeButton: TButton;
    ExposureTimeEdit: TEdit;
    Label1: TLabel;
    DecimationCheckBox: TCheckBox;
    ResolutionComboBox: TComboBox;
    XStartEdit: TEdit;
    YStartEdit: TEdit;
    Label2: TLabel;
    Label3: TLabel;
    SetXYStartButton: TButton;
    RedGainEdit: TEdit;
    GreenGainEdit: TEdit;
    BlueGainEdit: TEdit;
    Label4: TLabel;
    Label5: TLabel;
    Label6: TLabel;
    SetGainButton: TButton;
    SetResolutionButton: TButton;
    SimTriggerButton: TButton;
    GetCurrentFrameButton: TButton;
    TriggerTestButton: TButton;
    GPIOTimer: TTimer;
    procedure FormShow(Sender: TObject);
    procedure DevicesComboBoxChange(Sender: TObject);
    procedure ShowFactoryPanelBitBtnClick(Sender: TObject);
    procedure StartStopGrabbingButtonClick(Sender: TObject);
    procedure FormClose(Sender: TObject; var Action: TCloseAction);
    procedure ReSelectDeviceBitBtnClick(Sender: TObject);
    procedure SetToNormalModeButtonClick(Sender: TObject);
    procedure SetToTriggerModeButtonClick(Sender: TObject);
    procedure SetExposureTimeButtonClick(Sender: TObject);
    procedure SetResolutionButtonClick(Sender: TObject);
    procedure SetXYStartButtonClick(Sender: TObject);
    procedure SetGainButtonClick(Sender: TObject);
    procedure SimTriggerButtonClick(Sender: TObject);
    procedure GetCurrentFrameButtonClick(Sender: TObject);
    procedure TriggerTestButtonClick(Sender: TObject);
    procedure GPIOTimerTimer(Sender: TObject);
  private
    { Private declarations }

    procedure DeviceReInitializeNeededMsg( var Msg : TMessage ); message WM_USER_NOTIFY;
  public
    { Public declarations }
  end;

var
  MainForm: TMainForm;
  ImageCount : integer;
  DebugBuffer : Array [0..$400000] of Byte;
  RepeatRecord: Array[0..$10000] of integer;
  startTimeTick, endTimeTick : integer;
  frameRate : integer;
  CurrentWorkingMode : integer;
  CurrentFrameDataArray : Array of Byte;
  CurrentFrameDataArray2 : Array of Word;
  IsGPIO1High : boolean;

implementation

{$R *.DFM}

uses DeviceSelect;

procedure DeviceFaultCallBack( DeviceID: integer; DevieType: integer ); cdecl;
begin
  {
    JTZ: DeviceFaultCallBack is invoked by camera engine while the device mapping is
    changed ( device plug/unplug ) OR device error occurred. In this case, the camera
    engine invokes this hooker, the engine will do the following before invoke tha
    callback:
    1). Internally, engine stops to grab frames from any device
    2). Engine will unhook this hooker...so this is a "One-Shot" hooker.
    In this case, camera engine is in "Sleep" state and can only be woke up by
    re-initialization of the devices.
    So application is supposed to do the following:
    1). Post a message for further operations, it's NOT recommended to do
    cleaning and re-initialization in this callback.
    2). In the message handler, it should do all the house keeping as following:
       SSClassic_USBInstallFrameHooker( 0, nil);
       SSClassic_USBInstallUSBDeviceHooker( nil);
       SSClassic_USBStopCameraEngine();
       SSClassic_USBUnInitDevice();
    3). And then, it's supposed to do ( User may also show a warning and close
        application):
       SSClassic_USBInitDevice();
       ...
  }
  PostMessage( MainForm.Handle, WM_USER_NOTIFY, 0 , 0 );
end;

procedure FrameCallBack( ImageProperty : PProcessedDataProperty; Buffer : PByte ); cdecl;
var
  i : integer;
  Row, Column, FrameSize : integer;
  SPtr : PByte;
  RepeatCnt : integer;
  LastByte : Byte;
  TimeEllapse : integer;
  ReadByte : Byte;
begin
  {
    JTZ: For Linear camera, the callback is invoked in PanelFrom.Timer..which is in the
    main thread..so we're free to do any UI operations. Blocking the callback may pause
    the camera engine for a little while...the result is lower frame rate.
  }
  {
    In this sample code, we only process the frame from the current selected Device, note that
    camera engine will invoke frame callback for each Frame from any camera which was added
    in working set. In case of multiple cameras are used, the ImageProperty.CameraID is
    the identity of the camera (it's the same number userd for calling SSClassic_USBAddDeviceToWorkingSet()
  }
  if ImageProperty.CameraID = SelectedDevice then // This is the Data flow from current camera.
    begin
      Inc( ImageCount );
      if ImageCount = 1 then
        startTimeTick := ImageProperty.TimeStamp;
      MainForm.StatusBar1.Panels[3].Text := IntToStr( ImageProperty.ExposureTime * 50 ) + ' us';
      MainForm.StatusBar1.Panels[4].Text := IntToStr( ImageProperty.TimeStamp );
      MainForm.StatusBar1.Panels[5].Text := IntToStr( ImageProperty.TriggerOccurred);
      MainForm.StatusBar1.Panels[6].Text := IntToStr( ImageProperty.TriggerEventCount);
      MainForm.StatusBar1.Panels[7].Text := IntToStr( ImageProperty.FrameProcessType);

      Row := ImageProperty.Row div BIN_DATA_FACTOR[ImageProperty.Bin];
      Column := ImageProperty.Column div BIN_DATA_FACTOR[ImageProperty.Bin];
      if ImageProperty.FrameProcessType = 0 then // RAW_DATA
        begin
          FrameSize := Row * Column;
          SPtr := Buffer;
          //LastByte := $0;
          //RepeatCnt := 0;
          (*
          for i:=0 to FrameSize-1 do
            begin
              DebugBuffer[i] := SPtr^;
              {
              if SPtr^ = LastByte then
                begin
                  RepeatRecord[RepeatCnt] := i;
                  Inc(RepeatCnt);
                end;
              LastByte := SPtr^;
              }
              Inc( SPtr );
            end;
          *)
          //MainForm.StatusBar1.Panels[2].Text := IntToStr( RepeatCnt );
        end
      else  // BMP_DATA
        begin
          if Pos( 'B0', CameraModuleNo[ImageProperty.CameraID] ) <> 0 then  // It's Mono Camera
            FrameSize := Row * Column  { it's 8bit BITMAP, same as the RAW_DATA}
          else
            FrameSize := Row * Column * 3; { it's 24bit BITMAP }
          (*
          SPtr := Buffer;
          for i:=0 to FrameSize-1 do
            begin
              DebugBuffer[i] := SPtr^;
              Inc( SPtr );
            end;
          *)
        end;
    end;
end;

procedure TMainForm.DeviceReInitializeNeededMsg( var Msg : TMessage );
begin
  if TMessage(Msg).Msg = WM_USER_NOTIFY then
    begin
      // In this sample code, the "ReselectDevice" Button will do all the works
      // this message handler is supposed to do.
      ShowMessage( 'Device Error!' );
      //ReSelectDeviceBitBtnClick( Self );
    end;
end;


procedure TMainForm.FormShow(Sender: TObject);
var
  device : integer;
  i : integer;
begin
  i:=0;
  for device :=1 to CurrentDevices do
    begin
      {
        Note that CurrentDevices is the number of current connected BUF Camera
        Devices, but not all the them might be selected as "Working" camera,
        for example, we might have 5 BUF cameras connected on USB, but only
        2 and 4 are selected, in this case, we have:
        CameraSelected[] = (0, 1, 0, 1, 0);   2 and 4 are selected.
        Note that 2 and 4 are also the CameraID returned in the frame callback.
      }
      if CameraSelected[device] then
        begin
          SSClassicUSBAddDeviceToWorkingSet( device );
          DevicesComboBox.Items.Add( CameraSerialNo[device] );
          // The purpose of SelectableDevices is for quick getting of the
          // SelectedDevice ( SelectableDevices[DevicesComboBox.ItemIndex] )
          // Here the SelectedDevices is "1" based SELECTED device no.
          SelectableDevices[i] := device;
          Inc(i);
        end;
    end;
  // Let's install two callbacks here.
  SSClassicUSBInstallFrameHooker( 0, FrameCallBack );
  SSClassicUSBInstallUSBDeviceHooker( DeviceFaultCallBack );
  //SSClassicUSBStartCameraEngine( Handle, 8, 2, 0 ); // 8bit, 2 Processing Threads, CallingBack in MessageLoop
  SSClassicUSBStartCameraEngine( Handle, 8, 2, 1 ); // 8bit, 2 Processing Threads, CallingBack in working thread
  DevicesComboBox.ItemIndex := 0;
  ResolutionComboBox.ItemIndex := 7;
  SelectedDevice := SelectableDevices[DevicesComboBox.ItemIndex];

  StatusBar1.Panels[0].Text := 'Selected: ' + IntToStr(CurrentSelectedDevices)
                               + ' Cameras';
  StatusBar1.Panels[1].Text := CameraModuleNo[SelectedDevice] + ':' +
                               CameraSerialNo[SelectedDevice];
  StartStopGrabbingButton.Caption := 'Start Grab';
  CurrentWorkingMode := 0;

  // For testing GetCurrentFrame
  SetLength( CurrentFrameDataArray, 10000 ); // For first 10000 bytes only.
  SetLength( CurrentFrameDataArray2, 20000 ); // For first 10000 words only.

  SSClassicUSBSetGPIOConfig( SelectedDevice, $03 );
end;

procedure TMainForm.DevicesComboBoxChange(Sender: TObject);
begin
  SelectedDevice := SelectableDevices[DevicesComboBox.ItemIndex];
  StatusBar1.Panels[1].Text := CameraModuleNo[SelectedDevice] + ':' +
                               CameraSerialNo[SelectedDevice];
end;



procedure TMainForm.ShowFactoryPanelBitBtnClick(Sender: TObject);
begin
  SSClassicUSBShowFactoryControlPanel( SelectedDevice, '123456' );
end;

procedure TMainForm.StartStopGrabbingButtonClick(Sender: TObject);
begin
  // Start or Stop Frame Grabbing.
  if StartStopGrabbingButton.Caption = 'Start Grab' then
    begin
      // Start the grabbing
      StartStopGrabbingButton.Caption := 'Stop Grab'; // Show "STOP" icon
      SSClassicUSBStartFrameGrab( ALL_DEVICES, FOREVER_TARGET_FRAMES );   // Get 10000 Frames only.
    end
  else // "STOP" icon is showing.
    begin
      // Stop the grabbing
      StartStopGrabbingButton.Caption := 'Start Grab'; // Show "START" icon.
      SSClassicUSBStopFrameGrab( ALL_DEVICES );
    end;
end;

procedure TMainForm.FormClose(Sender: TObject; var Action: TCloseAction);
begin
  // When user exit the application.
  SSClassicUSBStopCameraEngine();
  SSClassicUSBUnInitDevice();
  SetLength( CurrentFrameDataArray, 0 );
  SetLength( CurrentFrameDataArray2, 0 );
end;

procedure TMainForm.ReSelectDeviceBitBtnClick(Sender: TObject);
var
  DevideForm : TDeviceForm;
begin
  // 1. House Keeping.
  SSClassicUSBInstallFrameHooker( 0, nil);
  SSClassicUSBInstallUSBDeviceHooker( nil);
  SSClassicUSBStopCameraEngine();
  SSClassicUSBUnInitDevice();
  DevicesComboBox.Items.Clear;
  DevicesComboBox.ItemIndex := -1;
  
  // 2. Show Device Selection Form again
  DeviceForm := TDeviceForm.Create( Application );
  DeviceForm.ShowModal;
  DeviceForm.Free;

  // 3. Explicitly call FormShow();
  // In FormShow(), it will install hookers and start the camera engine.
  FormShow( Self );
end;

procedure TMainForm.SetToNormalModeButtonClick(Sender: TObject);
begin
    //SSClassic_USBSetCameraWorkMode( SelectedDevice, 1 );
    CurrentWorkingMode := 0;
    SSClassicUSBSetCameraWorkMode( SelectedDevice, 0 );
end;

procedure TMainForm.SetToTriggerModeButtonClick(Sender: TObject);
begin
  CurrentWorkingMode := 1;
  SSClassicUSBSetCameraWorkMode( SelectedDevice, 1 );
  // Set GPIO1 to output
  //SSClassicUSBSetGPIOConfig( SelectedDevice, $0E ); // 00001110;
end;

procedure TMainForm.SetExposureTimeButtonClick(Sender: TObject);
var
  ExposureTime : integer;
begin
  ExposureTime := StrToInt( ExposureTimeEdit.Text );
  ExposureTime := ExposureTime div 50; // in 50us unit.
  SSClassicUSBSetExposureTime( SelectedDevice, ExposureTime );
end;

procedure TMainForm.SetResolutionButtonClick(Sender: TObject);
var
  Resolution : integer;
  Bin : integer;
begin
  Resolution := ResolutionComboBox.ItemIndex + 1;
  Bin := 0;
  if DecimationCheckBox.Checked then
    Bin := 1;
  //SSClassicUSBSetResolution( SelectedDevice, Resolution, Bin, 1 );
  SSClassicUSBSetCustomizedResolution( SelectedDevice, IMAGE_COLS[Resolution],
                                       IMAGE_ROWS[Resolution], Bin, 0 );
end;

procedure TMainForm.SetXYStartButtonClick(Sender: TObject);
var
  XStart, YStart : integer;
begin
  XStart := StrToInt( XStartEdit.Text );
  YStart := StrToInt( YStartEdit.Text );
  SSClassicUSBSetXYStart( SelectedDevice, XStart, YStart );
end;

procedure TMainForm.SetGainButtonClick(Sender: TObject);
var
  RGain, GGain, BGain : integer;
begin
  RGain := StrToInt( RedGainEdit.Text );
  GGain := StrToInt( GreenGainEdit.Text );
  BGain := StrToInt( BlueGainEdit.Text );
  SSClassicUSBSetGains( SelectedDevice, RGain, GGain, BGain );
end;

procedure TMainForm.SimTriggerButtonClick(Sender: TObject);
begin
  // Software Simulation
  SSCLASSUSBSoftTrigger( SelectedDevice );
end;


procedure TMainForm.GetCurrentFrameButtonClick(Sender: TObject);
var
  sPtr, rPtr : PByte;
  i,j : integer;
begin
  // Uninstall the frame hooker.
  SSClassicUSBInstallFrameHooker( 0, nil );
  //for j:=0 to 100 do
    begin
      rPtr := SSCLASSICUSBGetCurrentFrame( 0, SelectedDevice, sPtr );
      if rPtr <> nil then
        begin
          for i:=0 to 9999 do
            begin
              CurrentFrameDataArray[i] := sPtr^;
              Inc( sPtr );
            end;
        end;
    end;
  ImageCount := i;
end;

procedure TMainForm.TriggerTestButtonClick(Sender: TObject);
begin
  // Enable the timer.
  if GPIOTimer.Enabled = False then
    begin
      GPIOTimer.Enabled := True;
      TriggerTestButton.Caption := 'Test...';
      IsGPIO1High := True;
    end
  else
    begin
      GPIOTimer.Enabled := False;
      TriggerTestButton.Caption := 'Auto Trigger Test';
    end;
end;

procedure TMainForm.GPIOTimerTimer(Sender: TObject);
var
  InByte : Byte;
begin
  // Toggle the GPIO1 in each interval
  if IsGPIO1High = True then
    SSCLASSUSBSoftTrigger( SelectedDevice );
  IsGPIO1High := Not IsGPIO1High;
end;

end.
