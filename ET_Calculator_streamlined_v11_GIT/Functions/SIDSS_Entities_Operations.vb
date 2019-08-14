
Imports System.Data.Entity
Imports ET_Calculator_streamlined_v11_GIT.MainWindow.Shared_controls
Imports ET_Calculator_streamlined_v11_GIT.SIDSS_Entities

Public Class SIDSS_Entities_Operations
    Public Sub Reset_RefET_Table()
        Using SIDSS_context = New SIDSS_Entities()

            Try
                ' Delete all rows of the table.
                SIDSS_context.Database.ExecuteSqlCommand("DELETE FROM Ref_ET_Table;")
                'Clean extra space leftover from previous data.
                SIDSS_context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "VACUUM;")

                '//Reset the ID number to start from 1.
                SIDSS_context.Database.ExecuteSqlCommand("update SQLITE_SEQUENCE set seq = 0 where name = 'Ref_ET_Table';")

            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try

        End Using

        Load_RefET_DagaGrid()

    End Sub
    Public Sub Load_RefET_DagaGrid()

        ' Encapsulating database in "using" statement to close the database immediately.
        Using entity_table As New SIDSS_Entities()
            ' Read database table from Entity Framework database and convert it to list for displaying into datagrid.
            main_window_shared.DgvRefET.ItemsSource = entity_table.Ref_ET_Table.ToList()
        End Using

    End Sub
    Public Sub Load_WaterBalance_DagaGrid()

        ' Encapsulating database in "using" statement to close the database immediately.
        Using entity_table As New ET_Calculator_streamlined_v11_GIT.SIDSS_Entities()
            ' Read database table from Entity Framework database and convert it to list for displaying into datagrid.
            main_window_shared.dgvWaterBalance.ItemsSource = entity_table.SMD_Daily.ToList()
        End Using

    End Sub
End Class
