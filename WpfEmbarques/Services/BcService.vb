Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text.Json
Imports System.Text

Public Class BcService

    Private Const bcUrl As String = "https://api.businesscentral.dynamics.com/v2.0/..."
    Private Const companyName As String = "COPLACA"

    Public Shared Async Function EnviarCabeceraABusinessCentral(cab As CabVale, token As String) As Task(Of Boolean)
        Try
            Dim httpClient As New HttpClient()
            httpClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", token)
            httpClient.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))

            Dim json As String = JsonSerializer.Serialize(New With {
                .idVale = cab.idVale_cva,
                .idCoop = cab.idCoop_cva,
                .idEmpa = cab.idEmpa_cva,
                .Sem = cab.Sem_cva,
                .Fecha = cab.Fecha_cva?.ToString("yyyy-MM-dd"),
                .Matr = cab.Matr_cva,
                .Trans = cab.Trans_cva,
                .Temp = cab.Temp_cva,
                .Destino = cab.Destino_cva
             })

            Dim content As New StringContent(json, Encoding.UTF8, "application/json")
            Dim response = Await httpClient.PostAsync($"{bcUrl}/Company('{companyName}')/CabVale", content)

            Return response.IsSuccessStatusCode

        Catch ex As Exception
            MessageBox.Show("Error al enviar a BC: " & ex.Message)
            Return False
        End Try
    End Function

End Class

