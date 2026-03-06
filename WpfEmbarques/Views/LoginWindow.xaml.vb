Imports WpfEmbarques.ServicioGlobal
Public Class LoginWindow
    Inherits Window
    Private interno As Boolean = False
    Private passwordInterno As String = ""
    Public Sub New()

        ' Esta llamada es exigida por el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
        txtUsuario.Text = My.Settings.UsuarioSession
        txtContrasena.Password = My.Settings.PwdSesion

    End Sub
    Private Sub BtnIniciarSesion_Click(sender As Object, e As RoutedEventArgs)
        Dim usuario = txtUsuario.Text
        Dim contrasena = CalcularHash(txtContrasena.Password)
        Dim rsUsuario As New UsuarioServices
        Dim Config As New ConfiguracionService
        'CoplacaBcc2025
        ' usuario = "Administrador"
        ' contrasena = "1234"

        If usuario = "Administrador" AndAlso PwdProgram = contrasena Then
            'Guardar sesión
            SesionUsuario.UsuarioActual = usuario
            SesionUsuario.EstadoAutentificado = True
            SesionUsuario.CoopActual = "9999"
            SesionUsuario.UsuarioActual = "Administrador"
            SesionUsuario.idEmpa = "99"
            SesionUsuario.IdCoop = "99"
            SesionUsuario.globalGap = True
            SesionUsuario.SoloPlatanos = True
            My.Settings.UsuarioSession = SesionUsuario.UsuarioActual
            My.Settings.PwdSesion = txtContrasena.Password
            My.Settings.Save()
            ' Establecer resultado positivo y cerrar ventana de login
            Me.DialogResult = True
            SesionUsuario.NivelUsuario = 9
        Else
            Dim ListaUsuario = rsUsuario.ObtenerUnUsuario(usuario)
            If ListaUsuario IsNot Nothing Then
                If ListaUsuario.password_usu = CalcularHash(txtContrasena.Password) Then
                    SesionUsuario.UsuarioActual = ListaUsuario.id_usu
                    SesionUsuario.EstadoAutentificado = True
                    SesionUsuario.CoopActual = ListaUsuario.coop_usu
                    SesionUsuario.UsuarioActual = ListaUsuario.id_usu
                    SesionUsuario.NivelUsuario = ListaUsuario.niv_usu
                    If SesionUsuario.CoopActual IsNot Nothing AndAlso Len(SesionUsuario.CoopActual) = 4 Then
                        SesionUsuario.idEmpa = Mid(SesionUsuario.CoopActual, 1, 2)
                        SesionUsuario.IdCoop = Mid(SesionUsuario.CoopActual, 3, 2)
                    End If
                    Dim configEmp = Config.ObtenerUnaConfiguracion(SesionUsuario.CoopActual)
                    If configEmp IsNot Nothing Then
                        SesionUsuario.globalGap = configEmp.global_conf
                        SesionUsuario.SoloPlatanos = configEmp.SoloPlatanos_conf
                    End If
                    My.Settings.UsuarioSession = SesionUsuario.UsuarioActual
                    My.Settings.PwdSesion = txtContrasena.Password
                    My.Settings.Save()
                    Me.DialogResult = True
                Else
                    MessageBox.Show("Credenciales incorrectas", "Login", MessageBoxButton.OK, MessageBoxImage.Error)

                End If
            Else

                MessageBox.Show("Credenciales incorrectas", "Login", MessageBoxButton.OK, MessageBoxImage.Error)

            End If
        End If
    End Sub
    Private Sub txtContrasena_PasswordChanged(sender As Object, e As RoutedEventArgs)
        If interno Then Exit Sub

        passwordInterno = txtContrasena.Password

        interno = True
        TxtVisible.Text = passwordInterno
        interno = False
    End Sub

    Private Sub txtVisible_TextChanged(sender As Object, e As TextChangedEventArgs)
        If interno Then Exit Sub

        passwordInterno = TxtVisible.Text

        interno = True
        txtContrasena.Password = passwordInterno
        interno = False
    End Sub

    Private Sub chkMostrar_Checked(sender As Object, e As RoutedEventArgs)
        txtContrasena.Visibility = Visibility.Collapsed
        TxtVisible.Visibility = Visibility.Visible
    End Sub

    Private Sub chkMostrar_Unchecked(sender As Object, e As RoutedEventArgs)
        txtContrasena.Visibility = Visibility.Visible
        TxtVisible.Visibility = Visibility.Collapsed
    End Sub

    ' Método para obtener la contraseña desde fuera:
    Public Function ObtenerPassword() As String
        Return passwordInterno
    End Function
End Class
