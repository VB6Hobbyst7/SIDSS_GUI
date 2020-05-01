Imports System.ComponentModel
Imports System.Data
Imports System.Data.Entity
Imports System.Data.Entity.Validation
Imports System.Data.SQLite
Imports System.IO
Imports System.Windows.Forms
Imports DotSpatial.Symbology
Imports Hourly_Ref_ET_Calculator


Class MainWindow

#Region "Public Vars"

    'Public app_path As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)
    Public app_path As String = Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location) & "\SIDSS_Entity_database.db"
    Public dgv As FormDGV
    Public user_control As OutputPath
    Public Property Value As DateTime
    Public final_table As New DataTable
    Dim Tbase As Integer = 50
    Dim cmd As New SQLiteCommand
    Dim message_shown As Boolean = False

#End Region

    Public Class Shared_controls

        'Public Shared dgvWaterbalance As Windows.Controls.DataGrid
        'Public Shared DgvRefET As Windows.Controls.DataGrid
        Public Shared main_window_shared As MainWindow

    End Class

    Public Sub Load_WaterBalance_DagaGrid()
        ' Encapsulating database in "using" statement to close the database immediately.
        Using entity_table As New SIDSS_Entities()
            ' Read database table from Entity Framework database and convert it to list for displaying into datagrid.
            dgvWaterBalance.ItemsSource = entity_table.SMD_Daily.ToList()
        End Using

    End Sub

    Public Sub Load_RefET_DagaGrid()
        ' Encapsulating database in "using" statement to close the database immediately.
        Using entity_table As New SIDSS_Entities()
            ' Read database table from Entity Framework database and convert it to list for displaying into datagrid.
            DgvRefET.ItemsSource = entity_table.Ref_ET_Table.ToList()
        End Using

    End Sub

    ''' <summary>
    ''' Resets the selected data table from SIDSS_Entity_database and cleanup the empty space to reduce the size of database.
    ''' </summary>
    ''' <param name="table_name">A text string representing table name to be emptied.</param>
    Private Function Reset_SIDSS_Table(ByVal table_name As String)
        Dim dlg_result = MessageBox.Show($"You are about to reset {table_name} table.", "Warning!!!", MessageBoxButtons.OKCancel)
        If dlg_result = MessageBoxResult.Cancel Then
            Return False
        End If

        Using SIDSS_context = New SIDSS_Entities()

            Try
                ' Delete all rows of the table.
                SIDSS_context.Database.ExecuteSqlCommand($"DELETE FROM {table_name};")
                'Clean extra space leftover from previous data.
                SIDSS_context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "VACUUM;")

                '//Reset the ID number to start from 1.
                SIDSS_context.Database.ExecuteSqlCommand($"update SQLITE_SEQUENCE set seq = 0 where name = '{table_name}';")
            Catch ex As Exception

                MessageBox.Show(ex.Message)

            End Try

        End Using

        'Load_RefET_DagaGrid()
        'Load_WaterBalance_DagaGrid()
        Return True
    End Function

    Private Sub Btn_tiff_Click(sender As Object, e As RoutedEventArgs) Handles btn_load_weather_data_csv.Click
        If MessageBox.Show("By loading new weather data you will be resetting current data table. Are you sure?", "Dialog", MessageBoxButtons.YesNo) = vbYes Then
            Reset_SIDSS_Table("Ref_ET_Table")

        Else
            Return
        End If

        Dim file_name As String = Nothing

        Dim OpenFileDialog1 As New OpenFileDialog

        OpenFileDialog1.Filter = "Open CSV file|*.csv"
        'openFileDialog1.ShowDialog();
        If (OpenFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK) Then
            file_name = OpenFileDialog1.FileName
        Else
            Return
        End If

        Load_Weather_Data_CSV(file_name)

    End Sub

    Private Sub Load_Weather_Data_CSV(file_name As String)
        If File.Exists(file_name) Then
            Dim string_stream = New StreamReader(File.OpenRead(file_name))
            Dim SIDSS_Database = New SIDSS_Entities

            While Not string_stream.EndOfStream
                Dim curr_line As String = string_stream.ReadLine
                Dim split_line() As String = curr_line.Split(",")
                Dim converted_value As Integer = Nothing
                Dim IsNumeric As Boolean = Integer.TryParse(split_line(0), converted_value)
                If IsNumeric Then
                    Dim date_string As String = String.Format("{0}/{1}/{2}", split_line(1), split_line(2), split_line(0))
                    Dim cur_date As DateTime = Convert.ToDateTime(date_string)
                    'var ref_ET_table = SIDSS_Database.Ref_ET;
                    Dim ref_ET_table = New Ref_ET_Table

                    ref_ET_table.Date = date_string
                    ref_ET_table.DOY = Convert.ToString(cur_date.DayOfYear)
                    ref_ET_table.StdTime = split_line(3)
                    ref_ET_table.AirTemp = split_line(4)
                    ref_ET_table.RH = split_line(5)
                    ref_ET_table.Rs = split_line(6)
                    ref_ET_table.wind__spd = split_line(7)
                    SIDSS_Database.Ref_ET_Table.Add(ref_ET_table)
                End If

            End While
            SIDSS_Database.SaveChanges()
            Load_RefET_DagaGrid()
            tbx_csv_path_string.Text = file_name.ToString()
        End If

    End Sub

    Private Sub Open_tif(sender As Object, e As RoutedEventArgs) Handles btn_KC_MS_tiff.Click
        KC_MS_file_path.Text = get_file_path("Tiff Files", "TIF", "Select NRG calibrated image %ge values (0-100)")
        set_parameter_file()

    End Sub

    Private Function get_file_path(ByVal file_info As String, ByVal extension As String, ByVal title As String)
        Dim open_file As New Microsoft.Win32.OpenFileDialog With {
            .Filter = String.Format("{0}|*.{1}", file_info, extension),
            .Title = String.Format("{0}", title),
            .FileName = ""
        }
        open_file.ShowDialog()

        Return open_file.FileName()
    End Function

    Private Sub Main_window_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles main_window.SizeChanged
        Dim tab_control_h As Integer = CType((e.NewSize.Height), Integer) - 70

        Dim total_tabs As Integer = tab_control.Items.Count
        For Each tab_item As TabItem In tab_control.Items
            tab_item.Height = tab_control_h / total_tabs - 1
        Next

        Dynamic_grid_resize()
    End Sub

    Private Sub Main_window_Loaded(sender As Object, e As RoutedEventArgs) Handles main_window.Loaded
        'Load_RefET_DagaGrid()
        ' Return

