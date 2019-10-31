unit GPIO;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, RzRadChk, RzButton, RzRadGrp, ExtCtrls, RzPanel, DataTypes,
  DllInterface;

type
  TGPIOForm = class(TForm)
    GPIOGroup: TRzGroupBox;
    OutputCheckGroup: TRzCheckGroup;
    SetGpioConfigBitBtn: TRzBitBtn;
    RzGroupBox18: TRzGroupBox;
    Out1CheckBox: TRzCheckBox;
    Out2CheckBox: TRzCheckBox;
    Out3CheckBox: TRzCheckBox;
    Out4CheckBox: TRzCheckBox;
    RzGroupBox19: TRzGroupBox;
    In1CheckBox: TRzCheckBox;
    In2CheckBox: TRzCheckBox;
    In3CheckBox: TRzCheckBox;
    In4CheckBox: TRzCheckBox;
    SetGpioBitBtn: TRzBitBtn;
    OKButton: TButton;
    procedure OKButtonClick(Sender: TObject);
    procedure SetGpioConfigBitBtnClick(Sender: TObject);
    procedure SetGpioBitBtnClick(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  GPIOForm: TGPIOForm;

implementation

{$R *.DFM}

procedure TGPIOForm.OKButtonClick(Sender: TObject);
begin
  Close;
end;

procedure TGPIOForm.SetGpioConfigBitBtnClick(Sender: TObject);
var
  GpioConfigByte : Byte ;
begin
  // collect the value and write to device.
  GpioConfigByte := $FF;
  if OutputCheckGroup.Checks[0].Checked then
    begin
      GpioConfigByte := GpioConfigByte and $FE;
      Out1CheckBox.Enabled := True;
    end
  else
    begin
      Out1CheckBox.Enabled := False;
    end;
  if OutputCheckGroup.Checks[1].Checked then
    begin
      GpioConfigByte := GpioConfigByte and $FD;
      Out2CheckBox.Enabled := True;
    end
  else
    begin
      Out2CheckBox.Enabled := False;
    end;
  if OutputCheckGroup.Checks[2].Checked then
    begin
      GpioConfigByte := GpioConfigByte and $FB;
      Out3CheckBox.Enabled := True;
    end
  else
    begin
      Out3CheckBox.Enabled := False;
    end;
  if OutputCheckGroup.Checks[3].Checked then
    begin
      GpioConfigByte := GpioConfigByte and $F7;
      Out4CheckBox.Enabled := True;
    end
  else
    begin
      Out4CheckBox.Enabled := False;
    end;
  CCDUSBSetGPIOConifg(SelectedDevice, GpioConfigByte );
end;

procedure TGPIOForm.SetGpioBitBtnClick(Sender: TObject);
var
  GpioCurrentSet, GpioState : Byte;
begin
  GpioCurrentSet := 0;
  if Out1CheckBox.Enabled AND Out1CheckBox.Checked then
    GpioCurrentSet := GpioCurrentSet or $1;
  if Out2CheckBox.Enabled AND Out2CheckBox.Checked then
    GpioCurrentSet := GpioCurrentSet or $2;
  if Out3CheckBox.Enabled AND Out3CheckBox.Checked then
    GpioCurrentSet := GpioCurrentSet or $4;
  if Out4CheckBox.Enabled AND Out4CheckBox.Checked then
    GpioCurrentSet := GpioCurrentSet or $8;
  CCDUSBSetGPIOInOut( SelectedDevice, GpioCurrentSet, @GpioState );
  In1CheckBox.Checked := False;
  In2CheckBox.Checked := False;
  In3CheckBox.Checked := False;
  In4CheckBox.Checked := False;
  if ( GpioState and $1 ) = $1 then
     In1CheckBox.Checked := True;
  if ( GpioState and $2 ) = $2 then
     In2CheckBox.Checked := True;
  if ( GpioState and $4 ) = $4 then
     In3CheckBox.Checked := True;
  if ( GpioState and $8 ) = $8 then
     In4CheckBox.Checked := True;
end;

end.
