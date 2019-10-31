Imports System
Imports System.Windows.Forms

Public Class frmMain
    Public WinHwnd As IntPtr

    Protected Overrides Sub WndProc(ByRef m As Message)
        Dim myBMP As Bitmap
        Dim pal As System.Drawing.Imaging.ColorPalette
        Dim i As Integer


        If m.Msg = &H401 Then
            FrameInfoLabel.Text = "Brightest pixel: " & BrightestPixel & vbCr & "Frames: " & FrameCount
            'We hard coded here, user might set the Bitmap size according to the ImageProperty.
            'To use the code directly, user should use 640x480 resolution.
            myBMP = New Bitmap(640, 480, 640, System.Drawing.Imaging.PixelFormat.Format8bppIndexed, _pImage)
            'For Format8bppIndexed, we have to construct a palette as following.
            pal = myBMP.Palette
            For i = 0 To pal.Entries.Length - 1
                pal.Entries(i) = Color.FromArgb(255, i, i, i)
            Next
            myBMP.Palette = pal
            PBImage.Image = myBMP  'Show the image
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
