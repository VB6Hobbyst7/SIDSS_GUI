Imports System.Data
Imports System.IO
Public Class DataTable2CSV
    Public Sub Save2CSV(ByVal file_name As String, ByRef dt As DataTable)

        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim col_name As String = Nothing
        Dim n_cols As Integer = dt.Columns.Count
        Dim n_rows As Integer = dt.Rows.Count
        Dim curr_cell As String = Nothing
        For i = 0 To n_cols - 1
            curr_cell = Replace(dt.Columns.Item(i).Caption, ",", ":")
            col_name &= curr_cell & ","
        Next

        Dim row_data As String = Nothing

        i = 0
        j = 0
        row_data = col_name & vbCrLf
        For i = 0 To n_rows - 1
            For j = 0 To n_cols - 1
                row_data &= dt.Rows(i)(j) & ","
            Next
            row_data &= vbCrLf
        Next
        File.WriteAllText(file_name, row_data)

    End Sub
End Class
