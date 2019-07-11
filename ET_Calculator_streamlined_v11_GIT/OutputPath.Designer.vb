<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OutputPath
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnOutputPathOK = New System.Windows.Forms.Button()
        Me.btnOutputPathCancel = New System.Windows.Forms.Button()
        Me.tbxOutputPath = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'btnOutputPathOK
        '
        Me.btnOutputPathOK.Location = New System.Drawing.Point(3, 96)
        Me.btnOutputPathOK.Name = "btnOutputPathOK"
        Me.btnOutputPathOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOutputPathOK.TabIndex = 0
        Me.btnOutputPathOK.Text = "OK"
        Me.btnOutputPathOK.UseVisualStyleBackColor = True
        '
        'btnOutputPathCancel
        '
        Me.btnOutputPathCancel.Location = New System.Drawing.Point(292, 96)
        Me.btnOutputPathCancel.Name = "btnOutputPathCancel"
        Me.btnOutputPathCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnOutputPathCancel.TabIndex = 1
        Me.btnOutputPathCancel.Text = "Cancel"
        Me.btnOutputPathCancel.UseVisualStyleBackColor = True
        '
        'tbxOutputPath
        '
        Me.tbxOutputPath.Location = New System.Drawing.Point(0, 59)
        Me.tbxOutputPath.Name = "tbxOutputPath"
        Me.tbxOutputPath.Size = New System.Drawing.Size(367, 20)
        Me.tbxOutputPath.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(3, 43)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(101, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Select Output folder"
        '
        'OutputPath
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.tbxOutputPath)
        Me.Controls.Add(Me.btnOutputPathCancel)
        Me.Controls.Add(Me.btnOutputPathOK)
        Me.Name = "OutputPath"
        Me.Size = New System.Drawing.Size(373, 173)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnOutputPathOK As Forms.Button
    Friend WithEvents btnOutputPathCancel As Forms.Button
    Friend WithEvents tbxOutputPath As Forms.TextBox
    Friend WithEvents Label1 As Forms.Label
End Class
