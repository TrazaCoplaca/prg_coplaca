Imports System.Text.Json.Serialization

Public Class ClasImportCajas
    <JsonPropertyName("id")>
    Public Property idCaja As Integer
    <JsonPropertyName("name")>
    Public Property desCaja As String
    <JsonPropertyName("caption")>
    Public Property caption As String

End Class

Public Class ImpCajasResponse
    Public Property enumName As String
    Public Property values As List(Of ClasImportCajas)
End Class
