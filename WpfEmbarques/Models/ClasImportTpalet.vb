Imports System.Text.Json.Serialization

Public Class ClasImportTpalet
    <JsonPropertyName("id")>
    Public Property idPalet As Integer

    <JsonPropertyName("name")>
    Public Property NomPalet As String

    <JsonPropertyName("caption")>
    Public Property Caption As String

End Class
Public Class ImpTpaletResponse
    Public Property values As List(Of ClasImportTpalet)
End Class

