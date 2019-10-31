Imports System
Imports System.Windows.Forms

Public Class frmMain
    Public WinHwnd As IntPtr

    Protected Overrides Sub WndProc(ByRef m As Message)
        Dim myBMP As Bitmap
        If m.Msg = &H401 Then
            FrameInfoLabel.Text = "Brightest pixel: " & BrightestPixel & vbCr & "Frames: " & FrameCount
            'We hard coded here, user might set the Bitmap size according to the ImageProperty.
            'This sample code assumes the camera is 3M color camera.
            myBMP = New Bitmap(2048, 1536, 3 * 2048, System.Drawing.Imaging.PixelFormat.Format24bppRgb, _pImage)

            PBImage.Image = myBMP
        Else
            MyBase.WndProc(m)
        End If
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        WinHwnd = Me.Handle
        CameraSTART()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        CameraSTOP()
        'MsgBox("Camera Stopped")
        FrameInfoLabel.Text = "Camera Stopped!"
    End Sub
End Class
