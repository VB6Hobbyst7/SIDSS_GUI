Imports MapWinGIS
Imports AxMapWinGIS
Imports DotSpatial
Imports System.Windows.Forms

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
        Map1.AddLayer(DotSpatial.Controls.MapRasterLayer.OpenFile(raster_name))
        ''raster_data.Filename = raster_name
        'raster_data.FileType = Data.RasterFileType.GeoTiff

        ''raster_data.BandType = Data.ImageBandType.RGB
        'Map1.Layers.Add(raster_data)

    End Sub

    'Private Sub AxMap1_MouseDownEvent(sender As Object, e As _DMapEvents_MouseDownEvent) Handles AxMap1.MouseDownEvent
    '    Dim coordinates As New System.Drawing.Point
    '    AxMap1.PointToClient(coordinates)
    '    Dim info As Object = tkCursorMode.cmIdentify
    '    Dim projx, projy, minx, miny, maxx, maxy As Double
    '    Dim resx, resy As Double
    '    AxMap1.PixelToProj(e.x, e.y, projx, projy)

    '    Dim raster As New Image With {
    '        .UseRgbBandMapping = True
    '    }
    '    raster = AxMap1.get_Image(layerHandle:=AxMap1.get_LayerHandle(0))
    '    Dim tif_name As String = Nothing
    '    tif_name = IO.Path.GetFileName(Global_varaiables.raster_name)

    '    Dim no_of_bands = raster.NoBands

    '    Dim extnt As New Extents
    '    extnt = raster.Extents
    '    minx = extnt.xMin
    '    miny = extnt.yMin
    '    maxx = extnt.xMax
    '    maxy = extnt.yMax
    '    resx = raster.OriginalDX
    '    resy = raster.OriginalDY

    '    Dim col, row, pix_size As Integer
    '    Dim min, max, stdev, count, avg_pix_val As Double
    '    col = Int((projx - minx) / resx)
    '    row = Int((maxy - projy) / resy)
    '    Dim avg_area As Integer = Convert.ToInt16(TextBox1.Text)
    '    If avg_area < 3 Then
    '        MessageBox.Show("Averaging area should be greater than 3x3")
    '        avg_area = 3
    '    End If
    '    pix_size = Math.Round((avg_area - 1) / 2, 0)

    '    raster.ActiveBand.ComputeLocalStatistics(col, row, pix_size, min, max, avg_pix_val, stdev, count)

    '    Label1.Text = (tif_name & vbCrLf & "Average pixel value=" & Math.Round(avg_pix_val, 2) & "; X=" & Math.Round(projx, 2) & "; Y=" & Math.Round(projy, 2) & "; (averaging " & count ^ 0.5 & " x " & count ^ 0.5 & " pixels)")

    'End Sub

End Class
