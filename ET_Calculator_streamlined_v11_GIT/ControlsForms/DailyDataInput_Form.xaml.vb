Public Class DailyDataInput_Form
    Private Sub BtnSaveDailyData_Click(sender As Object, e As RoutedEventArgs) Handles btnSaveDailyData.Click

        Dim range = New TextRange(rtbxDailyData.Document.ContentStart, rtbxDailyData.Document.ContentEnd)
        'Dim dlg_result = MessageBox.Show("You are about to reset and start new calculations.", "Warning!!!", MessageBoxButton.OKCancel)
        'If dlg_result = MessageBoxResult.Cancel Then
        '    Return
        'End If

        Dim allText = range.Text
        Dim input_data_lines = allText.Split(vbCrLf)
        Dim Tmax, Tmin, Precip, Irrig, ETr As New Double
        Dim date_value As String = ""
        Try
            Using sidss_context As New SIDSS_Entities

                Dim smd_data = sidss_context.SMD_Daily
                Dim smd_rows_count As Integer = sidss_context.SMD_Daily.Count

                For j = 1 To input_data_lines.Count - 2
                    Dim current_row = input_data_lines(j).Replace(vbLf, "").Split(vbTab)
                    If current_row.Length = 6 Then
                        date_value = current_row(0)
                        Tmax = current_row(1)
                        Tmin = current_row(2)
                        Precip = current_row(3)
                        Irrig = current_row(4)
                        ETr = current_row(5)
                    End If

                    Dim new_row = New SMD_Daily
                    new_row.Date = Format("MM/dd/yyyy", Convert.ToDateTime(date_value))
                    new_row.DOY = Convert.ToDateTime(date_value).DayOfYear
                    new_row.Tmax = Math.Round(Convert.ToDecimal(Tmax), 4)
                    new_row.Tmin = Math.Round(Convert.ToDecimal(Tmin), 4)
                    new_row.Precip = Math.Round(Convert.ToDecimal(Precip), 4)
                    new_row.Irrig = Math.Round(Convert.ToDecimal(Irrig), 4)
                    new_row.ETr = Math.Round(Convert.ToDecimal(ETr), 4)
                    new_row.SNo = j
                    smd_data.Add(new_row)
                Next

                sidss_context.SaveChanges()
                MessageBox.Show("Saved changes to the database.")

            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try


    End Sub
End Class
