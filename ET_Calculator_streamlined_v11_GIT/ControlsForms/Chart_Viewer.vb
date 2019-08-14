Imports System.Data.SQLite
Imports System.Data
'Imports ET_Calculator_streamlined_v11_GIT.Graphs_Viewer
Imports ET_Calculator_streamlined_v11_GIT
Imports ET_Calculator_streamlined_v11_GIT.MainWindow
Imports System.Text
Imports System
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Linq
Imports System.Windows.Forms

Public Class Graphs_Viewer
    'Dim myConnection As New SQLiteConnection("Data Source=SIDSS_database.db; Version=3")
    'Dim cmd As New SQLiteCommand

    Private Sub ChooseGraphToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Load_Chart()

    End Sub

    Private Sub Load_Chart()
        'Clearing previously plotted lines from the chart area.
        chrtWaterBalance.Series.Clear()
        'Disabling X & Y axis lines as they were distracting and making hard to review the graph.
        chrtWaterBalance.ChartAreas(0).AxisX.MajorGrid.Enabled = False
        chrtWaterBalance.ChartAreas(0).AxisY.MajorGrid.Enabled = False
        chrtWaterBalance.ChartAreas(0).AxisX.MinorGrid.Enabled = False
        chrtWaterBalance.ChartAreas(0).AxisY.MinorGrid.Enabled = False
        chrtWaterBalance.ChartAreas(0).AxisY2.MajorGrid.LineDashStyle = ChartDashStyle.Dot
        chrtWaterBalance.ChartAreas(0).AxisX.Name = "GDD"
        chrtWaterBalance.ChartAreas(0).AxisY.LabelAutoFitStyle = LabelAutoFitStyles.None
        chrtWaterBalance.ChartAreas(0).AxisY.TitleFont = New System.Drawing.Font("Aerial", 12, System.Drawing.FontStyle.Bold)
        chrtWaterBalance.ChartAreas(0).AxisX.TitleFont = New System.Drawing.Font("Aerial", 12, System.Drawing.FontStyle.Bold)
        chrtWaterBalance.ChartAreas(0).AxisY2.TitleFont = New System.Drawing.Font("Aerial", 12, System.Drawing.FontStyle.Bold)
        'Get data from the SQL database using function Load_SQL_Table
        Dim main_table As DataTable
        main_table = Load_SQL_Table()
        Dim summary_dictionary As New Dictionary(Of String, Double)

        ' TODO: Add summary about the calculated resuts.
        summary_dictionary = Calculate_summary(main_table)

        Dim axis_type As String = Nothing
        If ToolStripComboBox1.SelectedItem = "GDD" Then
            axis_type = "GDD"
        Else
            axis_type = "Date"
        End If

        For i = 0 To chkGraphOptions.CheckedItems.Count - 1
            Dim current_item As String
            current_item = chkGraphOptions.CheckedItems(i)
            chrtWaterBalance.Series.Add(current_item)
            chrtWaterBalance.Series(i).BorderWidth = 2
            chrtWaterBalance.ChartAreas(0).AxisX.Title = axis_type
            chrtWaterBalance.ChartAreas(0).AxisY.Title = "Eff_Irrig, Eff_Precip, Di, Dmax (in/day)"
            chrtWaterBalance.ChartAreas(0).AxisY2.Title = "Kc, ETr, ETc (in/day)"
            'chrtWaterBalance.ChartAreas(0).AxisY.LabelStyle.Font.

            'Customize each chart line/graph according to its data range e.g. Kc, ETr, ETc are small numbers so they are plottd on
            'Secondary axis, whereas irrigation, precip, MAD's depth, current deplition level etc. on main Y axis.

            Select Case current_item
                Case "Kc"
                    'curr_item_title = "Kc"
                    chrtWaterBalance.Series(i).ChartType = SeriesChartType.Line
                    chrtWaterBalance.Series(i).YAxisType = AxisType.Secondary
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.Black
                Case "ETr"
                    'curr_item_title = "ETr"
                    chrtWaterBalance.Series(i).ChartType = SeriesChartType.Line
                    chrtWaterBalance.Series(i).YAxisType = AxisType.Secondary
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.IndianRed
                Case "ETc"
                    'curr_item_title = "ETc"
                    chrtWaterBalance.Series(i).ChartType = SeriesChartType.Line
                    chrtWaterBalance.Series(i).YAxisType = AxisType.Secondary
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.DarkRed
                Case "Di"
                    'curr_item_title = "Deficit, current"
                    chrtWaterBalance.Series(i).ChartType = SeriesChartType.Line
                    chrtWaterBalance.ChartAreas(0).AxisY.IsReversed = True
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.BlueViolet
                Case "Dmax"
                    'curr_item_title = "Maximum allowed Deficit"
                    chrtWaterBalance.Series(i).ChartType = SeriesChartType.Line
                    chrtWaterBalance.ChartAreas(0).AxisY.IsReversed = True
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.Red
                Case "Eff__Precip"
                    'curr_item_title = "Effective Precip after surface runoff"
                    chrtWaterBalance.Series(i).ChartType = SeriesChartType.Column
                    chrtWaterBalance.Series(i)("PixelPointWidth") = "10"
                    chrtWaterBalance.ChartAreas(0).AxisY.IsReversed = True
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.ForestGreen
                Case "Eff__Irrig"
                    'curr_item_title = "Effective Irrig @ irrigation efficiency"
                    chrtWaterBalance.Series(i).ChartType = SeriesChartType.Column
                    chrtWaterBalance.ChartAreas(0).AxisY.IsReversed = True
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.Blue
                    chrtWaterBalance.Series(i)("PixelPointWidth") = "10"
            End Select

            'Populate each line/cloumn with the corresponding data. Note: Checkbox items name the sql data table column names,
            'that Is how I am matching the data to the correct graph.
            chrtWaterBalance.Series(current_item).Points.DataBindXY(main_table.Rows, axis_type, main_table.Rows, current_item)
        Next
        chrtWaterBalance.Invalidate()
    End Sub

    Private Function Load_SQL_Table()
        'Using database_context As New SIDSS_Entities
        '    Dim smd_table = database_context.SMD_Daily.ToArray()
        '    Dim water_table As New DataTable


        '    Return water_table
        'End Using


        'Connect to local SQLite database file. The text part is called connectionstring.
        Dim myConnection As New SQLiteConnection("Data Source=SIDSS_Entity_database.db; Version=3")
        'Open connection to the database file, within the program.
        If myConnection.State = ConnectionState.Open Then
            myConnection.Close()
        End If
        myConnection.Open()

        ' Select all columns from the database file to display in WPF datagrid.
        ' Ignoring all dates where Tmax & Tmin = 32F, i.e. where GDD is Zero. 
        ' I manually set Tmax and Tmin = 32F for the dates in future where no data is available.
        ' By doing so, I can remove no-data values from the datatable and just plot graph of only available data.

        Dim cmd As New SQLiteCommand
        cmd.Connection = myConnection
        cmd.CommandText = "Select * from SMD_Daily WHERE Tmax>32 AND Tmin>32;"
        'cmd.CommandText = "Select * from SMD_Daily;"

        Dim reader As SQLiteDataReader = cmd.ExecuteReader
        Dim dt As New DataTable

        'Load SQL database values into the following datable.
        dt.Load(reader)

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
        Dim item_count As Integer = chkGraphOptions.Items.Count
        For i = 0 To item_count - 1
            If chkGraphOptions.Items(i).ToString = "ETr" Then
                chkGraphOptions.SetItemChecked(i, False)
            End If
            If chkGraphOptions.Items(i).ToString = "ETc" Then
                chkGraphOptions.SetItemChecked(i, False)
            End If
        Next

        ToolStripComboBox1.SelectedItem = "GDD"
        Load_Chart()

    End Sub

    Private Sub SaveAsImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveAsImageToolStripMenuItem.Click
        SaveFileDialog_chrt.Filter = "Png Images (*.png*)|*.png"
        If SaveFileDialog_chrt.ShowDialog = Windows.Forms.DialogResult.OK _
       Then
            chrtWaterBalance.SaveImage(SaveFileDialog_chrt.FileName, format:=ChartImageFormat.Png)
        End If
    End Sub

    Private Function Calculate_summary(ByVal water_balance_table As DataTable)

        Return Nothing
    End Function

    Private Sub ToolStripComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripComboBox1.SelectedIndexChanged
        Load_Chart()
    End Sub


End Class