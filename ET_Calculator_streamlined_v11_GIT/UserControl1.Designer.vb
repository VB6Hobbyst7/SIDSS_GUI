<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UserControl1
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UserControl1))
        Me.AxMap1 = New AxMapWinGIS.AxMap()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        CType(Me.AxMap1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'AxMap1
        '
        Me.AxMap1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.AxMap1.Enabled = True
        Me.AxMap1.Location = New System.Drawing.Point(0, 63)
        Me.AxMap1.Margin = New System.Windows.Forms.Padding(20)
        Me.AxMap1.Name = "AxMap1"
        Me.AxMap1.OcxState = CType(resources.GetObject("AxMap1.OcxState"), System.Windows.Forms.AxHost.State)
        Me.AxMap1.Padding = New System.Windows.Forms.Padding(10)
        Me.AxMap1.Size = New System.Drawing.Size(741, 416)
        Me.AxMap1.TabIndex = 0
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(10, 10)
        Me.Button1.Margin = New System.Windows.Forms.Padding(10)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "Open TIF"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(184, 15)
        Me.Label1.Margin = New System.Windows.Forms.Padding(10, 15, 0, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(240, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Select pixel averaging size (odd no. e.g. 3, 5,..25)"
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel1.Controls.Add(Me.Button1)
        Me.FlowLayoutPanel1.Controls.Add(Me.TextBox1)
        Me.FlowLayoutPanel1.Controls.Add(Me.Label1)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(3, 0)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(738, 57)
        Me.FlowLayoutPanel1.TabIndex = 4
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(98, 12)
        Me.TextBox1.Margin = New System.Windows.Forms.Padding(3, 12, 3, 3)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(73, 20)
        Me.TextBox1.TabIndex = 3
        Me.TextBox1.Text = "3"
        '
        'UserControl1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Controls.Add(Me.AxMap1)
        Me.Name = "UserControl1"
        Me.Size = New System.Drawing.Size(741, 479)
        CType(Me.AxMap1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents AxMap1 As AxMapWinGIS.AxMap
    Friend WithEvents Button1 As Forms.Button
    Friend WithEvents Label1 As Forms.Label
    Friend WithEvents OpenFileDialog1 As Forms.OpenFileDialog
    Friend WithEvents FlowLayoutPanel1 As Forms.FlowLayoutPanel
    Friend WithEvents TextBox1 As Forms.TextBox
End Class
