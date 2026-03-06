Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Security
Imports System.Text
Imports System.Text.Json
Imports Azure
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Serialization

Public Class BcEmbarqueService


    Public Shared Async Function EnviarDatosCabecera(embarque As CabVale, token As String) As Task(Of Boolean)

        Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/WS/PRUEBAS/Codeunit/WsCabCargasDirecto"


        If embarque.idVale_cva <> 0 Then
            embarque.idVale_cva = Mid(Date.Today.Year, 3, 2) & Mid(embarque.idEmpa_cva, 2, 1) & embarque.idCoop_cva & embarque.idVale_cva
        End If
        ' Obtén el token de acceso
        Dim accessToken As String = token

        Dim jsonData As String = JsonConvert.SerializeObject(New List(Of CabVale) From {embarque}, Newtonsoft.Json.Formatting.Indented)



        Dim soapEnvelope As String = $"
        <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:urn='urn:microsoft-dynamics-schemas/codeunit/WsCabCargasDirecto'>
            <soapenv:Header/>
            <soapenv:Body>
                <urn:InsCabCargasDirecto>
                    <urn:data>{SecurityElement.Escape(jsonData)}</urn:data>
                </urn:InsCabCargasDirecto>
            </soapenv:Body>
        </soapenv:Envelope>"

        Dim request As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)
        request.Method = "POST"
        request.ContentType = "text/xml;charset=utf-8"
        request.Headers.Add("SOAPAction", "urn:microsoft-dynamics-schemas/codeunit/WsCabCargasDirecto:InsCabCargasDirecto")
        request.Headers.Add("Authorization", "Bearer " + accessToken) ' Usar Bearer token

        Dim byteData As Byte() = Encoding.UTF8.GetBytes(soapEnvelope)
        request.ContentLength = byteData.Length
        Using dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteData, 0, byteData.Length)
        End Using

        Try
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim responseText As String = reader.ReadToEnd()
                    'txtCampo.Text = txtCampo.Text & responseText

                    Console.WriteLine("Respuesta del servidor: " & responseText)
                    Return True
                End Using
            End Using
        Catch ex As WebException
            Using reader As New StreamReader(ex.Response.GetResponseStream())
                Dim errorText As String = reader.ReadToEnd()

                MessageBox.Show("Error al enviar embarque: " & ex.Message)
                Return False

            End Using
        End Try
    End Function

    Public Shared Async Function EnviarDatosCabeceraIA(embarque As CabVale, token As String) As Task(Of Boolean)
        Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/xxxxx/SandboxAP/WS/PRUEBAS/Codeunit/WsCabCargasDirecto"

        ' Modificar ID si es necesario
        If embarque.idVale_cva <> 0 Then
            embarque.idVale_cva = Mid(Date.Today.Year, 3, 2) & Mid(embarque.idEmpa_cva, 2, 1) & embarque.idCoop_cva & embarque.idVale_cva
        End If

        ' Configurar serialización de JSON (booleans en minúscula)
        Dim settings As New JsonSerializerSettings With {
        .ContractResolver = New CamelCasePropertyNamesContractResolver(),
        .NullValueHandling = NullValueHandling.Ignore,
        .Formatting = Formatting.None
    }

        ' Serializar JSON
        Dim jsonData As String = JsonConvert.SerializeObject(New List(Of CabVale) From {embarque}, settings)

        ' Incluir JSON dentro del envelope SOAP
        Dim soapEnvelope As String = $"
        <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:urn='urn:microsoft-dynamics-schemas/codeunit/WsCabCargasDirecto'>
            <soapenv:Header/>
            <soapenv:Body>
                <urn:InsCabCargasDirecto>
                    <urn:data>{SecurityElement.Escape(jsonData)}</urn:data>
                </urn:InsCabCargasDirecto>
            </soapenv:Body>
        </soapenv:Envelope>"

        ' Preparar request
        Dim request As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)
        request.Method = "POST"
        request.ContentType = "text/xml;charset=UTF-8"
        request.Headers.Add("SOAPAction", "urn:microsoft-dynamics-schemas/codeunit/WsCabCargasDirecto:InsCabCargasDirecto")
        request.Headers.Add("Authorization", "Bearer " & token)

        Dim byteData As Byte() = Encoding.UTF8.GetBytes(soapEnvelope)
        request.ContentLength = byteData.Length
        Using dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteData, 0, byteData.Length)
        End Using

        Try
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim responseText As String = reader.ReadToEnd()
                    Console.WriteLine("Respuesta del servidor: " & responseText)
                    Return True
                End Using
            End Using
        Catch ex As WebException
            Using reader As New StreamReader(ex.Response.GetResponseStream())
                Dim errorText As String = reader.ReadToEnd()
                Console.WriteLine("Error al enviar embarque: " & errorText)
                Return False
            End Using
        End Try
    End Function
    Public Shared Async Function EnviarDatosLineaBC(linea As List(Of LineaVale), token As String) As Task(Of Boolean)


        Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/WS/PRUEBAS/Codeunit/WslineaEmbarquesDirecto"


        ' Obtén el token de acceso
        Dim accessToken As String = token

        Dim jsonData As String = JsonConvert.SerializeObject(linea, Newtonsoft.Json.Formatting.Indented)



        Dim soapEnvelope As String = $"
        <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:urn='urn:microsoft-dynamics-schemas/codeunit/WslineaEmbarquesDirecto'>
            <soapenv:Header/>
            <soapenv:Body>
                <urn:InsLineaEmbarqueDirecto>
                    <urn:data>{SecurityElement.Escape(jsonData)}</urn:data>
                </urn:InsLineaEmbarqueDirecto>
            </soapenv:Body>
        </soapenv:Envelope>"

        Dim request As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)
        request.Method = "POST"
        request.ContentType = "text/xml;charset=utf-8"
        request.Headers.Add("SOAPAction", "urn:microsoft-dynamics-schemas/codeunit/WsCabCargasDirecto:InsLineaEmbarqueDirecto")
        request.Headers.Add("Authorization", "Bearer " + accessToken) ' Usar Bearer token

        Dim byteData As Byte() = Encoding.UTF8.GetBytes(soapEnvelope)
        request.ContentLength = byteData.Length
        Using dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteData, 0, byteData.Length)
        End Using

        Try
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim responseText As String = reader.ReadToEnd()
                    'txtCampo.Text = txtCampo.Text & responseText

                    Console.WriteLine("Respuesta del servidor: " & responseText)
                    Return True
                End Using
            End Using
        Catch ex As WebException
            Using reader As New StreamReader(ex.Response.GetResponseStream())
                Dim errorText As String = reader.ReadToEnd()

                MessageBox.Show("Error al enviar embarque: " & ex.Message)
                Return False

            End Using
        End Try
    End Function

    Public Shared Async Function EliminarLineasBc(LinEmbarque As LineaVale, token As String) As Task(Of Boolean)


        Dim url As String = "https://api.businesscentral.dynamics.com/v2.0/c036d0fb-e0d2-4094-920d-eaa0f8ecd5ce/SandboxAP/WS/PRUEBAS/Codeunit/WslineaEmbarquesDirecto"
        Dim EliminaLinea As New ClasBorrarLin

        If LinEmbarque.idCab_lva <> 0 Then
            EliminaLinea.idCab_lva = Mid(Date.Today.Year, 3, 2) & Mid(SesionUsuario.idEmpa, 2, 1) & SesionUsuario.IdCoop & LinEmbarque.idCab_lva
            EliminaLinea.idEmpa_lva = SesionUsuario.idEmpa
            EliminaLinea.idCoop_lva = SesionUsuario.IdCoop
        End If
        ' Obtén el token de acceso
        Dim accessToken As String = token


        Dim jsonData As String = JsonConvert.SerializeObject(EliminaLinea, Newtonsoft.Json.Formatting.Indented)


        Dim soapEnvelope As String = $"
        <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:urn='urn:microsoft-dynamics-schemas/codeunit/WslineaEmbarquesDirecto'>
            <soapenv:Header/>
                <soapenv:Body>
                    <urn:BorradoLineas>
                        <urn:request>{SecurityElement.Escape(jsondata)}</urn:request>
                    </urn:BorradoLineas>
                </soapenv:Body>
        </soapenv:Envelope>"

        Dim request As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)
        request.Method = "POST"
        request.ContentType = "text/xml;charset=utf-8"
        request.Headers.Add("SOAPAction", "urn:microsoft-dynamics-schemas/codeunit/WslineaEmbarquesDirecto:WsLineaEmbarquesDirecto")
        request.Headers.Add("Authorization", "Bearer " + accessToken) ' Usar Bearer token

        Dim byteData As Byte() = Encoding.UTF8.GetBytes(soapEnvelope)
        request.ContentLength = byteData.Length
        Using dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteData, 0, byteData.Length)
        End Using

        Try
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim responseText As String = reader.ReadToEnd()
                    'txtCampo.Text = txtCampo.Text & responseText

                    Console.WriteLine("Respuesta del servidor: " & responseText)
                    Return True
                End Using
            End Using
        Catch ex As WebException
            Using reader As New StreamReader(ex.Response.GetResponseStream())
                Dim errorText As String = reader.ReadToEnd()

                MessageBox.Show("Error al enviar embarque: " & ex.Message)
                Return False

            End Using
        End Try
    End Function




End Class

