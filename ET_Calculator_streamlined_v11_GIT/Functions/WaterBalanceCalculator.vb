Imports System.Data.SQLite
Imports System.Data
Imports ET_Calculator_streamlined_v11_GIT.SQL_table_operation

Public Class WaterBalanceCalculator

#Region "Inits"
    Private ReadOnly myConnection As New SQLiteConnection("Data Source=C:\SIDSS_Database\SIDSS_database.db; Version=3")
    Private cmd As New SQLiteCommand
    Property Tbase As Double
    Property RootMin As Double
    Property RootMax As Double
    Property Di_pref As Double = 0
    Property Drz_1 As Double
    Property Drz_2 As Double
    Property Drz_3 As Double
    Property Drz_4 As Double
    Property Drz_5 As Double
    Property RAW1 As Double
    Property RAW2 As Double
    Property RAW3 As Double
    Property RAW4 As Double
    Property RAW5 As Double
    Property MAD_fraction As Double
    Property Runoff_CN As Double
    Property Irrigation_Efficiency_Fraction As Double
#End Region

    Public Sub Calculate_Grid_Cols(ByVal Tbase As Integer)

        Dim input_data_table As DataTable
        Dim input_data_complete As New SQL_table_operation
        input_data_table = input_data_complete.Load_SQL_DataTable("SMD_Daily")
        Using SIDSS_Context As New SIDSS_Entities
            Dim SMD_table = SIDSS_Context.SMD_Daily.ToArray()

        End Using

        Eff_Precip_Calculate(input_data_table)
        Eff_Irrig_Calculate(input_data_table)
        GDD_Calculate(input_data_table, Tbase)
        Kc_Calculate(input_data_table)
        Root_Profile(input_data_table)
        Calc_MAD(input_data_table)
        ETC_Calculate(input_data_table)
        Calculate_Di(input_data_table)
        'Calculate_DP(input_data_table)
        input_data_complete.Write_WaterBalance_Final_Table(input_data_table)
    End Sub

    Private Sub Eff_Precip_Calculate(ByRef input_data_table As DataTable)
        Dim Precip_in, Precip_mm As New Double
        Dim eff_precip_in, eff_precip_mm As New Double
        Dim runoff As New Double
        Dim param_s As New Double
        param_s = 250 * (100 / Runoff_CN - 1)
        Dim cutoff = param_s * 0.2
        For i = 0 To input_data_table.Rows.Count - 1
            Precip_in = Convert.ToDouble(input_data_table.Rows(i)("Precip"))
            Precip_mm = 25.4 * Precip_in

            If Precip_mm > cutoff Then
                runoff = Math.Pow((Precip_mm - 0.2 * param_s), 2) / (Precip_mm + 0.8 * param_s)
                eff_precip_mm = Precip_mm - runoff
                eff_precip_in = eff_precip_mm / 25.4
            Else
                runoff = 0
                eff_precip_in = 0
            End If
            input_data_table.Rows(i)("Eff__Precip") = eff_precip_in
            input_data_table.Rows(i)("Surface__Runoff") = runoff / 25.4

        Next
    End Sub

    Private Sub Eff_Irrig_Calculate(ByRef input_data_table As DataTable)
        Dim irrig As New Double
        Dim eff_irrig As New Double
        For i = 0 To input_data_table.Rows.Count - 1
            irrig = Convert.ToDouble(input_data_table.Rows(i)("Irrig"))
            eff_irrig = irrig * Irrigation_Efficiency_Fraction
            input_data_table.Rows(i)("Eff__Irrig") = eff_irrig
        Next
    End Sub

    Private Sub GDD_Calculate(ByRef input_data_table As DataTable, ByVal Tbase As Integer)

        Dim GDD As Double = 0
        Dim gdd_temp As Double = 0
        Dim Tmax As Double
        Dim Tmin As Double

        For i = 0 To input_data_table.Rows.Count - 1
            Tmax = Convert.ToDouble(input_data_table.Rows(i)("Tmax"))
            Tmin = Convert.ToDouble(input_data_table.Rows(i)("Tmin"))
            If Tmax > 86 Then
                Tmax = 86
            End If
            gdd_temp = (Tmax + Tmin) / 2 - Tbase
            If gdd_temp < 0 Then
                GDD += 0
            Else
                GDD += gdd_temp
            End If
            input_data_table.Rows(i)("GDD") = GDD
        Next

    End Sub

    Private Sub Kc_Calculate(ByRef input_data_table As DataTable)
        Dim GDD_val As Double
        Dim Kc_val As Double
        For i = 0 To input_data_table.Rows.Count - 1
            GDD_val = Convert.ToDouble(input_data_table.Rows(i)("GDD"))
            Kc_val = 0.29 + (-0.001 * GDD_val + 0.000003899 * GDD_val ^ 2 - 0.000000003388 * GDD_val ^ 3 + 0.00000000000119 * GDD_val ^ 4 - 0.000000000000000153 * GDD_val ^ 5)
            input_data_table.Rows(i)("Kc") = Kc_val
        Next

        'Modifying initial dip in the Kc values and keeping them constant to the value of Kc_ini on the very first day.
        Dim Kc_ini = Convert.ToDouble(input_data_table.Rows(0)("Kc"))

        ' Observing few Kc graphs the initial phase where the dip occurs is less than 30 days.
        ' This value of 30 days is just an arbitrary number to avoid dipping down of the Kc curve (i.e. ommit values lower than Kc_ini).
        For i = 0 To 30
            Kc_val = Convert.ToDouble(input_data_table.Rows(i)("Kc"))
            If Kc_val < Kc_ini Then
                input_data_table.Rows(i)("Kc") = Kc_ini
            End If
        Next

    End Sub

    Private Sub Root_Profile(ByRef input_data_table As DataTable)
        Dim Drz As Double = RootMin
        Dim Drz_prev As Double = RootMin
        Dim Kc_val As Double
        Dim Kc_min As Double = 10
        Dim Kc_max As Double = 1
        For i = 0 To input_data_table.Rows.Count - 1
            Kc_val = Convert.ToDouble(input_data_table.Rows(i)("Kc"))
            If Kc_val > Kc_max Then
                Kc_max = Kc_val
            End If
            If Kc_val < Kc_min Then
                Kc_min = Kc_val
            End If
        Next

        ' Setting max value of Kc_max to 1, instead of calculating it, since with partial dataset it is difficult to get the max value.
        Kc_max = 1
        For i = 0 To input_data_table.Rows.Count - 1
            Kc_val = Convert.ToDouble(input_data_table.Rows(i)("Kc"))
            Drz = RootMin + (Kc_val - Kc_min) / (Kc_max - Kc_min) * (RootMax - RootMin)
            If Drz > Drz_prev Then
                input_data_table.Rows(i)("Drz") = Drz
            Else
                Drz = Drz_prev
                input_data_table.Rows(i)("Drz") = Drz
            End If
            If i > 0 Then
                Drz_prev = Drz
            End If
        Next
    End Sub


    Public Sub Calc_MAD(ByRef input_data_table As DataTable)
        Dim deficit_old As Double = 0
        Dim deficit As Double = 0
        Dim Drz As Double
        Dim Drz_ini As Double = 0

        'Input current root depth as Dr
        Dim Dmax As Double = 0
        Dim MAD_ini As Double = 0

        For i = 0 To input_data_table.Rows.Count - 1
            Drz = Convert.ToDouble(input_data_table.Rows(i)("Drz"))
            If Drz > Drz_1 And i = 0 Then
                'Management Allowed Deficit at the planting depth. in/in.
                MAD_ini = MAD_fraction * (Drz_1 * RAW1 + (Drz - Drz_1) * RAW2)
                Dmax = MAD_ini
                Drz_ini = Drz
            Else
                Select Case Drz
                    Case 0 To Drz_1
                        Dmax = MAD_fraction * RAW1 * (Drz - Drz_ini)
                    Case Drz_1 + 0.00001 To Drz_2
                        Dmax = MAD_fraction * RAW2 * (Drz - Drz_ini)
                    Case Drz_2 + 0.00001 To Drz_3
                        Dmax = MAD_fraction * RAW3 * (Drz - Drz_ini)
                    Case Drz_3 + 0.00001 To Drz_4
                        Dmax = MAD_fraction * RAW4 * (Drz - Drz_ini)
                    Case Drz_4 + 0.00001 To Drz_5
                        Dmax = MAD_fraction * RAW5 * (Drz - Drz_ini)
                End Select
                Dmax += MAD_ini
                MAD_ini = Dmax
            End If
            Drz_ini = Drz
            input_data_table.Rows(i)("Dmax") = Dmax
        Next

    End Sub

    Private Sub ETC_Calculate(ByRef input_data_table As DataTable)
        Dim ETr As Double
        Dim Kc As Double
        For i = 0 To input_data_table.Rows.Count - 1
            ETr = Convert.ToDouble(input_data_table.Rows(i)("ETr"))
            Kc = Convert.ToDouble(input_data_table.Rows(i)("Kc"))
            input_data_table.Rows(i)("ETc") = ETr * Kc
        Next
    End Sub

    Private Sub Calculate_Di(ByRef input_data_table As DataTable)
        Dim ETc As Double
        Dim Effective_Precipitation As Double
        Dim Eff_Irrig As Double
        Dim Di_prev As Double = 0
        Dim Di As Double = 0
        Dim DP As Double = 0
        'Dim DP_pref As Double = 0
        Dim Dmax As Double = 0

        ' Manually setting initial deficit (i.e. planting date) as zero.
        input_data_table.Rows(0)("Di") = 0
        input_data_table.Rows(0)("DP") = 0


        For i = 1 To input_data_table.Rows.Count - 1
            Dmax = input_data_table.Rows(i)("Dmax")
            ETc = Convert.ToDouble(input_data_table.Rows(i)("ETc"))
            Effective_Precipitation = Convert.ToDouble(input_data_table.Rows(i)("Eff__Precip"))
            Eff_Irrig = Convert.ToDouble(input_data_table.Rows(i - 1)("Eff__Irrig"))
            If ((Effective_Precipitation + Eff_Irrig) > (ETc + Di_prev)) Then
                DP = (Effective_Precipitation + Eff_Irrig) - (ETc + Di_prev)
                Di = 0
            Else
                Di = -(Effective_Precipitation + Eff_Irrig - ETc - Di_prev)
            End If

            input_data_table.Rows(i)("DP") = DP
            input_data_table.Rows(i)("Di") = Di
            Di_prev = Di
            DP = 0

        Next

    End Sub
End Class
