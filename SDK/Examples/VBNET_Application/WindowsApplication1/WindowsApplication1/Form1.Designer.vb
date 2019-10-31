<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Button1 = New System.Windows.Forms.Button
        Me.Button2 = New System.Windows.Forms.Button
        Me.ModuleNoLabel = New System.Windows.Forms.Label
        Me.SerialNoLabel = New System.Windows.Forms.Label
        Me.FrameInfoLabel = New System.Windows.Forms.Label
        Me.PBImage = New System.Windows.Forms.PictureBox
        CType(Me.PBImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(49, 196)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(89, 27)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "Start Camera"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(49, 229)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(89, 26)
        Me.Button2.TabIndex = 1
        Me.Button2.Text = "Stop Camera"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'ModuleNoLabel
        '
        Me.ModuleNoLabel.AutoSize = True
        Me.ModuleNoLabel.Location = New System.Drawing.Point(49, 38)
        Me.ModuleNoLabel.Name = "ModuleNoLabel"
        Me.ModuleNoLabel.Size = New System.Drawing.Size(39, 13)
        Me.ModuleNoLabel.TabIndex = 2
        Me.ModuleNoLabel.Text = "Label1"
        '
        'SerialNoLabel
        '
        Me.SerialNoLabel.AutoSize = True
        Me.SerialNoLabel.Location = New System.Drawing.Point(49, 64)
        Me.SerialNoLabel.Name = "SerialNoLabel"
        Me.SerialNoLabel.Size = New System.Drawing.Size(39, 13)
        Me.SerialNoLabel.TabIndex = 3
        Me.SerialNoLabel.Text = "Label2"
        '
        'FrameInfoLabel
        '
        Me.FrameInfoLabel.AutoSize = True
        Me.FrameInfoLabel.Location = New System.Drawing.Point(49, 155)
        Me.FrameInfoLabel.Name = "FrameInfoLabel"
        Me.FrameInfoLabel.Size = New System.Drawing.Size(39, 13)
        Me.FrameInfoLabel.TabIndex = 4
        Me.FrameInfoLabel.Text = "Label3"
        '
        'PBImage
        '
        Me.PBImage.Location = New System.Drawing.Point(194, 12)
        Me.PBImage.Name = "PBImage"
        Me.PBImage.Size = New System.Drawing.Size(482, 363)
        Me.PBImage.TabIndex = 5
        Me.PBImage.TabStop = False
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(691, 404)
        Me.Controls.Add(Me.PBImage)
        Me.Controls.Add(Me.FrameInfoLabel)
        Me.Controls.Add(Me.SerialNoLabel)
        Me.Controls.Add(Me.ModuleNoLabel)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Name = "frmMain"
        Me.Text = "Form1"
        CType(Me.PBImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents ModuleNoLabel As System.Windows.Forms.Label
    Friend WithEvents SerialNoLabel As System.Windows.Forms.Label
    Friend WithEvents FrameInfoLabel As System.Windows.Forms.Label
    Friend WithEvents PBImage As System.Windows.Forms.PictureBox

End Class
