Imports System.IO
Imports Newtonsoft.Json

Public Class EmailConfig
    Public Property SmtpServer As String
    Public Property SmtpPort As Integer
    Public Property EnableSSL As Boolean
    Public Property Remitente As String
    Public Property ClaveApp As String
    Public Property Destinatarios As List(Of String)
    Public Property CC As List(Of String)
    Public Property CCO As List(Of String)
End Class

Public Class EmailConfigLoader

    Public Shared Function LoadConfig(path As String) As EmailConfig

        If Not File.Exists(path) Then
            Throw New FileNotFoundException("El archivo de configuración no existe: " & path)
        End If

        Dim json = File.ReadAllText(path)
        Dim root = JsonConvert.DeserializeObject(Of Dictionary(Of String, EmailConfig))(json)

        Return root("EmailSettings")
    End Function

End Class

