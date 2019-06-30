﻿Imports System.Data
Imports System.Configuration
Imports System.Data.Objects
Imports System
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.FileIO
Imports System.ComponentModel
Imports System.Windows.Forms
Imports MapWinGIS
Imports AxMapWinGIS
Imports System.Data.SQLite
Imports ET_Calculator_streamlined_v11_GIT.SQL_table_operation
Imports ET_Calculator_streamlined_v11_GIT.Calculate
Imports ET_Calculator_streamlined_v11_GIT.Graphs_Viewer
Imports ET_Calculator_streamlined_v11_GIT.Create_Empty_SQL_Data_Tables
Imports System.Collections.Generic
Class MainWindow
#Region "Public Vars"
    'Public app_path As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)
    Public app_path As String = Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location) & "\SIDSS_database.db"
    Public dgv As FormDGV
    Public Property Value As DateTime
    Public final_table As New DataTable
    Dim Tbase As Integer = 50
    Dim myConnection As New SQLiteConnection(String.Format("Data Source={0}; Version=3;", app_path))
    Dim cmd As New SQLiteCommand
#End Region

    Private Sub Load_DataGrid_RefET()
        'Connect to local SQLite database file. The text part is called connectionstring.
        'Dim myConnection As New SQLiteConnection(String.Format("Data Source={0}SIDSS_database.db; Version=3", app_path))
        'Open connection to the database file, within the program.
        If myConnection.State = ConnectionState.Closed Then
            myConnection.Open()
        Else
            MessageBox.Show("Couldn't open database.")
        End If
        'Select all columns from the database file to display in WPF datagrid.

        Dim cmd As New SQLiteCommand With {
            .Connection = myConnection,
            .CommandText = "Select * from Ref_ET_Table"
        }
        Dim reader As SQLiteDataReader = cmd.ExecuteReader()
        Dim total_cols As Integer = reader.FieldCount
        'For Each field In reader
        '    Dim field_vals As String = field()
        'Next

        Dim dt As New DataTable
        dt.Load(reader)

        'Close connection to the database.
        reader.Close()
        myConnection.Close()

        DgvRefET.ItemsSource = dt.DefaultView

    End Sub
    Private Sub Btn_tiff_Click(sender As Object, e As RoutedEventArgs) Handles btn_load_weather_data_csv.Click
        Dim reset_ref_et As New SQL_table_operation
        ' Reset old data in the ref et database.
        'Delete all the contents of the table name matching the string.
        reset_ref_et.Reset_SQL_Table("Ref_ET_Table")

        Dim open_file As New Microsoft.Win32.OpenFileDialog With {
            .Filter = "CSV weather data|*.csv"
        }
        open_file.ShowDialog()
        tbx_csv1.Text = open_file.FileName()

        Dim csv_datatable As New DataTable
        Dim csv2dgv As New WeatherData2DataGridVIew.Csv2dgv_converter
        csv2dgv._csv_path = tbx_csv1.Text
        csv_datatable = csv2dgv.Csv2dgv

        'DgvRefET.ItemsSource = full_results_Table.DefaultView

        Dim Write_SNo_Col As New SQL_table_operation
        'Write SNo column to populate the database with correct number of rows i.e. equal to the rows in csv data.
        Write_SNo_Col.Write_SNo_Column(csv_datatable.Rows.Count, "Ref_ET_Table")

        Dim index As Integer = 0
        For Each column As DataColumn In csv_datatable.Columns
            Dim col_name As String = column.ColumnName
            Dim populate_col_in_db As New SQL_table_operation
            populate_col_in_db.Write_SQL_Col("Ref_ET_Table", col_name, index, csv_datatable)
            index += 1

        Next

        Dim load_full_sql_table As New SQL_table_operation
        Dim full_sql_table As New DataTable
        full_sql_table = load_full_sql_table.Load_Datagrid("Ref_ET_Table")
        DgvRefET.ItemsSource = full_sql_table.DefaultView


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

    Private Sub Tb_size(sender As Object, e As SizeChangedEventArgs) Handles tab_control.SizeChanged
        Dim tab_control_h As Integer = Me.tab_control.ActualHeight
        Dim total_tabs As Integer = tab_control.Items.Count
        For Each tab_item As TabItem In tab_control.Items
            tab_item.Height = tab_control_h / total_tabs - 1
        Next
    End Sub

    Private Sub set_parameter_file()
        Dim Lat, Lm, Elev, Lz, df, const_g, const_k, const_Cpa, const_Z_u, const_Z_T, const_min_u As Decimal
        Dim tif_KC_MS_file_path, csv_file_path, tif_EB_MS_file_path, tif_EB_Thermal_file_path As String
        Dim EB_Ta_txt, EB_Rs_txt, EB_RH_txt, EB_Wind_Spd_txt, EB_YYYYMMDDHH_txt, EB_wind_dir_txt As String

        Lat = Validate_decimal(tbx_lat.Text)
        Lm = Validate_decimal(tbx_lon.Text)
        Elev = Validate_decimal(tbx_elev.Text)
        Lz = Validate_decimal(cbx_lon_center.Text)
        'df = Validate_decimal(tbx_freq.Text)
        const_g = 9.81
        'const_k = Validate_decimal(tbx_k.Text)
        'const_Cpa = Validate_decimal(tbx_cpa.Text)
        const_Z_u = Validate_decimal(tbx_zu.Text)
        const_Z_T = Validate_decimal(tbx_zt.Text)
        'const_min_u = Validate_decimal(tbx_min_u.Text)
        tif_KC_MS_file_path = KC_MS_file_path.Text
        tif_EB_MS_file_path = tbx_EB_MS.Text
        tif_EB_Thermal_file_path = tbx_EB_Thermal.Text

        EB_Ta_txt = tbx_Ta.Text
        EB_Rs_txt = tbx_Rs.Text
        EB_RH_txt = tbx_RH.Text
        EB_Wind_Spd_txt = tbx_Wind_Spd.Text
        EB_YYYYMMDDHH_txt = tbx_YYYMMDDHH.Text
        EB_wind_dir_txt = tbx_Wind_Dir.Text

        csv_file_path = tbx_csv1.Text

        Dim file = My.Computer.FileSystem.OpenTextFileWriter("parameters_ref_ET.py", False)
        file.WriteLine("Lat=" & Lat)
        file.WriteLine("Lm=" & Lm)
        file.WriteLine("Elev=" & Elev)
        file.WriteLine("Lz=" & Lz)
        file.WriteLine("df=" & df)
        file.WriteLine("const_g=" & const_g)
        file.WriteLine("const_k=" & const_k)
        file.WriteLine("const_Cpa=" & const_Cpa)
        file.WriteLine("const_Z_u=" & const_Z_u)
        file.WriteLine("const_Z_T=" & const_Z_T)
        file.WriteLine("const_min_u=" & const_min_u)
        file.WriteLine("KC_MS_file_path=" & "r" & """" & tif_KC_MS_file_path & """")
        file.WriteLine("EB_MS_file_path=" & "r" & """" & tif_EB_MS_file_path & """")
        file.WriteLine("EB_Thermal_file_path=" & "r" & """" & tif_EB_Thermal_file_path & """")
        file.WriteLine("csv_file_path=" & "r" & """" & csv_file_path & """")

        file.WriteLine("EB_Ta_txt=" & EB_Ta_txt)
        file.WriteLine("EB_Rs_txt=" & EB_Rs_txt)
        file.WriteLine("EB_RH_txt=" & EB_RH_txt)
        file.WriteLine("EB_Wind_Spd_txt=" & EB_Wind_Spd_txt)
        file.WriteLine("EB_YYYYMMDDHH_txt=" & "r" & """" & EB_YYYYMMDDHH_txt & """")
        file.WriteLine("EB_wind_dir_txt=" & EB_wind_dir_txt)

        file.Close()
    End Sub
    Private Function Validate_decimal(ByVal tbx_sting)
        Dim decimal_vlaue As Decimal
        If Decimal.TryParse(tbx_sting, decimal_vlaue) Then
            Return decimal_vlaue
        Else
            MessageBox.Show("Please enter a decimal number instead of " & tbx_sting)
            End
            Return Nothing
        End If

    End Function

    Private Function ReadCSV(ByVal FileName As String) As DataTable
        Dim csvDataTable As DataTable = New DataTable("csvdata")
        DgvRefET.Columns.Clear()

        Try
            Dim csvData As String() = File.ReadAllLines(FileName)

            If csvData.Length = 0 Then
                Return csvDataTable
            End If

            Dim headings As String() = csvData(0).Split(","c)

            For i As Integer = 0 To headings.Length - 1
                If headings(i) = "" Then
                    csvDataTable.Columns.Add("Index")
                Else
                    csvDataTable.Columns.Add(headings(i))
                End If

            Next

            For i As Integer = 1 To csvData.Length - 1
                Dim row As DataRow = csvDataTable.NewRow()

                For j As Integer = 0 To headings.Length - 1
                    row(j) = csvData(i).Split(","c)(j)
                Next

                csvDataTable.Rows.Add(row)
                DgvRefET.ItemsSource = csvDataTable.DefaultView
            Next

        Catch ex As Exception
            'System.Windows.Forms.MessageBox.Show(ex.Message.ToString())
        End Try

        Return csvDataTable
    End Function

    Private Sub Daily_ET_raster(sender As Object, e As RoutedEventArgs) Handles btn_et_daily.Click
        Dim OpenCMD
        OpenCMD = CreateObject("wscript.shell")
        Dim command2 As String = "python.exe " & """Crop_Coefficient_ET.py"""
        OpenCMD.run(command2, 1, True)
        'btn_launch_qgis.IsEnabled = True

    End Sub

    Private Sub Grid_host_GotFocus(sender As Object, e As RoutedEventArgs)
        Dim host As New System.Windows.Forms.Integration.WindowsFormsHost()
        Dim map_win As New UserControl1
        host.Child = map_win
        Me.grid_host.Children.Add(host)
    End Sub

    Private Sub Tb_3_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles tabImageView.MouseUp
        Dim host As New System.Windows.Forms.Integration.WindowsFormsHost()
        Dim map_win As New UserControl1
        host.Child = map_win
        Me.grid_host.Children.Add(host)
    End Sub


    Private Function Update_Main_Grid(ByVal index As Integer)
        Dim i As Integer = 0
        Dim curr_row As DataRow
        Dim doy_mod As Integer
        For Each curr_row In final_table.Rows
            curr_row = final_table.Rows.Item(index)
            doy_mod = Convert.ToInt16(curr_row(1))
            doy_mod *= 2
            curr_row(1) = doy_mod.ToString
            final_table.AcceptChanges()
            i += 1
        Next
        Return final_table
    End Function

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

    Private Sub BtnWeatherData_Click(sender As Object, e As RoutedEventArgs) Handles btnPrecip.Click
        'Dim index As Integer = 1
        'Dim precip_table As New DataTable
        Dim col_name As String = "Precip"
        Launch_Col_Input_From(col_name)
        'Populate_main_datagrid()
        Load_Datagrid("WaterBalance_Table")
        dgvWaterBalance.Items.Refresh()

    End Sub

    Private Sub BtnIrrig_Click(sender As Object, e As RoutedEventArgs) Handles btnIrrig.Click
        Dim index As Integer = 1
        Dim Irrig_table As New DataTable
        Dim col_name As String = "Irrig"
        Launch_Col_Input_From(col_name)
        Load_Datagrid("WaterBalance_Table")
        dgvWaterBalance.Items.Refresh()
    End Sub

    Private Sub BtnET_Click(sender As Object, e As RoutedEventArgs) Handles btnETr.Click
        Dim index As Integer = 1
        Dim ETr_table As New DataTable
        Dim col_name As String = "ETr"
        Launch_Col_Input_From(col_name)
        Load_Datagrid("WaterBalance_Table")
        dgvWaterBalance.Items.Refresh()

    End Sub


    Private Sub BtnTmax_Click(sender As Object, e As RoutedEventArgs) Handles btnTmax.Click
        Dim index As Integer = 1
        Dim Tmax_table As New DataTable
        Dim col_name As String = "[Tmax, F]"
        Launch_Col_Input_From(col_name)
        Load_Datagrid("WaterBalance_Table")
        dgvWaterBalance.Items.Refresh()

    End Sub

    Private Sub BtnTmin_Click(sender As Object, e As RoutedEventArgs) Handles btnTmin.Click
        Dim index As Integer = 1
        Dim Tmin_table As New DataTable
        Dim col_name As String = "[Tmin, F]"
        Launch_Col_Input_From(col_name)
        Load_Datagrid("WaterBalance_Table")
        dgvWaterBalance.Items.Refresh()

    End Sub

    Private Sub Dataset_Update()
        Dim ConnectionString As String = ""
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

            Dim date_string As String
            interval = H_doy - P_doy

            For i = 0 To interval
                current_date = PlantDate.SelectedDate.Value.AddDays(i)
                date_string = Format(current_date, "yyyy/MM/dd")
                datatable_date.Rows.Add(date_string)
                datatable_doy.Rows.Add(current_date.DayOfYear)
            Next

            Dim set_date As New SQL_table_operation

            set_date.Write_Water_Balance_Dates("Date", datatable_date)
            set_date.Write_SQL_Col("WaterBalance_Table", "DOY", 1, datatable_doy)
            Load_Datagrid("WaterBalance_Table")
            dgvWaterBalance.Items.Refresh()
        End If

    End Sub

    Private Sub Tb_4_Loaded(sender As Object, e As RoutedEventArgs) Handles tabWaterBalance.Loaded
        Load_Datagrid("WaterBalance_Table")
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

        dgvWaterBalance.ItemsSource = dt.DefaultView
        dgvWaterBalance.Items.Refresh()

    End Sub

    Public Sub Load_Datagrid2(Optional ByVal save_csv As Boolean = False)
        'Connect to local SQLite database file. The text part is called connectionstring.
        Dim myConnection As New SQLiteConnection("Data Source=SIDSS_database.db; Version=3")
        'Open connection to the database file, within the program.
        myConnection.Open()

        'Select all columns from the database file to display in WPF datagrid.
        Dim cmd As New SQLiteCommand With {
            .Connection = myConnection,
            .CommandText = "Select * from Ref_ET_Table"
        }
        Dim reader As SQLiteDataReader = cmd.ExecuteReader
        Dim dt As New DataTable

        'Load SQL database values into the following datable.
        dt.Load(reader)

        'Close connection to the database.
        reader.Close()
        myConnection.Close()

        If save_csv = True Then
            Dim csv_save As New DataTable2CSV
            csv_save.Save2CSV("ETo_calculated.csv", dt)
        End If

        DgvRefET.ItemsSource = dt.DefaultView
        DgvRefET.Items.Refresh()

    End Sub

    Private Sub BtnCalculate_Click(sender As Object, e As RoutedEventArgs) Handles btnCalculate.Click

        Dim calc_cols As New Calculate

        calc_cols.Set_root_depth(tbxMinRootDepth.Text, tbxMaxRootDepth.Text)
        Dim soil_prop As New List(Of String)
        soil_prop.Add(tbxSoilDepth_1.Text)
        soil_prop.Add(tbxSoilDepth_2.Text)
        soil_prop.Add(tbxSoilDepth_3.Text)
        soil_prop.Add(tbxSoilDepth_4.Text)
        soil_prop.Add(tbxSoilDepth_5.Text)
        soil_prop.Add(tbxRAW_1.Text)
        soil_prop.Add(tbxRAW_2.Text)
        soil_prop.Add(tbxRAW_3.Text)
        soil_prop.Add(tbxRAW_4.Text)
        soil_prop.Add(tbxRAW_5.Text)
        soil_prop.Add(tbxMAD_perecnt.Text)
        calc_cols.Set_Soil_Profile = soil_prop
        calc_cols.Calculate_Grid_Cols(Tbase)
        Load_Datagrid(True)

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
        Dim OpenCMD
        OpenCMD = CreateObject("wscript.shell")
        Dim command2 As String = "python.exe " & """ET_Calculation_Field_data_with_cpp_v2.py"""
        OpenCMD.run(command2, 1, True)
    End Sub

    Private Sub RbCotton_Click(sender As Object, e As RoutedEventArgs) Handles rbCotton.Click
        If rbCotton.IsChecked = True Then
            rbCotton.Content = "Corn"
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
        chrt_view.Show()
    End Sub

    Private Sub Main_window_Loaded(sender As Object, e As RoutedEventArgs) Handles main_window.Loaded

        KC_MS_file_path.Text = My.Settings.KC_MS_file_path_settings
        tbx_csv1.Text = My.Settings.tbx_csv1_settings
        tbx_EB_MS.Text = My.Settings.tbx_EB_MS_settings
        tbx_EB_Thermal.Text = My.Settings.tbx_EB_Thermal_settings


        'Checks to see if the database exisits in the executable direcory, if not, then an empty database is created.
        If Not File.Exists("SIDSS_database.db") Then
            Dim fresh_db As New Create_Empty_SQL_Data_Tables
            fresh_db.Create_empyt_tables()
        End If

        'Load Ref ET datagrid with old values from database.
        Dim load_full_sql_table As New SQL_table_operation
        Dim full_sql_table As New DataTable
        full_sql_table = load_full_sql_table.Load_Datagrid("Ref_ET_Table")
        DgvRefET.ItemsSource = full_sql_table.DefaultView
    End Sub

    Private Sub Btn_calc_ref_ET_Click(sender As Object, e As RoutedEventArgs) Handles btn_calc_ref_ET.Click

        Dim full_results_Table As New DataTable

        ''Column header names are in the same order as the results recieved from HourlyRefET class.
        'Dim header_names() As String = {"DOY", "StdTime", "AirTemp", "RH", "solar__rad", "wind__spd", "Sc",
        '    "omega", "dr", "omega__1", "omega__2", "omega__s", "Ra", "Rso", "fcd", "TKhr", "es", "ea", "Rnl", "Rns", "Rn", "G", "P", "gamma", "u2", "Cn",
        '    "Cd", "delta__vapor", "delta__angle", "phi", "beta", "ETsz"}
        ''Create column names to match the HourlyRefET results by name to the SQLite datatable names.
        'For Each header In header_names
        '    full_results_Table.Columns.Add(header)
        'Next

        Dim ref_et_calc As New Hourly_Ref_ET_Calculator.HourlyRefET

        ref_et_calc._Lm_longitude = Convert.ToDouble(tbx_lon.Text)
        ref_et_calc._Lz_longitude = Convert.ToDouble(cbx_lon_center.Text)

        ref_et_calc._phi_degree = Convert.ToDouble(tbx_lat.Text)
        ref_et_calc._z_elevation = Convert.ToDouble(tbx_elev.Text)
        ref_et_calc._ref_crop = cbxRefCrop.Text.ToLower
        ref_et_calc._Zw_agl_WindRH_measurement = Convert.ToDouble(tbx_zu.Text)

        If Convert.ToDouble(tbx_zt.Text) < 1.5 Or Convert.ToDouble(tbx_zt.Text) > 2.5 Then
            MessageBox.Show("Please Note: Recommended temperautre measurement height is 1.5 to 2.5 m.")
        End If

        Dim std_time, doy, tair, humidity, rad, windspd As New DataTable
        Dim read_single_col As New SQL_table_operation


        Dim load_full_sql_table As New SQL_table_operation
        Dim full_sql_table As New DataTable
        full_sql_table = load_full_sql_table.Load_Datagrid("Ref_ET_Table")
        Dim csv_row_count = full_sql_table.Rows.Count

        std_time = read_single_col.Read_SQL_Col("Ref_ET_Table", "StdTime", full_sql_table)
        doy = read_single_col.Read_SQL_Col("Ref_ET_Table", "DOY", full_sql_table)
        tair = read_single_col.Read_SQL_Col("Ref_ET_Table", "AirTemp", full_sql_table)
        humidity = read_single_col.Read_SQL_Col("Ref_ET_Table", "RH", full_sql_table)
        rad = read_single_col.Read_SQL_Col("Ref_ET_Table", "Rs", full_sql_table)
        windspd = read_single_col.Read_SQL_Col("Ref_ET_Table", "wind__spd", full_sql_table)

        Dim first_row As Boolean = True

        For j = 0 To 1
            ' Running same loop twice, first looop is to determine the minimum fcd values  17 degrees or 0.3 radians.
            For i = 0 To csv_row_count - 1
                ' Std. time, hour.
                ref_et_calc._t_std_time = Convert.ToDouble(std_time.Rows(i).ItemArray(0))
                ' Day of Year.
                ref_et_calc._J_doy = Convert.ToDouble(doy.Rows(i).ItemArray(0))
                ' Air temperature Ta.
                ref_et_calc._Ta_air_Temperature = Convert.ToDouble(tair.Rows(i).ItemArray(0))
                ' Relative Humidity, RH%.
                ref_et_calc._RH_humidity = Convert.ToDouble(humidity.Rows(i).ItemArray(0))
                ' Solar Radiation.
                ref_et_calc._Rs_measured_rad = Convert.ToDouble(rad.Rows(i).ItemArray(0))
                ' Wind Speed.
                ref_et_calc._Uz_WindSpeed = Convert.ToDouble(windspd.Rows(i).ItemArray(0))

                If j = 1 Then
                    Dim curr_full_row As DataRow = Nothing
                    'results_table.Rows.Add(ref_et_calc.Main_Calculation_Module())
                    Dim results_row As Dictionary(Of String, Double) = ref_et_calc.Main_Calculation_Module()



                    If first_row = True Then
                        For Each kvp As KeyValuePair(Of String, Double) In results_row
                            full_results_Table.Columns.Add(kvp.Key)
                        Next
                    End If
                    first_row = False

                    full_results_Table.Rows.Add()
                    Dim k As Integer = 0
                    For Each kvp As KeyValuePair(Of String, Double) In results_row
                        full_results_Table.Rows(i)(k) = Math.Round(kvp.Value, 4)
                        k += 1


                    Next

                Else
                    ref_et_calc.Main_Calculation_Module()
                End If

            Next
        Next

        Dim index As Integer = 0
        For Each column As DataColumn In full_results_Table.Columns
            Dim col_name As String = column.ColumnName
            Dim populate_col_in_db As New SQL_table_operation
            populate_col_in_db.Write_SQL_Col("Ref_ET_Table", col_name, index, full_results_Table)
            index += 1

        Next

        full_sql_table = load_full_sql_table.Load_Datagrid("Ref_ET_Table")
        DgvRefET.ItemsSource = full_sql_table.DefaultView

    End Sub

    Private Sub Btn_Save_ETrz_Click(sender As Object, e As RoutedEventArgs) Handles btn_Save_ETrz.Click
        Load_DataGrid_RefET()
    End Sub

    Private Sub SetCellValue(ByVal myGrid As DataGrid)
        Dim myCell As New DataGridCell()
        ' Use an arbitrary cell.
        myCell.RowNumber = 1
        myCell.ColumnNumber = 1
        ' Change the cell's value using the CurrentCell.
        myGrid(myCell) = "New Value"
    End Sub

    Private Sub GetCellValue(ByVal myGrid As DataGrid)
        Dim myCell As New DataGridCell()
        ' Use an arbitrary cell.
        myCell.RowNumber = 1
        myCell.ColumnNumber = 1
        Console.WriteLine(myGrid(myCell))
    End Sub

    Private Sub DgSiteInfo_Loaded_1(sender As Object, e As RoutedEventArgs) Handles dgSiteInfo.Loaded
        Load_siteinfo()
    End Sub


    Private Sub Load_siteinfo()
        Dim dgSiteInfo_table As New DataTable
        Dim read_database As New SQL_table_operation
        dgSiteInfo_table = read_database.Load_Datagrid("Site_Info_Summary")
        Dim row_count As Integer = dgSiteInfo_table.Rows.Count

        dgSiteInfo.ItemsSource = dgSiteInfo_table.DefaultView()
    End Sub

    Private Sub DgSiteInfo_SelectedCellsChanged(sender As Object, e As SelectedCellsChangedEventArgs) Handles dgSiteInfo.SelectedCellsChanged

        Dim curr_row As DataRowView
        Dim lon_mid As String
        Try
            curr_row = dgSiteInfo.SelectedItem
            tbx_lat.Text = curr_row("Latitude").ToString()
            tbx_lon.Text = curr_row("Longitude").ToString()
            tbx_elev.Text = curr_row("Elevation").ToString()
            tbx_zt.Text = curr_row("Z__t").ToString()
            tbx_zu.Text = curr_row("Z__u").ToString()
            tbxSiteSummary.Text = curr_row("Summary").ToString()
            tbxSiteName.Text = curr_row("SiteName").ToString()
            lon_mid = curr_row("Center_Longi").ToString()
        Catch ex As Exception
            'MessageBox.Show(ex.Message.ToString())
            Exit Sub
        End Try

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

        Dim dgSiteInfo_table As New DataTable

        Dim selected_row As Integer = dgSiteInfo.Items.IndexOf(dgSiteInfo.CurrentItem)

    End Sub

    Private Sub BtnEditSiteInfo_Click(sender As Object, e As RoutedEventArgs) Handles btnEditSiteInfo.Click
        Dim curr_row As DataRowView
        Try
            curr_row = dgSiteInfo.SelectedItem
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
            Exit Sub
        End Try
        Dim site_index As String = curr_row("SNo")

        Dim SiteName As String = tbxSiteName.Text
        Dim Latitude As String = tbx_lat.Text
        Dim Longitude As String = tbx_lon.Text
        Dim Center_Longi As String = cbx_lon_center.SelectionBoxItem.ToString
        Dim Elevation As String = tbx_elev.Text
        Dim Z__u As String = tbx_zu.Text
        Dim Z__t As String = tbx_zt.Text
        Dim Summary As String = tbxSiteSummary.Text
        If myConnection.State = ConnectionState.Open Then
            myConnection.Close()
        End If
        Summary = Summary.Replace("'", "''")
        Dim cmd_string As String =
       String.Format("UPDATE Site_Info_Summary SET SiteName='{0}', Latitude={1}, Longitude={2}, Center_Longi={3}, Elevation={4}, Z__u={5}, Z__t={6}, Summary='{7}' WHERE SNo={8};", SiteName, Latitude, Longitude, Center_Longi, Elevation, Z__u, Z__t, Summary, site_index)
        myConnection.Open()
        cmd.Connection = myConnection
        cmd.CommandText = cmd_string
        cmd.ExecuteNonQuery()
        myConnection.Close()
        Load_siteinfo()
    End Sub

    Private Sub btnSaveSiteInfo_Click(sender As Object, e As RoutedEventArgs) Handles btnSaveSiteInfo.Click
        Dim dgSiteInfo_table As New DataTable
        Dim read_database As New SQL_table_operation
        dgSiteInfo_table = read_database.Load_Datagrid("Site_Info_Summary")

        Dim myConnection As New SQLiteConnection("Data Source=SIDSS_database.db; Version=3")
        Dim cmd As New SQLiteCommand
        'Open connection to the database file, within the program.

        Dim SiteName As String = tbxSiteName.Text
        Dim Latitude As String = tbx_lat.Text
        Dim Longitude As String = tbx_lon.Text
        Dim Center_Longi As String = cbx_lon_center.SelectionBoxItem.ToString
        Dim Elevation As String = tbx_elev.Text
        Dim Z__u As String = tbx_zu.Text
        Dim Z__t As String = tbx_zt.Text
        Dim Summary As String = tbxSiteSummary.Text
        If myConnection.State = ConnectionState.Open Then
            myConnection.Close()
        End If
        Summary = Summary.Replace("'", "''")
        Dim cmd_string As String =
       String.Format("INSERT INTO Site_Info_Summary (SiteName, Latitude, Longitude, Center_Longi, Elevation, Z__u, Z__t, Summary)
       VALUES ( '{0}', {1}, {2}, {3}, {4}, {5}, {6}, '{7}');", SiteName, Latitude, Longitude, Center_Longi, Elevation, Z__u, Z__t, Summary)
        cmd.Connection = myConnection
        cmd.CommandText = cmd_string
        myConnection.Open()
        cmd.ExecuteNonQuery()
        myConnection.Close()
        Load_siteinfo()

    End Sub

    Private Sub BtnDeleteSiteInfo_Click(sender As Object, e As RoutedEventArgs) Handles btnDeleteSiteInfo.Click
        Dim curr_row As DataRowView
        Try
            curr_row = dgSiteInfo.SelectedItem
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
            Exit Sub
        End Try
        Dim site_index As String = curr_row("SNo")
        Dim myConnection As New SQLiteConnection("Data Source=SIDSS_database.db; Version=3")
        Dim cmd As New SQLiteCommand
        If myConnection.State = ConnectionState.Open Then
            myConnection.Close()
        End If
        myConnection.Open()
        cmd.Connection = myConnection
        cmd.CommandText = String.Format("DELETE FROM Site_Info_Summary WHERE SNo={0};", site_index)
        cmd.ExecuteNonQuery()
        myConnection.Close()
        Load_siteinfo()
    End Sub

    Private Sub Main_window_Closing(sender As Object, e As CancelEventArgs) Handles main_window.Closing
        My.Settings.KC_MS_file_path_settings = KC_MS_file_path.Text
        My.Settings.tbx_csv1_settings = tbx_csv1.Text
        My.Settings.tbx_EB_MS_settings = tbx_EB_MS.Text
        My.Settings.tbx_EB_Thermal_settings = tbx_EB_Thermal.Text
        My.Settings.Save()

    End Sub
End Class
