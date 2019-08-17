Imports System.ComponentModel
Imports ET_Calculator_streamlined_v11_GIT.MainWindow.Shared_controls

Public Class Kcr_ETa_Window
    Private Sub Kcr_ETa_Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Using sidss_context As New SIDSS_Entities
            Dim gui_parameters = sidss_context.SIDS_GUI_Parameters.ToList(0)
            tbxSigmoid_a_8214.Text = gui_parameters.Sigmoid_a_8214.ToString
            tbxSigmoid_b_8214.Text = gui_parameters.Sigmoid_b_8214.ToString
            tbxSigmoid_c_8214.Text = gui_parameters.Sigmoid_c_8214.ToString
            tbxSigmoid_d_8214.Text = gui_parameters.Sigmoid_d_8214.ToString
            tbxSigmoid_a_9110.Text = gui_parameters.Sigmoid_a_9110.ToString
            tbxSigmoid_b_9110.Text = gui_parameters.Sigmoid_b_9110.ToString
            tbxSigmoid_c_9110.Text = gui_parameters.Sigmoid_c_9110.ToString
            tbxSigmoid_d_9110.Text = gui_parameters.Sigmoid_d_9110.ToString
            tbxSigmoid_a_9308.Text = gui_parameters.Sigmoid_a_9308.ToString
            tbxSigmoid_b_9308.Text = gui_parameters.Sigmoid_b_9308.ToString
            tbxSigmoid_c_9308.Text = gui_parameters.Sigmoid_c_9308.ToString
            tbxSigmoid_d_9308.Text = gui_parameters.Sigmoid_d_9308.ToString
        End Using
    End Sub

    Private Sub Kcr_ETa_Window_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Using sidss_context As New SIDSS_Entities
            Dim gui_parameters = sidss_context.SIDS_GUI_Parameters.ToList(0)
            gui_parameters.Sigmoid_a_8214 = Convert.ToDecimal(tbxSigmoid_a_8214.Text)
            gui_parameters.Sigmoid_b_8214 = Convert.ToDecimal(tbxSigmoid_b_8214.Text)
            gui_parameters.Sigmoid_c_8214 = Convert.ToDecimal(tbxSigmoid_c_8214.Text)
            gui_parameters.Sigmoid_d_8214 = Convert.ToDecimal(tbxSigmoid_d_8214.Text)
            gui_parameters.Sigmoid_a_9110 = Convert.ToDecimal(tbxSigmoid_a_9110.Text)
            gui_parameters.Sigmoid_b_9110 = Convert.ToDecimal(tbxSigmoid_b_9110.Text)
            gui_parameters.Sigmoid_c_9110 = Convert.ToDecimal(tbxSigmoid_c_9110.Text)
            gui_parameters.Sigmoid_d_9110 = Convert.ToDecimal(tbxSigmoid_d_9110.Text)
            gui_parameters.Sigmoid_a_9308 = Convert.ToDecimal(tbxSigmoid_a_9308.Text)
            gui_parameters.Sigmoid_b_9308 = Convert.ToDecimal(tbxSigmoid_b_9308.Text)
            gui_parameters.Sigmoid_c_9308 = Convert.ToDecimal(tbxSigmoid_c_9308.Text)
            gui_parameters.Sigmoid_d_9308 = Convert.ToDecimal(tbxSigmoid_d_9308.Text)
            sidss_context.SaveChanges()
        End Using
    End Sub

    Private Sub btnAddKcrETa_Click(sender As Object, e As RoutedEventArgs) Handles btnAddKcrETa.Click
        Dim range = New TextRange(rtbx_Kcr_ETa_input.Document.ContentStart, rtbx_Kcr_ETa_input.Document.ContentEnd)
        Dim allText = range.Text
        Dim input_data_lines = allText.Split(vbCrLf)
        Dim Kcr_8214, Kcr_9110, Kcr_9308, ETa_8214, ETa_9110, ETa_9308 As New Double
        Dim date_value As String = ""
        Using sidss_context As New SIDSS_Entities

            Dim smd_data = sidss_context.SMD_Daily.ToList()
            Dim smd_rows_count As Integer = sidss_context.SMD_Daily.Count
            ' Erase previous dataset in Kcr and ETa columns to make space for latest values.
            For r = 0 To smd_rows_count - 1
                smd_data(r).Kcr_8214 = 0
                smd_data(r).Kcr_9110 = 0
                smd_data(r).Kcr_9308 = 0
                smd_data(r).ETa_8214 = 0
                smd_data(r).ETa_9110 = 0
                smd_data(r).ETa_9308 = 0
            Next
            sidss_context.SaveChanges()
            Dim input_row = 1
            For j = 0 To smd_rows_count - 1
                For i = input_row To input_data_lines.Count - 1
                    Dim current_row = input_data_lines(i).Replace(vbLf, "").Split(vbTab)
                    If current_row.Length = 7 Then
                        date_value = current_row(0)
                        Kcr_8214 = current_row(1)
                        Kcr_9110 = current_row(2)
                        Kcr_9308 = current_row(3)
                        ETa_8214 = current_row(4)
                        ETa_9110 = current_row(5)
                        ETa_9308 = current_row(6)

                    End If
                    If Convert.ToDateTime(smd_data(j).Date) = Convert.ToDateTime(date_value) Then
                        smd_data(j).Kcr_8214 = Math.Round(Convert.ToDecimal(Kcr_8214), 4)
                        smd_data(j).Kcr_9110 = Math.Round(Convert.ToDecimal(Kcr_9110), 4)
                        smd_data(j).Kcr_9308 = Math.Round(Convert.ToDecimal(Kcr_9308), 4)
                        smd_data(j).ETa_8214 = Math.Round(Convert.ToDecimal(ETa_8214), 4)
                        smd_data(j).ETa_9110 = Math.Round(Convert.ToDecimal(ETa_9110), 4)
                        smd_data(j).ETa_9308 = Math.Round(Convert.ToDecimal(ETa_9308), 4)
                        'input_row = i + 1
                        Exit For
                    End If
                Next
            Next

            sidss_context.SaveChanges()
            MessageBox.Show("Data added successfully.")
            main_window_shared.Load_WaterBalance_DagaGrid()

            'ET_Calculator_streamlined_v11_GIT.MainWindow.Shared_controls.main_window_shared.dgvWaterBalance.ItemsSource = sidss_context.SMD_Daily.ToList()


        End Using
    End Sub

    Private Sub Calculate_missing_Kcr_ETa_Data()

        Using sidss_context As New SIDSS_Entities

            Dim smd_data = sidss_context.SMD_Daily.ToArray()
            Dim row_count As Integer = smd_data.Count
            Dim calc_Kcr, calc_ETa, GDD As Double
            Dim Kcr_i = ({"8214", "9110", "9308"})
            For i = 0 To row_count - 1
                GDD = smd_data(i).GDD
                calc_Kcr = Calc_Sigmoid_Kcr(GDD, Kcr_i(0))
                smd_data(i).Kcr_calculated = Math.Round(calc_Kcr, 4)
                calc_Kcr = 0
                If smd_data(i).Kcr_8214 = 0 Or smd_data(i).Kcr_9110 = 0 Or smd_data(i).Kcr_9308 = 0 Then

                    calc_Kcr = Calc_Sigmoid_Kcr(GDD, Kcr_i(0))

                    calc_ETa = smd_data(i).ETr * calc_Kcr
                    smd_data(i).Kcr_8214 = Math.Round(calc_Kcr, 4)
                    smd_data(i).ETa_8214 = Math.Round(calc_ETa, 4)
                    calc_Kcr = 0

                    calc_Kcr = Calc_Sigmoid_Kcr(GDD, Kcr_i(1))
                    calc_ETa = smd_data(i).ETr * calc_Kcr
                    smd_data(i).Kcr_9110 = Math.Round(calc_Kcr, 4)
                    smd_data(i).ETa_9110 = Math.Round(calc_ETa, 4)
                    calc_Kcr = 0

                    calc_Kcr = Calc_Sigmoid_Kcr(GDD, Kcr_i(2))
                    calc_ETa = smd_data(i).ETr * calc_Kcr
                    smd_data(i).Kcr_9308 = Math.Round(calc_Kcr, 4)
                    smd_data(i).ETa_9308 = Math.Round(calc_ETa, 4)
                    calc_Kcr = 0

                End If


            Next
            sidss_context.SaveChanges()
        End Using
        MessageBox.Show("All missing values interpolated.")

    End Sub

    Private Function Calc_Sigmoid_Kcr(ByVal GDD As Double, ByVal Kcr_i As String)
        Dim sig_a As Double
        Dim sig_b As Double
        Dim sig_c As Double
        Dim sig_d As Double
        If Kcr_i = "8214" Then
            sig_a = Convert.ToDouble(tbxSigmoid_a_8214.Text)
            sig_b = Convert.ToDouble(tbxSigmoid_b_8214.Text)
            sig_c = Convert.ToDouble(tbxSigmoid_c_8214.Text)
            sig_d = Convert.ToDouble(tbxSigmoid_d_8214.Text)
        End If
        If Kcr_i = "9110" Then
            sig_a = Convert.ToDouble(tbxSigmoid_a_9110.Text)
            sig_b = Convert.ToDouble(tbxSigmoid_b_9110.Text)
            sig_c = Convert.ToDouble(tbxSigmoid_c_9110.Text)
            sig_d = Convert.ToDouble(tbxSigmoid_d_9110.Text)
        End If
        If Kcr_i = "9308" Then
            sig_a = Convert.ToDouble(tbxSigmoid_a_9308.Text)
            sig_b = Convert.ToDouble(tbxSigmoid_b_9308.Text)
            sig_c = Convert.ToDouble(tbxSigmoid_c_9308.Text)
            sig_d = Convert.ToDouble(tbxSigmoid_d_9308.Text)
        End If
        Dim a = sig_a
        Dim b = sig_b
        Dim x0 = sig_c
        Dim y0 = sig_d
        'Dim calculated_Kcr = sig_a + (sig_b - sig_a) / (1 + Math.Pow((GDD / sig_c), sig_d))
        Dim SigmaPlot_Eq = y0 + a * Math.Exp(-0.5 * Math.Pow(Math.Log(GDD / x0) / b, 2)) / GDD
        Return SigmaPlot_Eq
    End Function

    Private Sub btnFillMissignKcrETa_Click(sender As Object, e As RoutedEventArgs) Handles btnFillMissignKcrETa.Click
        Calculate_missing_Kcr_ETa_Data()
        Calculate_ETa_Deficit()
        MessageBox.Show("Calculated missing Kcr and ETa values.")
        'main_window_shared.Load_WaterBalance_DagaGrid()

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
                ETa = smd_data(i).ETa_8214
                deficit_current = (deficit_ini + ETa) - (eff_irrig + eff_prefip)
                If deficit_current < 0 Then
                    deficit_current = 0
                End If
                smd_data(i).Deficit_8214 = Math.Round(deficit_current, 4)
                deficit_ini = deficit_current
            Next
            sidss_context.SaveChanges()

        End Using

    End Sub
End Class
