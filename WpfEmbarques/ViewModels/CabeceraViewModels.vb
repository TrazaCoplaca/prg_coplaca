Imports System.Collections.ObjectModel

Public Class CabeceraViewModels
    Public Property ListaNavieras As ObservableCollection(Of Naviera)

    Public Sub New()
        ' Cargar datos de ejemplo o desde DB
        Dim Servicio As New ServicioGlobal
        ' ListaNavieras = New ObservableCollection(Of Naviera)(Servicio.ObtenerNavieras)
    End Sub
End Class
