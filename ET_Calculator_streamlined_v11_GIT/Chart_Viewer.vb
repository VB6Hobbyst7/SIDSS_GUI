Imports System.Data.SQLite
Imports System.Data
Imports ET_Calculator_streamlined_v11_GIT.Graphs_Viewer

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

        'Get data from the SQL database using function Load_SQL_Table
        Dim main_table As DataTable
        main_table = Load_SQL_Table()

        For i = 0 To chkGraphOptions.CheckedItems.Count - 1
            Dim current_item As String
            current_item = chkGraphOptions.CheckedItems(i)
            chrtWaterBalance.Series.Add(current_item)
            chrtWaterBalance.Series(i).BorderWidth = 2
            chrtWaterBalance.ChartAreas(0).AxisX.Title = "GDD"
            chrtWaterBalance.ChartAreas(0).AxisY.Title = "Irrig, Precip, Di, Dmax (in/day)"
            chrtWaterBalance.ChartAreas(0).AxisY2.Title = "Kc, ETr, ETc (in/day)"

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
            chrtWaterBalance.Series(current_item).Points.DataBindXY(main_table.Rows, "GDD", main_table.Rows, current_item)
        Next
        chrtWaterBalance.Invalidate()

    End Sub


    Private Function Load_SQL_Table()
        'Connect to local SQLite database file. The text part is called connectionstring.
        Dim myConnection As New SQLiteConnection("Data Source=SIDSS_database.db; Version=3")
        'Open connection to the database file, within the program.
        myConnection.Open()

        'Select all columns from the database file to display in WPF datagrid.
        Dim cmd As New SQLiteCommand With {
            .Connection = myConnection,
            .CommandText = "Select * from WaterBalance_Table"
        }
        Dim reader As SQLiteDataReader = cmd.ExecuteReader
        Dim dt As New DataTable

        'Load SQL database values into the following datable.
        dt.Load(reader)

        'Close connection to the database.
        reader.Close()
        myConnection.Close()
        Return dt
    End Function

End Class