Imports System.Data
Imports System.Reflection

'Imports ET_Calculator_streamlined_v11_GIT.Graphs_Viewer
Imports System.Text
Imports System.Windows.Forms.DataVisualization.Charting

Public Class Graphs_Viewer
    Dim start_date As String
    Dim end_date As String

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
        Dim row_count As Integer = main_table.Rows.Count
        start_date = main_table.Rows(0)("Date")
        end_date = main_table.Rows(row_count - 1)("Date")
        'chrtWaterBalance.Titles(0).Text = vbCrLf & start_date & " to " & end_date

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
                    chrtWaterBalance.Series(i).BorderDashStyle = ChartDashStyle.Dash
                    chrtWaterBalance.Series(i).ChartType = SeriesChartType.Line
                    chrtWaterBalance.ChartAreas(0).AxisY.IsReversed = True
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.Blue
                    chrtWaterBalance.Series(i)("PixelPointWidth") = "1"
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

                Case "Deficit_plot"
                    'curr_item_title = "Effective Irrig @ irrigation efficiency"
                    chrtWaterBalance.Series(i).ChartType = SeriesChartType.Line
                    chrtWaterBalance.ChartAreas(0).AxisY.IsReversed = True
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.DarkSlateBlue
                    'chrtWaterBalance.Series(i)("PixelPointWidth") = "10"

                Case "Kcr_calculated"
                    chrtWaterBalance.Series(i).ChartType = SeriesChartType.Line
                    chrtWaterBalance.Series(i).YAxisType = AxisType.Secondary
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.DarkGray

                Case "Kcr_plot"
                    chrtWaterBalance.Series(i).ChartType = SeriesChartType.Point
                    chrtWaterBalance.Series(i).YAxisType = AxisType.Secondary
                    chrtWaterBalance.Series(i).Color = System.Drawing.Color.Black

            End Select

            'Populate each line/cloumn with the corresponding data. Note: Checkbox items name the sql data table column names,
            'that Is how I am matching the data to the correct graph.
            Try
                chrtWaterBalance.Series(current_item).Points.DataBindXY(main_table.Rows, axis_type, main_table.Rows, current_item)
            Catch ex As Exception

            End Try
        Next
        chrtWaterBalance.Invalidate()
    End Sub

    ''' <summary>
    ''' Provides data from Selected_Daily_SMD table where the Tmax and Tmin are greater than 32F
    ''' </summary>
    ''' <returns></returns>
    Public Function Load_SQL_Table()
        ' Select all columns from the database file to display in WPF datagrid.
        ' Ignoring all dates where Tmax & Tmin = 32F, i.e. where GDD is Zero.
        ' I manually set Tmax and Tmin = 32F for the dates in future where no data is available.
        ' By doing so, I can remove no-data values from the datatable and just plot graph of only available data.
        Dim dt As New DataTable
        Dim Selected_Daily_SMD As Object
        Dim da As IDbDataAdapter = Nothing

        Using SIDSS_Context As New SIDSS_Entities
            Selected_Daily_SMD = (From selected_rows In SIDSS_Context.SMD_Daily Where selected_rows.Tmin > 0 And selected_rows.Tmax > 0 Select selected_rows).ToList
            dt = LINQToDataTable(Selected_Daily_SMD)
        End Using

        Return dt
    End Function

    ''' <summary>
    ''' This function takes the input from LINQ to the selected table in SIDSS database.
    ''' And returns the data as DataTable which can be used as an input data for plotting graph.
    ''' </summary>
    ''' <typeparam name="T">SMD_Daily</typeparam>
    ''' <param name="Selected_Daily_SMD"></param>
    ''' <returns></returns>
    Public Function LINQToDataTable(Of T)(ByVal Selected_Daily_SMD As IEnumerable(Of T)) As DataTable
        Dim dtReturn As DataTable = New DataTable()
        Dim oProps As PropertyInfo() = Nothing
        If Selected_Daily_SMD Is Nothing Then Return dtReturn

        For Each rec As T In Selected_Daily_SMD
            'Dim follow As will

            If oProps Is Nothing Then
                oProps = (CType(rec.[GetType](), Type)).GetProperties()

                For Each pi As PropertyInfo In oProps
                    Dim colType As Type = pi.PropertyType

                    If (colType.IsGenericType) AndAlso (colType.GetGenericTypeDefinition() = GetType(Nullable(Of))) Then
                        colType = colType.GetGenericArguments()(0)
                    End If

                    dtReturn.Columns.Add(New DataColumn(pi.Name, colType))
                Next
            End If

            Dim dr As DataRow = dtReturn.NewRow()

            For Each pi As PropertyInfo In oProps
                dr(pi.Name) = If(pi.GetValue(rec, Nothing) Is Nothing, DBNull.Value, pi.GetValue(rec, Nothing))
            Next

            dtReturn.Rows.Add(dr)
        Next

        Return dtReturn
    End Function

    Private Sub ToolStripTextBox1_Click(sender As Object, e As EventArgs) Handles ToolStripTextBox1.TextChanged

        Try
            chrtWaterBalance.Titles(0).Text = ToolStripTextBox1.Text & vbCrLf & "(" & start_date & " to " & end_date & ")"
        Catch ex As Exception

        End Try

    End Sub

    Private Sub Graphs_Viewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim form_access As New SIDSS_Planner_GUI.MainWindow
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