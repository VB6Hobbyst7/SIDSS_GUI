<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Graphs_Viewer
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Dim ChartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend1 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series1 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Series2 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Series3 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Series4 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Series5 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Series6 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Series7 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Title1 As System.Windows.Forms.DataVisualization.Charting.Title = New System.Windows.Forms.DataVisualization.Charting.Title()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveAsImageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ChooseGraphToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripTextBox1 = New System.Windows.Forms.ToolStripTextBox()
        Me.ToolStripComboBox1 = New System.Windows.Forms.ToolStripComboBox()
        Me.chkGraphOptions = New System.Windows.Forms.CheckedListBox()
        Me.chrtWaterBalance = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.SaveFileDialog_chrt = New System.Windows.Forms.SaveFileDialog()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.rtbxInfo = New System.Windows.Forms.RichTextBox()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.chrtWaterBalance, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.ChooseGraphToolStripMenuItem, Me.ToolStripTextBox1, Me.ToolStripComboBox1})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(800, 27)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SaveAsImageToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 23)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'SaveAsImageToolStripMenuItem
        '
        Me.SaveAsImageToolStripMenuItem.AccessibleName = "mnuExport"
        Me.SaveAsImageToolStripMenuItem.Name = "SaveAsImageToolStripMenuItem"
        Me.SaveAsImageToolStripMenuItem.Size = New System.Drawing.Size(150, 22)
        Me.SaveAsImageToolStripMenuItem.Text = "Export graph..."
        '
        'ChooseGraphToolStripMenuItem
        '
        Me.ChooseGraphToolStripMenuItem.Name = "ChooseGraphToolStripMenuItem"
        Me.ChooseGraphToolStripMenuItem.Size = New System.Drawing.Size(92, 23)
        Me.ChooseGraphToolStripMenuItem.Text = "Display Graph"
        '
        'ToolStripTextBox1
        '
        Me.ToolStripTextBox1.Name = "ToolStripTextBox1"
        Me.ToolStripTextBox1.Size = New System.Drawing.Size(200, 23)
        Me.ToolStripTextBox1.Text = "Type new graph heading here"
        '
        'ToolStripComboBox1
        '
        Me.ToolStripComboBox1.Items.AddRange(New Object() {"GDD", "Date"})
        Me.ToolStripComboBox1.Name = "ToolStripComboBox1"
        Me.ToolStripComboBox1.Size = New System.Drawing.Size(121, 23)
        Me.ToolStripComboBox1.Text = "Select X-axis type"
        '
        'chkGraphOptions
        '
        Me.chkGraphOptions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.chkGraphOptions.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkGraphOptions.FormattingEnabled = True
        Me.chkGraphOptions.Items.AddRange(New Object() {"Kc", "ETr", "ETc", "Di", "Dmax", "Precip", "Irrig"})
        Me.chkGraphOptions.Location = New System.Drawing.Point(0, 0)
        Me.chkGraphOptions.Margin = New System.Windows.Forms.Padding(10, 3, 3, 3)
        Me.chkGraphOptions.Name = "chkGraphOptions"
        Me.chkGraphOptions.Size = New System.Drawing.Size(164, 184)
        Me.chkGraphOptions.TabIndex = 2
        '
        'chrtWaterBalance
        '
        ChartArea1.Name = "ChartArea1"
        Me.chrtWaterBalance.ChartAreas.Add(ChartArea1)
        Me.chrtWaterBalance.Dock = System.Windows.Forms.DockStyle.Fill
        Legend1.Name = "Legend1"
        Me.chrtWaterBalance.Legends.Add(Legend1)
        Me.chrtWaterBalance.Location = New System.Drawing.Point(0, 0)
        Me.chrtWaterBalance.Name = "chrtWaterBalance"
        Me.chrtWaterBalance.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Pastel
        Series1.ChartArea = "ChartArea1"
        Series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
        Series1.Legend = "Legend1"
        Series1.Name = "Kc"
        Series2.ChartArea = "ChartArea1"
        Series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
        Series2.Legend = "Legend1"
        Series2.Name = "ETr"
        Series3.ChartArea = "ChartArea1"
        Series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
        Series3.Legend = "Legend1"
        Series3.Name = "ETc"
        Series4.ChartArea = "ChartArea1"
        Series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
        Series4.Legend = "Legend1"
        Series4.Name = "Di"
        Series5.ChartArea = "ChartArea1"
        Series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
        Series5.Legend = "Legend1"
        Series5.Name = "Dmax"
        Series6.ChartArea = "ChartArea1"
        Series6.Legend = "Legend1"
        Series6.Name = "Precip"
        Series7.ChartArea = "ChartArea1"
        Series7.Legend = "Legend1"
        Series7.Name = "Irrig"
        Me.chrtWaterBalance.Series.Add(Series1)
        Me.chrtWaterBalance.Series.Add(Series2)
        Me.chrtWaterBalance.Series.Add(Series3)
        Me.chrtWaterBalance.Series.Add(Series4)
        Me.chrtWaterBalance.Series.Add(Series5)
        Me.chrtWaterBalance.Series.Add(Series6)
        Me.chrtWaterBalance.Series.Add(Series7)
        Me.chrtWaterBalance.Size = New System.Drawing.Size(632, 423)
        Me.chrtWaterBalance.TabIndex = 3
        Me.chrtWaterBalance.Text = "Chart1"
        Title1.Font = New System.Drawing.Font("Palatino Linotype", 15.75!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Title1.Name = "Title1"
        Title1.Text = "Water Balance Graphs"
        Me.chrtWaterBalance.Titles.Add(Title1)
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 27)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.SplitContainer2)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.chrtWaterBalance)
        Me.SplitContainer1.Size = New System.Drawing.Size(800, 423)
        Me.SplitContainer1.SplitterDistance = 164
        Me.SplitContainer1.TabIndex = 4
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.chkGraphOptions)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.rtbxInfo)
        Me.SplitContainer2.Size = New System.Drawing.Size(164, 423)
        Me.SplitContainer2.SplitterDistance = 184
        Me.SplitContainer2.TabIndex = 0
        '
        'rtbxInfo
        '
        Me.rtbxInfo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.rtbxInfo.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rtbxInfo.Location = New System.Drawing.Point(0, 0)
        Me.rtbxInfo.Name = "rtbxInfo"
        Me.rtbxInfo.Size = New System.Drawing.Size(164, 235)
        Me.rtbxInfo.TabIndex = 0
        Me.rtbxInfo.Text = ""
        '
        'Graphs_Viewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "Graphs_Viewer"
        Me.Text = "Graphs_Viewer"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        CType(Me.chrtWaterBalance, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents MenuStrip1 As Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As Forms.ToolStripMenuItem
    Friend WithEvents SaveAsImageToolStripMenuItem As Forms.ToolStripMenuItem
    Friend WithEvents ChooseGraphToolStripMenuItem As Forms.ToolStripMenuItem
    'Friend WithEvents DataSet1 As DataSet1
    Friend WithEvents chkGraphOptions As Forms.CheckedListBox
    Friend WithEvents chrtWaterBalance As Forms.DataVisualization.Charting.Chart
    Friend WithEvents SaveFileDialog_chrt As Forms.SaveFileDialog
    Friend WithEvents ToolStripTextBox1 As Forms.ToolStripTextBox
    Friend WithEvents SplitContainer1 As Forms.SplitContainer
    Friend WithEvents SplitContainer2 As Forms.SplitContainer
    Friend WithEvents rtbxInfo As Forms.RichTextBox
    Friend WithEvents ToolStripComboBox1 As Forms.ToolStripComboBox
End Class
