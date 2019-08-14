Public Class OutputPath
    Private Sub btnOutputPathCancel_Click(sender As Object, e As EventArgs) Handles btnOutputPathCancel.Click
        Exit Sub
    End Sub

    Private Sub btnOutputPathOK_Click(sender As Object, e As EventArgs) Handles btnOutputPathOK.Click
        Dim output_folder_path As New System.Windows.Forms.FolderBrowserDialog
        Dim result As System.Windows.Forms.DialogResult = output_folder_path.ShowDialog()
        If result = Forms.DialogResult.OK Then
            MessageBox.Show(output_folder_path.SelectedPath)
            tbxOutputPath.Text = output_folder_path.SelectedPath
            'comment
        End If

    End Sub
End Class
