Imports System.Data.SqlClient
Imports Microsoft.Data.SqlClient
Imports WpfEmbarques.ConexionDb
Imports WpfEmbarques.SesionUsuario



Public Class CabValeService



    Public Function ObtenerCabecerasConTransportista() As List(Of CabValeDTO)
        Dim lista As New List(Of CabValeDTO)

        Dim query As String = $"
            SELECT c.idVale_cva, c.idCoop_cva, c.idEmpa_cva, c.Sem_cva, c.SemEmb_cva, c.Fecha_cva, c.Matr_cva,c.Observaciones_cva, c.Trans_cva, t.Des_trans
            FROM CabVale c
            INNER JOIN Transportista t ON c.Trans_cva = t.id_trans where c.idCoop_cva='{SesionUsuario.IdCoop}' and c.idEmpa_cva='{SesionUsuario.idEmpa}'
            ORDER BY c.Fecha_cva DESC
        "

        Using conn = ConexionDb.GetConnection()
            Using cmd As New SqlCommand(query, conn)
                conn.Open()
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        lista.Add(New CabValeDto With {
                            .idVale_cva = Convert.ToInt64(reader("idVale_cva")),
                            .idCoop_cva = reader("idCoop_cva").ToString(),
                            .idEmpa_cva = reader("idEmpa_cva").ToString(),
                            .Sem_cva = reader("Sem_cva").ToString(),
                            .SemEmb_cva = reader("SemEmb_cva").ToString(),
                            .Fecha_cva = If(IsDBNull(reader("Fecha_cva")), Nothing, Convert.ToDateTime(reader("Fecha_cva"))),
                            .Matr_cva = reader("Matr_cva").ToString(),
                            .Trans_cva = If(IsDBNull(reader("Trans_cva")), Nothing, Convert.ToInt32(reader("Trans_cva"))),
                            .Des_trans = reader("Des_trans").ToString(),
                            .Observaciones_cva = If(IsDBNull(reader("Observaciones_cva")), Nothing, Convert.ToString(reader("observaciones_cva")))
                        })
                    End While
                End Using
            End Using
        End Using

        Return lista
    End Function
    Public Function ObtenerSiguienteIdVale(coop As String, empa As String) As Long
        Dim nuevoIdVale As Long = 1 ' Valor inicial si no hay registros

        Using conn = ConexionDb.GetConnection()
            conn.Open()

            Dim query As String = "SELECT MAX(idVale_cva) FROM CabVale WHERE idCoop_cva = @coop AND idEmpa_cva = @empa"

            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@coop", coop)
                cmd.Parameters.AddWithValue("@empa", empa)

                Dim result = cmd.ExecuteScalar()

                If result IsNot DBNull.Value AndAlso result IsNot Nothing Then
                    nuevoIdVale = CLng(result) + 1
                End If
            End Using
        End Using

        Return nuevoIdVale
    End Function
    Public Function ObtenerNumeroEmbarqueSemana(idCoop As String, idEmpa As String, Semana As String)
        Dim CantVales As Long = 1 ' Valor por defecto
        Using conn = ConexionDb.GetConnection()
            conn.Open()

            Dim query As String = "Select COUNT(*) from cabVale where idCoop_Cva= @idCoop and idEmpa_cva=@idEmpa and sem_Cva=@Semana"

            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@idCoop", idCoop)
                cmd.Parameters.AddWithValue("@idEmpa", idEmpa)
                cmd.Parameters.AddWithValue("@Semana", Semana)

                Dim result = cmd.ExecuteScalar()

                If result IsNot DBNull.Value AndAlso result IsNot Nothing Then
                    CantVales = CLng(result) + 1
                End If
            End Using
        End Using

        Return CantVales
    End Function
    Public Function EliminarCabecera(NVale As Long)

        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand("DELETE FROM CabVale WHERE idVale_cva=@idCab AND idCoop_cva=" & Mid(CoopActual, 3, 2) &
                                      " and idEmpa_cva=" & Mid(CoopActual, 1, 2), conn)
            cmd.Parameters.AddWithValue("@idCab", NVale)
            cmd.ExecuteNonQuery()
        End Using

    End Function
    Public Function ObtenUnaCabecera(idCabecera As Long) As CabVale
        Dim resultado As CabVale = Nothing

        Using conn = ConexionDb.GetConnection()
            conn.Open()

            Dim cmd As New SqlCommand("SELECT * FROM CabVale WHERE idVale_cva = @idCab AND idCoop_cva = @idCoop AND idEmpa_cva = @idEmpa", conn)
            cmd.Parameters.AddWithValue("@idCab", idCabecera)
            cmd.Parameters.AddWithValue("@idCoop", Mid(CoopActual, 3, 2))
            cmd.Parameters.AddWithValue("@idEmpa", Mid(CoopActual, 1, 2))

            Using reader = cmd.ExecuteReader()
                If reader.Read() Then
                    resultado = New CabVale With {
                      .idVale_cva = Convert.ToInt64(reader("idVale_cva")),
                    .idCoop_cva = reader("idCoop_cva").ToString(),
                    .idEmpa_cva = reader("idEmpa_cva").ToString(),
                    .Sem_cva = reader("Sem_cva").ToString(),
                    .Fecha_cva = If(IsDBNull(reader("Fecha_cva")), Nothing, Convert.ToDateTime(reader("Fecha_cva"))),
                    .Matr_cva = reader("Matr_cva").ToString(),
                    .Trans_cva = If(IsDBNull(reader("Trans_cva")), Nothing, Convert.ToInt32(reader("Trans_cva"))),
                    .Mattrac_cva = reader("Mattrac_cva").ToString(),
                    .Temp_cva = reader("Temp_cva").ToString(),
                    .NTerm_cva = reader("NTerm_cva").ToString(),
                    .Origen_cva = reader("Origen_cva").ToString(),
                    .Cargador_cva = If(IsDBNull(reader("Cargador_cva")), Nothing, Convert.ToInt32(reader("Cargador_cva"))),
                    .Naviera_cva = If(IsDBNull(reader("Naviera_cva")), Nothing, Convert.ToInt32(reader("Naviera_cva"))),
                    .Buque_cva = If(IsDBNull(reader("Buque_cva")), Nothing, Convert.ToInt32(reader("Buque_cva"))),
                    .Destino_cva = If(IsDBNull(reader("Destino_cva")), Nothing, Convert.ToInt32(reader("Destino_cva"))),
                    .SemEmb_cva = reader("SemEmb_cva").ToString(),
                    .PalRet_cva = If(IsDBNull(reader("PalRet_cva")), Nothing, Convert.ToBoolean(reader("PalRet_cva"))),
                    .NPalRet_cva = If(IsDBNull(reader("NPalRet_cva")), Nothing, Convert.ToInt32(reader("NPalRet_cva"))),
                    .CajPlast_cva = If(IsDBNull(reader("CajPlast_cva")), Nothing, Convert.ToBoolean(reader("CajPlast_cva"))),
                    .nCajPlast_cva = If(IsDBNull(reader("NCajPlast_cva")), Nothing, Convert.ToInt32(reader("NCajPlast_cva"))),
                    .FrutSemAnt_cva = If(IsDBNull(reader("FrutSemAnt_cva")), Nothing, Convert.ToBoolean(reader("FrutSemAnt_cva"))),
                    .observaciones_cva = reader("Observaciones_cva").ToString(),
                    .isla_cva = If(IsDBNull(reader("isla_cva")), Nothing, Convert.ToInt32(reader("isla_cva"))),
                    .nTerm1_cva = reader("nTerm1_cva").ToString(),
                    .nTerm2_cva = reader("nTerm2_cva").ToString(),
                    .BiTemp_cva = If(IsDBNull(reader("BiTemp_cva")), Nothing, Convert.ToBoolean(reader("BiTemp_cva"))),
                    .Temp2_cva = reader("Temp2_cva").ToString(),
                    .OrdExp_cva = reader("OrdExp_cva").ToString(),
                    .Traspasado_cva = If(IsDBNull(reader("Traspasado_cva")), Nothing, Convert.ToBoolean(reader("Traspasado_cva")))
                }
                End If
            End Using
        End Using

        Return resultado
    End Function
    Public Function InsertarCabecera(Cab As CabVale)
        Using conn = ConexionDb.GetConnection()
            conn.Open()

            Dim cmd As New SqlCommand("
            INSERT INTO CabVale (
                idVale_cva, idCoop_cva, idEmpa_cva, Sem_cva, Fecha_cva, Matr_cva, Trans_cva,
                Mattrac_cva, Temp_cva, NTerm_cva, Origen_cva, Cargador_cva, Naviera_cva,
                Buque_cva, Destino_cva, SemEmb_cva, PalRet_cva, NPalRet_cva, CajPlast_cva,
                NCajPlast_cva, FrutSemAnt_cva, Observaciones_cva, isla_cva, nTerm1_cva,
                nTerm2_cva, BiTemp_cva, Temp2_cva, OrdExp_cva, Traspasado_cva
            ) VALUES (
                @idVale, @idCoop, @idEmpa, @sem, @fecha, @matr, @trans,
                @mattrac, @temp, @nterm, @origen, @cargador, @naviera,
                @buque, @destino, @semEmb, @palRet, @nPalRet, @cajPlast,
                @nCajPlast, @frutSemAnt, @observaciones, @isla, @nTerm1,
                @nTerm2, @biTemp, @temp2, @ordExp, @Traspasado
            )", conn)
            cmd.Parameters.AddWithValue("@idVale", Cab.idVale_cva)
            cmd.Parameters.AddWithValue("@idCoop", Cab.idCoop_cva)
            cmd.Parameters.AddWithValue("@idEmpa", Cab.idEmpa_cva)
            cmd.Parameters.AddWithValue("@sem", DbOrNull(Cab.Sem_cva))
            cmd.Parameters.AddWithValue("@fecha", If(Cab.Fecha_cva.HasValue, CType(Cab.Fecha_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@matr", DbOrNull(Cab.Matr_cva))
            cmd.Parameters.AddWithValue("@trans", If(Cab.Trans_cva.HasValue, CType(Cab.Trans_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@mattrac", DbOrNull(Cab.Mattrac_cva))
            cmd.Parameters.AddWithValue("@temp", DbOrNull(Cab.Temp_cva))
            cmd.Parameters.AddWithValue("@nterm", DbOrNull(Cab.NTerm_cva))
            cmd.Parameters.AddWithValue("@origen", DbOrNull(Cab.Origen_cva))
            cmd.Parameters.AddWithValue("@cargador", If(Cab.Cargador_cva.HasValue, CType(Cab.Cargador_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@naviera", If(Cab.Naviera_cva.HasValue, CType(Cab.Naviera_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@buque", If(Cab.Buque_cva.HasValue, CType(Cab.Buque_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@destino", DbOrNull(Cab.Destino_cva))
            cmd.Parameters.AddWithValue("@semEmb", DbOrNull(Cab.SemEmb_cva))
            cmd.Parameters.AddWithValue("@palRet", If(Cab.PalRet_cva.HasValue, CType(Cab.PalRet_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@nPalRet", If(Cab.NPalRet_cva.HasValue, CType(Cab.NPalRet_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@cajPlast", If(Cab.CajPlast_cva.HasValue, CType(Cab.CajPlast_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@nCajPlast", If(Cab.nCajPlast_cva.HasValue, CType(Cab.nCajPlast_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@frutSemAnt", If(Cab.FrutSemAnt_cva.HasValue, CType(Cab.FrutSemAnt_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@observaciones", DbOrNull(Cab.observaciones_cva))
            cmd.Parameters.AddWithValue("@isla", If(Cab.isla_cva.HasValue, CType(Cab.isla_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@nTerm1", DbOrNull(Cab.nTerm1_cva))
            cmd.Parameters.AddWithValue("@nTerm2", DbOrNull(Cab.nTerm2_cva))
            cmd.Parameters.AddWithValue("@biTemp", If(Cab.BiTemp_cva.HasValue, CType(Cab.BiTemp_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@temp2", DbOrNull(Cab.Temp2_cva))
            cmd.Parameters.AddWithValue("@ordExp", DbOrNull(Cab.OrdExp_cva))
            cmd.Parameters.AddWithValue("@Traspasado", DbOrNull(Cab.Traspasado_cva))

            cmd.ExecuteNonQuery()
        End Using

    End Function
    Public Function ActualizarCabecera(Cab As CabVale)
        Using conn = ConexionDb.GetConnection()
            conn.Open()

            Dim cmd As New SqlCommand("
            UPDATE CabVale SET             
                Sem_cva = @sem,
                Fecha_cva = @fecha,
                Matr_cva = @matr,
                Trans_cva = @trans,
                Mattrac_cva = @mattrac,
                Temp_cva = @temp,
                NTerm_cva = @nterm,
                Origen_cva = @origen,
                Cargador_cva = @cargador,
                Naviera_cva = @naviera,
                Buque_cva = @buque,
                Destino_cva = @destino,
                SemEmb_cva = @semEmb,
                PalRet_cva = @palRet,
                NPalRet_cva = @nPalRet,
                CajPlast_cva = @cajPlast,
                NCajPlast_cva = @nCajPlast,
                FrutSemAnt_cva = @frutSemAnt,
                Observaciones_cva = @observaciones,
                isla_cva = @isla,
                nTerm1_cva = @nTerm1,
                nTerm2_cva = @nTerm2,
                BiTemp_cva = @biTemp,
                Temp2_cva = @temp2,
                OrdExp_cva = @ordExp,
                Traspasado_cva = @Traspasado
            WHERE idVale_cva = @idVale
                    AND idCoop_cva = @idCoop
                    AND idEmpa_cva = @idEmpa", conn)

            ' Parámetros
            cmd.Parameters.AddWithValue("@idVale", Cab.idVale_cva)
            cmd.Parameters.AddWithValue("@idCoop", Cab.idCoop_cva)
            cmd.Parameters.AddWithValue("@idEmpa", Cab.idEmpa_cva)
            cmd.Parameters.AddWithValue("@sem", DbOrNull(Cab.Sem_cva))
            cmd.Parameters.AddWithValue("@fecha", If(Cab.Fecha_cva.HasValue, CType(Cab.Fecha_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@matr", DbOrNull(Cab.Matr_cva))
            cmd.Parameters.AddWithValue("@trans", If(Cab.Trans_cva.HasValue, CType(Cab.Trans_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@mattrac", DbOrNull(Cab.Mattrac_cva))
            cmd.Parameters.AddWithValue("@temp", DbOrNull(Cab.Temp_cva))
            cmd.Parameters.AddWithValue("@nterm", DbOrNull(Cab.NTerm_cva))
            cmd.Parameters.AddWithValue("@origen", DbOrNull(Cab.Origen_cva))
            cmd.Parameters.AddWithValue("@cargador", If(Cab.Cargador_cva.HasValue, CType(Cab.Cargador_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@naviera", If(Cab.Naviera_cva.HasValue, CType(Cab.Naviera_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@buque", If(Cab.Buque_cva.HasValue, CType(Cab.Buque_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@destino", DbOrNull(Cab.Destino_cva))
            cmd.Parameters.AddWithValue("@semEmb", DbOrNull(Cab.SemEmb_cva))
            cmd.Parameters.AddWithValue("@palRet", If(Cab.PalRet_cva.HasValue, CType(Cab.PalRet_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@nPalRet", If(Cab.NPalRet_cva.HasValue, CType(Cab.NPalRet_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@cajPlast", If(Cab.CajPlast_cva.HasValue, CType(Cab.CajPlast_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@nCajPlast", If(Cab.nCajPlast_cva.HasValue, CType(Cab.nCajPlast_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@frutSemAnt", If(Cab.FrutSemAnt_cva.HasValue, CType(Cab.FrutSemAnt_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@observaciones", DbOrNull(Cab.observaciones_cva))
            cmd.Parameters.AddWithValue("@isla", If(Cab.isla_cva.HasValue, CType(Cab.isla_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@nTerm1", DbOrNull(Cab.nTerm1_cva))
            cmd.Parameters.AddWithValue("@nTerm2", DbOrNull(Cab.nTerm2_cva))
            cmd.Parameters.AddWithValue("@biTemp", If(Cab.BiTemp_cva.HasValue, CType(Cab.BiTemp_cva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@temp2", DbOrNull(Cab.Temp2_cva))
            cmd.Parameters.AddWithValue("@ordExp", DbOrNull(Cab.OrdExp_cva))
            cmd.Parameters.AddWithValue("@Traspasado", DbOrNull(Cab.Traspasado_cva))
            cmd.ExecuteNonQuery()
        End Using
    End Function

    Private Function DbOrNull(value As Object) As Object
        Return If(value IsNot Nothing, value, DBNull.Value)
    End Function

End Class
