Imports System.Windows.Threading
Partial Public Class InicioView
    Inherits UserControl

    Private reloj As DispatcherTimer
    Public Sub New()
        InitializeComponent()
        CargarDatosUsuario()
        IniciarReloj()
    End Sub

    Private Sub CargarDatosUsuario()
        txtUsuarioActual.Text = $"Usuario: {SesionUsuario.UsuarioActual} "
    End Sub

    Private Sub IniciarReloj()
        reloj = New DispatcherTimer()
        reloj.Interval = TimeSpan.FromSeconds(1)
        AddHandler reloj.Tick, AddressOf ActualizarHora
        reloj.Start()
    End Sub

    Private Sub ActualizarHora(sender As Object, e As EventArgs)
        txtHoraActual.Text = DateTime.Now.ToString("HH:mm:ss")
    End Sub
End Class
