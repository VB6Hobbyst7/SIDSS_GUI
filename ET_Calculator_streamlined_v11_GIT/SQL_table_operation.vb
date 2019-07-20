Imports System.Data.SQLite
Imports System.Data
Imports System.IO
Public Class SQL_table_operation
    Dim myConnection As New SQLiteConnection("Data Source=SIDSS_database.db; Version=3")
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
            If (col_index > 1) Then
                If Double.IsNaN(col_data.Rows(i)(col_index)) Then
                    cell_value = 0
                Else
                    cell_value = col_data.Rows(i)(col_index)
                End If

            Else
                Try
                    cell_value = Convert.ToDouble(col_data.Rows(i)(col_index))
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
        Reset_SQL_Table("WaterBalance_Table")
        myConnection.Open()
        cmd.Connection = myConnection
        cmd.CommandText = Nothing
        Dim i As Integer
        Dim count As Integer = col_data.Rows.Count
        Dim doy As Double
        For i = 0 To count - 1
            doy = Math.Pow(2, i) / Math.Pow(i, 2)
            cmd.CommandText &= String.Format("INSERT INTO WaterBalance_Table (Date) VALUES ('{0}');", col_data.Rows(i)(0).ToString) & Environment.NewLine
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
        'Connect to local SQLite database file. The text part is called connectionstring.
        Dim myConnection As New SQLiteConnection("Data Source=SIDSS_database.db; Version=3")
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
        Dim app_path As String = Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location) & "\SIDSS_database.db"
        Try
            dt.Load(reader)
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & "Please check if some row is empty or too much text in Site Name(max 50 character) or Summary column (max 500 character).")

        End Try


        'Close connection to the database.
        reader.Close()
        myConnection.Close()
        Return dt


    End Function

    Public Sub Write_Final_Table(ByRef col_data As DataTable)
        cmd.Connection = myConnection
        myConnection.Open()
        cmd.CommandText = Nothing
        Dim val2 As String
        Dim val3, val4, val5, val6, val7, val8, val9, val10, val11, val12, val13, val14 As Double
        Dim n_cols As Integer = col_data.Columns.Count
        Dim n_rows As Integer = col_data.Rows.Count
        For i = 0 To n_rows - 1
            val2 = col_data.Rows(i)(1)
            val3 = Math.Round(Convert.ToDouble(col_data.Rows(i)(2)), 3).ToString
            val4 = Math.Round(Convert.ToDouble(col_data.Rows(i)(3)), 3).ToString
            val5 = Math.Round(Convert.ToDouble(col_data.Rows(i)(4)), 3).ToString
            val6 = Math.Round(Convert.ToDouble(col_data.Rows(i)(5)), 3).ToString
            val7 = Math.Round(Convert.ToDouble(col_data.Rows(i)(6)), 3).ToString
            val8 = Math.Round(Convert.ToDouble(col_data.Rows(i)(7)), 3).ToString
            val9 = Math.Round(Convert.ToDouble(col_data.Rows(i)(8)), 3).ToString
            val10 = Math.Round(Convert.ToDouble(col_data.Rows(i)(9)), 3).ToString
            val11 = Math.Round(Convert.ToDouble(col_data.Rows(i)(10)), 3).ToString
            val12 = Math.Round(Convert.ToDouble(col_data.Rows(i)(11)), 3).ToString
            val13 = Math.Round(Convert.ToDouble(col_data.Rows(i)(12)), 3).ToString
            val14 = Math.Round(Convert.ToDouble(col_data.Rows(i)(13)), 3).ToString
            'cmd.CommandText &= String.Format("INSERT INTO WaterBalance_Table (Date, DOY, Precip, Irrig, [Tmax, F], [Tmin, F], GDD, Kc, ETr, ETc, Drz, Dmax, Di) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}');" _
            '                            , val0, val1, val2, val3, val4, val5, val6, val7, val8, val9, val10, val11, val12) & Environment.NewLine
            cmd.CommandText &= String.Format("UPDATE WaterBalance_Table SET GDD='{0}', Kc='{1}', ETc='{2}', Drz='{3}', Dmax='{4}', Di='{5}' WHERE SNo={6};" _
                                        , val8, val9, val11, val12, val13, val14, i + 1) & Environment.NewLine
        Next
        Dim tr As SQLiteTransaction = myConnection.BeginTransaction
        Using tr
            cmd.ExecuteNonQuery()
            tr.Commit()
        End Using
        myConnection.Close()

    End Sub


End Class
