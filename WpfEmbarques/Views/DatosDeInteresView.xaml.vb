Imports System.Collections.ObjectModel
Imports DocumentFormat.OpenXml.Wordprocessing

Partial Public Class DatosDeInteresView
    Inherits Window

    Private idVale As Long
    Private idCoop As String
    Private idEmpa As String
    Private repo As New DatoInteresRepository()
    Public Property Lineas As ObservableCollection(Of ClasDatiInteres)

    Public Sub New(idVale As Long, idCoop As String, idEmpa As String)
        InitializeComponent()
        Me.idVale = idVale
        Me.idCoop = idCoop
        Me.idEmpa = idEmpa
        Lineas = New ObservableCollection(Of ClasDatiInteres)(repo.ObtenerPorCabecera(idVale, idCoop, idEmpa))
        DataContext = Me
    End Sub

    Private Sub BtnAddLinea_Click(sender As Object, e As RoutedEventArgs)
        Dim nueva As New ClasDatiInteres With {
            .idVale_din = idVale,
            .idCoop_din = idCoop,
            .idEmpa_din = idEmpa,
            .Nlin_din = If(Lineas.Count = 0, 1, Lineas.Max(Function(x) x.Nlin_din) + 1)
        }
        Lineas.Add(nueva)
    End Sub

    Private Sub BtnGuardar_Click(sender As Object, e As RoutedEventArgs)
        ' Guardar todas las líneas (puedes mejorar esto con validaciones y control de cambios)
        For Each linea In Lineas
            If repo.ObtenerPorCabecera(idVale, idCoop, idEmpa).Any(Function(x) x.Nlin_din = linea.Nlin_din) Then
                repo.Actualizar(linea)
            Else
                repo.Insertar(linea)
            End If
        Next
        MessageBox.Show("Datos guardados.")
        Me.Close()
    End Sub

    Private Sub BtnEliminarLinea_Click(sender As Object, e As RoutedEventArgs)
        Dim seleccionada = CType(dataGridLineas.SelectedItem, ClasDatiInteres)
        If MsgBox("Desea eliminar la linea seleccionada", vbYesNo) = vbYes Then
            If seleccionada IsNot Nothing Then
                repo.Eliminar(seleccionada)
                Lineas.Remove(seleccionada)
            End If
        End If
    End Sub

    Private Sub BtnEditarLinea_Click(sender As Object, e As RoutedEventArgs)
        Dim ventana As New EditarDatoInteresWindow(dataGridLineas.SelectedItem)
        ventana.Owner = Me
        ventana.ShowDialog()
        For Each linea In Lineas
            If repo.ObtenerPorCabecera(idVale, idCoop, idEmpa).Any(Function(x) x.Nlin_din = linea.Nlin_din) Then
                repo.Actualizar(linea)
            Else
                repo.Insertar(linea)
            End If
        Next
        Dim nuevasLineas = repo.ObtenerPorCabecera(idVale, idCoop, idEmpa)
        Lineas.Clear()
        For Each l In nuevasLineas
            Lineas.Add(l)
        Next
        Dim nuevaSeleccion = Lineas.FirstOrDefault(Function(x) x.Nlin_din = ventana.Dato.Nlin_din)
        If nuevaSeleccion IsNot Nothing Then
            dataGridLineas.SelectedItem = nuevaSeleccion
            dataGridLineas.ScrollIntoView(nuevaSeleccion)
        End If
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim ventana As New EditarDatoInteresWindow(Nothing)
        ventana.Owner = Me
        If ventana.ShowDialog() = True Then
            ventana.Dato.idCoop_din = idCoop
            ventana.Dato.idEmpa_din = idEmpa
            ventana.Dato.idVale_din = idVale

            repo.Insertar(ventana.Dato)
            Dim nuevasLineas = repo.ObtenerPorCabecera(idVale, idCoop, idEmpa)
            Lineas.Clear()
            For Each l In nuevasLineas
                Lineas.Add(l)
            Next
            Dim nuevaSeleccion = Lineas.FirstOrDefault(Function(x) x.Nlin_din = ventana.Dato.Nlin_din)
            If nuevaSeleccion IsNot Nothing Then
                dataGridLineas.SelectedItem = nuevaSeleccion
                dataGridLineas.ScrollIntoView(nuevaSeleccion)
            End If

        End If
    End Sub
End Class
