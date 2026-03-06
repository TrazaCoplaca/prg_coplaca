Imports System.Data.SqlClient
Imports Microsoft.Data.SqlClient
Imports WpfEmbarques.SesionUsuario

Public Class LineasValeService

    Public Function CargarUnaLinea(idCabecera As Long, idLinea As Long) As LineaVale
        Dim resultado As LineaVale = Nothing

        Using conn = ConexionDb.GetConnection()
            conn.Open()

            Dim cmd As New SqlCommand("SELECT * FROM LineasVale WHERE idCab_lva = @idCab AND idLinea_lva = @idLinea AND idCoop_lva = @idCoop AND idEmpa_lva = @idEmpa", conn)
            cmd.Parameters.AddWithValue("@idCab", idCabecera)
            cmd.Parameters.AddWithValue("@idLinea", idLinea)
            cmd.Parameters.AddWithValue("@idCoop", Mid(CoopActual, 3, 2))
            cmd.Parameters.AddWithValue("@idEmpa", Mid(CoopActual, 1, 2))

            Using reader = cmd.ExecuteReader()
                If reader.Read() Then
                    resultado = New LineaVale With {
                    .idCoop_lva = reader("idCoop_lva").ToString(),
                    .idEmpa_lva = reader("idEmpa_lva").ToString(),
                    .idCab_lva = Convert.ToInt64(reader("idCab_lva")),
                    .idLinea_lva = Convert.ToInt32(reader("idLinea_lva")),
                    .prod_lva = reader("prod_lva").ToString(),
                    .pNeto_lva = If(IsDBNull(reader("pNeto_lva")), Nothing, Convert.ToInt32(reader("pNeto_lva"))),
                    .cajas_lva = If(IsDBNull(reader("cajas_lva")), Nothing, Convert.ToInt32(reader("cajas_lva"))),
                    .totkil_lva = If(IsDBNull(reader("totkil_lva")), Nothing, Convert.ToInt32(reader("totkil_lva"))),
                    .estado_lva = reader("estado_lva").ToString(),
                    .nPalet_lva = If(IsDBNull(reader("nPalet_lva")), Nothing, Convert.ToInt32(reader("nPalet_lva"))),
                    .tipPalet_lva = reader("tipPalet_lva").ToString(),
                    .tipCaja_lva = reader("tipCaja_lva")
                }
                End If
            End Using
        End Using

        Return resultado
    End Function

    Public Function ObtenerLineas(idCabecera As Long) As List(Of LineaVale)
        Dim lista As New List(Of LineaVale)

        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand("SELECT * FROM LineasVale WHERE idCab_lva = @idCab and idCoop_lva=" & Mid(CoopActual, 3, 2) &
                                      " and idEmpa_lva=" & Mid(CoopActual, 1, 2), conn)
            cmd.Parameters.AddWithValue("@idCab", idCabecera)

            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    lista.Add(New LineaVale With {
                        .idCoop_lva = reader("idCoop_lva").ToString(),
                        .idEmpa_lva = reader("idEmpa_lva").ToString(),
                        .idCab_lva = Convert.ToInt64(reader("idCab_lva")),
                        .idLinea_lva = Convert.ToInt32(reader("idLinea_lva")),
                        .prod_lva = reader("prod_lva").ToString(),
                        .pNeto_lva = If(IsDBNull(reader("pNeto_lva")), Nothing, Convert.ToInt32(reader("pNeto_lva"))),
                        .cajas_lva = If(IsDBNull(reader("cajas_lva")), Nothing, Convert.ToInt32(reader("cajas_lva"))),
                        .totkil_lva = If(IsDBNull(reader("totkil_lva")), Nothing, Convert.ToInt32(reader("totkil_lva"))),
                        .estado_lva = reader("estado_lva").ToString(),
                        .nPalet_lva = If(IsDBNull(reader("nPalet_lva")), Nothing, Convert.ToInt32(reader("nPalet_lva"))),
                        .tipPalet_lva = reader("tipPalet_lva").ToString(),
                        .tipCaja_lva = reader("tipCaja_lva")
                    })
                End While
            End Using
        End Using

        Return lista
    End Function
    Public Async Sub InsertarLinea(linea As LineaVale)
        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand("INSERT INTO LineasVale (idCoop_lva, idEmpa_lva, idCab_lva, idLinea_lva,
                                        prod_lva, pNeto_lva, cajas_lva, totkil_lva, estado_lva, nPalet_lva, tipPalet_lva,
                                        tipCaja_lva) VALUES (@idCoop, @idEmpa, @idCab, @idLinea, @prod, @pNeto, @cajas, @totkil, @estado, @nPalet, @tipPalet, @tipCaja)", conn)
            cmd.Parameters.AddWithValue("@idCoop", Mid(CoopActual, 3, 2))
            cmd.Parameters.AddWithValue("@idEmpa", Mid(CoopActual, 1, 2))
            cmd.Parameters.AddWithValue("@idCab", linea.idCab_lva)
            cmd.Parameters.AddWithValue("@idLinea", linea.idLinea_lva)
            cmd.Parameters.AddWithValue("@prod", linea.prod_lva)
            cmd.Parameters.AddWithValue("@pNeto", If(linea.pNeto_lva.HasValue, CType(linea.pNeto_lva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@cajas", If(linea.cajas_lva.HasValue, CType(linea.cajas_lva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@totkil", If(linea.totkil_lva.HasValue, CType(linea.totkil_lva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@estado", "0")
            cmd.Parameters.AddWithValue("@nPalet", If(linea.nPalet_lva.HasValue, CType(linea.nPalet_lva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@tipPalet", linea.tipPalet_lva)
            cmd.Parameters.AddWithValue("@tipCaja", linea.tipCaja_lva)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Async Sub ActualizarLinea(linea As LineaVale)
        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand("UPDATE LineasVale SET prod_lva=@prod, pNeto_lva=@pNeto, cajas_lva=@cajas, totkil_lva=@totkil, 
                                        estado_lva=@estado, nPalet_lva=@nPalet, tipPalet_lva=@tipPalet, tipCaja_lva=@tipCaja
                                        WHERE idCab_lva=@idCab AND idLinea_lva=@idLinea and idCoop_lva=" & Mid(CoopActual, 3, 2) & " and idEmpa_lva=" & Mid(CoopActual, 1, 2), conn)
            cmd.Parameters.AddWithValue("@idCab", linea.idCab_lva)
            cmd.Parameters.AddWithValue("@idLinea", linea.idLinea_lva)
            cmd.Parameters.AddWithValue("@prod", linea.prod_lva)
            cmd.Parameters.AddWithValue("@pNeto", If(linea.pNeto_lva.HasValue, CType(linea.pNeto_lva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@cajas", If(linea.cajas_lva.HasValue, CType(linea.cajas_lva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@totkil", If(linea.totkil_lva.HasValue, CType(linea.totkil_lva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@estado", "0")
            cmd.Parameters.AddWithValue("@nPalet", If(linea.nPalet_lva.HasValue, CType(linea.nPalet_lva, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@tipPalet", linea.tipPalet_lva)
            cmd.Parameters.AddWithValue("@tipCaja", linea.tipCaja_lva)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Sub EliminarLinea(idCab As Long, idLinea As Integer)
        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand("DELETE FROM LineasVale WHERE idCab_lva=@idCab AND idLinea_lva=@idLinea and idCoop_lva=" & Mid(CoopActual, 3, 2) &
                                      " and idEmpa_lva=" & Mid(CoopActual, 1, 2), conn)
            cmd.Parameters.AddWithValue("@idCab", idCab)
            cmd.Parameters.AddWithValue("@idLinea", idLinea)
            cmd.ExecuteNonQuery()
        End Using
    End Sub
    Public Function ObtenerLineasDeVista(idCabecera As Long) As List(Of VistaLineasVale)
        Dim lista As New List(Of VistaLineasVale)

        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand("SELECT * FROM VistaLineasVale WHERE idCab_lva = @idCabecera and idCoop_lva=" & Mid(CoopActual, 3, 2) &
                                      " and idEmpa_lva=" & Mid(CoopActual, 1, 2), conn)
            cmd.Parameters.AddWithValue("@idCabecera", idCabecera)

            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    Dim linea As New VistaLineasVale With {
                    .idCoop_lva = reader("idCoop_lva").ToString(),
                    .idEmpa_lva = reader("idEmpa_lva").ToString(),
                    .idCab_lva = Convert.ToInt64(reader("idCab_lva")),
                    .idLinea_lva = Convert.ToInt32(reader("idLinea_lva")),
                    .prod_lva = reader("Prod_lva").ToString(),
                    .desProd_lva = reader("Des_prod").ToString(),
                    .pNeto_lva = If(IsDBNull(reader("pNeto_lva")), Nothing, Convert.ToInt32(reader("pNeto_lva"))),
                    .cajas_lva = If(IsDBNull(reader("cajas_lva")), Nothing, Convert.ToInt32(reader("cajas_lva"))),
                    .totkil_lva = If(IsDBNull(reader("totkil_lva")), Nothing, Convert.ToInt32(reader("totkil_lva"))),
                    .estado_lva = reader("estado_lva").ToString(),
                    .nPalet_lva = If(IsDBNull(reader("nPalet_lva")), Nothing, Convert.ToInt32(reader("nPalet_lva"))),
                    .desPalet_lva = reader("TipoPalet_Tpal").ToString,
                    .desCaja_lva = reader("TipoCaja_tCaja").ToString,
                    .KilosCaja = If(IsDBNull(reader("KilosCaja")), Nothing, Convert.ToInt32(reader("kilosCaja"))),
                    .tipPalet_lva = reader("tipPalet_lva").ToString,
                    .tipCaja_lva = reader("tipCaja_lva").ToString
                }

                    lista.Add(linea)
                End While
            End Using
        End Using

        Return lista
    End Function
    Public Async Function InsertarOActualizarLineas(lineas As LineaVale) As Task
        Using conn = ConexionDb.GetConnection()
            Await conn.OpenAsync()

            If lineas.idLinea_lva = 0 Then
                lineas.idLinea_lva = Await ObtenerNuevoIdLinea(lineas.idCab_lva)
            End If


            Dim cmdCheck As New SqlCommand("SELECT COUNT(*) FROM LineasVale WHERE idCab_lva = @idCab AND idLinea_lva = @idLinea and idCoop_lva=" & Mid(CoopActual, 3, 2) &
                                      " and idEmpa_lva=" & Mid(CoopActual, 1, 2), conn)
            cmdCheck.Parameters.AddWithValue("@idCab", lineas.idCab_lva)
            cmdCheck.Parameters.AddWithValue("@idLinea", lineas.idLinea_lva)

            Dim existe = Convert.ToInt32(Await cmdCheck.ExecuteScalarAsync()) > 0

                If existe Then
                    ' Update
                    Dim cmdUpdate As New SqlCommand("UPDATE LineasVale SET prod_lva = @prod, pNeto_lva = @pNeto, cajas_lva = @cajas, totkil_lva = @totkil, estado_lva = @estado,
                                                    nPalet_lva = @nPalet, tipPalet_lva = @tipPalet, tipCaja_lva = @tipCaja, traspasado_lva = @traspasado
                                                    WHERE idCab_lva = @idCab AND idLinea_lva = @idLinea", conn)
                AñadirParametrosLinea(cmdUpdate, lineas)
                Await cmdUpdate.ExecuteNonQueryAsync()
                Else
                ' Insert
                lineas.idLinea_lva = Await ObtenerNuevoIdLinea(lineas.idCab_lva)

                Dim cmdInsert As New SqlCommand("INSERT INTO LineasVale (idCoop_lva, idEmpa_lva, idCab_lva, idLinea_lva, prod_lva, pNeto_lva, cajas_lva, totkil_lva, estado_lva,
                                                    nPalet_lva, tipPalet_lva, tipCaja_lva, traspasado_lva)
                                                    VALUES (@coop, @empa, @idCab, @idLinea, @prod, @pNeto, @cajas, @totkil, @estado,
                                                    @nPalet, @tipPalet, @tipCaja, @traspasado)", conn)
                AñadirParametrosLinea(cmdInsert, lineas)
                Await cmdInsert.ExecuteNonQueryAsync()
                End If

        End Using
    End Function

    Private Sub AñadirParametrosLinea(cmd As SqlCommand, linea As LineaVale)
        cmd.Parameters.AddWithValue("@coop", Mid(CoopActual, 3, 2))
        cmd.Parameters.AddWithValue("@empa", Mid(CoopActual, 1, 2))
        cmd.Parameters.AddWithValue("@idCab", linea.idCab_lva)
        cmd.Parameters.AddWithValue("@idLinea", linea.idLinea_lva)
        cmd.Parameters.AddWithValue("@prod", linea.prod_lva)
        cmd.Parameters.AddWithValue("@pNeto", If(linea.pNeto_lva.HasValue, linea.pNeto_lva, DBNull.Value))
        cmd.Parameters.AddWithValue("@cajas", If(linea.cajas_lva.HasValue, linea.cajas_lva, DBNull.Value))
        cmd.Parameters.AddWithValue("@totkil", If(linea.totkil_lva.HasValue, linea.totkil_lva, DBNull.Value))
        cmd.Parameters.AddWithValue("@estado", "0")
        cmd.Parameters.AddWithValue("@nPalet", If(linea.nPalet_lva.HasValue, linea.nPalet_lva, DBNull.Value))
        cmd.Parameters.AddWithValue("@tipPalet", linea.tipPalet_lva)
        cmd.Parameters.AddWithValue("@tipCaja", linea.tipCaja_lva)
        cmd.Parameters.AddWithValue("@traspasado", If(linea.traspasado_lva.HasValue, linea.traspasado_lva, DBNull.Value))
    End Sub

    Public Async Function ObtenerNuevoIdLinea(idCabecera As Long) As Task(Of Integer)
        Using conn = ConexionDb.GetConnection()
            Await conn.OpenAsync()
            Dim cmd As New SqlCommand("SELECT ISNULL(MAX(idLinea_lva), 0) FROM LineasVale WHERE idCab_lva = @idCab and idCoop_lva=" & Mid(CoopActual, 3, 2) &
                                      " and idEmpa_lva=" & Mid(CoopActual, 1, 2), conn)
            cmd.Parameters.AddWithValue("@idCab", idCabecera)
            Dim resultado = Await cmd.ExecuteScalarAsync()
            Return Convert.ToInt32(resultado) + 1
        End Using
    End Function
    Public Function ObtenerLineasVale(idCoop As String, idEmpa As String, idCab As Long) As List(Of LineaVale)
        Dim listaLineas As New List(Of LineaVale)

        Using conn = ConexionDb.GetConnection
            conn.Open()

            Dim query As String = "
            SELECT idCoop_lva, idEmpa_lva, idCab_lva, idLinea_lva, prod_lva, Prod_lva, 
                   pNeto_lva, cajas_lva, totkil_lva, nPalet_lva, tipPalet_lva, tipCaja_lva, 
                   traspasado_lva, estado_lva
            FROM LineasVale
            WHERE idCoop_lva = @idCoop AND idEmpa_lva = @idEmpa AND idCab_lva = @idCab
        "

            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@idCoop", idCoop)
                cmd.Parameters.AddWithValue("@idEmpa", idEmpa)
                cmd.Parameters.AddWithValue("@idCab", idCab)
                ' embarque.idVale_cva = Mid(Date.Today.Year, 3, 2) & Mid(embarque.idEmpa_cva, 2, 1) & embarque.idCoop_cva & embarque.idVale_cva
                Using reader As SqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim linea As New LineaVale With {
                            .idCoop_lva = reader("idCoop_lva").ToString(),
                            .idEmpa_lva = reader("idEmpa_lva").ToString(),
                            .idCab_lva = Mid(Date.Today.Year, 3, 2) & Mid(.idEmpa_lva, 2, 1) & .idCoop_lva & Convert.ToInt64(reader("idCab_lva")),
                            .idLinea_lva = Convert.ToInt32(reader("idLinea_lva")),
                            .prod_lva = reader("prod_lva").ToString(),
                            .pNeto_lva = If(IsDBNull(reader("pNeto_lva")), Nothing, Convert.ToInt32(reader("pNeto_lva"))),
                            .cajas_lva = If(IsDBNull(reader("cajas_lva")), Nothing, Convert.ToInt32(reader("cajas_lva"))),
                            .totkil_lva = If(IsDBNull(reader("totkil_lva")), Nothing, Convert.ToInt32(reader("totkil_lva"))),
                            .nPalet_lva = If(IsDBNull(reader("nPalet_lva")), Nothing, Convert.ToInt32(reader("nPalet_lva"))),
                            .tipPalet_lva = reader("tipPalet_lva").ToString(),
                            .tipCaja_lva = reader("tipCaja_lva").ToString(),
                            .traspasado_lva = If(IsDBNull(reader("traspasado_lva")), Nothing, Convert.ToInt32(reader("traspasado_lva"))),
                            .estado_lva = reader("estado_lva").ToString()
                        }
                        listaLineas.Add(linea)
                    End While
                End Using
            End Using
        End Using

        Return listaLineas
    End Function

End Class
