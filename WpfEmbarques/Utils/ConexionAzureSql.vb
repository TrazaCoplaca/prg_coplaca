Imports Microsoft.Identity.Client
Imports Microsoft.Data.SqlClient
Imports System.Threading.Tasks

Public Class ConexionAzureSql

    Private Const clientId As String = "8c10f6ca-fe7b-4eb9-a659-291386561d0c"
    Private Const tenantId As String = "c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce"
    Private Const sqlServer As String = "coplaca.database.windows.net"
    Private Const database As String = "Trazabilidad"

    Public Shared Async Function ConectarAsync() As Task(Of SqlConnection)
        Try
            ' Crear la aplicación pública para obtener el token
            Dim app = PublicClientApplicationBuilder.Create(clientId) _
                .WithAuthority(AzureCloudInstance.AzurePublic, tenantId) _
                .WithRedirectUri("http://localhost") _
                .Build()

            Dim scopes As String() = {"https://database.windows.net//.default"}

            ' Obtener el token interactivo
            Dim result = Await app.AcquireTokenInteractive(scopes).ExecuteAsync()

            ' Crear la cadena de conexión
            Dim connString As String = $"Server=tcp:{sqlServer},1433;" &
                                       $"Initial Catalog={database};" &
                                       $"Encrypt=True;" &
                                       $"TrustServerCertificate=False;"

            Dim conn As New SqlConnection(connString)
            conn.AccessToken = result.AccessToken
            Await conn.OpenAsync()

            MsgBox("✅ Conectado con éxito a Azure SQL usando Azure AD")
            Return conn

        Catch ex As Exception
            MsgBox("❌ Error al conectar: " & ex.Message)
            Return Nothing
        End Try
    End Function
End Class
