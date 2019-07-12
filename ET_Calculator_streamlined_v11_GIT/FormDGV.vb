Imports System.Windows.Forms
Imports ET_Calculator_streamlined_v11_GIT.SQL_table_operation

Public Class FormDGV
    'Public x_y As New Data.DataTable
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim full_table As New Data.DataTable
        Dim x_y As New Data.DataTable
        Dim col_name As String = TextBox1.Text
        x_y = data_input()
        Dim save_col As New SQL_table_operation
        save_col.Write_SQL_Col("WaterBalance_Table", col_name, 0, x_y)
        'full_table = save_col.Load_Datagrid()
        'Dim main_window As New MainWindow
        'main_window.dgvWaterBalance.ItemsSource = full_table.DefaultView
        Me.Close()

    End Sub


    Private Sub DataGridView1_KeyDown(sender As Object, e As Forms.KeyEventArgs) Handles DataGridView1.KeyDown
        If e.Control AndAlso e.KeyCode = Keys.V Then
            Dim clip_text As String = Clipboard.GetText()
            Try
                Dim i As Integer = 0
                For Each line As String In clip_text.Split(vbNewLine)
                    Me.DataGridView1.Rows(i).Cells(1).Value = line
                    i += 1
                Next

            Catch ex As Exception
                MessageBox.Show(ex.Message, My.Application.Info.Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Function data_input()
        Dim sel_row As Forms.DataGridViewRow
        Dim n_rows As Integer
        Dim n_cols As Integer
        Dim val As String
        n_rows = DataGridView1.Rows.Count
        n_cols = DataGridView1.Columns.GetColumnCount(Forms.DataGridViewElementStates.Displayed)
        Dim x_y As New Data.DataTable

        x_y.Columns.Add()

        For i = 0 To n_rows - 2
            sel_row = DataGridView1.Rows(i)
            'val1 = sel_row.Cells(0).FormattedValue.ToString()
            val = sel_row.Cells(1).FormattedValue.ToString()
            ' x_y.Rows.Add(val1, val2)
            x_y.Rows.Add(val)
        Next
        Return (x_y)
    End Function

End Class