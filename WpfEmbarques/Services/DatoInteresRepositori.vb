Imports System.Data.SqlClient
Imports Microsoft.Data.SqlClient

Public Class DatoInteresRepository


    Public Function ObtenerPorCabecera(idVale As Long, idCoop As String, idEmpa As String) As List(Of ClasDatiInteres)
        Dim lista As New List(Of ClasDatiInteres)

        Using conn = ConexionDb.GetConnection()
            Dim query As String = "
                SELECT * FROM DatoInteres 
                WHERE idVale_din = @idVale AND idCoop_din = @idCoop AND idEmpa_din = @idEmpa
                ORDER BY Nlin_din"
            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@idVale", idVale)
            cmd.Parameters.AddWithValue("@idCoop", idCoop)
            cmd.Parameters.AddWithValue("@idEmpa", idEmpa)

            conn.Open()
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    lista.Add(New ClasDatiInteres With {
                        .idVale_din = CLng(reader("idVale_din")),
                        .idCoop_din = reader("idCoop_din").ToString(),
                        .idEmpa_din = reader("idEmpa_din").ToString(),
                        .Nlin_din = CLng(reader("Nlin_din")),
                        .Producto_din = reader("Producto_din").ToString(),
                        .Termografo_din = reader("Termografo_din").ToString(),
                        .Temp_din = reader("Temp_din").ToString()
                    })
                End While
            End Using
        End Using

        Return lista
    End Function

    Public Sub Insertar(dato As ClasDatiInteres)
        Using conn = ConexionDb.GetConnection()

            ' 1️⃣ Obtener el siguiente número de línea
            Dim queryMax As String = "
            SELECT ISNULL(MAX(Nlin_din), 0) + 1 
            FROM DatoInteres
            WHERE idVale_din = @idVale
              AND idCoop_din = @idCoop
              AND idEmpa_din = @idEmpa"

            Dim cmdMax As New SqlCommand(queryMax, conn)
            cmdMax.Parameters.AddWithValue("@idVale", dato.idVale_din)
            cmdMax.Parameters.AddWithValue("@idCoop", dato.idCoop_din)
            cmdMax.Parameters.AddWithValue("@idEmpa", dato.idEmpa_din)

            conn.Open()
            Dim nextLine As Long = CLng(cmdMax.ExecuteScalar())
            dato.Nlin_din = nextLine   ' 🔹 Asignamos el nuevo número de línea

            ' 2️⃣ Insertar la línea con Nlin_din ya calculado
            Dim query As String = "
            INSERT INTO DatoInteres 
                (idVale_din, idCoop_din, idEmpa_din, Nlin_din, Producto_din, Termografo_din, Temp_din)
            VALUES 
                (@idVale, @idCoop, @idEmpa, @Nlin, @Producto, @Termografo, @Temp)"

            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@idVale", dato.idVale_din)
            cmd.Parameters.AddWithValue("@idCoop", dato.idCoop_din)
            cmd.Parameters.AddWithValue("@idEmpa", dato.idEmpa_din)
            cmd.Parameters.AddWithValue("@Nlin", dato.Nlin_din)
            cmd.Parameters.AddWithValue("@Producto", dato.Producto_din)
            cmd.Parameters.AddWithValue("@Termografo", dato.Termografo_din)
            cmd.Parameters.AddWithValue("@Temp", dato.Temp_din)

            cmd.ExecuteNonQuery()
        End Using
    End Sub


    Public Sub Actualizar(dato As ClasDatiInteres)
        Using conn = ConexionDb.GetConnection()
            Dim query As String = "
                UPDATE DatoInteres 
                SET Producto_din = @Producto,
                    Termografo_din = @Termografo,
                    Temp_din = @Temp
                WHERE idVale_din = @idVale AND idCoop_din = @idCoop AND idEmpa_din = @idEmpa AND Nlin_din = @Nlin"
            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@idVale", dato.idVale_din)
            cmd.Parameters.AddWithValue("@idCoop", dato.idCoop_din)
            cmd.Parameters.AddWithValue("@idEmpa", dato.idEmpa_din)
            cmd.Parameters.AddWithValue("@Nlin", dato.Nlin_din)
            cmd.Parameters.AddWithValue("@Producto", dato.Producto_din)
            cmd.Parameters.AddWithValue("@Termografo", dato.Termografo_din)
            cmd.Parameters.AddWithValue("@Temp", dato.Temp_din)

            conn.Open()
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Sub Eliminar(dato As ClasDatiInteres)
        Using conn = ConexionDb.GetConnection()
            Dim query As String = "
                DELETE FROM DatoInteres 
                WHERE idVale_din = @idVale AND idCoop_din = @idCoop AND idEmpa_din = @idEmpa AND Nlin_din = @Nlin"
            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@idVale", dato.idVale_din)
            cmd.Parameters.AddWithValue("@idCoop", dato.idCoop_din)
            cmd.Parameters.AddWithValue("@idEmpa", dato.idEmpa_din)
            cmd.Parameters.AddWithValue("@Nlin", dato.Nlin_din)

            conn.Open()
            cmd.ExecuteNonQuery()
        End Using
    End Sub
    Public Function ObteneListaDatoInteres(idVale As Long, idCoop As String, idEmpa As String) As List(Of ClasDatiInteres)
        Dim lista As New List(Of ClasDatiInteres)()

        Using conn = ConexionDb.GetConnection

            conn.Open()
            Dim query As String =
                "SELECT idVale_din, idCoop_din, idEmpa_din, Nlin_din, " &
                "Producto_din, Termografo_din, Temp_din " &
                "FROM DatoInteres " &
                "WHERE idVale_din = @idVale " &
                "  AND idCoop_din = @idCoop " &
                "  AND idEmpa_din = @idEmpa " &
                "ORDER BY Nlin_din"

            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@idVale", idVale)
                cmd.Parameters.AddWithValue("@idCoop", idCoop)
                cmd.Parameters.AddWithValue("@idEmpa", idEmpa)

                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim dato As New ClasDatiInteres With {
                            .idVale_din = CLng(reader("idVale_din")),
                            .idCoop_din = reader("idCoop_din").ToString(),
                            .idEmpa_din = reader("idEmpa_din").ToString(),
                            .Nlin_din = CLng(reader("Nlin_din")),
                            .Producto_din = If(IsDBNull(reader("Producto_din")), Nothing, reader("Producto_din").ToString()),
                            .Termografo_din = If(IsDBNull(reader("Termografo_din")), Nothing, reader("Termografo_din").ToString()),
                            .Temp_din = If(IsDBNull(reader("Temp_din")), Nothing, reader("Temp_din").ToString())
                        }

                        lista.Add(dato)
                    End While
                End Using
            End Using
        End Using

        Return lista
    End Function
End Class

