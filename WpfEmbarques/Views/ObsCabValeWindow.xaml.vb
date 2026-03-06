Public Class ObsCabValeWindow
    Public Property Observaciones As String = ""
    Public Sub New(Optional _Observaciones As String = "")

        ' Esta llamada es exigida por el diseñador.
        InitializeComponent()
        Observaciones = _Observaciones
        txtObservaciones.Text = Observaciones
        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().

    End Sub

    Private Sub BtnGuardar_Click(sender As Object, e As RoutedEventArgs)
        Observaciones = If(String.IsNullOrEmpty(txtObservaciones.Text), "", txtObservaciones.Text)
        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub BtnCancelar_Click(sender As Object, e As RoutedEventArgs)

    End Sub
End Class
