Imports System.Windows.Forms
Imports DotSpatial.Symbology
Imports DotSpatial.Controls
Imports DotSpatial.Data
Imports DotSpatial.Topology



Public Class MapWInGIS_Control
    Public Class Global_varaiables
        Public Shared raster_name As String = Nothing
    End Class
    Private Sub Button1_Click(sender As Object, e As EventArgs)

        OpenFileDialog1.DefaultExt = "tif"
        OpenFileDialog1.Multiselect = False
        OpenFileDialog1.Filter = "Tiff MS or single band|*.tif"
        OpenFileDialog1.FileName = ""
        'OpenFileDialog1.ShowDialog()

        If OpenFileDialog1.ShowDialog = DialogResult.OK Then
            'Dim raster_name As String
            Global_varaiables.raster_name = OpenFileDialog1.FileName
            Load_map(Global_varaiables.raster_name)
        Else
            MessageBox.Show("File not selected")
        End If

    End Sub
    Private Sub Load_map(ByVal raster_name As String)
        'AxMap1.Clear()
        'AxMap1.CursorMode = tkCursorMode.cmNone
        'AxMap1.SendMouseDown = True
        'AxMap1.AddLayerFromFilename(raster_name, tkFileOpenStrategy.fosAutoDetect, 1)
        Map1.ClearLayers()
        'Map1.AddLayer(DotSpatial.Controls.MapRasterLayer.OpenFile(raster_name))
        ''raster_data.Filename = raster_name
        'raster_data.FileType = Data.RasterFileType.GeoTiff

        ''raster_data.BandType = Data.ImageBandType.RGB
        'Map1.Layers.Add(raster_data)

    End Sub

    Private Sub Map1_Load(sender As Object, e As EventArgs) Handles Map1.Click
        Dim coordinates As New System.Drawing.Point
        Dim proj_coordinates As New System.Drawing.Point
        Map1.PointToClient(coordinates)
        ' Dim info As Object = tkCursorMode.cmIdentify
        'Dim projx, projy, minx, miny, maxx, maxy As Double
        'Dim resx, resy As Double
        Map1.PixelToProj(proj_coordinates)
        'Map1.PixelToProj(e.x, e.y, projx, projy)
        'Dim raster As Object = Legend1.
        'Dim e_vals As Object = e

    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Map1.AddLayer()
    End Sub

    Private Sub SpatialToolStrip1_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles SpatialToolStrip1.ItemClicked

    End Sub

End Class
