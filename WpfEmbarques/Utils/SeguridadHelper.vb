Imports System.IO
Imports System.Security.Cryptography
Imports System.Text



Public Class SeguridadHelper
    Private Shared ReadOnly Clave As String = "@lcb3399Dcc2505B" ' 16, 24 o 32 caracteres

    Public Shared Function Encriptar(texto As String) As String
        Dim key = Encoding.UTF8.GetBytes(Clave)
        Using aes As New AesCng
            aes.Key = key
            aes.GenerateIV()
            Dim iv = aes.IV

            Using encryptor = aes.CreateEncryptor(aes.Key, iv),
                      ms = New MemoryStream(),
                      cs = New CryptoStream(ms, encryptor, CryptoStreamMode.Write)
                Using sw = New StreamWriter(cs)
                    sw.Write(texto)
                End Using
                Dim cifrado = ms.ToArray()
                Return Convert.ToBase64String(iv.Concat(cifrado).ToArray())
            End Using
        End Using
    End Function

        Public Shared Function Desencriptar(texto As String) As String
            Dim full = Convert.FromBase64String(texto)
            Dim iv = full.Take(16).ToArray()
            Dim cipher = full.Skip(16).ToArray()
            Dim key = Encoding.UTF8.GetBytes(Clave)

        Using aes As New AesCng
            aes.Key = key
            aes.IV = iv
            Using decryptor = aes.CreateDecryptor(aes.Key, aes.IV),
                      ms = New MemoryStream(cipher),
                      cs = New CryptoStream(ms, decryptor, CryptoStreamMode.Read),
                      sr = New StreamReader(cs)
                Return sr.ReadToEnd()
            End Using
        End Using
    End Function
    End Class
