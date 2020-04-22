Imports System.Linq
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

                Dim smd_data = sidss_context.SMD_Daily.ToArray()
                Dim smd_data_table = sidss_context.SMD_Daily
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

                    'Dim SMD_Daily_Table = sidss_context.SMD_Daily
                    Dim formatted_date = Format("MM/dd/yyyy", Convert.ToDateTime(date_value))
                    'new_row.Date = Format("MM/dd/yyyy", Convert.ToDateTime(date_value))
                    Dim search_row = From smd_data_row In smd_data
                                     Where CType(smd_data_row.Date, Date) = CType(formatted_date, Date)
                    'Select smd_data_row.Date
                    Dim result = search_row.ToArray()

                    '.Select(Of SMD_Daily(Where(smd_row.Date = formatted_date)
                    'Dim search_row_1 = search_row.Date
                    If result.Length > 0 Then

                        'new_row.DOY = Convert.ToDateTime(date_value).DayOfYear
                        'smd_data.. = Math.Round(Convert.ToDecimal(Tmax), 4)
                        'search_row.Tmin = Math.Round(Convert.ToDecimal(Tmin), 4)
                        'search_row.Precip = Math.Round(Convert.ToDecimal(Precip), 4)
                        'search_row.Irrig = Math.Round(Convert.ToDecimal(Irrig), 4)
                        'search_row.ETr = Math.Round(Convert.ToDecimal(ETr), 4)
                        'search_row.SNo = j
                        'smd_data.Add(search_row)
                    End If
                    MessageBox.Show(result.ToString())

                Next

                sidss_context.SaveChanges()
                MessageBox.Show("Saved changes to the database.")
                Me.Close()

            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try


    End Sub
End Class
