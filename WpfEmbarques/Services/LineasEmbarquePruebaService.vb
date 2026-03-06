Imports System.Data.SqlClient
Imports Microsoft.Data.SqlClient

Public Class LineasEmbarquePruebaService

    Private ReadOnly _connectionString As String

    Public Sub New(connectionString As String)
        _connectionString = connectionString
    End Sub

    ' MÉTODO PRINCIPAL
    Public Sub TraspasarLineas(idVale As Integer, idCoop As String, idEmpa As String)

        Dim lista As List(Of LineaVale) = ObtenerLineasVale(idVale, idCoop, idEmpa)

        For Each lin In lista

            Dim destino As LineasEmbarquePrueba = MapearLinea(lin, idCoop, idEmpa)

            If ExisteLinea(destino.id_Coop_lin, destino.id_cab_lin, destino.id_linea_lin) Then
                ActualizarLinea(destino)
            Else
                InsertarLinea(destino)
            End If

            MarcarLineaValeTraspasada(idVale, idCoop, idEmpa, lin.idLinea_lva)
        Next

    End Sub


    ' 1. OBTENER LÍNEAS ORIGEN
    Private Function ObtenerLineasVale(idVale As Integer, idCoop As String, idEmpa As String) As List(Of LineaVale)

        Dim lista As New List(Of LineaVale)

        Using conn As New SqlConnection(_connectionString)
            conn.Open()

            Dim sql As String =
                "SELECT * FROM LineasVale 
                 WHERE idCab_lva = @idVale
                   AND idCoop_lva = @idCoop
                   AND idEmpa_lva = @idEmpa"

            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@idVale", idVale)
                cmd.Parameters.AddWithValue("@idCoop", idCoop)
                cmd.Parameters.AddWithValue("@idEmpa", idEmpa)

                Using rd = cmd.ExecuteReader()
                    While rd.Read()
                        Dim l As New LineaVale With {
                            .idCab_lva = rd("idCab_lva"),
                            .idCoop_lva = rd("idCoop_lva").ToString(),
                            .idEmpa_lva = rd("idEmpa_lva").ToString(),
                            .idLinea_lva = rd("idLinea_lva"),
                            .prod_lva = rd("Prod_lva").ToString(),
                            .pNeto_lva = If(IsDBNull(rd("PNeto_lva")), Nothing, rd("PNeto_lva")),
                            .cajas_lva = If(IsDBNull(rd("Cajas_lva")), Nothing, rd("Cajas_lva")),
                            .totkil_lva = If(IsDBNull(rd("totKil_lva")), Nothing, rd("totKil_lva")),
                            .estado_lva = rd("Estado_lva").ToString(),
                            .nPalet_lva = If(IsDBNull(rd("nPalet_lva")), Nothing, rd("Npalet_lva")),
                            .tipPalet_lva = If(IsDBNull(rd("TipPalet_lva")), Nothing, rd("TipPalet_lva")),
                            .tipCaja_lva = If(IsDBNull(rd("TipCaja_lva")), Nothing, rd("TipCaja_lva"))
                        }
                        lista.Add(l)
                    End While
                End Using
            End Using
        End Using

        Return lista
    End Function


    ' 2. MAPEAR ORIGEN → DESTINO
    Private Function MapearLinea(src As LineaVale, idCoop As String, idEmpa As String) As LineasEmbarquePrueba

        Dim dst As New LineasEmbarquePrueba With {
            .id_Coop_lin = idEmpa & idCoop,
            .id_cab_lin = src.idCab_lva,
            .id_linea_lin = src.idLinea_lva,
            .Prod_lin = src.prod_lva,
            .PNeto_lin = src.pNeto_lva,
            .Cajas_lin = src.cajas_lva,
            .totkil_lin = src.totkil_lva,
            .Estado_Lin = src.estado_lva,
            .nPalet_Lin = src.nPalet_lva,
            .traspasado_lin = False
        }
        Dim val As Integer
        If Integer.TryParse(src.tipPalet_lva, Val) Then
            dst.TipPalet_Lin = val
        Else
            dst.TipPalet_Lin = Nothing
        End If

        ' TipCaja
        If Integer.TryParse(src.tipCaja_lva, Val) Then
            dst.TipCaja_Lin = val
        Else
            dst.TipCaja_Lin = Nothing
        End If

        Return dst

        Return dst
    End Function


    ' 3. EXISTE?
    Private Function ExisteLinea(idCoop As String, idCab As Integer, idLinea As Integer) As Boolean
        Using conn As New SqlConnection(_connectionString)
            conn.Open()

            Dim sql As String =
                "SELECT 1 FROM LineasEmbarque
                 WHERE id_Coop_lin=@coop AND id_cab_lin=@cab AND id_linea_lin=@lin"

            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@coop", idCoop)
                cmd.Parameters.AddWithValue("@cab", idCab)
                cmd.Parameters.AddWithValue("@lin", idLinea)

                Return cmd.ExecuteScalar() IsNot Nothing
            End Using
        End Using
    End Function


    ' 4. INSERTAR
    Private Sub InsertarLinea(l As LineasEmbarquePrueba)

        Using conn As New SqlConnection(_connectionString)
            conn.Open()

            Dim sql As String =
                "INSERT INTO LineasEmbarque(
                    id_Coop_lin,id_cab_lin,id_linea_lin,
                    Prod_lin,PNeto_lin,Cajas_lin,totkil_lin,
                    Estado_Lin,nPalet_Lin,TipPalet_Lin,TipCaja_Lin,traspasado_lin
                )
                VALUES(
                    @coop,@cab,@lin,
                    @prod,@pneto,@cajas,@totkil,
                    @estado,@npalet,@tpalet,@tcaja,@tras
                )"

            Using cmd As New SqlCommand(sql, conn)
                RellenarParams(cmd, l)
                cmd.ExecuteNonQuery()
            End Using
        End Using

    End Sub


    ' 5. ACTUALIZAR
    Private Sub ActualizarLinea(l As LineasEmbarquePrueba)

        Using conn As New SqlConnection(_connectionString)
            conn.Open()

            Dim sql As String =
                "UPDATE LineasEmbarque SET
                    Prod_lin=@prod,
                    PNeto_lin=@pneto,
                    Cajas_lin=@cajas,
                    totkil_lin=@totkil,
                    Estado_Lin=@estado,
                    nPalet_Lin=@npalet,
                    TipPalet_Lin=@tpalet,
                    TipCaja_Lin=@tcaja,
                    traspasado_lin=@tras
                 WHERE id_Coop_lin=@coop 
                   AND id_cab_lin=@cab
                   AND id_linea_lin=@lin"

            Using cmd As New SqlCommand(sql, conn)
                RellenarParams(cmd, l)
                cmd.ExecuteNonQuery()
            End Using
        End Using

    End Sub


    ' 6. RELLENAR PARÁMETROS
    Private Sub RellenarParams(cmd As SqlCommand, l As LineasEmbarquePrueba)

        cmd.Parameters.AddWithValue("@coop", l.id_Coop_lin)
        cmd.Parameters.AddWithValue("@cab", l.id_cab_lin)
        cmd.Parameters.AddWithValue("@lin", l.id_linea_lin)

        cmd.Parameters.AddWithValue("@prod", If(l.Prod_lin, DBNull.Value))
        cmd.Parameters.AddWithValue("@pneto", If(l.PNeto_lin, DBNull.Value))
        cmd.Parameters.AddWithValue("@cajas", If(l.Cajas_lin, DBNull.Value))
        cmd.Parameters.AddWithValue("@totkil", If(l.totkil_lin, DBNull.Value))
        cmd.Parameters.AddWithValue("@estado", If(l.Estado_Lin, DBNull.Value))
        cmd.Parameters.AddWithValue("@npalet", If(l.nPalet_Lin, DBNull.Value))
        cmd.Parameters.AddWithValue("@tpalet", If(l.TipPalet_Lin, DBNull.Value))
        cmd.Parameters.AddWithValue("@tcaja", If(l.TipCaja_Lin, DBNull.Value))
        cmd.Parameters.AddWithValue("@tras", l.traspasado_lin)

    End Sub


    ' 7. MARCAR ORIGINAL COMO TRASPASADA
    Private Sub MarcarLineaValeTraspasada(idVale As Integer, idCoop As String, idEmpa As String, nLinea As Integer)

        Using conn As New SqlConnection(_connectionString)
            conn.Open()

            Dim sql As String =
                "UPDATE LineasVale 
                 SET traspasado_lva = 0
                 WHERE idCab_lva=@idVale 
                   AND idCoop_lva=@idCoop
                   AND idEmpa_lva=@idEmpa
                   AND idLinea_lva=@nLin"

            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@idVale", idVale)
                cmd.Parameters.AddWithValue("@idCoop", idCoop)
                cmd.Parameters.AddWithValue("@idEmpa", idEmpa)
                cmd.Parameters.AddWithValue("@nLin", nLinea)
                cmd.ExecuteNonQuery()
            End Using
        End Using

    End Sub

End Class

