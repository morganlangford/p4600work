VERSION 5.00
Begin VB.Form Form1 
   Caption         =   "Form1"
   ClientHeight    =   8505
   ClientLeft      =   60
   ClientTop       =   450
   ClientWidth     =   8175
   LinkTopic       =   "Form1"
   ScaleHeight     =   8505
   ScaleWidth      =   8175
   StartUpPosition =   3  'Windows Default
   Begin VB.TextBox Text1 
      Height          =   1455
      Left            =   240
      MultiLine       =   -1  'True
      TabIndex        =   21
      Text            =   "ExampleForm.frx":0000
      Top             =   6120
      Width           =   7695
   End
   Begin VB.CommandButton Command22 
      Caption         =   "UnInstall Frame Hooker"
      Height          =   375
      Left            =   5880
      TabIndex        =   20
      Top             =   5400
      Width           =   2175
   End
   Begin VB.CommandButton Command21 
      Caption         =   "Install Frame Hooker"
      Height          =   375
      Left            =   5880
      TabIndex        =   18
      Top             =   4920
      Width           =   2175
   End
   Begin VB.CommandButton Command20 
      Caption         =   "Set Output"
      Height          =   375
      Left            =   2040
      TabIndex        =   17
      Top             =   5640
      Width           =   1695
   End
   Begin VB.CommandButton Command19 
      Caption         =   "Set IO Config"
      Height          =   375
      Left            =   120
      TabIndex        =   16
      Top             =   5640
      Width           =   1695
   End
   Begin VB.CommandButton Command18 
      Caption         =   "Set To Trigger Mode"
      Height          =   375
      Left            =   2040
      TabIndex        =   15
      Top             =   4920
      Width           =   1695
   End
   Begin VB.CommandButton Command17 
      Caption         =   "Set to Video Mode"
      Height          =   375
      Left            =   120
      TabIndex        =   14
      Top             =   4920
      Width           =   1695
   End
   Begin VB.CommandButton Command15 
      Caption         =   "Set Exposure Time"
      Height          =   375
      Left            =   3720
      TabIndex        =   13
      Top             =   3360
      Width           =   1815
   End
   Begin VB.CommandButton Command14 
      Caption         =   "Set Gains"
      Height          =   375
      Left            =   2040
      TabIndex        =   12
      Top             =   3360
      Width           =   1455
   End
   Begin VB.CommandButton Command13 
      Caption         =   "Set Resolution"
      Height          =   375
      Left            =   120
      TabIndex        =   11
      Top             =   3360
      Width           =   1695
   End
   Begin VB.CommandButton Command12 
      Caption         =   "Stop Grab"
      Height          =   375
      Left            =   1920
      TabIndex        =   10
      Top             =   2640
      Width           =   1575
   End
   Begin VB.CommandButton Command11 
      Caption         =   "Start Grab"
      Height          =   375
      Left            =   120
      TabIndex        =   9
      Top             =   2640
      Width           =   1695
   End
   Begin VB.CommandButton Command9 
      Caption         =   "Hide Control Panel"
      Height          =   375
      Left            =   6120
      TabIndex        =   8
      Top             =   2400
      Width           =   1935
   End
   Begin VB.CommandButton Command8 
      Caption         =   "Show Control Panel"
      Height          =   375
      Left            =   6120
      TabIndex        =   7
      Top             =   1920
      Width           =   1935
   End
   Begin VB.CommandButton Command7 
      Caption         =   "Stop Camera Engine"
      Height          =   375
      Left            =   4440
      TabIndex        =   6
      Top             =   120
      Width           =   1815
   End
   Begin VB.CommandButton Command6 
      Caption         =   "Start Camera Engine"
      Height          =   375
      Left            =   120
      TabIndex        =   5
      Top             =   2160
      Width           =   1695
   End
   Begin VB.CommandButton Command4 
      Caption         =   "Get Module No."
      Height          =   375
      Left            =   120
      TabIndex        =   2
      Top             =   1080
      Width           =   1575
   End
   Begin VB.CommandButton Command3 
      Caption         =   "Uninitialize Devices"
      Height          =   375
      Left            =   6360
      TabIndex        =   1
      Top             =   120
      Width           =   1695
   End
   Begin VB.CommandButton Command1 
      Caption         =   "Initialize Devices"
      Height          =   375
      Left            =   120
      TabIndex        =   0
      Top             =   120
      Width           =   1575
   End
   Begin VB.Label StatusLabel 
      Height          =   375
      Left            =   240
      TabIndex        =   19
      Top             =   8040
      Width           =   7815
   End
   Begin VB.Label SerialNoLabel 
      Caption         =   "Serial No.:"
      Height          =   375
      Left            =   4080
      TabIndex        =   4
      Top             =   1200
      Width           =   2175
   End
   Begin VB.Label ModuleNoLabel 
      Caption         =   "Module No.:"
      Height          =   255
      Left            =   1920
      TabIndex        =   3
      Top             =   1200
      Width           =   1935
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Dim DeviceHandle As Long

