Public Class UsuarioWindowxaml
    Public Property UsuarioEditado As Usuario = Nothing

    Public Sub New(Optional usuario As Usuario = Nothing)
        InitializeComponent()
        If usuario IsNot Nothing Then
            UsuarioEditado = usuario
            txtUsuario.Text = usuario.id_usu
            txtUsuario.IsEnabled = False
            txtPassword.Password = usuario.password_usu
            txtDescripcion.Text = usuario.des_usu
            txtCoop.Text = usuario.coop_usu
            cmbNivel.Text = usuario.niv_usu.ToString()
        End If
    End Sub
    Private Sub BtnGuardar_Click(sender As Object, e As RoutedEventArgs)
        Dim nuevoUsuario As New Usuario With {
            .id_usu = txtUsuario.Text,
            .password_usu = txtPassword.Password,
            .des_usu = txtDescripcion.Text,
            .coop_usu = txtCoop.Text,
            .niv_usu = CInt(cmbNivel.Text)
        }

        Dim servicio As New UsuarioServices()
        If UsuarioEditado Is Nothing Then
            servicio.Insertar(nuevoUsuario)
        Else
            servicio.Actualizar(nuevoUsuario)
        End If

        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub BtnCancelar_Click(sender As Object, e As RoutedEventArgs)
        Me.DialogResult = False
        Me.Close()
    End Sub

    Private Sub UsuarioWindowxaml_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
    End Sub
End Class
