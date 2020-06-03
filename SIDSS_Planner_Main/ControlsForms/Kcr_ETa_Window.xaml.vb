Imports System.ComponentModel
Imports ET_Calculator_streamlined_v11_GIT.MainWindow.Shared_controls
Imports MathNet.Numerics

Public Class Kcr_ETa_Window
    Private Sub Kcr_ETa_Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Using sidss_context As New SIDSS_Entities
            Dim gui_parameters = sidss_context.SIDS_GUI_Parameters.ToList(0)
            tbxFourier_a0.Text = gui_parameters.Fourier_a0.ToString
            tbxFourier_a1.Text = gui_parameters.Fourier_a1.ToString
            tbxFourier_a2.Text = gui_parameters.Fourier_a2.ToString
            'tbxFourier_a3.Text = gui_parameters.Fourier_a3.ToString
            tbxFourier_b1.Text = gui_parameters.Fourier_b1.ToString
            tbxFourier_b2.Text = gui_parameters.Fourier_b2.ToString
            'tbxFourier_b3.Text = gui_parameters.Fourier_b3.ToString
            tbxFourier_w.Text = gui_parameters.Fourier_w.ToString

        End Using
    End Sub

    Private Sub Kcr_ETa_Window_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Using sidss_context As New SIDSS_Entities
            Dim gui_parameters = sidss_context.SIDS_GUI_Parameters.ToList(0)
            gui_parameters.Fourier_a0 = Convert.ToDecimal(tbxFourier_a0.Text)
            gui_parameters.Fourier_a1 = Convert.ToDecimal(tbxFourier_a1.Text)
            gui_parameters.Fourier_a2 = Convert.ToDecimal(tbxFourier_a2.Text)
            'gui_parameters.Fourier_a3 = Convert.ToDecimal(tbxFourier_a3.Text)
            gui_parameters.Fourier_b1 = Convert.ToDecimal(tbxFourier_b1.Text)
            gui_parameters.Fourier_b2 = Convert.ToDecimal(tbxFourier_b2.Text)
            'gui_parameters.Fourier_b3 = Convert.ToDecimal(tbxFourier_b3.Text)
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
            Dim smd_rows_count As Integer = smd_data.Count
            ' Erase previous dataset in Kcr and ETa columns to make space for latest values.
            For r = 0 To smd_rows_count - 1
                smd_data(r).Kcr_plot = 0
                smd_data(r).ETa_plot = 0
            Next
            sidss_context.SaveChanges()
            Dim current_row = input_data_lines(0).Replace(vbLf, "").Split(vbTab)
            If current_row.Length <> 3 Then
                MessageBox.Show("Please make shure there are 3 columns of data pasted, i.e. Date, Kc & ETa.")
                Exit Sub
            End If

            Dim input_row = 1
            For j = 0 To smd_rows_count - 1
                For i = input_row To input_data_lines.Count - 1
                    current_row = input_data_lines(i).Replace(vbLf, "").Split(vbTab)
                    If current_row.Length = 3 Then
                        Try
                            date_value = current_row(0)
                            Kcr_plot = current_row(1)
                            ETa_plot = current_row(2)
                            If Convert.ToDateTime(smd_data(j).Date) = Convert.ToDateTime(date_value) Then
                                smd_data(j).Kcr_plot = Math.Round(Convert.ToDecimal(Kcr_plot), 4)
                                smd_data(j).ETa_plot = Math.Round(Convert.ToDecimal(ETa_plot), 4)
                                Exit For
                            End If
                        Catch ex As Exception
                            'MessageBox.Show(ex.Message)
                        End Try
                    End If
                Next
            Next

            sidss_context.SaveChanges()
            MessageBox.Show("Data added successfully.")

        End Using
        Me.Close()
    End Sub

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
                Dim calc_Kcr, GDD As Double

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
                For i = 0 To row_count - 1
                    interpolated_kcr_poly6 = Interpolated_Kcr_vals(1)(i)
                    interpolated_kcr_spline = Interpolated_Kcr_vals(0)(i)
                    smd_data(i).Kcr_calculated = interpolated_kcr_poly6
                    If smd_data(i).Kcr_plot = 0 Then
                        smd_data(i).Kcr_plot = interpolated_kcr_spline
                        smd_data(i).ETa_plot = smd_data(i).ETr * interpolated_kcr_spline
                    Else
                        smd_data(i).ETa_plot = smd_data(i).ETr * smd_data(i).Kcr_plot
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
        Dim y = GDD_Kcr_vals(1).ToArray()
        Dim t = GDD_vals
        Dim t_size = t.Count
        Dim y_predicted As Double()
        ReDim Preserve y_predicted(t_size - 1)
        Dim y_predicted_pol As Double()
        ReDim Preserve y_predicted_pol(t_size - 1)

        Dim spline_xy = Interpolation.LinearSpline.Interpolate(x, y)
        Dim poly_coeffs As Double() = Fit.Polynomial(x, y, 6)
        For i = 0 To t.Count - 1
            'Dim t_ As Double = 255
            y_predicted(i) = spline_xy.Interpolate(t(i))
            y_predicted_pol(i) = poly_coeffs(0) + poly_coeffs(1) * t(i) + poly_coeffs(2) * t(i) ^ 2 + poly_coeffs(3) * t(i) ^ 3 + poly_coeffs(4) * t(i) ^ 4 + poly_coeffs(5) * t(i) ^ 5 + poly_coeffs(6) * t(i) ^ 6
        Next

        Return {y_predicted, y_predicted_pol}

    End Function

    Private Function Calc_Fourier_Curve_Fit(ByVal GDD As Double)

        Dim Fourier_result As Double
        Dim a0 = Convert.ToDouble(tbxFourier_a0.Text)
        Dim w = Convert.ToDouble(tbxFourier_w.Text)
        Dim a1 = Convert.ToDouble(tbxFourier_a1.Text)
        Dim a2 = Convert.ToDouble(tbxFourier_a2.Text)
        'Dim a3 = Convert.ToDouble(tbxFourier_a3.Text)
        Dim b1 = Convert.ToDouble(tbxFourier_b1.Text)
        Dim b2 = Convert.ToDouble(tbxFourier_b2.Text)
        'Dim b3 = Convert.ToDouble(tbxFourier_b3.Text)

        Fourier_result = a0 + a1 * Math.Cos(GDD * w) + b1 * Math.Sin(GDD * w) + a2 * Math.Cos(2 * GDD * w) + b2 * Math.Sin(2 * GDD * w)
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