#Region "Load Settings"

        Using SIDS_GUI_context As New SIDSS_Entities()
            Try
                Dim parameter_row = SIDS_GUI_context.SIDS_GUI_Parameters.Find(1)
                Dim item_name As String = ""
                KC_MS_file_path.Text = parameter_row.Kcb_MS_Tiff
                tbx_csv_path_string.Text = parameter_row.RefET_hourly_CSV
                tbx_EB_MS.Text = parameter_row.EB_MS_Tiff
                tbx_EB_Thermal.Text = parameter_row.EB_Thermal_Tiff
                tbxSoilDepth_1.Text = parameter_row.SoilDepth_1
                tbxSoilDepth_2.Text = parameter_row.SoilDepth_2
                tbxSoilDepth_3.Text = parameter_row.SoilDepth_3
                tbxSoilDepth_4.Text = parameter_row.SoilDepth_4
                tbxSoilDepth_5.Text = parameter_row.SoilDepth_5
                tbxTAW_1.Text = parameter_row.TAW_1
                tbxTAW_2.Text = parameter_row.TAW_2
                tbxTAW_3.Text = parameter_row.TAW_3
                tbxTAW_4.Text = parameter_row.TAW_4
                tbxTAW_5.Text = parameter_row.TAW_5
                tbxMinRootDepth.Text = parameter_row.Min_Root_Depth
                tbxMaxRootDepth.Text = parameter_row.Max_Root_Depth
                HarvestDate.SelectedDate = Convert.ToDateTime(parameter_row.Harvest_Date)
                HarvestDate.DisplayDate = Convert.ToDateTime(parameter_row.Harvest_Date)
                PlantDate.SelectedDate = Convert.ToDateTime(parameter_row.Plant_Date)
                PlantDate.DisplayDate = Convert.ToDateTime(parameter_row.Plant_Date)
                tbx_lat.Text = parameter_row.Latitude
                tbx_lon.Text = parameter_row.Longitude
                tbx_elev.Text = parameter_row.Elevation
                tbx_zt.Text = parameter_row.T_Air_H
                tbx_zu.Text = parameter_row.W_Spd_H
                tbxSiteName.Text = parameter_row.Site_Name
                tbxSiteSummary.Text = parameter_row.Site_Summary
                tbxMAD_perecnt.Text = parameter_row.MAD
                tbxRunoffCN.Text = parameter_row.CN_Number
                tbxIrrigEff.Text = parameter_row.Irrig_Efficiency
                cbx_lon_center.Text = parameter_row.Longitude_Centere
                tbxMAD_perecnt.Text = parameter_row.MAD
                tbxRunoffCN.Text = parameter_row.CN_Number
                tbxIrrigEff.Text = parameter_row.Irrig_Efficiency
                tbx_Ta.Text = parameter_row.EB_Tair
                tbx_Rs.Text = parameter_row.EB_Ra
                tbx_RH.Text = parameter_row.EB_RH
                tbx_Wind_Spd.Text = parameter_row.EB_WindSpd
                tbx_Wind_Dir.Text = parameter_row.EB_WindDir
                tbx_EB_MS.Text = parameter_row.EB_MS_Tiff
                tbx_EB_Thermal.Text = parameter_row.EB_Thermal_Tiff
                Date_EB_Image.SelectedDate = Convert.ToDateTime(parameter_row.EB_Date)
                StdTime_EB_Image.Text = parameter_row.EB_StdTime
                tbx_ETr.Text = parameter_row.ETr_in
                'tbx_EB_RH_measurement_height.Text = parameter_row.EB_RH_h
                'tbx_EB_WindSpd_height.Text = parameter_row.EB_WindSpd_h
            Catch ex As Exception

            End Try

        End Using

