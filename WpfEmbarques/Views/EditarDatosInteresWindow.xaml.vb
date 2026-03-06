Partial Public Class EditarDatoInteresWindow
    Inherits Window

    Public Property Dato As ClasDatiInteres

    Public Sub New(linea As ClasDatiInteres)
        InitializeComponent()
        If linea Is Nothing Then
            Dato = New ClasDatiInteres
        Else
            Dato = linea

        End If

        TxtProducto.Text = Dato.Producto_din
        TxtTermografo.Text = Dato.Termografo_din
        TxtTemperatura.Text = Dato.Temp_din
    End Sub

    Private Sub BtnGuardar_Click(sender As Object, e As RoutedEventArgs)
        If String.IsNullOrWhiteSpace(txtProducto.Text) Then
            MessageBox.Show("El campo Producto no puede estar vacío.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning)
            txtProducto.Focus()
            Return
        End If

        If String.IsNullOrWhiteSpace(txtTermografo.Text) Then
            MessageBox.Show("El campo Termógrafo no puede estar vacío.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning)
            txtTermografo.Focus()
            Return
        End If

        If String.IsNullOrWhiteSpace(txtTemperatura.Text) Then
            MessageBox.Show("El campo Temperatura no puede estar vacío.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning)
            txtTemperatura.Focus()
            Return
        End If

        ' Asignar valores
        Dato.Producto_din = txtProducto.Text.Trim()
        Dato.Termografo_din = txtTermografo.Text.Trim()
        Dato.Temp_din = txtTemperatura.Text.Trim()

        DialogResult = True
    End Sub

    Private Sub BtnCancelar_Click(sender As Object, e As RoutedEventArgs)
        DialogResult = False
    End Sub



    Private Sub TxtTemperatura_LostFocus(sender As Object, e As RoutedEventArgs) Handles TxtTemperatura.LostFocus
        Dim texto As String = TxtTemperatura.Text.Trim()

        ' Intentamos convertirlo a número
        Dim numero As Double
        If Double.TryParse(texto.Replace("°C", "").Trim().Replace(",", "."), Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, numero) Then
            ' Si es válido, lo mostramos con formato "número °C"
            TxtTemperatura.Text = numero.ToString("+0.##", Globalization.CultureInfo.InvariantCulture) & " °C"
        Else
            ' Si no es válido, puedes dejarlo vacío o mostrar advertencia
            TxtTemperatura.Text = ""
            ' MessageBox.Show("Introduce una temperatura válida", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning)
        End If
    End Sub
End Class

