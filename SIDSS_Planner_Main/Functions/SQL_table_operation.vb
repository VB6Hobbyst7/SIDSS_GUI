Imports System.Data
Imports System.Data.SQLite
Imports System.IO

Public Class SQL_table_operation
    Dim myConnection As New SQLiteConnection("Data Source=C:\SIDSS_Database\SIDSS_Entity_database.db; Version=3")
    Dim cmd As New SQLiteCommand

    Public Function Read_SQL_Col(ByVal table_name As String, ByVal col_name As String, ByVal col_data As DataTable)
        'Connect to local SQLite database file. The text part is called connectionstring.
        'Open connection to the database file, within the program.
        myConnection.Open()
        cmd.Connection = myConnection

        'Select all columns from the database file to display in WPF datagrid.

        Dim DOY As Integer = col_data.Rows(0)(4)
        cmd.CommandText = String.Format("Select {0} from {1} WHERE DOY={2};", col_name, table_name, DOY)

        Dim reader As SQLiteDataReader = cmd.ExecuteReader
        Dim dt As New DataTable

        'Load SQL database values into the following datable.
        dt.Load(reader)

        'Close connection to the database.
        reader.Close()
        myConnection.Close()

        'Return the selected single table.

        Return dt
    End Function

    Public Function Write_SQL_Col(ByVal table_name As String, ByVal col_name As String, ByVal col_index As Integer, ByVal col_data As DataTable, Optional ByVal curr_day_data As DataTable = Nothing)

        If myConnection.State = ConnectionState.Open Then
            myConnection.Close()
        End If
        myConnection.Open()
        cmd.Connection = myConnection
        cmd.CommandText = Nothing
        Dim i As Integer
        Dim count As Integer = col_data.Rows.Count
        Dim cell_value As Object
        For i = 0 To count - 1
            If (col_index = 1) Then
                cell_value = col_data.Rows(i)(col_index)

            ElseIf (col_index > 1) Then
                If Double.IsNaN(col_data.Rows(i)(col_index)) Then
                    cell_value = 0
                Else
                    cell_value = col_data.Rows(i)(col_index)
                End If
            Else
                Try
                    cell_value = Math.Round(Convert.ToDouble(col_data.Rows(i)(col_index)), 3)
                Catch ex As Exception
                    cell_value = col_data.Rows(i)(col_index)
                End Try

            End If
            If curr_day_data Is Nothing Then
                'Command format for water balance table
                cmd.CommandText &= String.Format("UPDATE {0} SET {1}='{2}' WHERE SNo={3};", table_name, col_name, cell_value, i + 1) & Environment.NewLine
            Else
                'Command formatting for Ref_ET table.
                Dim SNo As Integer = Convert.ToInt16(curr_day_data(i)(0))
                cmd.CommandText &= String.Format("UPDATE {0} SET {1}='{2}' WHERE SNo={3};", table_name, col_name, cell_value, SNo) & Environment.NewLine

            End If
        Next

        Dim tr As SQLiteTransaction = myConnection.BeginTransaction
        cmd.Transaction = tr
        Using tr
            cmd.ExecuteNonQuery()
            tr.Commit()
        End Using

        myConnection.Close()
        Return ""

    End Function

    Public Sub Reset_SQL_Table(ByVal table_name As String)
        cmd.Connection = myConnection
        cmd.CommandText = Nothing
        myConnection.Open()
        cmd.CommandText = String.Format("DELETE FROM {0}", table_name)
        cmd.ExecuteNonQuery()
        cmd.CommandText = "VACUUM"
        cmd.ExecuteNonQuery()
        myConnection.Close()

    End Sub

    Public Function Write_Water_Balance_Dates(ByVal col_name As String, ByVal col_data As DataTable)
        'When Starting with saving date values to the table, reset old table to make space for the new data entry.
        'Populating date values will automatically reset old date in the table.
        Reset_SQL_Table("SMD_Daily")
        myConnection.Open()
        cmd.Connection = myConnection
        cmd.CommandText = Nothing
        Dim i As Integer
        Dim count As Integer = col_data.Rows.Count
        Dim doy As Double
        For i = 0 To count - 1
            doy = Math.Pow(2, i) / Math.Pow(i, 2)
            cmd.CommandText &= String.Format("INSERT INTO SMD_Daily (Date) VALUES ('{0}');", col_data.Rows(i)(0).ToString) & Environment.NewLine
        Next

        Dim tr As SQLiteTransaction = myConnection.BeginTransaction
        Using tr
            cmd.ExecuteNonQuery()
            tr.Commit()
        End Using

        myConnection.Close()
        Return ""
    End Function

    Public Function Write_SNo_Column(ByVal no_of_rows As Integer, ByVal table_name As String)
        'Firstly reset all the old content
        Reset_SQL_Table(table_name)
        myConnection.Open()
        cmd.Connection = myConnection
        cmd.CommandText = Nothing
        Dim i As Integer

        For i = 0 To no_of_rows - 1
            cmd.CommandText &= String.Format("INSERT INTO {0} (SNo) VALUES ('{1}');", table_name, i + 1) & Environment.NewLine
        Next

        Dim tr As SQLiteTransaction = myConnection.BeginTransaction
        cmd.Transaction = tr
        Using tr
            cmd.ExecuteNonQuery()
            tr.Commit()
        End Using

        myConnection.Close()

        Return 0
    End Function

    Public Function Load_SQL_DataTable(ByVal table_name)

        Using dbContext As New SIDSS_Entities()
            Dim query = From p In dbContext.SMD_Daily
                        Where p.SNo > -1
                        Select p
            Dim products As IEnumerable(Of SMD_Daily) = query.ToList()
            MessageBox.Show("")

        End Using

        'Using DBConnection As New SIDSS_Entities
        '    Dim SMD_Daily = DBConnection.SMD_Daily.ToDictionary(Of String, String)
        '    Dim col_names As String()
        '    Dim curr_col As Dictionary(Of String, String) = SMD_Daily(0)
        '    Dim i As Integer = 0
        '    Do
        '        'Dim col = curr_col(i)
        '        i += 1
        '    Loop

        '    Dim data_table As New DataTable
        '    Dim conn = DBConnection.Database.Connection
        '    Dim connectionstate = conn.State
        '    Try
        '        If connectionstate <> ConnectionState.Open Then
        '            Dim data_cmd = conn.CreateCommand()
        '            data_cmd.CommandText = "GetAvailableItems"
        '            data_cmd.CommandType = CommandType.StoredProcedure
        '            Dim data_reader = data_cmd.ExecuteReader()
        '            data_table.Load(data_reader)
        '        End If

        '    Catch ex As Exception
        '        MessageBox.Show(ex.Message.ToString())
        '    End Try
        '    Dim temp = SMD_Daily

        'End Using

        'Connect to local SQLite database file. The text part is called connectionstring.
        Dim myConnection As New SQLiteConnection("Data Source=C:\SIDSS_Database\SIDSS_Entity_database.db; Version=3")
        'Open connection to the database file, within the program.
        If myConnection.State = ConnectionState.Closed Then
            myConnection.Open()
        End If

        'Select all columns from the database file to display in WPF datagrid.
        Dim cmd As New SQLiteCommand With {
            .Connection = myConnection,
            .CommandText = String.Format("Select * from {0};", table_name)
         }
        Dim reader As SQLiteDataReader = cmd.ExecuteReader

        Dim dt As New DataTable

        'Load SQL database values into the following datable.
        Dim app_path As String = Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location) & "\SIDSS_Entity_database.db"
        Try
            dt.Load(reader)
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf &
                   "Please check if any textbox is empty or too much text in Site Name(max 50 character) or Summary column (max 500 character).")

        End Try

        'Close connection to the database.
        reader.Close()
        myConnection.Close()
        Return dt

    End Function

    Public Sub Write_WaterBalance_Final_Table(ByRef col_data As DataTable)

        Dim n_cols As Integer = col_data.Columns.Count
        Dim n_rows As Integer = col_data.Rows.Count
        Using sidss_database As New SIDSS_Entities
            Dim SMD_Table = sidss_database.SMD_Daily.ToList()
            Dim i As Integer = 0
            For Each SMD_Row In SMD_Table
                SMD_Row.GDD = Math.Round(Convert.ToDouble(col_data.Rows(i)("GDD")), 3).ToString
                SMD_Row.Kc = Math.Round(Convert.ToDouble(col_data.Rows(i)("Kc")), 3).ToString
                SMD_Row.ETr = Math.Round(Convert.ToDouble(col_data.Rows(i)("ETr")), 3).ToString
                SMD_Row.ETc = Math.Round(Convert.ToDouble(col_data.Rows(i)("ETc")), 3).ToString
                SMD_Row.Drz = Math.Round(Convert.ToDouble(col_data.Rows(i)("Drz")), 3).ToString
                SMD_Row.Dmax = Math.Round(Convert.ToDouble(col_data.Rows(i)("Dmax")), 3).ToString
                SMD_Row.Di = Math.Round(Convert.ToDouble(col_data.Rows(i)("Di")), 3).ToString
                SMD_Row.DP = Math.Round(Convert.ToDouble(col_data.Rows(i)("DP")), 3).ToString
                SMD_Row.Eff__Irrig = Math.Round(Convert.ToDouble(col_data.Rows(i)("Eff__Irrig")), 3).ToString
                SMD_Row.Eff__Precip = Math.Round(Convert.ToDouble(col_data.Rows(i)("Eff__Precip")), 3).ToString
                SMD_Row.Surface__Runoff = Math.Round(Convert.ToDouble(col_data.Rows(i)("Surface__Runoff")), 3).ToString
                i += 1
            Next
            sidss_database.SaveChanges()
        End Using

    End Sub

End Class