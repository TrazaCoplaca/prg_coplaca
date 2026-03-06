Imports Microsoft.Data.SqlClient

Public Class ConfiguracionService

    Public Shared Function ObtenerConfiguraciones() As List(Of Configuracion)
        Dim lista As New List(Of Configuracion)

        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand("SELECT id_conf, idCoop_conf, idEmpa_conf, NomEmp_conf, Dire_Conf, Direc2_conf, isla_conf, SoloPlatanos_conf, global_conf FROM Configuracion", conn)

            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    Dim config As New Configuracion With {
                        .id_conf = reader("id_conf").ToString(),
                        .idCoop_conf = reader("idCoop_conf").ToString(),
                        .idEmpa_conf = reader("idEmpa_conf").ToString(),
                        .NomEmp_conf = reader("NomEmp_conf").ToString(),
                        .Dire_Conf = reader("Dire_Conf").ToString(),
                        .Direc2_conf = reader("Direc2_conf").ToString(),
                        .isla_conf = reader("isla_conf").ToString(),
                        .SoloPlatanos_conf = If(IsDBNull(reader("SoloPlatanos_conf")), Nothing, Convert.ToBoolean(reader("SoloPlatanos_conf"))),
                        .global_conf = If(IsDBNull(reader("global_conf")), Nothing, Convert.ToBoolean(reader("global_conf")))
                    }
                    lista.Add(config)
                End While
            End Using
        End Using

        Return lista
    End Function

    Public Function ObtenerUnaConfiguracion(id As String) As Configuracion
        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand("SELECT * FROM Configuracion WHERE id_conf = @id", conn)
            cmd.Parameters.AddWithValue("@id", id)

            Using reader = cmd.ExecuteReader()
                If reader.Read() Then
                    Return New Configuracion With {
                        .id_conf = reader("id_conf").ToString(),
                        .idCoop_conf = reader("idCoop_conf").ToString(),
                        .idEmpa_conf = reader("IdEmpa_conf").ToString(),
                        .NomEmp_conf = reader("NomEmp_conf").ToString(),
                        .Dire_Conf = reader("Dire_Conf").ToString(),
                        .Direc2_conf = reader("Direc2_conf").ToString(),
                        .isla_conf = reader("isla_conf").ToString(),
                        .SoloPlatanos_conf = If(IsDBNull(reader("SoloPlatanos_conf")), Nothing, Convert.ToBoolean(reader("SoloPlatanos_conf"))),
                        .global_conf = If(IsDBNull(reader("global_conf")), Nothing, Convert.ToBoolean(reader("global_conf")))
                    }
                End If
            End Using
        End Using

        Return Nothing
    End Function

    Public Shared Function ActualizarConfiguraciones(configs As List(Of Configuracion)) As Boolean
        Try
            Using cnn = ConexionDb.GetConnection()
                cnn.Open()

                For Each config In configs
                    Dim cmd As New SqlCommand("
                        IF EXISTS (SELECT 1 FROM Configuracion WHERE id_conf = @id)
                            UPDATE Configuracion SET 
                                idCoop_conf = @idCoop,
                                idEmpa_conf = @idEmpa,
                                NomEmp_conf = @nom, 
                                Dire_Conf = @dir, 
                                Direc2_conf = @dir2, 
                                isla_conf = @isla, 
                                SoloPlatanos_conf = @platanos, 
                                global_conf = @global 
                            WHERE id_conf = @id
                        ELSE
                            INSERT INTO Configuracion (id_conf, idCoop_conf, idEmpa_conf, NomEmp_conf, Dire_Conf, Direc2_conf, isla_conf, SoloPlatanos_conf, global_conf) 
                            VALUES (@id, @idCoop, @idEmpa, @nom, @dir, @dir2, @isla, @platanos, @global)
                    ", cnn)

                    cmd.Parameters.AddWithValue("@id", config.id_conf)
                    cmd.Parameters.AddWithValue("@idCoop", config.idCoop_conf)
                    cmd.Parameters.AddWithValue("@idEmpa", config.idEmpa_conf)
                    cmd.Parameters.AddWithValue("@nom", config.NomEmp_conf)
                    cmd.Parameters.AddWithValue("@dir", config.Dire_Conf)
                    cmd.Parameters.AddWithValue("@dir2", config.Direc2_conf)
                    cmd.Parameters.AddWithValue("@isla", config.isla_conf)
                    cmd.Parameters.AddWithValue("@platanos", If(config.SoloPlatanos_conf.HasValue, config.SoloPlatanos_conf.Value, DBNull.Value))
                    cmd.Parameters.AddWithValue("@global", If(config.global_conf.HasValue, config.global_conf.Value, DBNull.Value))

                    cmd.ExecuteNonQuery()
                Next
            End Using

            Return True
        Catch ex As Exception
            MessageBox.Show("Error al actualizar configuraciones: " & ex.Message)
            Return False
        End Try
    End Function

End Class
