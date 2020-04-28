<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MapWInGIS_Control
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MapWInGIS_Control))
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.SpatialStatusStrip1 = New DotSpatial.Controls.SpatialStatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripProgressBar1 = New System.Windows.Forms.ToolStripProgressBar()
        Me.SpatialToolStrip1 = New DotSpatial.Controls.SpatialToolStrip()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripTextBox1 = New System.Windows.Forms.ToolStripTextBox()
        Me.ToolStripLabel2 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
        Me.Map1 = New DotSpatial.Controls.Map()
        Me.Legend1 = New DotSpatial.Controls.Legend()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.TabMain = New System.Windows.Forms.TabControl()
        Me.tabLegend = New System.Windows.Forms.TabPage()
        Me.TabToolbox = New System.Windows.Forms.TabPage()
        Me.AppManager1 = New DotSpatial.Controls.AppManager()
        Me.SpatialHeaderControl1 = New DotSpatial.Controls.SpatialHeaderControl()
        Me.SpatialStatusStrip1.SuspendLayout()
        Me.SpatialToolStrip1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TabMain.SuspendLayout()
        Me.tabLegend.SuspendLayout()
        CType(Me.SpatialHeaderControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'SpatialStatusStrip1
        '
        Me.SpatialStatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1, Me.ToolStripProgressBar1})
        Me.SpatialStatusStrip1.Location = New System.Drawing.Point(0, 457)
        Me.SpatialStatusStrip1.Name = "SpatialStatusStrip1"
        Me.SpatialStatusStrip1.ProgressBar = Me.ToolStripProgressBar1
        Me.SpatialStatusStrip1.ProgressLabel = Me.ToolStripStatusLabel1
        Me.SpatialStatusStrip1.Size = New System.Drawing.Size(741, 22)
        Me.SpatialStatusStrip1.TabIndex = 5
        Me.SpatialStatusStrip1.Text = "SpatialStatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(624, 17)
        Me.ToolStripStatusLabel1.Spring = True
        Me.ToolStripStatusLabel1.Text = "ready"
        '
        'ToolStripProgressBar1
        '
        Me.ToolStripProgressBar1.Name = "ToolStripProgressBar1"
        Me.ToolStripProgressBar1.Size = New System.Drawing.Size(100, 16)
        '
        'SpatialToolStrip1
        '
        Me.SpatialToolStrip1.ApplicationManager = Nothing
        Me.SpatialToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1, Me.ToolStripTextBox1, Me.ToolStripLabel2, Me.ToolStripSeparator1, Me.ToolStripButton1})
        Me.SpatialToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.SpatialToolStrip1.Map = Me.Map1
        Me.SpatialToolStrip1.Name = "SpatialToolStrip1"
        Me.SpatialToolStrip1.Size = New System.Drawing.Size(741, 25)
        Me.SpatialToolStrip1.TabIndex = 6
        Me.SpatialToolStrip1.Text = "SpatialToolStrip1"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(87, 22)
        Me.ToolStripLabel1.Text = "Pixel size N x N"
        '
        'ToolStripTextBox1
        '
        Me.ToolStripTextBox1.Name = "ToolStripTextBox1"
        Me.ToolStripTextBox1.Size = New System.Drawing.Size(35, 25)
        Me.ToolStripTextBox1.Text = "3"
        '
        'ToolStripLabel2
        '
        Me.ToolStripLabel2.Name = "ToolStripLabel2"
        Me.ToolStripLabel2.Size = New System.Drawing.Size(81, 22)
        Me.ToolStripLabel2.Text = "Average value"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton1.Image = CType(resources.GetObject("ToolStripButton1.Image"), System.Drawing.Image)
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton1.Text = "ToolStripButton1"
        '
        'Map1
        '
        Me.Map1.AllowDrop = True
        Me.Map1.BackColor = System.Drawing.Color.Black
        Me.Map1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Map1.CollectAfterDraw = False
        Me.Map1.CollisionDetection = False
        Me.Map1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Map1.ExtendBuffer = False
        Me.Map1.FunctionMode = DotSpatial.Controls.FunctionMode.None
        Me.Map1.IsBusy = False
        Me.Map1.IsZoomedToMaxExtent = True
        Me.Map1.Legend = Me.Legend1
        Me.Map1.Location = New System.Drawing.Point(0, 0)
        Me.Map1.Name = "Map1"
        Me.Map1.ProgressHandler = Me.SpatialStatusStrip1
        Me.Map1.ProjectionModeDefine = DotSpatial.Controls.ActionMode.PromptOnce
        Me.Map1.ProjectionModeReproject = DotSpatial.Controls.ActionMode.Prompt
        Me.Map1.RedrawLayersWhileResizing = False
        Me.Map1.SelectionEnabled = True
        Me.Map1.Size = New System.Drawing.Size(606, 432)
        Me.Map1.TabIndex = 0
        Me.Map1.ZoomOutFartherThanMaxExtent = False
        '
        'Legend1
        '
        Me.Legend1.BackColor = System.Drawing.Color.White
        Me.Legend1.ControlRectangle = New System.Drawing.Rectangle(0, 0, 117, 400)
        Me.Legend1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Legend1.DocumentRectangle = New System.Drawing.Rectangle(0, 0, 187, 428)
        Me.Legend1.HorizontalScrollEnabled = True
        Me.Legend1.Indentation = 30
        Me.Legend1.IsInitialized = False
        Me.Legend1.Location = New System.Drawing.Point(3, 3)
        Me.Legend1.MinimumSize = New System.Drawing.Size(5, 5)
        Me.Legend1.Name = "Legend1"
        Me.Legend1.ProgressHandler = Me.SpatialStatusStrip1
        Me.Legend1.ResetOnResize = False
        Me.Legend1.SelectionFontColor = System.Drawing.Color.Black
        Me.Legend1.SelectionHighlight = System.Drawing.Color.FromArgb(CType(CType(215, Byte), Integer), CType(CType(238, Byte), Integer), CType(CType(252, Byte), Integer))
        Me.Legend1.Size = New System.Drawing.Size(117, 400)
        Me.Legend1.TabIndex = 0
        Me.Legend1.Text = "Legend1"
        Me.Legend1.VerticalScrollEnabled = True
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 25)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.TabMain)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.Map1)
        Me.SplitContainer1.Size = New System.Drawing.Size(741, 432)
        Me.SplitContainer1.SplitterDistance = 131
        Me.SplitContainer1.TabIndex = 7
        '
        'TabMain
        '
        Me.TabMain.Controls.Add(Me.tabLegend)
        Me.TabMain.Controls.Add(Me.TabToolbox)
        Me.TabMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabMain.Location = New System.Drawing.Point(0, 0)
        Me.TabMain.Name = "TabMain"
        Me.TabMain.SelectedIndex = 0
        Me.TabMain.Size = New System.Drawing.Size(131, 432)
        Me.TabMain.TabIndex = 0
        '
        'tabLegend
        '
        Me.tabLegend.Controls.Add(Me.Legend1)
        Me.tabLegend.Location = New System.Drawing.Point(4, 22)
        Me.tabLegend.Name = "tabLegend"
        Me.tabLegend.Padding = New System.Windows.Forms.Padding(3)
        Me.tabLegend.Size = New System.Drawing.Size(123, 406)
        Me.tabLegend.TabIndex = 0
        Me.tabLegend.Text = "Legend"
        Me.tabLegend.UseVisualStyleBackColor = True
        '
        'TabToolbox
        '
        Me.TabToolbox.Location = New System.Drawing.Point(4, 22)
        Me.TabToolbox.Name = "TabToolbox"
        Me.TabToolbox.Padding = New System.Windows.Forms.Padding(3)
        Me.TabToolbox.Size = New System.Drawing.Size(123, 406)
        Me.TabToolbox.TabIndex = 1
        Me.TabToolbox.Text = "Toolbox"
        Me.TabToolbox.UseVisualStyleBackColor = True
        '
        'AppManager1
        '
        Me.AppManager1.Directories = CType(resources.GetObject("AppManager1.Directories"), System.Collections.Generic.List(Of String))
        Me.AppManager1.DockManager = Nothing
        Me.AppManager1.HeaderControl = Nothing
        Me.AppManager1.Legend = Me.Legend1
        Me.AppManager1.Map = Me.Map1
        Me.AppManager1.ProgressHandler = Me.SpatialStatusStrip1
        Me.AppManager1.ShowExtensionsDialogMode = DotSpatial.Controls.ShowExtensionsDialogMode.[Default]
        '
        'SpatialHeaderControl1
        '
        Me.SpatialHeaderControl1.ApplicationManager = Me.AppManager1
        Me.SpatialHeaderControl1.MenuStrip = Nothing
        Me.SpatialHeaderControl1.ToolbarsContainer = Nothing
        '
        'MapWInGIS_Control
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.SpatialToolStrip1)
        Me.Controls.Add(Me.SpatialStatusStrip1)
        Me.Name = "MapWInGIS_Control"
        Me.Size = New System.Drawing.Size(741, 479)
        Me.SpatialStatusStrip1.ResumeLayout(False)
        Me.SpatialStatusStrip1.PerformLayout()
        Me.SpatialToolStrip1.ResumeLayout(False)
        Me.SpatialToolStrip1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.TabMain.ResumeLayout(False)
        Me.tabLegend.ResumeLayout(False)
        CType(Me.SpatialHeaderControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents OpenFileDialog1 As Forms.OpenFileDialog
    Friend WithEvents SpatialStatusStrip1 As DotSpatial.Controls.SpatialStatusStrip
    Friend WithEvents ToolStripStatusLabel1 As Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripProgressBar1 As Forms.ToolStripProgressBar
    Friend WithEvents SpatialToolStrip1 As DotSpatial.Controls.SpatialToolStrip
    Friend WithEvents SplitContainer1 As Forms.SplitContainer
    Friend WithEvents TabMain As Forms.TabControl
    Friend WithEvents tabLegend As Forms.TabPage
    Friend WithEvents Legend1 As DotSpatial.Controls.Legend
    Friend WithEvents TabToolbox As Forms.TabPage
    Friend WithEvents Map1 As DotSpatial.Controls.Map
    Friend WithEvents AppManager1 As DotSpatial.Controls.AppManager
    Friend WithEvents ToolStripLabel1 As Forms.ToolStripLabel
    Friend WithEvents ToolStripTextBox1 As Forms.ToolStripTextBox
    Friend WithEvents ToolStripSeparator1 As Forms.ToolStripSeparator
    Friend WithEvents ToolStripLabel2 As Forms.ToolStripLabel
    Friend WithEvents ToolStripButton1 As Forms.ToolStripButton
    Private WithEvents SpatialHeaderControl1 As DotSpatial.Controls.SpatialHeaderControl
End Class
