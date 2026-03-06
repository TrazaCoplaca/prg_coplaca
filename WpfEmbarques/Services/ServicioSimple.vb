Imports Microsoft.Data.SqlClient

Public Class ServicioSimple
    Public Async Function ObtenerIdTipoCaja(nombre As String) As Task(Of Integer?)
        Dim query As String = "SELECT TOP 1 idCodigo_tCaja FROM TipoCajas WHERE TipoCaja_tCaja = @nombre"

        Using conn = ConexionDb.GetConnection(),
              cmd As New SqlCommand(query, conn)

            cmd.Parameters.AddWithValue("@nombre", nombre)

            Try
                Await conn.OpenAsync()
                Dim resultado = Await cmd.ExecuteScalarAsync()
                Return If(resultado IsNot Nothing, Convert.ToInt32(resultado), CType(Nothing, Integer?))
            Catch ex As Exception
                MessageBox.Show("Error al obtener ID de Tipo Caja: " & ex.Message)
                Return Nothing
            End Try
        End Using
    End Function

    Public Async Function ObtenerIdTipoPalet(nombre As String) As Task(Of Integer?)
        Dim query As String = "SELECT TOP 1 idCodigo_tPal FROM TipoPalets WHERE TipoPalet_tPal = @nombre"

        Using conn = ConexionDb.GetConnection(),
              cmd As New SqlCommand(query, conn)

            cmd.Parameters.AddWithValue("@nombre", nombre)

            Try
                Await conn.OpenAsync()
                Dim resultado = Await cmd.ExecuteScalarAsync()
                Return If(resultado IsNot Nothing, Convert.ToInt32(resultado), CType(Nothing, Integer?))
            Catch ex As Exception
                MessageBox.Show("Error al obtener ID de Tipo Palet: " & ex.Message)
                Return Nothing
            End Try
        End Using
    End Function
End Class
