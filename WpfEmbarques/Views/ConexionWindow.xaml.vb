Public Class ConexionWindow

    Private Sub BtnGuardar_Click(sender As Object, e As RoutedEventArgs)
        My.Settings.clientIdBc = SeguridadHelper.Encriptar(txtDataSource.Text)
        My.Settings.EncryptedCatalog = SeguridadHelper.Encriptar(txtCatalog.Text)
        My.Settings.EncryptedUserID = SeguridadHelper.Encriptar(txtUserId.Text)
        My.Settings.EncryptedPassword = SeguridadHelper.Encriptar(txtPassword.Password)
        My.Settings.Save()

        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub BtnCancelar_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub ConexionWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Try
            If Not String.IsNullOrWhiteSpace(My.Settings.clientIdBc) Then
                txtDataSource.Text = SeguridadHelper.Desencriptar(My.Settings.clientIdBc)
            End If

            If Not String.IsNullOrWhiteSpace(My.Settings.EncryptedCatalog) Then
                txtCatalog.Text = SeguridadHelper.Desencriptar(My.Settings.EncryptedCatalog)
            End If

            If Not String.IsNullOrWhiteSpace(My.Settings.EncryptedUserID) Then
                txtUserId.Text = SeguridadHelper.Desencriptar(My.Settings.EncryptedUserID)
            End If

            If Not String.IsNullOrWhiteSpace(My.Settings.EncryptedPassword) Then
                txtPassword.Password = SeguridadHelper.Desencriptar(My.Settings.EncryptedPassword)
            End If

        Catch ex As Exception
            MessageBox.Show("Error al cargar la configuración: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class
