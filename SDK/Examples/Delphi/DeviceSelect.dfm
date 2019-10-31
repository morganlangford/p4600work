object DeviceForm: TDeviceForm
  Left = 421
  Top = 129
  Width = 237
  Height = 308
  Caption = 'Device Selection'
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  OnShow = FormShow
  PixelsPerInch = 96
  TextHeight = 13
  object OKButton: TButton
    Left = 150
    Top = 240
    Width = 75
    Height = 25
    Caption = 'OK'
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clWindowText
    Font.Height = -11
    Font.Name = 'MS Sans Serif'
    Font.Style = [fsBold]
    ParentFont = False
    TabOrder = 0
    OnClick = OKButtonClick
  end
  object DeviceGroupBox: TGroupBox
    Left = 0
    Top = 0
    Width = 225
    Height = 217
    Caption = ' Device Selection '
    TabOrder = 1
    object Device1CheckBox: TCheckBox
      Left = 24
      Top = 19
      Width = 193
      Height = 17
      Caption = 'Device1CheckBox'
      TabOrder = 0
    end
    object Device2CheckBox: TCheckBox
      Left = 24
      Top = 42
      Width = 193
      Height = 17
      Caption = 'Device2CheckBox'
      TabOrder = 1
    end
    object Device3CheckBox: TCheckBox
      Left = 24
      Top = 64
      Width = 193
      Height = 17
      Caption = 'Device3CheckBox'
      TabOrder = 2
    end
    object Device4CheckBox: TCheckBox
      Left = 24
      Top = 88
      Width = 193
      Height = 17
      Caption = 'Device4CheckBox'
      TabOrder = 3
    end
    object Device5CheckBox: TCheckBox
      Left = 24
      Top = 112
      Width = 193
      Height = 17
      Caption = 'Device5CheckBox'
      TabOrder = 4
    end
    object Device6CheckBox: TCheckBox
      Left = 24
      Top = 136
      Width = 193
      Height = 17
      Caption = 'Device6CheckBox'
      TabOrder = 5
    end
    object Device7CheckBox: TCheckBox
      Left = 24
      Top = 160
      Width = 193
      Height = 17
      Caption = 'Device7CheckBox'
      TabOrder = 6
    end
    object Device8CheckBox: TCheckBox
      Left = 24
      Top = 184
      Width = 193
      Height = 17
      Caption = 'Device8CheckBox'
      TabOrder = 7
    end
  end
end
