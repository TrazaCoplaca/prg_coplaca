Imports System.Data.SqlClient
Imports Microsoft.Data.SqlClient

Public Class CabEmbarquePruebaService
    Private ReadOnly _connectionString As String

    Public Sub New(connectionString As String)
        _connectionString = connectionString
    End Sub

    '========================================
    '   MÉTODO PRINCIPAL
    '========================================
    Public Sub TraspasarDesdeCabVale(idVale As Integer, idCoop As String, idEmpa As String)

        Dim cabVale As CabVale = ObtenerCabVale(idVale, idCoop, idEmpa)

        If cabVale Is Nothing Then
            Throw New Exception($"No existe la CabVale con clave ({idVale}, {idCoop}, {idEmpa})")
        End If

        ' Construimos destino
        Dim destino As CabEmbarquePrueba = MapearCabecera(cabVale)

        ' UPSERT
        If ExisteCabEmbarque(destino.idCoop_cab, destino.idNum_cab) Then
            ActualizarCabEmbarque(destino)
        Else
            InsertarCabEmbarque(destino)
        End If

        ' Marcar traspasado
        MarcarCabValeTraspasado(idVale, idCoop, idEmpa)

    End Sub


    '========================================
    '   1. Obtener CabVale
    '========================================
    Private Function ObtenerCabVale(idVale As Integer, idCoop As String, idEmpa As String) As CabVale

        Using conn As New SqlConnection(_connectionString)
            conn.Open()

            Dim sql As String =
                "SELECT * 
                 FROM CabVale
                 WHERE idVale_cva = @idVale
                   AND idCoop_cva = @idCoop
                   AND idEmpa_cva = @idEmpa"

            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@idVale", idVale)
                cmd.Parameters.AddWithValue("@idCoop", idCoop)
                cmd.Parameters.AddWithValue("@idEmpa", idEmpa)

                Using rd = cmd.ExecuteReader()
                    If rd.Read() Then
                        Dim c As New CabVale With {
                            .idVale_cva = rd("idVale_cva"),
                            .idCoop_cva = rd("idCoop_cva").ToString(),
                            .idEmpa_cva = rd("idEmpa_cva").ToString(),
                            .Fecha_cva = If(IsDBNull(rd("fecha_cva")), Nothing, rd("fecha_cva")),
                            .Sem_cva = If(IsDBNull(rd("sem_cva")), Nothing, rd("sem_cva")),
                            .Matr_cva = rd("Matr_cva").ToString(),
                            .PalRet_cva = If(IsDBNull(rd("PalRet_cva")), Nothing, rd("PalRet_cva")),
                            .NPalRet_cva = If(IsDBNull(rd("Npalret_cva")), Nothing, rd("Npalret_cva")),
                            .NTerm_cva = rd("nTerm_cva").ToString(),
                            .CajPlast_cva = If(IsDBNull(rd("CajPlast_cva")), Nothing, rd("CajPlast_cva")),
                            .nCajPlast_cva = If(IsDBNull(rd("nCajPlast_cva")), Nothing, rd("nCajPlast_cva")),
                            .FrutSemAnt_cva = If(IsDBNull(rd("FrutSemAnt_cva")), Nothing, rd("FrutSemAnt_cva")),
                            .Origen_cva = rd("Origen_cva").ToString(),
                            .Temp_cva = rd("Temp_cva").ToString(),
                            .Destino_cva = rd("Destino_cva").ToString(),
                            .Buque_cva = rd("Buque_cva").ToString(),
                            .Naviera_cva = rd("Naviera_cva").ToString(),
                            .SemEmb_cva = If(IsDBNull(rd("SemEmb_cva")), Nothing, rd("SemEmb_cva")),
                            .Trans_cva = rd("trans_cva").ToString(),
                            .observaciones_cva = rd("Observaciones_cva").ToString(),
                            .Traspasado_cva = rd("traspasado_cva")
                            }

                        Return c
                    End If
                End Using
            End Using
        End Using

        Return Nothing
    End Function


    '========================================
    '   2. Mapear origen → destino
    '========================================
    Private Function MapearCabecera(src As CabVale) As CabEmbarquePrueba
        Dim anyo As Date
        anyo = src.Fecha_cva
        Dim dst As New CabEmbarquePrueba With {
            .idCoop_cab = src.idEmpa_cva & src.idCoop_cva, ' 🔥 concatenado!!
            .idNum_cab = src.idVale_cva,
            .fecha_cab = src.Fecha_cva,
            .sem_cab = src.Sem_cva,
            .Matr_cab = src.Matr_cva,
            .PalRet_cab = False,
            .NpalReg_cab = 0,
            .Term_cab = True,
            .NumTerm_cab = src.NTerm_cva,
            .cajPlas_cab = False,
            .NcajaPlas_cab = 0,
            .frutaSemaant_cab = src.FrutSemAnt_cva,
            .puerto_cab = src.Origen_cva,
            .Estado_cab = 1,
            .TempTrans_cab = src.Temp_cva,
            .Destino_cab = ObtenerDestino(src.Destino_cva),
            .Buque_cab = ObtenerBuque(src.Buque_cva),
            .Naviera_cab = ObtenerNaviera(src.Naviera_cva),
            .OrdSemana_cab = src.SemEmb_cva,
            .TipoTrans_Cab = src.Trans_cva,
            .observaciones_cab = src.observaciones_cva,
            .traspasado = src.Traspasado_cva,
            .fechaTraspasado = DateTime.Now,
            .anyo_cab = anyo.Year,
            .Membar_cab = 0,
            .SemEmbar_cab = src.Sem_cva
            }

        Return dst
    End Function


    '========================================
    '   3. Comprobar existencia en destino
    '========================================
    Private Function ExisteCabEmbarque(idCoop As String, idNum As Integer) As Boolean

        Using conn As New SqlConnection(_connectionString)
            conn.Open()

            Dim sql As String =
                "SELECT 1 FROM CabEmbarque
                 WHERE idCoop_cab = @coop AND idNum_cab = @num"

            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@coop", idCoop)
                cmd.Parameters.AddWithValue("@num", idNum)

                Return cmd.ExecuteScalar() IsNot Nothing
            End Using
        End Using

    End Function


    '========================================
    '   4. INSERT destino
    '========================================
    Private Sub InsertarCabEmbarque(c As CabEmbarquePrueba)

        Using conn As New SqlConnection(_connectionString)
            conn.Open()

            Dim sql As String =
                "INSERT INTO CabEmbarque(
                    idCoop_cab, idNum_cab, fecha_cab, sem_cab, Matr_cab,
                    PalRet_cab, NpalReg_cab, Term_cab, NumTerm_cab,
                    cajPlas_cab, NcajaPlas_cab, frutaSemaant_cab,
                    puerto_cab, Estado_cab, TempTrans_cab,
                    Destino_cab, Buque_cab, Naviera_cab,
                    OrdSemana_cab, TipoTrans_Cab, observaciones_cab,
                    traspasado, fechaTraspasado, anyo_cab, Nembar_cab, SemEmbar_cab
                )
                VALUES(
                    @coop, @num, @fec, @sem, @matr,
                    @palret, @npalreg, @term, @nterm,
                    @cajplas, @ncajplas, @frutaant,
                    @puerto, @estado, @temp,
                    @dest, @buque, @naviera,
                    @ordsem, @tipotrans, @obs,
                    @tras, @ftrasp, @anyo, @Nembar, @SemEmbar
                )"

            Using cmd As New SqlCommand(sql, conn)
                RellenarParametros(cmd, c)
                cmd.ExecuteNonQuery()
            End Using
        End Using

    End Sub


    '========================================
    '   5. UPDATE destino (UPSERT)
    '========================================
    Private Sub ActualizarCabEmbarque(c As CabEmbarquePrueba)

        Using conn As New SqlConnection(_connectionString)
            conn.Open()

            Dim sql As String =
                "UPDATE CabEmbarque SET
                    fecha_cab=@fec, sem_cab=@sem, Matr_cab=@matr,
                    PalRet_cab=@palret, NpalReg_cab=@npalreg, Term_cab=@term,
                    NumTerm_cab=@nterm, cajPlas_cab=@cajplas, NcajaPlas_cab=@ncajplas,
                    frutaSemaant_cab=@frutaant, puerto_cab=@puerto,
                    Estado_cab=@estado, TempTrans_cab=@temp,
                    Destino_cab=@dest, Buque_cab=@buque, Naviera_cab=@naviera,
                    OrdSemana_cab=@ordsem, TipoTrans_Cab=@tipotrans,
                    observaciones_cab=@obs, traspasado=@tras,
                    fechaTraspasado=@ftrasp, anyo_cab=@anyo, Nembar_Cab = @Nembar, SemEmbar_cab = @semEmbar
                 WHERE idCoop_cab=@coop AND idNum_cab=@num"

            Using cmd As New SqlCommand(sql, conn)
                RellenarParametros(cmd, c)
                cmd.ExecuteNonQuery()
            End Using
        End Using

    End Sub


    '========================================
    '   6. Marcar CabVale traspasado
    '========================================
    Private Sub MarcarCabValeTraspasado(idVale As Integer, idCoop As String, idEmpa As String)

        Using conn As New SqlConnection(_connectionString)
            conn.Open()

            Dim sql As String =
                "UPDATE CabVale
                 SET traspasado_cva = 1
                 WHERE idVale_cva = @idVale
                   AND idCoop_cva = @idCoop
                   AND idEmpa_cva = @idEmpa"

            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@idVale", idVale)
                cmd.Parameters.AddWithValue("@idCoop", idCoop)
                cmd.Parameters.AddWithValue("@idEmpa", idEmpa)
                cmd.ExecuteNonQuery()
            End Using
        End Using

    End Sub


    '========================================
    '   7. Parametrización
    '========================================
    Private Sub RellenarParametros(cmd As SqlCommand, c As CabEmbarquePrueba)

        cmd.Parameters.AddWithValue("@coop", c.idCoop_cab)
        cmd.Parameters.AddWithValue("@num", c.idNum_cab)
        cmd.Parameters.AddWithValue("@fec", If(c.fecha_cab, DBNull.Value))
        cmd.Parameters.AddWithValue("@sem", If(c.sem_cab, DBNull.Value))
        cmd.Parameters.AddWithValue("@matr", c.Matr_cab)
        cmd.Parameters.AddWithValue("@palret", If(c.PalRet_cab, DBNull.Value))
        cmd.Parameters.AddWithValue("@npalreg", If(c.NpalReg_cab, DBNull.Value))
        cmd.Parameters.AddWithValue("@term", If(c.Term_cab, DBNull.Value))
        cmd.Parameters.AddWithValue("@nterm", c.NumTerm_cab)
        cmd.Parameters.AddWithValue("@cajplas", If(c.cajPlas_cab, DBNull.Value))
        cmd.Parameters.AddWithValue("@ncajplas", If(c.NcajaPlas_cab, DBNull.Value))
        cmd.Parameters.AddWithValue("@frutaant", If(c.frutaSemaant_cab, DBNull.Value))
        cmd.Parameters.AddWithValue("@puerto", c.puerto_cab)
        cmd.Parameters.AddWithValue("@estado", If(c.Estado_cab, DBNull.Value))
        cmd.Parameters.AddWithValue("@temp", c.TempTrans_cab)
        cmd.Parameters.AddWithValue("@dest", c.Destino_cab)
        cmd.Parameters.AddWithValue("@buque", c.Buque_cab)
        cmd.Parameters.AddWithValue("@naviera", c.Naviera_cab)
        cmd.Parameters.AddWithValue("@ordsem", If(c.OrdSemana_cab, DBNull.Value))
        cmd.Parameters.AddWithValue("@tipotrans", c.TipoTrans_Cab)
        cmd.Parameters.AddWithValue("@obs", c.observaciones_cab)
        cmd.Parameters.AddWithValue("@tras", If(c.traspasado, DBNull.Value))
        cmd.Parameters.AddWithValue("@ftrasp", If(c.fechaTraspasado, DBNull.Value))
        cmd.Parameters.AddWithValue("@anyo", If(c.anyo_cab, DBNull.Value))
        cmd.Parameters.AddWithValue("@SemEmbar", If(c.sem_cab, DBNull.Value))
        cmd.Parameters.AddWithValue("@NemBar", 0)


    End Sub
    Private Function ObtenerNaviera(idNav As Integer?) As String
        If idNav Is Nothing Then Return Nothing

        Using conn As New SqlConnection(_connectionString)
            conn.Open()

            Dim sql As String = "SELECT Des_nav FROM Naviera WHERE id_nav = @id"
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@id", idNav)
                Dim result = cmd.ExecuteScalar()
                Return If(result Is Nothing, Nothing, result.ToString())
            End Using
        End Using
    End Function
    Private Function ObtenerBuque(idBuq As Integer?) As String
        If idBuq Is Nothing Then Return Nothing

        Using conn As New SqlConnection(_connectionString)
            conn.Open()

            Dim sql As String = "SELECT des_buq FROM Buque WHERE id_buq = @id"
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@id", idBuq)
                Dim result = cmd.ExecuteScalar()
                Return If(result Is Nothing, Nothing, result.ToString())
            End Using
        End Using
    End Function
    Private Function ObtenerDestino(idDes As Integer?) As String
        If idDes Is Nothing Then Return Nothing

        Using conn As New SqlConnection(_connectionString)
            conn.Open()

            Dim sql As String = "SELECT des_des FROM Destino WHERE id_des = @id"
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@id", idDes)
                Dim result = cmd.ExecuteScalar()
                Return If(result Is Nothing, Nothing, result.ToString())
            End Using
        End Using
    End Function


End Class