Private Sub Command1_Click()
    Dim TotalDevices As Long
    Dim MyStr As String

    TotalDevices = SSClassicUSB_InitDevice
    MyStr = ("There are " & TotalDevices & " devices.")
    MsgBox (MyStr)
End Sub


Private Sub Command11_Click()
    SSClassicUSB_StartFrameGrab &H88, 100000
End Sub

Private Sub Command12_Click()
    SSClassicUSB_StopFrameGrab &H88
End Sub

Private Sub Command13_Click()
    SSClassicUSB_SetCustomizedResolution 1, 640, 480, 0, 0
End Sub

Private Sub Command14_Click()
    SSClassicUSB_SetGains 1, 12, 12, 12
End Sub

Private Sub Command15_Click()
    SSClassicUSB_SetExposureTime 1, 1000  '1000x50us = 50ms
End Sub

Private Sub Command17_Click()
    SSClassicUSB_SetCameraWorkMode 1, NORMAL_MODE
End Sub

Private Sub Command18_Click()
    SSClassicUSB_SetCameraWorkMode 1, TRIGGER_MODE
End Sub

Private Sub Command19_Click()
    SSClassicUSB_SetGPIOConifg 1, &H3  'Pin1&2 as Input, 3&4 as Output
End Sub

Private Sub Command20_Click()
    Dim InputState As Byte
    SSClassicUSB_SetGPIOInOut 1, &H4, InputState   ' High for Pin3, Low for Pin4
End Sub

Private Sub Command21_Click()
    SSClassicUSB_InstallFrameHooker RAWDATA_IMAGE, AddressOf MyFrameHooker
End Sub

Private Sub Command22_Click()
    SSClassicUSB_InstallFrameHooker RAWDATA_IMAGE, 0
End Sub

Private Sub Command3_Click()
    SSClassicUSB_UnInitDevice
End Sub

Private Sub Command4_Click()
    Dim ModuleName As String
    Dim SerialName As String
    ModuleName = Space(32)
    SerialName = Space(32)
    SSClassicUSB_GetModuleNoSerialNo 1, ModuleName, SerialName
    ModuleNoLabel.Caption = "ModuleNo.: " & ModuleName
    SerialNoLabel.Caption = "SerialNo.: " & SerialName
End Sub

Private Sub Command6_Click()
    SSClassicUSB_AddDeviceToWorkingSet 1
    SSClassicUSB_StartCameraEngine hWnd, 8, 2, 0  '8bit camera
End Sub

Private Sub Command7_Click()
    SSClassicUSB_StopCameraEngine
End Sub

Private Sub Command8_Click()
    SSClassicUSB_ShowFactoryControlPanel 1, "123456"
End Sub

Private Sub Command9_Click()
    SSClassicUSB_HideFactoryControlPanel
End Sub

Private Sub Form_Load()
    FrameCount = 0
End Sub
