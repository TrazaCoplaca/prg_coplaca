Imports Microsoft.Data.SqlClient
Imports System.IO
Imports Newtonsoft.Json.Linq

Public Class ConexionDb
    Public Shared connectionString As String

    Shared Sub New()
        ' Try
        ' Dim jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json")
        ' Dim jsonText = File.ReadAllText(jsonPath)
        ' Dim json As JObject = JObject.Parse(jsonText)
        connectionString = ConfigGlobal.CadenaConexion
        ' Catch ex As Exception
        ' MessageBox.Show("Error leyendo la cadena de conexión: " & ex.Message)
        ' End Try
    End Sub

    Public Shared Function GetConnection() As SqlConnection
        Return New SqlConnection(ConfigGlobal.CadenaConexion)
    End Function
End Class

