
Imports System.Collections.ObjectModel

Partial Public Class UsuariosView
    Inherits UserControl

    Private usuarios As ObservableCollection(Of Usuario)
    Private repositorio As New UsuarioServices()

    Public Sub New()
        InitializeComponent()
        CargarUsuarios()
    End Sub

    Private Async Sub CargarUsuarios()
        Try
            Dim ListaUsuarios = Await repositorio.ObtenerUsuarios()
            dataGridUsuarios.ItemsSource = ListaUsuarios
        Catch ex As Exception
            MessageBox.Show("Error al cargar los usuarios " & ex.Message, vbAbort)
        End Try
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As RoutedEventArgs) Handles BtnAdd.Click
        Dim ventana As New UsuarioWindowxaml() ' O UsuarioWindow, asegúrate del nombre correcto
        ventana.Owner = Window.GetWindow(Me) ' Establece la ventana principal como propietaria
        ' Dim resultado = ventana.ShowDialog()

        If ventana.ShowDialog = True Then
            ' Vuelve a cargar los datos de usuarios si se agregó uno nuevo
            CargarUsuarios()
        End If
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As RoutedEventArgs) Handles btnEdit.Click
        Dim usuarioSeleccionado As Usuario = CType(dataGridUsuarios.SelectedItem, Usuario)
        If usuarioSeleccionado IsNot Nothing Then
            Dim ventana As New UsuarioWindowxaml(usuarioSeleccionado)
            If ventana.ShowDialog() = True Then
                CargarUsuarios()
            End If
        End If
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As RoutedEventArgs) Handles BtnDelete.Click
        Dim usuarioSeleccionado As Usuario = CType(dataGridUsuarios.SelectedItem, Usuario)
        If usuarioSeleccionado IsNot Nothing Then
            If MessageBox.Show($"¿Seguro que deseas eliminar al usuario {usuarioSeleccionado.id_usu}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) = MessageBoxResult.Yes Then
                Dim servicio As New UsuarioServices()
                servicio.Eliminar(usuarioSeleccionado.id_usu)
                CargarUsuarios()
            End If
        End If
    End Sub
End Class
