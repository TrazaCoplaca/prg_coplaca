Imports System.Windows
Imports System.Windows.Controls
Partial Public Class CabeceraView

    Inherits UserControl

    Private ReadOnly servicioCabecera As New CabValeService()

    Public Sub New()
        InitializeComponent()
        CargarCabeceras()
    End Sub

    Private Sub CargarCabeceras()
        Try
            Dim listaCabeceras = servicioCabecera.ObtenerCabecerasConTransportista()
            dataGridCabeceras.ItemsSource = listaCabeceras
        Catch ex As Exception
            MessageBox.Show("Error al cargar las cabeceras: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As RoutedEventArgs)
        Dim form As New CabeceraWindow() ' Ventana que debes crear para añadir
        If form.ShowDialog() = True Then
            CargarCabeceras()
        End If
    End Sub

    Private Sub BtnEdit_Click(sender As Object, e As RoutedEventArgs)
        Dim cabeceraSeleccionada As CabValeDto = TryCast(dataGridCabeceras.SelectedItem, CabValeDto)
        If cabeceraSeleccionada IsNot Nothing Then
            Dim form As New CabeceraWindow(cabeceraSeleccionada)
            If form.ShowDialog() = True Then
                CargarCabeceras()
            End If
        Else
        MessageBox.Show("Selecciona una cabecera para editar.", "Editar", MessageBoxButton.OK, MessageBoxImage.Information)
        End If
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As RoutedEventArgs)
        Dim cabeceraSeleccionada As CabValeDto = TryCast(dataGridCabeceras.SelectedItem, CabValeDto)
        If cabeceraSeleccionada IsNot Nothing Then
            Dim resultado = MessageBox.Show("¿Estás seguro de eliminar esta cabecera?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning)
            If resultado = MessageBoxResult.Yes Then
                servicioCabecera.EliminarCabecera(cabeceraSeleccionada.idVale_cva)
                CargarCabeceras()
            End If
        Else
            MessageBox.Show("Selecciona una cabecera para borrar.", "Eliminar", MessageBoxButton.OK, MessageBoxImage.Information)
        End If
    End Sub
End Class
