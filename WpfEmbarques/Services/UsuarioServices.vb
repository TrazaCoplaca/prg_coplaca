Imports System.Data.SqlClient
Imports Microsoft.Data.SqlClient
Imports System.IO
Imports Newtonsoft.Json
Imports System.Configuration
Imports System.Xml.XPath

Public Class UsuarioServices
    Private ReadOnly connectionString As String
    Private ReadOnly connectionStringAzure = ConfigurationManager.ConnectionStrings("ConexionAzureSql").ConnectionString

    '  Public Sub New()
    '      Dim configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json")
    '      Dim json = File.ReadAllText(configPath)
    '      Dim config = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(json)
    '      connectionString = config("ConnectionString")
    '  End Sub

    Public Async Function ObtenerUsuarios() As Task(Of List(Of Usuario))
        Try
            Dim lista As New List(Of Usuario)
            Dim conn As SqlConnection = ConexionDb.GetConnection()

            If conn IsNot Nothing Then
                Using conn
                    conn.Open()
                    Dim cmd As New SqlCommand("SELECT * FROM dbUsuario", conn)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            lista.Add(New Usuario With {
                        .id_usu = reader("id_usu").ToString(),
                        .password_usu = reader("password_usu").ToString(),
                        .coop_usu = reader("coop_usu").ToString(),
                        .niv_usu = Convert.ToInt32(reader("niv_usu")),
                        .des_usu = reader("des_usu").ToString()
                    })
                        End While
                    End Using
                End Using
            End If
            Return lista
        Catch ex As Exception
            MsgBox("error al conectar", ex.Message)
            Return Nothing
        End Try

    End Function

    Public Function ObtenerUnUsuario(idUsuario As String) As Usuario

Reintentar:

        Try
            Using conn = ConexionDb.GetConnection()
                conn.Open()

                Dim cmd As New SqlCommand(
                "SELECT * FROM dbUsuario WHERE id_usu = @idUsu", conn)
                cmd.Parameters.AddWithValue("@idUsu", idUsuario)

                Using reader = cmd.ExecuteReader()
                    If reader.Read() Then
                        Dim rsUsuario As New Usuario With {
                        .id_usu = reader("id_usu").ToString(),
                        .password_usu = reader("password_usu").ToString(),
                        .coop_usu = reader("coop_usu").ToString(),
                        .niv_usu = Convert.ToInt32(reader("niv_usu")),
                        .des_usu = reader("des_usu").ToString()
                    }
                        Return rsUsuario
                    Else
                        Return Nothing
                    End If
                End Using
            End Using

        Catch ex As Exception

            ' Abrir ventana de conexión
            Dim resultado As Boolean? = Nothing

            Application.Current.Dispatcher.Invoke(
            Sub()
                Dim w As New ConexionWindow()
                resultado = w.ShowDialog()
            End Sub
        )

            If resultado = True Then
                GoTo Reintentar
            Else
                Return Nothing
            End If

        End Try

    End Function

    Public Sub Insertar(usuario As Usuario)
        Using conn As New SqlConnection(connectionString)
            conn.Open()
            Dim cmd As New SqlCommand("
                INSERT INTO dbUsuario (id_usu, password_usu, coop_usu, niv_usu, des_usu) 
                VALUES (@id, @pass, @coop, @niv, @desc)", conn)

            cmd.Parameters.AddWithValue("@id", usuario.id_usu)
            cmd.Parameters.AddWithValue("@pass", CalcularHash(usuario.password_usu))
            cmd.Parameters.AddWithValue("@coop", usuario.coop_usu)
            cmd.Parameters.AddWithValue("@niv", usuario.niv_usu)
            cmd.Parameters.AddWithValue("@desc", usuario.des_usu)

            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Sub Actualizar(usuario As Usuario)
        Using conn As New SqlConnection(connectionString)
            conn.Open()
            Dim cmd As New SqlCommand("
                UPDATE dbUsuario SET 
                    password_usu = @pass,
                    coop_usu = @coop,
                    niv_usu = @niv,
                    des_usu = @desc 
                WHERE id_usu = @id", conn)

            cmd.Parameters.AddWithValue("@id", usuario.id_usu)
            cmd.Parameters.AddWithValue("@pass", CalcularHash(usuario.password_usu))
            cmd.Parameters.AddWithValue("@coop", usuario.coop_usu)
            cmd.Parameters.AddWithValue("@niv", usuario.niv_usu)
            cmd.Parameters.AddWithValue("@desc", usuario.des_usu)

            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Sub Eliminar(idUsuario As String)
        Using conn As New SqlConnection(connectionString)
            conn.Open()
            Dim cmd As New SqlCommand("DELETE FROM dbUsuario WHERE id_usu = @id", conn)
            cmd.Parameters.AddWithValue("@id", idUsuario)
            cmd.ExecuteNonQuery()
        End Using
    End Sub
End Class
