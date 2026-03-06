Imports Microsoft.Data.SqlClient

Public Class ServicioGlobal
    Public Shared PwdProgram As String = "41d9970d6477c16cdd21c1d035f769f24891fed49731f71cfe8a46936e5e7186"

    Public Function ObtenerNavieras(Codigo As Integer) As List(Of Naviera)
        Dim lista As New List(Of Naviera)

        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand($"SELECT * FROM naviera where id_nav= {Codigo}", conn)
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    lista.Add(New Naviera With {
                        .id_nav = Convert.ToInt64(reader("id_nav")),
                        .Des_nav = reader("Des_nav").ToString()
                    })
                End While
            End Using
        End Using

        Return lista
    End Function
    Public Function ObtenerNavieraPorBuque(codigo As Integer) As Integer

        Dim _buque As New Buque
        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand($"SELECT * FROM buque where id_buq= {codigo}", conn)
            Using reader = cmd.ExecuteReader()
                If reader.Read Then
                    _buque.nav_buq = Convert.ToInt64(reader("nav_buq"))
                    Return _buque.nav_buq
                Else
                    Return 0
                End If
            End Using
        End Using

    End Function
    Public Function ObtenerBuques() As List(Of Buque)
        Dim lista As New List(Of Buque)

        Using conn = ConexionDb.GetConnection()
            conn.Open()
            ' Dim cmd As New SqlCommand($"SELECT * FROM Buque where nav_buq={Naviera}", conn)
            Dim cmd As New SqlCommand($"select * from buque", conn)
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    lista.Add(New Buque With {
                        .id_buq = Convert.ToInt64(reader("id_buq")),
                        .Des_buq = reader("Des_Buq").ToString()
                    })
                End While
            End Using
        End Using

        Return lista
    End Function
    Public Function ObtenerCargador() As List(Of Cargador)
        Dim lista As New List(Of Cargador)

        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand("SELECT * FROM Cargador", conn)
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    lista.Add(New Cargador With {
                        .id_carg = Convert.ToInt64(reader("id_carg")),
                        .Nomb_carg = reader("Nomb_carg").ToString()
                    })
                End While
            End Using
        End Using

        Return lista
    End Function
    Public Function ObtenerTransportista() As List(Of Transportista)
        Dim lista As New List(Of Transportista)

        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand("SELECT * FROM Transportista", conn)
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    lista.Add(New Transportista With {
                        .id_trans = Convert.ToInt64(reader("id_trans")),
                        .Des_trans = reader("des_trans").ToString()
                    })
                End While
            End Using
        End Using

        Return lista
    End Function
    Public Function ObtenerDestino() As List(Of Destino)
        Dim lista As New List(Of Destino)

        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand("SELECT * FROM Destino", conn)
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    lista.Add(New Destino With {
                        .id_des = Convert.ToInt64(reader("id_des")),
                        .Des_des = reader("des_des").ToString()
                    })
                End While
            End Using
        End Using

        Return lista
    End Function
    Public Function ObtenerTipoPalet() As List(Of TipoPalet)
        Dim lista As New List(Of TipoPalet)

        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand("SELECT * FROM TipoPalets", conn)
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    lista.Add(New TipoPalet With {
                        .idCodigo_tPal = reader("idCodigo_tpal").ToString(),
                        .TipoPalet_tPal = reader("TipoPalet_tpal").ToString()
                    })
                End While
            End Using
        End Using

        Return lista
    End Function
    Public Function ObtenerTipoCaja() As List(Of TipoCaja)
        Dim lista As New List(Of TipoCaja)

        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand("SELECT * FROM TipoCajas", conn)
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    lista.Add(New TipoCaja With {
                        .idCodigo_tCaja = reader("idCodigo_tCaja"),
                        .TipoCaja_tCaja = reader("TipoCaja_tCaja").ToString()
                    })
                End While
            End Using
        End Using

        Return lista
    End Function
    Public Function ObtenerIsla() As List(Of Isla)
        Dim lista As New List(Of Isla)

        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand("SELECT * FROM DestinoIsla", conn)
            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    lista.Add(New Isla With {
                        .id_Isla = Convert.ToInt64(reader("id_Isla")),
                        .des_Isla = reader("des_Isla").ToString()
                    })
                End While
            End Using
        End Using

        Return lista
    End Function
    Public Function ObtenDatosCargador(idCargador As Integer) As Cargador
        Dim _Cargador As New Cargador
        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand("SELECT * FROM CARGADOR WHERE id_Carg=" & idCargador, conn)
            Using Reader = cmd.ExecuteReader
                If Reader.Read() Then
                    _Cargador.nif_carg = Reader("nif_Carg").ToString()
                    _Cargador.Direcc_carg = Reader("Direcc_Carg").ToString()
                    _Cargador.pob_carg = Reader("Pob_carg").ToString()
                    Return _Cargador
                End If
            End Using
        End Using
        Return Nothing
    End Function
    Public Function ObtenNifTransportista(idTrans As Integer) As Transportista
        Dim _Transportista As New Transportista
        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand("SELECT * FROM TRANSPORTISTA WHERE id_Trans=" & idTrans, conn)
            Using Reader = cmd.ExecuteReader
                If Reader.Read() Then
                    _Transportista.Nif_trans = Reader("nif_trans").ToString()
                    Return _Transportista
                End If
            End Using
        End Using
        Return Nothing
    End Function
    Public Function ObtenerUsuario(id_usu As String) As Usuario
        Dim _usuario As New Usuario
        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand("SELECT * FROM Usuario WHERE id_usu=" & id_usu, conn)
            Using Reader = cmd.ExecuteReader
                If Reader.Read() Then
                    _usuario.id_usu = Reader("id_usu").ToString()
                    _usuario.password_usu = Reader("")
                    Return _usuario
                End If
            End Using
        End Using
        Return Nothing
    End Function
    Public Shared Function ActualizarTipoCajaEnBd(cajas As List(Of ClasImportCajas))

        Using cnn = ConexionDb.GetConnection()
            cnn.Open()

            For Each caja In cajas
                Dim cmd As New SqlCommand("IF EXISTS (SELECT 1 FROM TipoCajas WHERE idCodigo_tCaja = @id)
                UPDATE TipoCajas SET TipoCaja_tCaja = @des where idCodigo_tCaja=@id
                ELSE
                INSERT INTO TipoCajas (idCodigo_tCaja, TipoCaja_tCaja) VALUES (@id, @des)", cnn)

                cmd.Parameters.AddWithValue("@id", caja.idCaja)
                cmd.Parameters.AddWithValue("@des", caja.desCaja)
                cmd.ExecuteNonQuery()
            Next
        End Using
    End Function
    Public Shared Function ActualizarTipoPaletEnBd(tPalets As List(Of ClasImportTpalet))

        Using cnn = ConexionDb.GetConnection()
            cnn.Open()

            For Each tPalet In tPalets
                Dim cmd As New SqlCommand("IF EXISTS (SELECT 1 FROM TipoPalets WHERE idCodigo_tPal = @id)
                UPDATE TipoPalets SET TipoPalet_tPal = @des where idCodigo_tPal=@id
                ELSE
                INSERT INTO TipoPalets (idCodigo_tPal, TipoPalet_tPal) VALUES (@id, @des)", cnn)

                cmd.Parameters.AddWithValue("@id", tPalet.idPalet)
                cmd.Parameters.AddWithValue("@des", tPalet.NomPalet)
                cmd.ExecuteNonQuery()
            Next
        End Using
    End Function
End Class
