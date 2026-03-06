Imports WpfEmbarques.ServicioGlobal
Imports WpfEmbarques.ProductoService
Imports System.IO
Imports System.Configuration


Public Class PaginaPrincipal
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim ListaCabecera As New CabeceraView()
        MainContentControl.Content = ListaCabecera
    End Sub
    Private Sub VerificarLogin()
        If Not SesionUsuario.EstadoAutentificado Then
            Dim loginWin As New LoginWindow()
            loginWin.Owner = Me
            loginWin.ShowDialog()

            If Not SesionUsuario.EstadoAutentificado Then
                MessageBox.Show("Debe iniciar sesión para continuar.")
                Me.Close()
            Else
                SincronizaInformacionBC()
            End If
        End If
    End Sub

    Private Sub PaginaPrincipal_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If String.IsNullOrWhiteSpace(My.Settings.clientIdBc) Then
            CargarDatosConexion()
        End If
        Dim dataSource = SeguridadHelper.Desencriptar(My.Settings.EncryptedDataSource)
        Dim catalog = SeguridadHelper.Desencriptar(My.Settings.EncryptedCatalog)
        Dim userId = SeguridadHelper.Desencriptar(My.Settings.EncryptedUserID)
        Dim password = SeguridadHelper.Desencriptar(My.Settings.EncryptedPassword)
        ConfigGlobal.CadenaConexion = $"Data Source={dataSource};Initial Catalog={catalog};User ID={userId};Password={password};TrustServerCertificate=True;"


        VerificarLogin()
        MostrarInicio()
        Dim Cooperativa As New ConfiguracionService()

        ' Cooperativa.ObtenerUnaConfiguracion("0107")
        Me.Title = $"Sistema de Embarque, Empaquetado:" & Cooperativa.ObtenerUnaConfiguracion(SesionUsuario.idEmpa & SesionUsuario.IdCoop).NomEmp_conf


    End Sub
    Private Function CargarDatosConexion()
        Dim configwin As New ConexionWindow()
        If configwin.ShowDialog() <> True Then
            'MessageBox.Show("Debe configurar la conexion para continuar.", "Conexión Requerida")
            Application.Current.Shutdown()
            Return True
        End If
        Dim dataSource = SeguridadHelper.Desencriptar(My.Settings.EncryptedDataSource)
        Dim catalog = SeguridadHelper.Desencriptar(My.Settings.EncryptedCatalog)
        Dim userId = SeguridadHelper.Desencriptar(My.Settings.EncryptedUserID)
        Dim password = SeguridadHelper.Desencriptar(My.Settings.EncryptedPassword)
        ConfigGlobal.CadenaConexion = $"Data Source={dataSource};Initial Catalog={catalog};User ID={userId};Password={password};TrustServerCertificate=True;"

    End Function
    Private Sub MostrarInicio()
        Dim vistaInicio As New InicioView()
        MainContentControl.Content = vistaInicio

    End Sub
    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        If SesionUsuario.NivelUsuario >= 3 Then
            Dim usuariosView As New UsuariosView()

            ' Asignar la instancia al ContentControl
            MainContentControl.Content = usuariosView
        Else
            MsgBox("No tiene credenciales para acceder")
        End If
    End Sub

    Private Sub Button_Click_2(sender As Object, e As RoutedEventArgs)
        Dim usuarioView As New InicioView
        MainContentControl.Content = usuarioView
    End Sub
    Private Async Sub SincronizaInformacionBC()
        Dim pantalla As New LoadingOverlay
        pantalla.Show("Sincronizando con Bussines Central")
        'MessageBox.Show(" Sincronizando Información")
        Try

            ' esto es cuando ya no este en bussines central en un enum

            Dim cajas = Await GetEnumTipoCajasODataAsync(GetAccessToken())
            ActualizarTipoCajaEnBd(cajas)

            Dim productos = Await ObtenerProductoBC(GetAccessToken)
            ActualizarProductos(productos)

            ' esto es cuando ya no este en bussines centran en un enum

            Dim tipoPalets = Await GetEnumTipoPaletODataAsync(GetAccessToken)
            ActualizarTipoPaletEnBd(tipoPalets)

        Catch ex As Exception
            MessageBox.Show("Error: ", ex.Message)
            CargarDatosConexion()
        Finally
            pantalla.Hide()
        End Try
    End Sub

    Private Sub Button_Click_3(sender As Object, e As RoutedEventArgs)
        If SesionUsuario.NivelUsuario >= 3 Then
            Dim configuracion As New ConfiguracionWindow()
            configuracion.ShowDialog()
        Else
            MsgBox("No tiene credenciales para acceder")
        End If
    End Sub

    Private Sub Button_Click_4(sender As Object, e As RoutedEventArgs)
        Dim loginWin As New LoginWindow()
        loginWin.Owner = Application.Current.MainWindow
        loginWin.ShowDialog()

        If Not SesionUsuario.EstadoAutentificado Then
            MessageBox.Show("Debe iniciar sesión para continuar.")
            Application.Current.Shutdown()
            Return
        End If
        Dim shell As Window = Application.Current.MainWindow

        Dim abiertas = Application.Current.Windows.Cast(Of Window)().ToList
        For Each w In abiertas
            If w IsNot shell Then w.Close()
        Next w
        shell.Show()
        shell.WindowState = WindowState.Maximized
        shell.Activate()
    End Sub

    Private Async Sub Button_Click_5(sender As Object, e As RoutedEventArgs)
        'Await ConexionAzureSql.ConectarAsync()
        If SesionUsuario.NivelUsuario = 9 Then
            CargarDatosConexion()
        Else
            MsgBox("No Tiene Credenciales para Acceder")
        End If
    End Sub
End Class
