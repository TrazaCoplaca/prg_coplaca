Imports System.Text.Json.Serialization


Public Class ClasImportProductos
    <JsonPropertyName("NoField")>
    Public Property idProducto As String

    <JsonPropertyName("DescriptionField")>
    Public Property Descripcion As String

    <JsonPropertyName("KilosCajaField")>
    Public Property Kilos As Integer

    <JsonPropertyName("TipoProd")>
    Public Property TipoProducto As String

    <JsonPropertyName("UNEGAP")>
    Public Property unegap As Boolean

    <JsonPropertyName("NAtur")>
    Public Property Natur As Boolean

    <JsonPropertyName("Produccion")>
    Public Property Produccion As String
End Class

Public Class ImpProductosResponse
    Public Property value As List(Of ClasImportProductos)
End Class
