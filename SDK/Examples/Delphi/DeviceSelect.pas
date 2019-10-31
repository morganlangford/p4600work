unit DeviceSelect;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, DataTypes, DllInterface, StdCtrls;

type
  TDeviceForm = class(TForm)
    OKButton: TButton;
    DeviceGroupBox: TGroupBox;
    Device1CheckBox: TCheckBox;
    Device2CheckBox: TCheckBox;
    Device3CheckBox: TCheckBox;
    Device4CheckBox: TCheckBox;
    Device5CheckBox: TCheckBox;
    Device6CheckBox: TCheckBox;
    Device7CheckBox: TCheckBox;
    Device8CheckBox: TCheckBox;
    procedure FormShow(Sender: TObject);
    procedure OKButtonClick(Sender: TObject);
  private
    { Private declarations }
    procedure SystemMessageHandler( var Msg : TWMSysCommand ); message WM_SYSCOMMAND;    
  public
    { Public declarations }
  end;

var
  DeviceForm: TDeviceForm;
  CheckBoxArray : Array[1..MAX_CAMERA_WORK_SET] of TCheckBox;

implementation

{$R *.DFM}

procedure TDeviceForm.SystemMessageHandler( var Msg : TWMSysCommand );
begin
  if Msg.CmdType = SC_CLOSE then
    begin
      // Do nothing
    end
  else
    begin
      Inherited;
    end;
end;

procedure TDeviceForm.FormShow(Sender: TObject);
var
  ModuleNo : Array[0..15] of AnsiChar;
  SerialNo : Array[0..15] of AnsiChar;
  device : integer;
begin
  ContinueRun := False;
  // BUFUSBInitDevice() will return the Mightex BUF Camera Devices Only.
  CheckBoxArray[1] := Device1CheckBox;
  CheckBoxArray[2] := Device2CheckBox;
  CheckBoxArray[3] := Device3CheckBox;
  CheckBoxArray[4] := Device4CheckBox;
  CheckBoxArray[5] := Device5CheckBox;
  CheckBoxArray[6] := Device6CheckBox;
  CheckBoxArray[7] := Device7CheckBox;
  CheckBoxArray[8] := Device8CheckBox;
  for device :=1 to MAX_CAMERA_WORK_SET do
    begin
      CheckBoxArray[device].Visible := False;
    end;

  CurrentDevices := SSClassicUSBInitDevice();
  for device:=1 to CurrentDevices do
    begin
      // Note that device is "1" based.
      CheckBoxArray[device].Visible := True;
      SSClassicUSBGetModuleNoSerialNo( device, ModuleNo, SerialNo );
      CheckBoxArray[device].Caption :=  String(ModuleNo) + ':' + String(SerialNo);
      CheckBoxArray[device].Checked := True; // By default, it's selected.
      CameraModuleNo[device] := String(ModuleNo);
      CameraSerialNo[device] := String(SerialNo);
      CameraSelected[device] := False;
    end;
end;

procedure TDeviceForm.OKButtonClick(Sender: TObject);
var
  device : integer;
begin
  CurrentSelectedDevices := 0;
  if ( CurrentDevices > 0 ) then
    begin
      for device:=1 to CurrentDevices do
        begin
          if CheckBoxArray[device].Checked = True then
            begin
              ContinueRun := True;
              (*
                CurrentSelectedDevices has the real selected device, and
                the details are in CameraSelected[].
              *)
              Inc( CurrentSelectedDevices );
              CameraSelected[device] := True;
            end;
        end;
    end;
  Close;
end;

end.
