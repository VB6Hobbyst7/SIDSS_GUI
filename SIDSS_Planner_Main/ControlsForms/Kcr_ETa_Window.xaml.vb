Imports System.ComponentModel
Imports MathNet.Numerics

Public Class Kcr_ETa_Window
    Private Sub Calculate_missing_Kcr_ETa_Data()

        Using sidss_context As New SIDSS_Entities
            Try
                Dim GDD_Kcr_vals As New List(Of List(Of Double))
                GDD_Kcr_vals.Add(New List(Of Double))
                GDD_Kcr_vals.Add(New List(Of Double))
                Dim GDD_vals As New List(Of Double)
                'GDD_vals.Add(New list(Double))
                Dim smd_data = sidss_context.SMD_Daily.ToArray()
                Dim row_count As Integer = smd_data.Count
                Dim GDD As Double

                For i = 0 To row_count - 1
                    GDD = smd_data(i).GDD
                    GDD_vals.Add(GDD)
                    'calc_Kcr = Calc_Fourier_Curve_Fit(GDD)
                    'smd_data(i).Kcr_calculated = Math.Round(calc_Kcr, 4)
                    If smd_data(i).Kcr_plot > 0 Then
                        ' Adding all non zero Kcr vlaues to the list for missing data interpolation.
                        GDD_Kcr_vals(0).Add(GDD)
                        GDD_Kcr_vals(1).Add(smd_data(i).Kcr_plot)
                    End If
                Next

                ' Generating GDD Vs Kcr values for linear interpolation.
                ' clean GDD_Kcr_vals of duplicate GDD values.
                Dim old_GDD As Double = 0
                Dim new_GDD As Double = 0
                Dim current_item_count = GDD_Kcr_vals(0).Count
                Dim currnet_index As Integer = 0
                For i = current_item_count - 1 To 0 Step -1
                    new_GDD = GDD_Kcr_vals(0)(currnet_index)
                    If old_GDD = new_GDD Then
                        GDD_Kcr_vals(0).RemoveAt(currnet_index)
                        GDD_Kcr_vals(1).RemoveAt(currnet_index)
                        currnet_index -= 1
                    End If
                    currnet_index += 1
                    old_GDD = new_GDD
                Next

                Dim Interpolated_Kcr_vals As Double()()
                Interpolated_Kcr_vals = Interpolate_GDD_Kcr(GDD_vals, GDD_Kcr_vals)

                row_count = Interpolated_Kcr_vals(0).Count
                'calc_Kcr, GDD =0
                Dim interpolated_kcr_poly6 As Double = 0
                Dim interpolated_kcr_spline As Double = 0
                Dim ETa_val As Double = 0
                For i = 0 To row_count - 1
                    interpolated_kcr_poly6 = Interpolated_Kcr_vals(1)(i)
                    interpolated_kcr_spline = Interpolated_Kcr_vals(0)(i)
                    smd_data(i).Kcr_calculated = Math.Round(interpolated_kcr_poly6, 3)
                    'smd_data(i).Kcr_calculated = Math.Round(interpolated_kcr_spline, 3)

                    If smd_data(i).Kcr_plot > 0 Then
                        ETa_val = smd_data(i).ETr * smd_data(i).Kcr_plot
                        smd_data(i).ETa_plot = Math.Round(ETa_val, 3)
                    Else
                        smd_data(i).Kcr_plot = Math.Round(interpolated_kcr_spline, 3)
                        ETa_val = smd_data(i).ETr * interpolated_kcr_spline
                        smd_data(i).ETa_plot = Math.Round(ETa_val, 3)
                    End If
                Next

                sidss_context.SaveChanges()
                MessageBox.Show("All missing values interpolated.")
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try

        End Using

    End Sub

    Private Function Interpolate_GDD_Kcr(ByVal GDD_vals As List(Of Double), ByVal GDD_Kcr_vals As List(Of List(Of Double)))
        Dim x As Double() = GDD_Kcr_vals(0).ToArray()
        Dim y As Double() = GDD_Kcr_vals(1).ToArray()
        Dim t = GDD_vals
        Dim t_size = t.Count
        Dim y_predicted As Double()
        ReDim Preserve y_predicted(t_size - 1)
        Dim y_predicted_pol As Double()
        ReDim Preserve y_predicted_pol(t_size - 1)

        'Dim spline_xy = Interpolation.LinearSpline.Interpolate(x, y)
        Dim spline_xy = Interpolation.CubicSpline.InterpolateAkimaSorted(x, y)

        Dim poly_coeffs As Double() = Fit.Polynomial(x, y, 6)
        For i = 0 To t.Count - 1
            'Dim t_ As Double = 255
            y_predicted(i) = spline_xy.Interpolate(t(i))
            y_predicted_pol(i) = poly_coeffs(0) + poly_coeffs(1) * t(i) + poly_coeffs(2) * t(i) ^ 2 + poly_coeffs(3) * t(i) ^ 3 + poly_coeffs(4) * t(i) ^ 4 + poly_coeffs(5) * t(i) ^ 5 + poly_coeffs(6) * t(i) ^ 6
        Next

        Return {y_predicted, y_predicted_pol}

    End Function


    Private Sub btnFillMissignKcrETa_Click(sender As Object, e As RoutedEventArgs) Handles btnFillMissignKcrETa.Click
        Calculate_missing_Kcr_ETa_Data()
        Calculate_ETa_Deficit()
        MessageBox.Show("Calculated missing Kcr and ETa values.")
        'main_window_shared.Load_WaterBalance_DagaGrid()
        Me.Close()
    End Sub

    Private Sub Calculate_ETa_Deficit()

        Using sidss_context As New SIDSS_Entities
            Dim smd_data = sidss_context.SMD_Daily.ToArray()
            Dim row_count As Integer = smd_data.Count
            Dim eff_prefip, eff_irrig, deficit_ini, deficit_current, ETa As Double
            deficit_ini = 0
            For i = 0 To row_count - 1
                eff_irrig = smd_data(i).Eff__Irrig
                eff_prefip = smd_data(i).Eff__Precip
                ETa = smd_data(i).ETa_plot
                deficit_current = (deficit_ini + ETa) - (eff_irrig + eff_prefip)
                If deficit_current < 0 Then
                    deficit_current = 0
                End If
                smd_data(i).Deficit_plot = Math.Round(deficit_current, 4)
                deficit_ini = deficit_current
            Next
            sidss_context.SaveChanges()

        End Using

    End Sub

    Private Sub BtnSaveDailyData_Click(sender As Object, e As RoutedEventArgs) Handles btnSaveKcrETa.Click

        Dim Kcr, ETa As New Double
        Dim date_value As String = Nothing
        Using sidss_data As New SIDSS_Entities
            Dim smd_data = sidss_data.SMD_Daily.ToArray()

            For Each row As Weather_Row In dgvKcrEta.Items
                Try
                    date_value = row.CSV_Date
                    Kcr = row.CSV_Kcr
                    ETa = row.CSV_ETa

                    Dim formatted_date = Format("MM/dd/yyyy", Convert.ToDateTime(date_value))
                    'new_row.Date = Format("MM/dd/yyyy", Convert.ToDateTime(date_value))
                    Dim search_row = From smd_data_row In smd_data
                                     Where CType(smd_data_row.Date, Date) = CType(formatted_date, Date)
                                     Select smd_data_row
                    search_row.First.Kcr_plot = Math.Round(Convert.ToDecimal(Kcr), 4)
                    search_row.First.ETa_plot = Math.Round(Convert.ToDecimal(ETa), 4)
                Catch ex As Exception
                    'MessageBox.Show("Please make sure the date range of CSV input file matches the SIDSS date range for daily weather data input table.")
                    'Exit Sub
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
        file_dialog.Title = "Open Kcr & ETa values calculated from imagery tabular csv file."

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
                    weather_row.CSV_Kcr = current_row(1)
                    weather_row.CSV_ETa = current_row(2)
                    dgvKcrEta.Items.Add(weather_row)
                End While
            Catch ex As Exception
            End Try
        End If
    End Sub

    Public Structure Weather_Row
        Public Property CSV_Date As String
        Public Property CSV_Kcr As Double
        Public Property CSV_ETa As Double
    End Structure


End Class