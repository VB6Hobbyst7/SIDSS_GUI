Imports System.ComponentModel
Imports ET_Calculator_streamlined_v11_GIT.MainWindow.Shared_controls

Public Class Kcr_ETa_Window
    Private Sub Kcr_ETa_Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Using sidss_context As New SIDSS_Entities
            Dim gui_parameters = sidss_context.SIDS_GUI_Parameters.ToList(0)
            tbxFourier_a0.Text = gui_parameters.Fourier_a0.ToString
            tbxFourier_a1.Text = gui_parameters.Fourier_a1.ToString
            tbxFourier_a2.Text = gui_parameters.Fourier_a2.ToString
            tbxFourier_a3.Text = gui_parameters.Fourier_a3.ToString
            tbxFourier_b1.Text = gui_parameters.Fourier_b1.ToString
            tbxFourier_b2.Text = gui_parameters.Fourier_b2.ToString
            tbxFourier_b3.Text = gui_parameters.Fourier_b3.ToString
            tbxFourier_w.Text = gui_parameters.Fourier_w.ToString

        End Using
    End Sub

    Private Sub Kcr_ETa_Window_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Using sidss_context As New SIDSS_Entities
            Dim gui_parameters = sidss_context.SIDS_GUI_Parameters.ToList(0)
            gui_parameters.Fourier_a0 = Convert.ToDecimal(tbxFourier_a0.Text)
            gui_parameters.Fourier_a1 = Convert.ToDecimal(tbxFourier_a1.Text)
            gui_parameters.Fourier_a2 = Convert.ToDecimal(tbxFourier_a2.Text)
            gui_parameters.Fourier_a3 = Convert.ToDecimal(tbxFourier_a3.Text)
            gui_parameters.Fourier_b1 = Convert.ToDecimal(tbxFourier_b1.Text)
            gui_parameters.Fourier_b2 = Convert.ToDecimal(tbxFourier_b2.Text)
            gui_parameters.Fourier_b3 = Convert.ToDecimal(tbxFourier_b3.Text)
            gui_parameters.Fourier_w = Convert.ToDecimal(tbxFourier_w.Text)
            sidss_context.SaveChanges()
        End Using
    End Sub

    Private Sub btnAddKcrETa_Click(sender As Object, e As RoutedEventArgs) Handles btnAddKcrETa.Click
        Dim range = New TextRange(rtbx_Kcr_ETa_input.Document.ContentStart, rtbx_Kcr_ETa_input.Document.ContentEnd)
        Dim allText = range.Text
        Dim input_data_lines = allText.Split(vbCrLf)
        Dim Kcr_plot, ETa_plot As New Double
        Dim date_value As String = ""
        Using sidss_context As New SIDSS_Entities

            Dim smd_data = sidss_context.SMD_Daily.ToList()
            Dim smd_rows_count As Integer = sidss_context.SMD_Daily.Count
            ' Erase previous dataset in Kcr and ETa columns to make space for latest values.
            For r = 0 To smd_rows_count - 1
                smd_data(r).Kcr_plot = 0
                smd_data(r).ETa_plot = 0
            Next
            sidss_context.SaveChanges()
            Dim input_row = 1
            For j = 0 To smd_rows_count - 1
                For i = input_row To input_data_lines.Count - 1
                    Dim current_row = input_data_lines(i).Replace(vbLf, "").Split(vbTab)
                    If current_row.Length = 7 Then
                        date_value = current_row(0)
                        Kcr_plot = current_row(1)
                        ETa_plot = current_row(4)
                    End If
                    If Convert.ToDateTime(smd_data(j).Date) = Convert.ToDateTime(date_value) Then
                        smd_data(j).Kcr_plot = Math.Round(Convert.ToDecimal(Kcr_plot), 4)
                        smd_data(j).ETa_plot = Math.Round(Convert.ToDecimal(ETa_plot), 4)
                        'input_row = i + 1
                        Exit For
                    End If
                Next
            Next

            sidss_context.SaveChanges()
            MessageBox.Show("Data added successfully.")
            Me.Close()
            'main_window_shared.Load_WaterBalance_DagaGrid()


            'ET_Calculator_streamlined_v11_GIT.MainWindow.Shared_controls.main_window_shared.dgvWaterBalance.ItemsSource = sidss_context.SMD_Daily.ToList()


        End Using
    End Sub

    Private Sub Calculate_missing_Kcr_ETa_Data()

        Using sidss_context As New SIDSS_Entities

            Dim smd_data = sidss_context.SMD_Daily.ToArray()
            Dim row_count As Integer = smd_data.Count
            Dim calc_Kcr, GDD As Double
            For i = 0 To row_count - 1
                GDD = smd_data(i).GDD
                calc_Kcr = Calc_Fourier_Curve_Fit(GDD)
                smd_data(i).Kcr_calculated = Math.Round(calc_Kcr, 4)
                If smd_data(i).Kcr_plot = 0 Then
                    smd_data(i).Kcr_plot = Math.Round(calc_Kcr, 4)
                    smd_data(i).ETa_plot = Math.Round(Convert.ToDouble(smd_data(i).ETr * calc_Kcr), 4)
                End If
            Next

            sidss_context.SaveChanges()

        End Using
        MessageBox.Show("All missing values interpolated.")

    End Sub

    Private Function Calc_Fourier_Curve_Fit(ByVal GDD As Double)

        'Dim calculated_Kcr = sig_a + (sig_b - sig_a) / (1 + Math.Pow((GDD / sig_c), sig_d))
        'Dim Sigmoid_result = y0 + a * Math.Exp(-0.5 * Math.Pow(Math.Log(GDD / x0) / b, 2)) / GDD
        Dim Fourier_result As Double
        Dim a0 = Convert.ToDouble(tbxFourier_a0.Text)
        Dim w = Convert.ToDouble(tbxFourier_w.Text)
        Dim a1 = Convert.ToDouble(tbxFourier_a1.Text)
        Dim a2 = Convert.ToDouble(tbxFourier_a2.Text)
        Dim a3 = Convert.ToDouble(tbxFourier_a3.Text)
        Dim b1 = Convert.ToDouble(tbxFourier_b1.Text)
        Dim b2 = Convert.ToDouble(tbxFourier_b2.Text)
        Dim b3 = Convert.ToDouble(tbxFourier_b3.Text)

        Fourier_result = a0 + a1 * Math.Cos(GDD * w) + b1 * Math.Sin(GDD * w) + a2 * Math.Cos(2 * GDD * w) + b2 * Math.Sin(2 * GDD * w) + b3 * Math.Sin(3 * GDD * w)
        Return Fourier_result

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
End Class
