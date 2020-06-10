Imports System.Windows.Forms
Imports System.Data

Public Class DailyDataInput_Form

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub BtnSaveDailyData_Click(sender As Object, e As RoutedEventArgs) Handles btnSaveDailyData.Click

        Dim Tmax, Tmin, Precip, Irrig, ETr As New Double
        Dim date_value As String = Nothing
        Using sidss_data As New SIDSS_Entities
            Dim smd_data = sidss_data.SMD_Daily.ToArray()

            For Each row As Weather_Row In dgvDailWeatherData.Items
                Try
                    date_value = row.CSV_Date
                    Tmax = row.CSV_THigh
                    Tmin = row.CSV_TLow
                    Precip = row.CSV_Precip
                    Irrig = row.CSV_Irrig
                    ETr = row.CSV_ETr

                    Dim formatted_date = Format("MM/dd/yyyy", Convert.ToDateTime(date_value))
                    'new_row.Date = Format("MM/dd/yyyy", Convert.ToDateTime(date_value))
                    Dim search_row = From smd_data_row In smd_data
                                     Where CType(smd_data_row.Date, Date) = CType(formatted_date, Date)
                                     Select smd_data_row
                    search_row.First.Tmax = Math.Round(Convert.ToDecimal(Tmax), 4)
                    search_row.First.Tmin = Math.Round(Convert.ToDecimal(Tmin), 4)
                    search_row.First.Precip = Math.Round(Convert.ToDecimal(Precip), 4)
                    search_row.First.Irrig = Math.Round(Convert.ToDecimal(Irrig), 4)
                    search_row.First.ETr = Math.Round(Convert.ToDecimal(ETr), 4)
                Catch ex As Exception
                    MessageBox.Show("Please make sure the date range of CSV input file matches the SIDSS date range for daily weather data input table.")
                    Exit Sub
                End Try

            Next
            sidss_data.SaveChanges()
            MessageBox.Show("Saved changes to the database.")
        End Using

        Me.Close()
    End Sub

    Private Sub BtnLoadCSV_Click(sender As Object, e As RoutedEventArgs) Handles btnLoadCSV.Click
        Dim file_dialog As New Forms.OpenFileDialog()
        file_dialog.Filter = "CSV Files|*.csv"
        file_dialog.Title = "Select daily weather data csv file."

        If file_dialog.ShowDialog = Forms.DialogResult.OK Then
            Dim csv_data As New FileIO.TextFieldParser(file_dialog.FileName)
            csv_data.SetDelimiters(",")
            Dim current_row As String() = Nothing
            current_row = csv_data.ReadFields()
            Try
                While Not csv_data.EndOfData
                    current_row = csv_data.ReadFields()
                    Dim weather_row As New Weather_Row
                    weather_row.CSV_Date = current_row(0)
                    weather_row.CSV_THigh = current_row(1)
                    weather_row.CSV_TLow = current_row(2)
                    weather_row.CSV_Precip = current_row(3)
                    weather_row.CSV_Irrig = current_row(4)
                    weather_row.CSV_ETr = current_row(5)
                    dgvDailWeatherData.Items.Add(weather_row)
                End While
            Catch ex As Exception
            End Try
        End If
    End Sub

    Public Structure Weather_Row
        Public Property CSV_Date As String
        Public Property CSV_THigh As Double
        Public Property CSV_TLow As Double
        Public Property CSV_Precip As Double
        Public Property CSV_Irrig As Double
        Public Property CSV_ETr As Double
    End Structure

End Class