#End Region

        Load_RefET_DagaGrid()

        Dynamic_grid_resize()

    End Sub

    Private Sub Dynamic_grid_resize()
        Try
            Dim WaterBalance_Grid As Integer = main_window.ActualHeight - 360
            dgvWaterBalance.Height = WaterBalance_Grid

            Dim infoGrid As Integer = main_window.ActualHeight - 246 - 10
            DgvRefET.Height = infoGrid

            Dim seteinfoGrid As Integer = main_window.ActualHeight - 530 - 15
            dgSiteInfo.Height = seteinfoGrid

            Dim rtbxHeight As Integer = main_window.ActualHeight - 360 - 15
            rtbxReflET.Height = rtbxHeight
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Creates a parameter file which is used by "ET_Calculation_Field_data_with_cpp_v3" python script as an input.
    ''' </summary>
    Private Sub set_parameter_file()
        Dim Lat, Lm, Elev, Lz, df, const_g, const_k, const_Cpa, const_Z_u, const_Z_T, const_min_u As Decimal
        Dim tif_KC_MS_file_path, csv_file_path, tif_EB_MS_file_path, tif_EB_Thermal_file_path As String
        Dim EB_Ta_txt, EB_Rs_txt, EB_RH_txt, EB_Wind_Spd_txt, EB_YYYYMMDDHH_txt, EB_wind_dir_txt, EB_STD_Time_txt, EB_SW_Rad_txt As String

        Lat = Validate_decimal(tbx_lat.Text, "Latitude")
        Lm = Validate_decimal(tbx_lon.Text, "Longitude")
        Elev = Validate_decimal(tbx_elev.Text, "Elevation")
        Lz = Validate_decimal(cbx_lon_center.Text, "Center Longitude")
        'df = Validate_decimal(tbx_freq.Text)
        const_g = 9.81
        'const_k = Validate_decimal(tbx_k.Text)
        'const_Cpa = Validate_decimal(tbx_cpa.Text)
        const_Z_u = Validate_decimal(tbx_zu.Text, "Wind measurement height")
        const_Z_T = Validate_decimal(tbx_zt.Text, "Temperature measurement height")
        'const_min_u = Validate_decimal(tbx_min_u.Text)
        tif_KC_MS_file_path = KC_MS_file_path.Text
        tif_EB_MS_file_path = tbx_EB_MS.Text
        tif_EB_Thermal_file_path = tbx_EB_Thermal.Text

        EB_Ta_txt = tbx_Ta.Text
        EB_Rs_txt = tbx_Rs.Text
        EB_RH_txt = tbx_RH.Text
        EB_Wind_Spd_txt = tbx_Wind_Spd.Text
        EB_YYYYMMDDHH_txt = Date_EB_Image.Text
        EB_STD_Time_txt = StdTime_EB_Image.Text
        EB_wind_dir_txt = tbx_Wind_Dir.Text

        csv_file_path = tbx_csv_path_string.Text

        Dim file = My.Computer.FileSystem.OpenTextFileWriter("parameters_ref_ET.py", False)
        file.WriteLine("Lat=" & Lat)
        file.WriteLine("Lm=" & Lm)
        file.WriteLine("Elev=" & Elev)
        file.WriteLine("Lz=" & Lz)
        'file.WriteLine("df=" & df)
        'file.WriteLine("const_g=" & const_g)
        'file.WriteLine("const_k=" & const_k)
        'file.WriteLine("const_Cpa=" & const_Cpa)
        file.WriteLine("const_Z_u=" & const_Z_u)
        file.WriteLine("const_Z_T=" & const_Z_T)
        file.WriteLine("const_min_u=" & const_min_u)
        file.WriteLine("KC_MS_file_path=" & "r" & """" & tif_KC_MS_file_path & """")
        file.WriteLine("EB_MS_file_path=" & "r" & """" & tif_EB_MS_file_path & """")
        file.WriteLine("EB_Thermal_file_path=" & "r" & """" & tif_EB_Thermal_file_path & """")
        file.WriteLine("csv_file_path=" & "r" & """ & csv_file_path & """)
        file.WriteLine("ETr = " & tbx_ETr.Text)
        file.WriteLine("EB_Ta_txt=" & EB_Ta_txt)
        file.WriteLine("EB_Rs_txt=" & EB_Rs_txt)
        file.WriteLine("EB_RH_txt=" & EB_RH_txt)
        file.WriteLine("EB_Wind_Spd_txt=" & EB_Wind_Spd_txt)
        file.WriteLine("EB_YYYYMMDDHH_txt=" & "r" & """" & EB_YYYYMMDDHH_txt & " " & EB_STD_Time_txt & """")
        file.WriteLine("EB_wind_dir_txt=" & EB_wind_dir_txt)
        'file.WriteLine("EB_WindSpd_h= " & tbx_EB_WindSpd_height.Text)
        'file.WriteLine("EB_RH_h= " & tbx_EB_WindSpd_height.Text)
        file.Close()
    End Sub

    ''' <summary>
    ''' Validates textbox fields to make sure if it is a decimal number.
    ''' </summary>
    Private Function Validate_decimal(ByVal tbx_string, ByVal type)
        Dim decimal_vlaue As Decimal
        If Decimal.TryParse(tbx_string, decimal_vlaue) Then
            Return decimal_vlaue
        Else

            MessageBox.Show("Please check " & type)
            End
            Return Nothing
        End If

    End Function

    Private Sub Daily_ET_raster(sender As Object, e As RoutedEventArgs) Handles btn_et_daily.Click
        Dim OpenCMD As Object = CreateObject("wscript.shell")
        Dim ET_Kcb_Python_Script As String = Nothing

        Dim tif_path As String = """" & KC_MS_file_path.Text & """"
        Dim Daily_ETr As String = RefET24hr.Text
        If Not rbBatch_ReflET_OFF.IsChecked Then
            tif_path = ""
            Daily_ETr = ""
            Dim textrange As New TextRange(rtbxReflET.Document.ContentStart, rtbxReflET.Document.ContentEnd)
            Dim Refl_ET_image_data As String = textrange.Text
            Dim all_lines() As String = Refl_ET_image_data.Split(Environment.NewLine)
            For Each str As String In all_lines
                str = str.Replace(vbLf, "")
                Try
                    If str.Length > 0 Then
                        Dim tiff_path_and_et_value() As String = str.Split(",")
                        tif_path = tiff_path_and_et_value(0)
                        Dim out_et_tif As String = tif_path.Replace(".tif", "_daily_ET.tif")
                        If File.Exists(out_et_tif) Then
                            MessageBox.Show(Path.GetFileName(out_et_tif) & "already exists")
                            Continue For
                        Else
                            tif_path = tif_path.Replace("\", "//")
                            Daily_ETr = tiff_path_and_et_value(1)
                            ET_Kcb_Python_Script = String.Format($"python.exe Crop_Coefficient_ET.py ""{tif_path}"" {Daily_ETr}")
                            OpenCMD.run(ET_Kcb_Python_Script, 1, True)
                        End If
                    End If
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try

            Next
        Else
            ET_Kcb_Python_Script = String.Format("python.exe Crop_Coefficient_ET.py {0} {1}", tif_path, Daily_ETr)

            OpenCMD.run(ET_Kcb_Python_Script, 1, True)

        End If

    End Sub

    Private Sub Grid_host_GotFocus(sender As Object, e As RoutedEventArgs)
        Dim host As New System.Windows.Forms.Integration.WindowsFormsHost()
        Dim map_win As New MapWInGIS_Control
        host.Child = map_win
        Me.grid_host.Children.Add(host)
    End Sub

    Private Sub Tb_3_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles tabImageView.MouseUp
        Try
            Dim host As New System.Windows.Forms.Integration.WindowsFormsHost()
            Dim map_win As New SIDSS_Planner_GUI.MapWInGIS_Control
            host.Child = map_win
            Me.grid_host.Children.Add(host)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Function Launch_Col_Input_From(ByVal col_name As String)
        dgv = New FormDGV
        dgv.Show()
        Dim interval As Int16
        Dim H_doy, P_doy As Int16
        H_doy = HarvestDate.SelectedDate.Value.DayOfYear
        P_doy = PlantDate.SelectedDate.Value.DayOfYear
        Dim next_date As DateTime
        'next_date = PlantDate.SelectedDate.Value.AddDays(1)
        Dim date_index As String
        interval = H_doy - P_doy
        For i = 0 To interval
            next_date = PlantDate.SelectedDate.Value.AddDays(i)
            date_index = next_date.Year.ToString & "/" & next_date.Month.ToString & "/" & next_date.Day.ToString
            dgv.DataGridView1.Rows.Add(date_index, "")
        Next
        dgv.TextBox1.Text = col_name

        Return ""
    End Function

    Private Sub BtnWeatherData_Click(sender As Object, e As RoutedEventArgs) Handles btnDailyWeatherData.Click

        '###############################################################################################################
        'Reset_SIDSS_Table("SMD_Daily")
        'Load_WaterBalance_DagaGrid()
        Dim daily_data_form As New DailyDataInput_Form
        Me.Hide()
        daily_data_form.ShowDialog()
        Load_WaterBalance_DagaGrid()
        Me.Show()
        '###############################################################################################################

    End Sub

    Private Sub BtnSetDates_Click(sender As Object, e As RoutedEventArgs) Handles btnSetDates.Click

        Dim datatable_date, datatable_doy As New DataTable
        datatable_date.Columns.Add()
        datatable_doy.Columns.Add()
        Dim interval As Int16
        Dim H_doy, P_doy As Int16
        If HarvestDate Is Nothing Or PlantDate Is Nothing Then
            Exit Sub
        Else
            H_doy = HarvestDate.SelectedDate.Value.DayOfYear
            P_doy = PlantDate.SelectedDate.Value.DayOfYear
            Dim current_date As DateTime

            'Dim date_string As String
            interval = H_doy - P_doy
            If interval <= 0 Then
                MessageBox.Show("Please make sure the planting and harvest dates are set correctly.")
                Exit Sub
            Else

                If Reset_SIDSS_Table("SMD_Daily") = False Then
                    Exit Sub
                End If

            End If

            Using SIDSS_Context As New SIDSS_Entities

                'water_table.Date

                For i = 0 To interval
                    Dim smd_daily_row = New SMD_Daily
                    current_date = PlantDate.SelectedDate.Value.AddDays(i)
                    smd_daily_row.SNo = i + 1
                    smd_daily_row.Date = Format(current_date, "MM/dd/yyyy")
                    smd_daily_row.DOY = current_date.DayOfYear
                    SIDSS_Context.SMD_Daily.Add(smd_daily_row)

                Next
                SIDSS_Context.SaveChanges()
            End Using
            Dim set_date As New SQL_table_operation

            'Load_Datagrid("WaterBalance_Table")
            Load_WaterBalance_DagaGrid()
            'dgvWaterBalance.Items.Refresh()
        End If

    End Sub

    Private Sub Tb_4_Loaded(sender As Object, e As RoutedEventArgs) Handles tabWaterBalance.Loaded
        Load_Datagrid("SMD_Daily")
    End Sub

    Public Sub Load_Datagrid(ByVal table_name As String, Optional ByVal save_csv As Boolean = False)
        'Connect to local SQLite database file. The text part is called connectionstring.
        Dim myConnection As New SQLiteConnection(String.Format("Data Source={0}; Version=3", app_path))
        'Open connection to the database file, within the program.
        myConnection.Open()

        'Select all columns from the database file to display in WPF datagrid.
        Dim cmd As New SQLiteCommand
        cmd.Connection = myConnection
        Dim conn_string As String = String.Format("Select * from {0};", table_name)
        cmd.CommandText = String.Format(conn_string)

        Dim reader As SQLiteDataReader = cmd.ExecuteReader
        Dim dt As New DataTable

        'Load SQL database values into the following datable.
        dt.Load(reader)

        'Close connection to the database.
        reader.Close()
        myConnection.Close()

        If save_csv = True Then
            Dim csv_save As New DataTable2CSV
            csv_save.Save2CSV(Format("{0}.csv", table_name), dt)
        End If

        'dgvWaterBalance.ItemsSource = dt.DefaultView
        'dgvWaterBalance.Items.Refresh()
        Load_WaterBalance_DagaGrid()
    End Sub

    Private Sub BtnCalculate_Click(sender As Object, e As RoutedEventArgs) Handles btnCalculateWaterBalance.Click

        Dim SMD_Parameters As New WaterBalanceCalculator

        'calc_water_balance_cols.Set_root_depth(tbxMinRootDepth.Text, tbxMaxRootDepth.Text)

        SMD_Parameters.Drz_1 = Convert.ToDouble(tbxSoilDepth_1.Text)
        SMD_Parameters.Drz_2 = Convert.ToDouble(tbxSoilDepth_2.Text)
        SMD_Parameters.Drz_3 = Convert.ToDouble(tbxSoilDepth_3.Text)
        SMD_Parameters.Drz_4 = Convert.ToDouble(tbxSoilDepth_4.Text)
        SMD_Parameters.Drz_5 = Convert.ToDouble(tbxSoilDepth_5.Text)

        SMD_Parameters.RAW1 = Convert.ToDouble(tbxRAW_1.Text)
        SMD_Parameters.RAW2 = Convert.ToDouble(tbxRAW_2.Text)
        SMD_Parameters.RAW3 = Convert.ToDouble(tbxRAW_3.Text)
        SMD_Parameters.RAW4 = Convert.ToDouble(tbxRAW_4.Text)
        SMD_Parameters.RAW5 = Convert.ToDouble(tbxRAW_5.Text)

        SMD_Parameters.MAD_fraction = Convert.ToDouble(tbxMAD_perecnt.Text) / 100
        SMD_Parameters.Irrigation_Efficiency_Fraction = Convert.ToDouble(tbxIrrigEff.Text) / 100
        SMD_Parameters.Runoff_CN = Convert.ToDouble(tbxRunoffCN.Text)
        SMD_Parameters.RootMax = Convert.ToDouble(tbxMaxRootDepth.Text)
        SMD_Parameters.RootMin = Convert.ToDouble(tbxMinRootDepth.Text)
        SMD_Parameters.Calculate_Grid_Cols(Tbase)

        Load_Datagrid("SMD_Daily")

    End Sub

    Private Sub Btn_EB_MS_Click(sender As Object, e As RoutedEventArgs) Handles btn_EB_MS.Click
        tbx_EB_MS.Text = get_file_path("MS image", "tif", "Select MS calibrated Image for Energy Balance method")
        set_parameter_file()
    End Sub

    Private Sub Btn_EB_Thermal_Click(sender As Object, e As RoutedEventArgs) Handles btn_EB_Thermal.Click
        tbx_EB_Thermal.Text = get_file_path("Thermal 1-band image", "tif", "Select Thermal calibrated Image for Energy Balance method")
        set_parameter_file()
    End Sub

    Private Sub Btn_EB_MS_run_Click(sender As Object, e As RoutedEventArgs) Handles btn_EB_run.Click
        set_parameter_file()
        Save_Parameters()
        Dim OpenCMD
        OpenCMD = CreateObject("wscript.shell")
        Dim command2 As String = "python.exe " & """ET_Calculation_Field_data_with_cpp_v3.py"""
        command2 = "%comspec% /k " & command2
        OpenCMD.run(command2, 1, True)
    End Sub

    Private Sub RbCotton_Click(sender As Object, e As RoutedEventArgs) Handles rbCotton.Click
        If rbCotton.IsChecked = True Then
            rbCorn.Content = "Corn"
            rbCotton.Content = "Cotton (Tbase=40)"
            rbWheat.Content = "Wheat"
            Tbase = 40
        End If

    End Sub

    Private Sub RbWheat_Click(sender As Object, e As RoutedEventArgs) Handles rbWheat.Click
        If rbWheat.IsChecked = True Then
            rbCorn.Content = "Corn"
            rbCotton.Content = "Cotton"
            rbWheat.Content = "Wheat (Tbase=60)"
            Tbase = 60
        End If

    End Sub

    Private Sub RbCorn_Click(sender As Object, e As RoutedEventArgs) Handles rbCorn.Click
        If rbCorn.IsChecked = True Then
            rbCorn.Content = "Corn (Tbase=50)"
            rbCotton.Content = "Cotton"
            rbWheat.Content = "Wheat"
            Tbase = 50
        End If
    End Sub

    Private Sub BtnChart_Click(sender As Object, e As RoutedEventArgs) Handles btnChart.Click
        Dim chrt_view As New Graphs_Viewer
        For i = 0 To chrt_view.chkGraphOptions.Items.Count - 1
            chrt_view.chkGraphOptions.SetItemCheckState(i, CheckState.Checked)
        Next

        chrt_view.ShowDialog()

    End Sub

    Private Function round_number(ByRef input_number As Double)
        Return Math.Round(input_number, 3)
    End Function

    Private Sub Btn_calc_ref_ET_Click(sender As Object, e As RoutedEventArgs) Handles btn_calc_ref_ET.Click

        Using database_context As New SIDSS_Entities()
            Dim Starting_DOY = Convert.ToInt32(database_context.Ref_ET_Table.Find(1).DOY)

            For i = Starting_DOY To 366
                Dim doy2string = i.ToString
                Dim daily_record = (From rec In database_context.Ref_ET_Table Where rec.DOY = doy2string).ToList()
                If daily_record.Count > 0 Then
                    ref_ET_Single_Day_calc(daily_record)
                End If
            Next

            Try
                database_context.SaveChanges()
            Catch ex As DbEntityValidationException
                For Each except In ex.EntityValidationErrors
                    For Each entity_error In except.ValidationErrors
                        MessageBox.Show("Property" & entity_error.PropertyName & "Error: " & entity_error.ErrorMessage)
                    Next
                Next
            End Try

        End Using
        Load_RefET_DagaGrid()

    End Sub

    Private Function ref_ET_Single_Day_calc(ByRef daily_record As List(Of Ref_ET_Table))

        Dim daily_results_table As New DataTable

        Dim ref_et_calc As New Hourly_Ref_ET_Calculator.HourlyRefET

        ref_et_calc._Lm_longitude = Convert.ToDouble(tbx_lon.Text)
        ref_et_calc._Lz_longitude = Convert.ToDouble(cbx_lon_center.Text)
        ref_et_calc._phi_degree = Convert.ToDouble(tbx_lat.Text)
        ref_et_calc._z_elevation = Convert.ToDouble(tbx_elev.Text)
        ref_et_calc._ref_crop = cbxRefCrop.Text.ToLower
        ref_et_calc._Zw_agl_WindRH_measurement = Convert.ToDouble(tbx_zu.Text)

        If message_shown = False Then
            If Convert.ToDouble(tbx_zt.Text) < 1.5 Or Convert.ToDouble(tbx_zt.Text) > 2.5 Then
                message_shown = True
                MessageBox.Show("Please Note: Recommended temperautre measurement height is 1.5 to 2.5 m.")
            End If
        End If

        Dim first_row As Boolean = True
        Dim daily_data_row_count As Integer = daily_record.Count
        For j = 0 To 1
            ' Running same loop twice, first looop is to determine the minimum fcd values  17 degrees or 0.3 radians.
            For i = 0 To daily_data_row_count - 1
                ' Std. time, hour.
                ref_et_calc._t_std_time = daily_record(i).StdTime
                ' Day of Year.
                ref_et_calc._J_doy = daily_record(i).DOY
                ' Air temperature Ta.
                ref_et_calc._Ta_air_Temperature = daily_record(i).AirTemp
                ' Relative Humidity, RH%.
                ref_et_calc._RH_humidity = daily_record(i).RH
                ' Solar Radiation.
                ref_et_calc._Rs_measured_rad = daily_record(i).Rs
                ' Wind Speed.
                ref_et_calc._Uz_WindSpeed = daily_record(i).wind__spd

                If j = 0 Then
                    ' For the first run, just to find the sunrise/sunset angle. No resutls are saved
                    ref_et_calc.Main_Calculation_Module()

                ElseIf j = 1 Then
                    ' For second run, resutls are saved this time.
                    Dim curr_full_row As DataRow = Nothing
                    ref_et_calc.Main_Calculation_Module()

                    Dim Mjph2Wm2 As Double = 277.7
                    daily_record(i).Sc = round_number(ref_et_calc.Sc)
                    daily_record(i).omega = round_number(ref_et_calc.omega)
                    daily_record(i).dr = round_number(ref_et_calc.dr)
                    daily_record(i).omega__1 = round_number(ref_et_calc.omega1)
                    daily_record(i).omega__2 = round_number(ref_et_calc.omega2)
                    daily_record(i).omega__s = round_number(ref_et_calc.omega_s)
                    daily_record(i).Ra = round_number(ref_et_calc.Ra * Mjph2Wm2)
                    daily_record(i).Rso = round_number(ref_et_calc.Rso * Mjph2Wm2)
                    daily_record(i).TKhr = round_number(ref_et_calc.TKhr)
                    daily_record(i).es = round_number(ref_et_calc.es)
                    daily_record(i).ea = round_number(ref_et_calc.ea)
                    daily_record(i).Rnl = round_number(ref_et_calc.Rnl * Mjph2Wm2)
                    daily_record(i).Rns = round_number(ref_et_calc.Rns * Mjph2Wm2)
                    daily_record(i).Rn = round_number(ref_et_calc.Rn * Mjph2Wm2)
                    daily_record(i).G = round_number(ref_et_calc.G * Mjph2Wm2)
                    daily_record(i).P = round_number(ref_et_calc.P)
                    daily_record(i).gamma = round_number(ref_et_calc.gamma)
                    daily_record(i).u2 = round_number(ref_et_calc.u2)
                    daily_record(i).Cn = round_number(ref_et_calc.Cn)
                    daily_record(i).Cd = round_number(ref_et_calc.Cd)
                    daily_record(i).delta__vapor = round_number(ref_et_calc.delta_vapor)
                    daily_record(i).delta__angle = round_number(ref_et_calc.delta_angle)
                    daily_record(i).phi = round_number(ref_et_calc.phi)
                    daily_record(i).Tmid = round_number(ref_et_calc.t_mid_time)
                    daily_record(i).Rs_Rso_adv = round_number(ref_et_calc.Rs_Rso_adv)
                    daily_record(i).fcd_adv = round_number(ref_et_calc.fcd_adv)
                    daily_record(i).Kd = round_number(ref_et_calc.Kd)
                    daily_record(i).Kb = round_number(ref_et_calc.Kb)
                    daily_record(i).Rso_adv = round_number(ref_et_calc.Rso_adv * Mjph2Wm2)
                    daily_record(i).W = round_number(ref_et_calc.W_precip_water)
                    daily_record(i).sin_phi = round_number(ref_et_calc.sin_phi)
                    daily_record(i).beta = round_number(ref_et_calc.beta)
                    If ref_et_calc.ETo <> 0 Then
                        daily_record(i).ETo = round_number(ref_et_calc.ETo)
                    End If
                    If ref_et_calc.ETr <> 0 Then
                        daily_record(i).ETr = round_number(ref_et_calc.ETr)
                    End If
                End If

            Next
        Next

        Return daily_results_table
    End Function

    Private Sub DgSiteInfo_Loaded_1(sender As Object, e As RoutedEventArgs) Handles dgSiteInfo.Loaded
        Load_siteinfo()
    End Sub

    Private Sub Load_siteinfo()
        Using entity_table As New SIDSS_Entities()
            ' Read database table from Entity Framework database and convert it to list for displaying into datagrid.
            Try
                If dgSiteInfo.Items.Count > 0 Then
                    dgSiteInfo.ItemsSource = Nothing
                End If
                dgSiteInfo.ItemsSource = entity_table.Site_Info_Summary.ToList()
            Catch ex As Exception
                'MessageBox.Show(ex.Message)
            End Try
        End Using
    End Sub

    Private Sub DgSiteInfo_SelectedCellsChanged(sender As Object, e As SelectedCellsChangedEventArgs) Handles dgSiteInfo.SelectedCellsChanged

        'Dim curr_row As DataRow
        Dim row_index As Integer
        Dim lon_mid As String = Nothing
        Try
            row_index = dgSiteInfo.SelectedIndex()
            If row_index <> -1 Then
                Dim curr_row As Site_Info_Summary = dgSiteInfo.SelectedItem
                tbx_lat.Text = curr_row.Center_Longi
                tbx_lon.Text = curr_row.Longitude
                tbx_elev.Text = curr_row.Elevation
                tbx_zt.Text = curr_row.Z__t
                tbx_zu.Text = curr_row.Z__u
                tbxSiteSummary.Text = curr_row.Summary
                tbxSiteName.Text = curr_row.SiteName
                lon_mid = curr_row.Center_Longi
            Else
                Exit Sub
            End If
            Select Case lon_mid
                Case "75"
                    cbx_lon_center.SelectedIndex = (0)
                Case "90"
                    cbx_lon_center.SelectedIndex = (1)
                Case "105"
                    cbx_lon_center.SelectedIndex = (2)
                Case "120"
                    cbx_lon_center.SelectedIndex = (3)

            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
            Exit Sub
        End Try

        Dim dgSiteInfo_table As New DataTable

        Dim selected_row As Integer = dgSiteInfo.Items.IndexOf(dgSiteInfo.CurrentItem)

    End Sub

    Private Sub BtnEditSiteInfo_Click(sender As Object, e As RoutedEventArgs) Handles btnEditSiteInfo.Click

        Dim site_index As Integer = Nothing
        Try
            If dgSiteInfo.SelectedItem IsNot Nothing Then
                site_index = DirectCast(dgSiteInfo.SelectedItem, Site_Info_Summary).SNo
                Using SIDSS_Database As New SIDSS_Entities
                    Dim Site_Info = (From site_info_row In SIDSS_Database.Site_Info_Summary Where site_info_row.SNo = site_index).ToList()(0)

                    Site_Info.SiteName = tbxSiteName.Text
                    Site_Info.Latitude = tbx_lat.Text
                    Site_Info.Longitude = tbx_lon.Text
                    Site_Info.Center_Longi = cbx_lon_center.SelectionBoxItem.ToString
                    Site_Info.Elevation = tbx_elev.Text
                    Site_Info.Z__u = tbx_zu.Text
                    Site_Info.Z__t = tbx_zt.Text
                    Site_Info.Summary = tbxSiteSummary.Text
                    SIDSS_Database.Site_Info_Summary.Add(Site_Info)
                    SIDSS_Database.SaveChanges()
                End Using
            Else
                MessageBox.Show("First select a row in the table below to edit it.")
                Exit Sub
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
            Exit Sub
        End Try

        Load_siteinfo()
    End Sub

    Private Sub btnSaveSiteInfo_Click(sender As Object, e As RoutedEventArgs) Handles btnSaveSiteInfo.Click

        Using SIDSS_context As New SIDSS_Entities()
            Dim site_summary_table = New Site_Info_Summary

            site_summary_table.SiteName = tbxSiteName.Text
            site_summary_table.Latitude = tbx_lat.Text
            site_summary_table.Longitude = tbx_lon.Text
            site_summary_table.Center_Longi = cbx_lon_center.SelectionBoxItem.ToString
            site_summary_table.Elevation = tbx_elev.Text
            site_summary_table.Z__u = tbx_zu.Text
            site_summary_table.Z__t = tbx_zt.Text
            site_summary_table.Summary = tbxSiteSummary.Text

            SIDSS_context.Site_Info_Summary.Add(site_summary_table)
            SIDSS_context.SaveChanges()
        End Using
        Load_siteinfo()

    End Sub

    Private Sub BtnDeleteSiteInfo_Click(sender As Object, e As RoutedEventArgs) Handles btnDeleteSiteInfo.Click
        Dim curr_row As Site_Info_Summary

        Try
            curr_row = dgSiteInfo.SelectedItem
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
            Exit Sub
        End Try

        Using GUI_parameter As New SIDSS_Entities
            Try
                Dim row_sn As Integer = curr_row.SNo

                Dim parameter_row = (From row_vals In GUI_parameter.Site_Info_Summary Where row_vals.SNo = row_sn).ToList(0)
                GUI_parameter.Site_Info_Summary.Remove(parameter_row)
                GUI_parameter.SaveChanges()
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                Exit Sub
            End Try
        End Using

        Load_siteinfo()
    End Sub

    Private Sub Main_window_Closing(sender As Object, e As CancelEventArgs) Handles main_window.Closing
        Save_Parameters()
        If System.Windows.Forms.Application.MessageLoop Then
            '// Use this since we are a WinForms app
            System.Windows.Forms.Application.Exit()
        Else
            '// Use this since we are a console app
            System.Environment.Exit(1)
        End If

    End Sub

    Private Sub Save_Parameters()
        Using GUI_parameter As New SIDSS_Entities
            Try
                Dim parameter_row = (From row_vals In GUI_parameter.SIDS_GUI_Parameters Where row_vals.ID = 1).ToList(0)
                parameter_row.Kcb_MS_Tiff = KC_MS_file_path.Text
                parameter_row.RefET_hourly_CSV = tbx_csv_path_string.Text
                parameter_row.EB_MS_Tiff = tbx_EB_MS.Text
                parameter_row.EB_Thermal_Tiff = tbx_EB_Thermal.Text
                parameter_row.SoilDepth_1 = tbxSoilDepth_1.Text
                parameter_row.SoilDepth_2 = tbxSoilDepth_2.Text
                parameter_row.SoilDepth_3 = tbxSoilDepth_3.Text
                parameter_row.SoilDepth_4 = tbxSoilDepth_4.Text
                parameter_row.SoilDepth_5 = tbxSoilDepth_5.Text
                parameter_row.TAW_1 = tbxTAW_1.Text
                parameter_row.TAW_2 = tbxTAW_2.Text
                parameter_row.TAW_3 = tbxTAW_3.Text
                parameter_row.TAW_4 = tbxTAW_4.Text
                parameter_row.TAW_5 = tbxTAW_5.Text
                parameter_row.Min_Root_Depth = tbxMinRootDepth.Text
                parameter_row.Max_Root_Depth = tbxMaxRootDepth.Text
                parameter_row.Harvest_Date = HarvestDate.SelectedDate
                parameter_row.Plant_Date = PlantDate.SelectedDate
                parameter_row.Latitude = tbx_lat.Text
                parameter_row.Longitude = tbx_lon.Text
                parameter_row.Elevation = tbx_elev.Text
                parameter_row.T_Air_H = tbx_zt.Text
                parameter_row.W_Spd_H = tbx_zu.Text
                parameter_row.Site_Name = tbxSiteName.Text
                parameter_row.Site_Summary = tbxSiteSummary.Text
                parameter_row.MAD = tbxMAD_perecnt.Text
                parameter_row.CN_Number = tbxRunoffCN.Text
                parameter_row.Irrig_Efficiency = tbxIrrigEff.Text
                parameter_row.Longitude_Centere = cbx_lon_center.SelectionBoxItem
                parameter_row.EB_Tair = tbx_Ta.Text
                parameter_row.EB_Ra = tbx_Rs.Text
                parameter_row.EB_RH = tbx_RH.Text
                parameter_row.EB_WindSpd = tbx_Wind_Spd.Text
                parameter_row.EB_WindDir = tbx_Wind_Dir.Text
                parameter_row.EB_MS_Tiff = tbx_EB_MS.Text
                parameter_row.EB_Thermal_Tiff = tbx_EB_Thermal.Text
                parameter_row.EB_StdTime = StdTime_EB_Image.SelectionBoxItem
                parameter_row.ETr_in = tbx_ETr.Text
                parameter_row.EB_Date = Convert.ToString(Convert.ToDateTime(Date_EB_Image.SelectedDate.Value).ToShortDateString)
                'parameter_row.EB_WindSpd_h = tbx_EB_WindSpd_height.Text
                'parameter_row.EB_RH_h = tbx_EB_RH_measurement_height.Text
                GUI_parameter.SaveChanges()
            Catch ex As Exception
                MessageBox.Show(ex.Message)
                MessageBox.Show(ex.InnerException.Message)
            End Try

        End Using
    End Sub

    Private Sub MnuOutputPath_Click(sender As Object, e As RoutedEventArgs) Handles mnuOutputPath.Click
        Dim control_window = New OutputPath
        main_window.Hide()
        control_window.Show()
        control_window.BringToFront()
        'Dim output_path As String = user_control.tbxOutputPath.ToString()
        'tbx_csv_path_string.Text = result.ToString()
    End Sub

    Private Sub TbxTAW_1_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbxTAW_1.TextChanged, tbxTAW_2.TextChanged, tbxTAW_3.TextChanged, tbxTAW_4.TextChanged, tbxTAW_5.TextChanged
        Try
            Dim MAD As Double = Convert.ToDouble(tbxMAD_perecnt.Text) / 100
            'Dim temp As String = (CType(tbxTAW_1.Text, Double) * MAD).ToString
            'tbxRAW_1.Text = temp
            Dim textbox_text As Windows.Controls.TextBox = sender

            If textbox_text.Text = "" Then
                textbox_text.Text = "0"
                sender = textbox_text
            End If
            tbxRAW_1.Text = (CType(tbxTAW_1.Text, Double) * MAD).ToString
            tbxRAW_2.Text = (CType(tbxTAW_2.Text, Double) * MAD).ToString
            tbxRAW_3.Text = (CType(tbxTAW_3.Text, Double) * MAD).ToString
            tbxRAW_4.Text = (CType(tbxTAW_4.Text, Double) * MAD).ToString
            tbxRAW_5.Text = (CType(tbxTAW_5.Text, Double) * MAD).ToString
        Catch ex As Exception
            'MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub Label_MouseEnter(sender As Object, e As Input.MouseEventArgs)
        lblPlantingDepth.FontWeight = FontWeights.Bold
    End Sub

    Private Sub LblPlantingDepth_MouseLeave(sender As Object, e As Input.MouseEventArgs)
        lblPlantingDepth.FontWeight = FontWeights.Normal
    End Sub

    Private Sub Btn_Daily_ET_Sum_Click(sender As Object, e As RoutedEventArgs) Handles btn_Daily_ET_Sum.Click

        Dim daily_ET_Sum As New DataTable
        daily_ET_Sum.Columns.Add("Date")
        daily_ET_Sum.Columns.Add("ETr__in")
        daily_ET_Sum.Columns.Add("ET0__in")

        Using database_context As New SIDSS_Entities()
            Dim Starting_DOY = Convert.ToInt32(database_context.Ref_ET_Table.Find(1).DOY)

            For i = Starting_DOY To 366
                Dim doy2string = i.ToString
                Dim daily_record = (From rec In database_context.Ref_ET_Table Where rec.DOY = doy2string).ToList()
                If daily_record.Count > 0 Then
                    Dim sum_ETr As Double = 0
                    Dim sum_ET0 As Double = 0
                    For Each record In daily_record
                        sum_ETr += record.ETr
                        sum_ET0 += record.ETo
                    Next
                    Dim data_row As DataRow = daily_ET_Sum.NewRow
                    data_row("Date") = daily_record(0).Date
                    data_row("ETr__in") = round_number(sum_ETr / 24.5) 'Converting mm to inches.
                    data_row("ET0__in") = round_number(sum_ET0 / 24.5) 'Converting mm to inches.
                    daily_ET_Sum.Rows.Add(data_row)
                End If

            Next
        End Using
        Dim daily_et_sum_window As New DailyET_Sum_Window
        daily_et_sum_window.Show()
        daily_et_sum_window.dgDailySumET.ItemsSource = daily_ET_Sum.DefaultView()

    End Sub

    Private Sub RbBatch_ReflET_OFF_Checked(sender As Object, e As RoutedEventArgs) Handles rbBatch_ReflET_OFF.Checked
        Try
            rtbxReflET.IsEnabled = False
        Catch ex As Exception
        End Try

        RefET24hr.IsEnabled = True
        btn_KC_MS_tiff.IsEnabled = True

    End Sub

    Private Sub RbBatch_ReflET_ON_Checked(sender As Object, e As RoutedEventArgs) Handles rbBatch_ReflET_ON.Checked
        rtbxReflET.IsEnabled = True
        RefET24hr.IsEnabled = False
        btn_KC_MS_tiff.IsEnabled = False
    End Sub

    Private Sub BtnKcr_ETa_data_Click(sender As Object, e As RoutedEventArgs) Handles btnKcr_ETa_data.Click
        Dim kcr_eta_panel = New Kcr_ETa_Window
        Me.Hide()
        Try
            kcr_eta_panel.ShowDialog()
        Catch ex As Exception

        End Try
        Me.Show()
        Load_WaterBalance_DagaGrid()
    End Sub

    ''' <summary>
    ''' Opens the url in default browser.
    ''' </summary>
    Private Sub Hyperlink_RequestNavigate(sender As Object, e As RequestNavigateEventArgs)
        Process.Start(New ProcessStartInfo(e.Uri.AbsoluteUri))
        e.Handled = True
    End Sub

End Class