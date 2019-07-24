Imports System.Data.SQLite
Imports System.Data
Imports ET_Calculator_streamlined_v11_GIT.SQL_table_operation

Public Class WaterBalanceCalculator
#Region "Inits"
    Private myConnection As New SQLiteConnection("Data Source=SIDSS_database.db; Version=3")
    Private cmd As New SQLiteCommand
    Private Tbase As Double
    Private RootMin As Double
    Private RootMax As Double
    Private Di_pref As Double = 0
    'Private Precip_col_index As Integer = 3
    'Private Irrig_col_index As Integer = 4
    'Private Tmax_col_index As Integer = 5
    'Private Tmin_col_index As Integer = 6
    'Private GDD_col_index As Integer = 7
    'Private Kc_col_index As Integer = 8
    'Private ETr_col_index As Integer = 9
    'Private ETc_col_index As Integer = 10
    'Private Drz_col_index As Integer = 11
    'Private Dmad_co_indexl As Integer = 12
    'Private Di_col_index As Integer = 13
    Private Drz_1 As Double
    Private Drz_2 As Double
    Private Drz_3 As Double
    Private Drz_4 As Double
    Private Drz_5 As Double
    Private RAW1 As Double
    Private RAW2 As Double
    Private RAW3 As Double
    Private RAW4 As Double
    Private RAW5 As Double
    Private MAD_fraction As Double
#End Region
    Public Sub Calculate_Grid_Cols(ByVal Tbase As Integer)
        Dim input_data_table As DataTable
        Dim input_data_complete As New SQL_table_operation
        input_data_table = input_data_complete.Load_SQL_DataTable("WaterBalance_Table")
        GDD_Calculate(input_data_table, Tbase)
        Kc_Calculate(input_data_table)
        Root_Profile(input_data_table)
        Calc_MAD(input_data_table)
        ETC_Calculate(input_data_table)
        Calculate_Di(input_data_table)
        'Calculate_DP(input_data_table)
        input_data_complete.Write_Final_Table(input_data_table)
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
        ' This value of 30 is just an arbitrary number to avoid dipping down of the Kc curve.
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

    Public Sub Set_root_depth(ByVal RMin As String, ByVal Rmax As String)
        RootMax = Convert.ToDouble(Rmax)
        RootMin = Convert.ToDouble(RMin)
    End Sub

    Public WriteOnly Property Set_Soil_Profile As List(Of String)
        Set(soil_prop As List(Of String))
            Drz_1 = Convert.ToDouble(soil_prop(0))
            Drz_2 = Convert.ToDouble(soil_prop(1))
            Drz_3 = Convert.ToDouble(soil_prop(2))
            Drz_4 = Convert.ToDouble(soil_prop(3))
            Drz_5 = Convert.ToDouble(soil_prop(4))
            RAW1 = Convert.ToDouble(soil_prop(5))
            RAW2 = Convert.ToDouble(soil_prop(6))
            RAW3 = Convert.ToDouble(soil_prop(7))
            RAW4 = Convert.ToDouble(soil_prop(8))
            RAW5 = Convert.ToDouble(soil_prop(9))
            MAD_fraction = Convert.ToDouble(soil_prop(10)) / 100
        End Set
    End Property


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
        Dim Precip As Double
        Dim Irrig As Double
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
            Precip = Convert.ToDouble(input_data_table.Rows(i)("Precip"))
            Irrig = Convert.ToDouble(input_data_table.Rows(i - 1)("Irrig"))
            If ((Precip + Irrig) > (ETc + Di_prev)) Then
                DP = (Precip + Irrig) - (ETc + Di_prev)
                Di = 0
            Else
                Di = -(Precip + Irrig - ETc - Di_prev)
            End If

            input_data_table.Rows(i)("DP") = DP
            input_data_table.Rows(i)("Di") = Di
            Di_prev = Di
            DP = 0

        Next

    End Sub
End Class
