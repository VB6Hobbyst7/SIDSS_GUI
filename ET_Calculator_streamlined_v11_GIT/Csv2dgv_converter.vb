Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Data
Imports System.IO

Public Class Csv2dgv_converter

    Private csv_path As String

    Public WriteOnly Property _csv_path As String
        Set
            Me.csv_path = Value
        End Set
    End Property

    Public get_data_table As DataTable

    Public ReadOnly Property Csv2dgv As DataTable
        Get
            Return Me._Csv2dgv(Me.csv_path)
        End Get
    End Property

    Public Function _Csv2dgv(ByVal csv_path As String) As DataTable
        Dim csv_data As StreamReader

        csv_data = New StreamReader(Me.csv_path)
        Dim curr_row = csv_data.ReadLine.Split(",")

        Dim csv_datatable = New DataTable
        csv_data.DiscardBufferedData()
        csv_data = New StreamReader(Me.csv_path)
        csv_datatable.Columns.Add("SNo")
        csv_datatable.Columns.Add("Date")
        csv_datatable.Columns.Add("StdTime")
        csv_datatable.Columns.Add("DOY")
        csv_datatable.Columns.Add("AirTemp")
        csv_datatable.Columns.Add("RH")
        csv_datatable.Columns.Add("Rs")
        csv_datatable.Columns.Add("wind__spd")
        Dim i As Integer = 1

        While Not csv_data.EndOfStream
            Dim table_row = csv_datatable.NewRow
            curr_row = csv_data.ReadLine.Split(",")

            ' combile date time
            Dim cur_date As DateTime
            'DateTime cur_date = new DateTime((int)Convert.ToDouble(curr_row[0]), (int)Convert.ToDouble(curr_row[1]), (int)Convert.ToDouble(curr_row[2]), (int)Convert.ToDouble(curr_row[3]),0,0);
            table_row("SNo") = i
            Dim _hour As Integer = 0
            Dim _year As Integer
            Dim _month As Integer
            Dim _date As Integer

            If Not (Integer.TryParse(curr_row(0), _hour)) Then
                Continue While
            End If

            If (curr_row(3) = "24") Then
                _year = CType(Convert.ToDouble(curr_row(0)), Integer)
                _month = CType(Convert.ToDouble(curr_row(1)), Integer)
                _date = CType(Convert.ToDouble(curr_row(2)), Integer)
                _hour = CType(Convert.ToDouble(curr_row(3)), Integer)
                cur_date = New DateTime(_year, _month, _date, 0, 0, 0)
                cur_date = cur_date.AddDays(1)
                table_row("Date") = String.Format("{0:MM/dd/yyy}", cur_date)
                table_row("StdTime") = String.Format("{0:HH}", Convert.ToDateTime("0:00"))
            Else
                _year = CType(Convert.ToDouble(curr_row(0)), Integer)
                _month = CType(Convert.ToDouble(curr_row(1)), Integer)
                _date = CType(Convert.ToDouble(curr_row(2)), Integer)
                _hour = CType(Convert.ToDouble(curr_row(3)), Integer)
                cur_date = New DateTime(_year, _month, _date, _hour, 0, 0)
                table_row("Date") = String.Format("{0:MM/dd/yyyy}", cur_date)
                table_row("StdTime") = String.Format("{0:HH}", cur_date)
            End If

            'table_row["StdTime"] = string.Format("{0:HH}", Convert.ToDateTime( curr_row[1]));
            table_row("DOY") = String.Format("{0}", cur_date.DayOfYear)
            table_row("AirTemp") = Math.Round(Convert.ToDouble(curr_row(4)), 3)
            table_row("RH") = Math.Round(Convert.ToDouble(curr_row(5)), 3)
            table_row("Rs") = Math.Round(Convert.ToDouble(curr_row(6)), 3)
            table_row("wind__spd") = Math.Round(Convert.ToDouble(curr_row(7)), 3)
            csv_datatable.Rows.Add(table_row)
            i = (i + 1)

        End While

        csv_data.Close()
        Return csv_datatable


    End Function
End Class

