Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Security.Cryptography.Pkcs
Imports System.Text
Imports System.Text.Json
Imports Newtonsoft.Json.Linq
Imports WpfEmbarques.ClasImportCajas
Module importarDatosBc


    Public Async Function ObtenerCajasDesdeBC(token As String) As Task(Of List(Of ClasImportCajas))
        'Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/ODataV4/Company('PRUEBAS')/ExpCajas"
        'Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/Production3608/ODataV4/Company('COPLACA')/ExporCajas"
        'Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/ODataV4/Company('PRUEBAS')/ExportTipProductos?$filter=NoField ge 'TC00' and NoField le 'TC99'"
        'Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/ODataV4/Company('PRUEBAS')/ExportacionProductos?$filter=NoField ge 'TC00' and NoField le 'TC99'"
        Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/ODataV4/Company('PRUEBAS')/ExportarProductos?$filter=NoField ge 'TC00' and NoField le 'TC99'"
        Using client As New HttpClient()
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", token)
            Dim response As HttpResponseMessage = Await client.GetAsync(url)

            If response.IsSuccessStatusCode Then
                Dim json As String = Await response.Content.ReadAsStringAsync()
                Dim cajasResponse As ImpCajasResponse = JsonSerializer.Deserialize(Of ImpCajasResponse)(json)
                Return cajasResponse.values
            Else
                Throw New Exception("Error en la respuesta: " & response.StatusCode)
            End If
        End Using
    End Function
    Public Async Function ObtenerCajasDesdeBCEnum(token As String) As Task(Of List(Of ClasImportCajas))
        'Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/ODataV4/Company('PRUEBAS')/ExportarProductos?$filter=NoField ge 'TC00' and NoField le 'TC99'"
        Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/ODataV4/Company('PRUEBAS')/ExportarTipoCajas"

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", token)
            Dim response As HttpResponseMessage = Await client.GetAsync(url)

            If response.IsSuccessStatusCode Then
                Dim json As String = Await response.Content.ReadAsStringAsync()
                Dim cajasResponse As ImpCajasResponse = JsonSerializer.Deserialize(Of ImpCajasResponse)(json)
                Return cajasResponse.values
            Else
                Throw New Exception("Error en la respuesta: " & response.StatusCode)
            End If
        End Using
    End Function
    Public Async Function ObtenerTipoPaletDesdeBC(token As String) As Task(Of List(Of ClasImportTpalet))
        'Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/ODataV4/Company('PRUEBAS')/ExpCajas"
        'Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/Production3608/ODataV4/Company('COPLACA')/ExporCajas"
        'Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/ODataV4/Company('PRUEBAS')/ExportarProductos?$filter=NoField ge 'TP00' and NoField le 'TP99'"
        Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/Production3608/ODataV4/Company('COPLACA')/ExportarProductos?$filter=NoField ge 'TP00' and NoField le 'TP99'"
        Using client As New HttpClient()
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", token)
            Dim response As HttpResponseMessage = Await client.GetAsync(url)

            If response.IsSuccessStatusCode Then
                Dim json As String = Await response.Content.ReadAsStringAsync()
                Dim TpaletResponse As ImpTpaletResponse = JsonSerializer.Deserialize(Of ImpTpaletResponse)(json)
                Return TpaletResponse.values
            Else
                Throw New Exception("Error en la respuesta: " & response.StatusCode)
            End If
        End Using
    End Function
    Public Async Function ObtenerTipoPaletDesdeBCEnum(token As String) As Task(Of List(Of ClasImportTpalet))
        '  Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/ODataV4/Company('PRUEBAS')/ExportarTipoPalets"
        Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/ODataV4/Company('PRUEBAS')/ExportarTipoPalets"


        Using client As New HttpClient()
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", token)
            Dim response As HttpResponseMessage = Await client.GetAsync(url)

            If response.IsSuccessStatusCode Then
                Dim json As String = Await response.Content.ReadAsStringAsync()
                Dim TpaletResponse As ImpTpaletResponse = JsonSerializer.Deserialize(Of ImpTpaletResponse)(json)
                Return TpaletResponse.values
            Else
                Throw New Exception("Error en la respuesta: " & response.StatusCode)
            End If
        End Using
    End Function

    Public Async Function ObtenerProductoBC(token As String) As Task(Of List(Of ClasImportProductos))
        'Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/ODataV4/Company('PRUEBAS')/ExportarProductos"
        'Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/Production3608/ODataV4/Company('COPLACA')/ExportarTipProductos"
        'Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/ODataV4/Company('PRUEBAS')/ExportTipProductos"
        Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/Production3608/ODataV4/Company('COPLACA')/ExportarProductos"

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", token)
            Dim response As HttpResponseMessage = Await client.GetAsync(url)

            If response.IsSuccessStatusCode Then
                Dim json As String = Await response.Content.ReadAsStringAsync()
                Dim productoResponse As ImpProductosResponse = JsonSerializer.Deserialize(Of ImpProductosResponse)(json)
                Return productoResponse.value
            Else
                Throw New Exception("Error en la respuesta: " & response.StatusCode)
            End If
        End Using
    End Function

    Function GetAccessToken() As String
        Dim clientId As String = My.Settings.clientIdBc
        Dim clientSecret As String = My.Settings.clientSecret
        Dim tenantId As String = My.Settings.tenanId
        Dim url As String = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token"
        Dim postData As String = $"client_id={clientId}&scope=https://api.businesscentral.dynamics.com/.default&client_secret={clientSecret}&grant_type=client_credentials"
        Dim data As Byte() = Encoding.UTF8.GetBytes(postData)

        Dim request As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)
        Try
            request.Method = "POST"
            request.ContentType = "application/x-www-form-urlencoded"
            request.ContentLength = data.Length

            Using stream As Stream = request.GetRequestStream()
                stream.Write(data, 0, data.Length)
            End Using

            Dim response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
            Using reader As New StreamReader(response.GetResponseStream())
                Dim responseText As String = reader.ReadToEnd()
                Dim jsonResponse As JObject = JObject.Parse(responseText)
                Return jsonResponse("access_token").ToString()
            End Using
        Catch ex As Exception
            'LogExcepcion(ex, "Obtener Token")
            MessageBox.Show("Error:", ex.Message)
        End Try
    End Function

    Public Async Function GetEnumTipoCajasAsync(token As String) As Task(Of List(Of ClasImportCajas))
        'Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/WS/PRUEBAS/Codeunit/WsEnumCajas"
        Dim url As String = "POST https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/ODataV4/Company('PRUEBAS')/WsEnumCajas_GetEnumTipoCajas"
        Using client As New HttpClient()
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", token)
            Dim response As HttpResponseMessage = Await client.GetAsync(url)

            If response.IsSuccessStatusCode Then
                Dim json As String = Await response.Content.ReadAsStringAsync()
                ' Dim tipoCaja As ImpTipoCaja = JsonSerializer.Deserialize(Of ImpTipoCaja)(json)
                Dim cajasResponse As ImpCajasResponse = JsonSerializer.Deserialize(Of ImpCajasResponse)(json)
                Return cajasResponse.values
            Else
                Throw New Exception("Error en la respuesta: " & response.StatusCode)
            End If
        End Using

    End Function
    Public Async Function GetEnumTipoCajasODataAsync(token As String) As Task(Of List(Of ClasImportCajas))

        Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/ODataV4/WsEnumCajas_GetEnumTipoCajas?company=PRUEBAS"

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", token)
            client.DefaultRequestHeaders.Accept.Clear()
            client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))

            Dim body = New StringContent("{}", Encoding.UTF8, "application/json")
            Dim resp = Await client.PostAsync(url, body)
            Dim raw = Await resp.Content.ReadAsStringAsync()

            If Not resp.IsSuccessStatusCode Then
                Throw New Exception($"BC OData error ({CInt(resp.StatusCode)}): {raw}")
            End If

            ' Respuesta típica: {"value":"{...json...}"}
            Using doc = JsonDocument.Parse(raw)
                Dim innerJson = doc.RootElement.GetProperty("value").GetString()

                Dim opts As New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True}
                Dim payload = JsonSerializer.Deserialize(Of ImpCajasResponse)(innerJson, opts)

                Return If(payload?.values, New List(Of ClasImportCajas)())
            End Using
        End Using

    End Function
    Public Async Function GetEnumTipoPaletODataAsync(token As String) As Task(Of List(Of ClasImportTpalet))

        Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/ODataV4/WsEnumPalets_GetEnumTipoPalets?company=PRUEBAS"

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", token)
            client.DefaultRequestHeaders.Accept.Clear()
            client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))

            Dim body = New StringContent("{}", Encoding.UTF8, "application/json")
            Dim resp = Await client.PostAsync(url, body)
            Dim raw = Await resp.Content.ReadAsStringAsync()

            If Not resp.IsSuccessStatusCode Then
                Throw New Exception($"BC OData error ({CInt(resp.StatusCode)}): {raw}")
            End If

            ' Respuesta típica: {"value":"{...json...}"}
            Using doc = JsonDocument.Parse(raw)
                Dim innerJson = doc.RootElement.GetProperty("value").GetString()

                Dim opts As New JsonSerializerOptions With {.PropertyNameCaseInsensitive = False}
                Dim payload = JsonSerializer.Deserialize(Of ImpTpaletResponse)(innerJson, opts)

                Return If(payload?.values, New List(Of ClasImportTpalet)())
            End Using
        End Using

    End Function


End Module
