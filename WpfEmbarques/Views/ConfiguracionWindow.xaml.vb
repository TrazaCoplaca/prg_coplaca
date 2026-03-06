Public Class ConfiguracionWindow
    Public Property ConfiguracionEditada As Configuracion = Nothing

    Public Sub New(Optional config As Configuracion = Nothing)
        InitializeComponent()
        Dim servicio As New ConfiguracionService
        config = servicio.ObtenerUnaConfiguracion(SesionUsuario.idEmpa & SesionUsuario.IdCoop)
        If config IsNot Nothing Then
            ConfiguracionEditada = config
            txtId.Text = config.id_conf
            txtId.IsEnabled = False
            txtCoop.Text = config.idCoop_conf
            txtEmpa.Text = config.idEmpa_conf
            txtNombre.Text = config.NomEmp_conf
            txtDireccion1.Text = config.Dire_Conf
            txtDireccion2.Text = config.Direc2_conf
            txtIsla.Text = config.isla_conf
            chkSoloPlatanos.IsChecked = config.SoloPlatanos_conf
            chkGlobal.IsChecked = config.global_conf
        End If
    End Sub

    Private Sub BtnGuardar_Click(sender As Object, e As RoutedEventArgs)
        Dim nuevaConfig As New Configuracion With {
            .id_conf = txtId.Text,
            .idCoop_conf = txtCoop.Text,
            .idEmpa_conf = txtEmpa.Text,
            .NomEmp_conf = txtNombre.Text,
            .Dire_Conf = txtDireccion1.Text,
            .Direc2_conf = txtDireccion2.Text,
            .isla_conf = txtIsla.Text,
            .SoloPlatanos_conf = chkSoloPlatanos.IsChecked,
            .global_conf = chkGlobal.IsChecked
        }

        Dim servicio As New ConfiguracionService()
        If ConfiguracionEditada Is Nothing Then
            servicio.ActualizarConfiguraciones(New List(Of Configuracion) From {nuevaConfig})
        Else
            servicio.ActualizarConfiguraciones(New List(Of Configuracion) From {nuevaConfig})
        End If

        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub BtnCancelar_Click(sender As Object, e As RoutedEventArgs)
        Me.DialogResult = False
        Me.Close()
    End Sub

    Private Sub ConfiguracionWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        ' Puedes cargar datos adicionales si los necesitas
    End Sub

    Private Sub BtnConfig_Click(sender As Object, e As RoutedEventArgs)
        Dim configwin As New ConexionWindow()
        If configwin.ShowDialog() <> True Then
            'MessageBox.Show("Debe configurar la conexion para continuar.", "Conexión Requerida")
            Application.Current.Shutdown()
            'Return True
        End If
        Dim dataSource = SeguridadHelper.Desencriptar(My.Settings.clientIdBc)
        Dim catalog = SeguridadHelper.Desencriptar(My.Settings.EncryptedCatalog)
        Dim userId = SeguridadHelper.Desencriptar(My.Settings.EncryptedUserID)
        Dim password = SeguridadHelper.Desencriptar(My.Settings.EncryptedPassword)
        ConfigGlobal.CadenaConexion = $"Data Source={dataSource};Initial Catalog={catalog};User ID={userId};Password={password};TrustServerCertificate=True;"

    End Sub
End Class
