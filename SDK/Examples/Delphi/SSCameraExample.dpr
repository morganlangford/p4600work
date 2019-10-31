program SSCameraExample;

uses
  Forms,
  Windows,
  Dialogs,
  SSCamera_Example in 'SSCamera_Example.pas' {MainForm},
  DataTypes in 'DataTypes.pas',
  DeviceSelect in 'DeviceSelect.pas' {DeviceForm},
  DllInterface in 'DllInterface.pas';

{$R *.RES}

begin
  Application.Initialize;
  OneInstaceMutex := 0;
  OneInstaceMutex := OpenMutex( MUTEX_ALL_ACCESS, False , 'MightexCCDCameraOneInstance' );
  if  OneInstaceMutex = 0 then
    begin
      OneInstaceMutex := CreateMutex( nil, False, 'MightexCCDCameraOneInstance' );
      DeviceForm := TDeviceForm.Create( Application );
      DeviceForm.ShowModal;
      DeviceForm.Free;
      if ContinueRun = True then
        begin
          Application.CreateForm(TMainForm, MainForm);
  Application.Run;
        end
      else
        begin
          SSClassicUSBUnInitDevice();
          Application.Terminate;
        end;
    end
  else
    begin
      ShowMessage( 'The Application is already running!' );
      Application.Terminate;
    end;
end.
