Imports System.Data.SQLite
Imports System.Data
Imports ET_Calculator_streamlined_v11_GIT.Graphs_Viewer
Imports ET_Calculator_streamlined_v11_GIT
Imports System.Text
Imports System
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Linq

Public Class Graphs_Viewer
    'Dim myConnection As New SQLiteConnection("Data Source=SIDSS_database.db; Version=3")
    'Dim cmd As New SQLiteCommand

    Private Sub ChooseGraphToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ChooseGraphToolStripMenuItem.Click
        'Clearing previously plotted lines from the chart area.
        chrtWaterBalance.Series.Clear()
        'Disabling X & Y axis lines as they were distracting and making hard to review the graph.
        chrtWaterBalance.ChartAreas(0).AxisX.MajorGrid.Enabled = False
        chrtWaterBalance.ChartAreas(0).AxisY.MajorGrid.Enabled = False
        chrtWaterBalance.ChartAreas(0).AxisX.MinorGrid.Enabled = False
        chrtWaterBalance.ChartAreas(0).AxisY.MinorGrid.Enabled = False
        chrtWaterBalance.ChartAreas(0).AxisY2.MajorGrid.LineDashStyle = Forms.DataVisualization.Charting.ChartDashStyle.Dot
        chrtWaterBalance.ChartAreas(0).AxisX.Name = "GDD"
        chrtWaterBalance.ChartAreas(0).AxisY.LabelAutoFitStyle = LabelAutoFitStyles.None
        chrtWaterBalance.ChartAreas(0).AxisY.TitleFont = New System.Drawing.Font("Aerial", 12, System.Drawing.FontStyle.Bold)
        'Get data from the SQL database using function Load_SQL_Table
        Dim main_table As DataTable
        main_table = Load_SQL_Table()
        Dim summary_dictionary As New Dictionary(Of String, Double)

        ' TODO: Add summary about the calculated resuts.
        summary_dictionary = Calculate_summary(main_table)

        Dim axis_type As String = Nothing
        If ToolStripComboBox1.SelectedIndex = 1 Then
            axis_type = "Date"
        Else
            axis_type = "GDD"
        End If

        For i = 0 To chkGraphOptions.CheckedItems.Count - 1
            Dim current_item As String
            current_item = chkGraphOptions.CheckedItems(i)
            chrtWaterBalance.Series.Add(current_item)
            chrtWaterBalance.Series(i).BorderWidth = 2
            chrtWaterBalance.ChartAreas(0).AxisX.Title = axis_type
            chrtWaterBalance.ChartAreas(0).AxisY.Title = "Irrig, Precip, Di, Dmax (in/day)"
            chrtWaterBalance.ChartAreas(0).AxisY2.Title = "Kc, ETr, ETc (in/day)"
            'chrtWaterBalance.ChartAreas(0).AxisY.LabelStyle.Font.

            'Customize each chart line/graph according to its data range e.g. Kc, ETr, ETc are small numbers so they are plottd on
            'Secondary axis, whereas irrigation, precip, MAD's depth, current deplition level etc. on main Y axis.


            Select Case current_item
                Case "Kc"
                    chrtWaterBalance.Series(i).ChartType = Forms.DataVisualization.Charting.SeriesChartType.Line
                    chrtWaterBalance.Series(i).YAxisType = Forms.DataVisualization.Charting.AxisType.Secondary
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.Black
                Case "ETr"
                    chrtWaterBalance.Series(i).ChartType = Forms.DataVisualization.Charting.SeriesChartType.Line
                    chrtWaterBalance.Series(i).YAxisType = Forms.DataVisualization.Charting.AxisType.Secondary
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.IndianRed
                Case "ETc"
                    chrtWaterBalance.Series(i).ChartType = Forms.DataVisualization.Charting.SeriesChartType.Line
                    chrtWaterBalance.Series(i).YAxisType = Forms.DataVisualization.Charting.AxisType.Secondary
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.DarkRed
                Case "Di"
                    chrtWaterBalance.Series(i).ChartType = Forms.DataVisualization.Charting.SeriesChartType.Line
                    chrtWaterBalance.ChartAreas(0).AxisY.IsReversed = True
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.BlueViolet
                Case "Dmax"
                    chrtWaterBalance.Series(i).ChartType = Forms.DataVisualization.Charting.SeriesChartType.Line
                    chrtWaterBalance.ChartAreas(0).AxisY.IsReversed = True
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.Red
                Case "Precip"
                    chrtWaterBalance.Series(i).ChartType = Forms.DataVisualization.Charting.SeriesChartType.Column
                    chrtWaterBalance.ChartAreas(0).AxisY.IsReversed = True
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.Blue
                Case "Irrig"
                    chrtWaterBalance.Series(i).ChartType = Forms.DataVisualization.Charting.SeriesChartType.Column
                    chrtWaterBalance.ChartAreas(0).AxisY.IsReversed = True
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.ForestGreen
            End Select

            'Populate each line/cloumn with the corresponding data. Note: Checkbox items name the sql data table column names,
            'that Is how I am matching the data to the correct graph.
            chrtWaterBalance.Series(current_item).Points.DataBindXY(main_table.Rows, axis_type, main_table.Rows, current_item)
        Next
        chrtWaterBalance.Invalidate()

    End Sub


    Private Function Load_SQL_Table()
        'Connect to local SQLite database file. The text part is called connectionstring.
        Dim myConnection As New SQLiteConnection("Data Source=SIDSS_database.db; Version=3")
        'Open connection to the database file, within the program.
        If myConnection.State = ConnectionState.Open Then
            myConnection.Close()
        End If
        myConnection.Open()

        'Select all columns from the database file to display in WPF datagrid.
        ' Ignoring all dates where Tmax & Tmin = 32F, i.e. where GDD is Zero. 
        ' I manually set Tmax and Tmin = 32F for the dates in future where no data is available.
        ' By doing so, I can remove no-data values from the datatable and just plot graph of only available data.

        Dim cmd As New SQLiteCommand
        cmd.Connection = myConnection
        cmd.CommandText = "Select * from WaterBalance_Table WHERE Tmax>32 AND Tmin>32;"

        Dim reader As SQLiteDataReader = cmd.ExecuteReader
        Dim dt As New DataTable

        'Load SQL database values into the following datable.
        dt.Load(reader)
        'Dim dt_filtered As New DataTable
        'Dim data_view As DataView
        'data_rows = data_view.("Tmin=32")
        'For Each row In data_rows

        'Next
        'Close connection to the database.
        reader.Close()
        myConnection.Close()
        Return dt
    End Function

    Private Sub ToolStripTextBox1_Click(sender As Object, e As EventArgs) Handles ToolStripTextBox1.TextChanged
        Try
            chrtWaterBalance.Titles(0).Text = ToolStripTextBox1.Text
        Catch ex As Exception

        End Try


    End Sub

    Private Sub Graphs_Viewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim form_access As New ET_Calculator_streamlined_v11_GIT.MainWindow
        Dim string_builder As New StringBuilder
        string_builder.AppendLine("Where:-")
        string_builder.AppendLine("Precip. & Irrig. are measured in inches.")
        string_builder.AppendLine("ETr = Reference alfalfa ET, inches")
        string_builder.AppendLine("Kc = Crop coefficient ranges from 0 - 1")
        string_builder.AppendLine("ETc = Crop potential ET, inches")
        string_builder.AppendLine("Dmax = Maximum allowed deficit based on the growth stage, Di should not go below this value.")
        string_builder.AppendLine("Di = Deficit at day (i), Di = 0 at field capacity and if deficit goes below max. allowed deficit, then the growth might be effected.")
        string_builder.AppendLine("Management allowd deficit = " & form_access.tbxMAD_perecnt.Text & "%")

        string_builder.AppendLine("Current root zone depth = xyz")
        string_builder.AppendLine("Total precipitation = abc")
        string_builder.AppendLine("Total irrigation = lmn")
        string_builder.AppendLine("Total ETc till now = uvw")
        string_builder.AppendLine("GDD at maturity = abc")
        string_builder.AppendLine("GDD at harvesing = nnn")
        string_builder.AppendLine("Total ETc till now = uvw")

        rtbxInfo.Text = string_builder.ToString

    End Sub

    Private Sub SaveAsImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveAsImageToolStripMenuItem.Click
        SaveFileDialog_chrt.Filter = "Png Images (*.png*)|*.png"
        If SaveFileDialog_chrt.ShowDialog = Windows.Forms.DialogResult.OK _
       Then
            chrtWaterBalance.SaveImage(SaveFileDialog_chrt.FileName, format:=Forms.DataVisualization.Charting.ChartImageFormat.Png)
        End If
    End Sub

    Private Function Calculate_summary(ByVal water_balance_table As DataTable)

        Return Nothing
    End Function


End Class