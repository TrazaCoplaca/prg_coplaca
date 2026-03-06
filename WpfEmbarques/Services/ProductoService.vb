
Imports Microsoft.Data.SqlClient

Public Class ProductoService

    Public Function ObtenerProductos() As List(Of Producto)
        Dim lista As New List(Of Producto)
        'saca todos los productos sin excepciones
        Dim sentencia_sql As String = "SELECT id_prod, Des_prod, [Kilos/Caja], Tipo_Prod, natur_prod, unegap_prod, produccion_prod FROM ProductoBC"
        Using conn = ConexionDb.GetConnection()
            conn.Open()
            If SesionUsuario.SoloPlatanos = True Then
                If SesionUsuario.globalGap = True Then ' saca todos los productos que son platanos, indiferentemente que sean o no de global pero tener en cuenta que si es global no sacara los que son Natur y no global
                    sentencia_sql = "SELECT id_prod, Des_prod, [Kilos/Caja], Tipo_Prod, natur_prod, unegap_prod, produccion_prod FROM ProductoBC where Tipo_Prod ='PLATANOS' and not (unegap_prod=0 and natur_prod=1)"
                Else
                    ' saca todos los productos que son platanos pero que no tengan el check de global marcado
                    sentencia_sql = "SELECT id_prod, Des_prod, [Kilos/Caja], Tipo_Prod, natur_prod, unegap_prod, produccion_prod FROM ProductoBC where Tipo_Prod ='PLATANOS' and unegap_Prod = 'False'"

                End If
            Else
                If SesionUsuario.globalGap = False Then
                    ' saca todos los productos que no tengan el check de global
                    sentencia_sql = "SELECT id_prod, Des_prod, [Kilos/Caja], Tipo_Prod, natur_prod, unegap_prod, produccion_prod FROM ProductoBC where Unegap_prod=0"
                Else
                    sentencia_sql = "SELECT id_prod, Des_prod, [Kilos/Caja], Tipo_Prod, natur_prod, unegap_prod, produccion_prod FROM ProductoBC where not (unegap_prod=0 and natur_prod=1)"
                End If
            End If

            Dim cmd As New SqlCommand(sentencia_sql, conn)

            Using reader = cmd.ExecuteReader()
                While reader.Read()
                    Dim prod As New Producto With {
                            .cod_prod = reader("id_prod").ToString(),
                            .des_prod = reader("Des_prod").ToString(),
                            .peso = If(IsDBNull(reader("Kilos/Caja")), Nothing, Convert.ToInt32(reader("Kilos/Caja"))),
                            .tipo_prod = reader("Tipo_Prod").ToString(),
                            .natur_prod = If(IsDBNull(reader("natur_prod")), Nothing, Convert.ToBoolean(reader("natur_prod"))),
                            .unegap_prod = If(IsDBNull(reader("unegap_prod")), Nothing, Convert.ToBoolean(reader("unegap_prod"))),
                            .produccion_prod = reader("produccion_prod").ToString
                        }
                    lista.Add(prod)
                End While
            End Using
        End Using

        Return lista
    End Function
    Public Function ObtenerUnProducto(idProducto As String) As Producto
        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand($"SELECT * FROM ProductoBc WHERE id_prod ={idProducto}", conn)
            Dim prod As New Producto
            Using reader = cmd.ExecuteReader()
                If reader.Read() Then
                    prod.cod_prod = reader("id_prod").ToString()
                    prod.des_prod = reader("Des_prod").ToString()
                    prod.peso = If(IsDBNull(reader("Kilos/Caja")), Nothing, Convert.ToInt32(reader("Kilos/Caja")))
                    prod.tipo_prod = reader("Tipo_Prod").ToString()
                    prod.unegap_prod = If(IsDBNull(reader("unegap_prod")), Nothing, Convert.ToBoolean(reader("unegap_prod")))
                    prod.natur_prod = If(IsDBNull(reader("natur_prod")), Nothing, Convert.ToBoolean(reader("natur_prod")))
                    prod.produccion_prod = reader("produccion_prod").ToString()
                    Return prod
                Else
                    Return Nothing
                End If

            End Using
        End Using

        Return Nothing
    End Function
    Public Function ObtenerAsterisco(idProducto As String) As String
        ' 1 marca igp
        ' 2 Global
        ' 3 Ecológico
        Using conn = ConexionDb.GetConnection()
            conn.Open()
            Dim cmd As New SqlCommand($"SELECT * FROM ProductoBc WHERE id_prod ={idProducto}", conn)
            Dim prod As New Producto
            Dim compAst As String = ""
            Using reader = cmd.ExecuteReader()
                If reader.Read() Then
                    prod.cod_prod = reader("id_prod").ToString()
                    prod.des_prod = reader("Des_prod").ToString()
                    prod.peso = If(IsDBNull(reader("Kilos/Caja")), Nothing, Convert.ToInt32(reader("Kilos/Caja")))
                    prod.tipo_prod = reader("Tipo_Prod").ToString()
                    prod.unegap_prod = If(IsDBNull(reader("unegap_prod")), Nothing, Convert.ToBoolean(reader("unegap_prod")))
                    prod.natur_prod = If(IsDBNull(reader("natur_prod")), Nothing, Convert.ToBoolean(reader("natur_prod")))
                    prod.produccion_prod = reader("produccion_prod").ToString()
                    If prod.tipo_prod = "PLATANOS" Then
                        compAst = "1 "
                    End If
                    If prod.unegap_prod Then
                        compAst = compAst & "2 "
                    End If
                    If prod.produccion_prod = " Ecológico" Then
                        compAst = compAst & "3 "
                    End If
                    If prod.produccion_prod = "BIOSUISSE" Then
                        compAst = compAst & "3 4"
                    End If
                    Return compAst
                    Else
                        Return Nothing
                End If

            End Using
        End Using

        Return Nothing
    End Function
    Public Shared Function ActualizarProductos(Productos As List(Of ClasImportProductos)) As Boolean
        Try
            Using cnn = ConexionDb.GetConnection()
                cnn.Open()
                For Each Prod In Productos
                    Dim cmd As New SqlCommand("IF EXISTS (SELECT 1 FROM ProductoBc WHERE id_prod = @id)
                                                UPDATE ProductoBc SET Des_prod = @des, [Kilos/Caja] = @kilos, 
                                                Tipo_Prod = @tipo,  unegap_prod = @unegap, Natur_prod = @natur,Produccion_prod = @produccion WHERE id_prod = @id 
                                            ELSE INSERT INTO ProductoBc (id_prod, Des_prod, [Kilos/Caja], Tipo_Prod, unegap_prod, natur_prod, Produccion_prod) 
                                            VALUES (@id, @des, @kilos, @tipo, @unegap, @natur, @produccion)", cnn)

                    cmd.Parameters.AddWithValue("@id", Prod.idProducto)
                    cmd.Parameters.AddWithValue("@des", Prod.Descripcion)
                    cmd.Parameters.AddWithValue("@kilos", Prod.Kilos) ' Ajusta este campo si cambia el nombre
                    cmd.Parameters.AddWithValue("@tipo", Prod.TipoProducto)   ' Ajusta este campo si cambia el nombre
                    cmd.Parameters.AddWithValue("@unegap", Prod.unegap)
                    cmd.Parameters.AddWithValue("@natur", Prod.Natur)
                    cmd.Parameters.AddWithValue("@produccion", Prod.Produccion)
                    cmd.ExecuteNonQuery()
                Next

            End Using
            Return True
        Catch ex As Exception
            MessageBox.Show("Error al actualizar productos: " & ex.Message)
        End Try


    End Function

End Class


