object GPIOForm: TGPIOForm
  Left = 265
  Top = 226
  Width = 225
  Height = 226
  Caption = 'GPIO Control'
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  PixelsPerInch = 96
  TextHeight = 13
  object GPIOGroup: TRzGroupBox
    Left = 4
    Top = 3
    Width = 209
    Height = 150
    Caption = ' GPIO Control '
    ParentColor = True
    TabOrder = 0
    object OutputCheckGroup: TRzCheckGroup
      Left = 8
      Top = 16
      Width = 81
      Caption = 'Set As Output'
      Items.Strings = (
        'Gpio1'
        'Gpio2'
        'Gpio3'
        'Gpio4')
      ParentColor = True
      TabOrder = 0
      TextStyle = tsRecessed
      CheckStates = (
        0
        0
        0
        0)
    end
    object SetGpioConfigBitBtn: TRzBitBtn
      Left = 14
      Top = 128
      Height = 17
      Caption = 'Set Config'
      ParentColor = True
      TabOrder = 1
      TextStyle = tsRecessed
      OnClick = SetGpioConfigBitBtnClick
    end
    object RzGroupBox18: TRzGroupBox
      Left = 96
      Top = 8
      Width = 105
      Height = 55
      ParentColor = True
      TabOrder = 2
      object Out1CheckBox: TRzCheckBox
        Left = 8
        Top = 8
        Width = 43
        Height = 17
        Caption = 'Out1'
        Enabled = False
        State = cbUnchecked
        TabOrder = 0
        TextStyle = tsRecessed
      end
      object Out2CheckBox: TRzCheckBox
        Left = 56
        Top = 8
        Width = 43
        Height = 17
        Caption = 'Out2'
        Enabled = False
        State = cbUnchecked
        TabOrder = 1
        TextStyle = tsRecessed
      end
      object Out3CheckBox: TRzCheckBox
        Left = 8
        Top = 32
        Width = 43
        Height = 17
        Caption = 'Out3'
        Enabled = False
        State = cbUnchecked
        TabOrder = 2
        TextStyle = tsRecessed
      end
      object Out4CheckBox: TRzCheckBox
        Left = 56
        Top = 32
        Width = 43
        Height = 17
        Caption = 'Out4'
        Enabled = False
        State = cbUnchecked
        TabOrder = 3
        TextStyle = tsRecessed
      end
    end
    object RzGroupBox19: TRzGroupBox
      Left = 96
      Top = 63
      Width = 105
      Height = 57
      ParentColor = True
      TabOrder = 3
      object In1CheckBox: TRzCheckBox
        Left = 8
        Top = 8
        Width = 43
        Height = 17
        Caption = 'In1'
        State = cbUnchecked
        TabOrder = 0
        TextStyle = tsRecessed
      end
      object In2CheckBox: TRzCheckBox
        Left = 56
        Top = 8
        Width = 43
        Height = 17
        Caption = 'In2'
        State = cbUnchecked
        TabOrder = 1
        TextStyle = tsRecessed
      end
      object In3CheckBox: TRzCheckBox
        Left = 8
        Top = 32
        Width = 43
        Height = 17
        Caption = 'In3'
        State = cbUnchecked
        TabOrder = 2
        TextStyle = tsRecessed
      end
      object In4CheckBox: TRzCheckBox
        Left = 56
        Top = 32
        Width = 43
        Height = 17
        Caption = 'In4'
        State = cbUnchecked
        TabOrder = 3
        TextStyle = tsRecessed
      end
    end
    object SetGpioBitBtn: TRzBitBtn
      Left = 125
      Top = 128
      Height = 17
      Caption = 'Set IO'
      ParentColor = True
      TabOrder = 4
      TextStyle = tsRecessed
      OnClick = SetGpioBitBtnClick
    end
  end
  object OKButton: TButton
    Left = 136
    Top = 160
    Width = 75
    Height = 25
    Caption = 'OK'
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clWindowText
    Font.Height = -11
    Font.Name = 'MS Sans Serif'
    Font.Style = [fsBold]
    ParentFont = False
    TabOrder = 1
    OnClick = OKButtonClick
  end
end
