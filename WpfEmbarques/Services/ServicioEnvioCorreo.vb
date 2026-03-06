Imports System.IO
Imports System.Net
Imports System.Net.Mail

Public Class ServicioEnvioCorreo
    Public Sub EnviarPdfDesdeConfig(rutaPdf As String, empaquetado As String, Nembarque As String, MatEmbarque As String)

        Dim configPath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "emailsettings.json")

        Dim cfg As EmailConfig = EmailConfigLoader.LoadConfig(configPath)

        Dim mensaje As New MailMessage()
        mensaje.From = New MailAddress(cfg.Remitente)
        mensaje.Subject = "Nuevo Embarque nº:" & Nembarque & " " & MatEmbarque & " de la Cooperativa " & empaquetado
        mensaje.Body = "Adjunto se envía el documento."
        mensaje.IsBodyHtml = False

        ' Destinatarios
        For Each correo In cfg.Destinatarios
            mensaje.To.Add(correo)
        Next

        ' CC
        For Each correo In cfg.CC
            mensaje.CC.Add(correo)
        Next

        ' CCO
        For Each correo In cfg.CCO
            mensaje.Bcc.Add(correo)
        Next

        ' Adjuntar PDF
        mensaje.Attachments.Add(New Attachment(rutaPdf))

        ' SMTP
        Dim smtp As New SmtpClient(cfg.SmtpServer, cfg.SmtpPort)
        smtp.EnableSsl = cfg.EnableSSL
        smtp.Credentials = New NetworkCredential(cfg.Remitente, cfg.ClaveApp)

        smtp.Send(mensaje)
    End Sub


    Public Sub EnviarPdfPorCorreo_GmailMulti(rutaPdf As String, NomCoop As String,
                                        remitente As String,
                                        claveApp As String,
                                        destinatariosPara As List(Of String),
                                        Optional destinatariosCC As List(Of String) = Nothing,
                                        Optional destinatariosCCO As List(Of String) = Nothing)

        If Not File.Exists(rutaPdf) Then
            Throw New FileNotFoundException("No se encuentra el PDF a enviar.", rutaPdf)
        End If

        Dim mensaje As New MailMessage()
        mensaje.From = New MailAddress(remitente)
        mensaje.Subject = "Correo del Empaquetado:" & NomCoop
        mensaje.Body = "Se adjunta el documento PDF correspondiente."
        mensaje.IsBodyHtml = False

        ' Para
        For Each correo In destinatariosPara
            mensaje.To.Add(correo.Trim())
        Next

        ' CC
        If destinatariosCC IsNot Nothing Then
            For Each correo In destinatariosCC
                mensaje.CC.Add(correo.Trim())
            Next
        End If

        ' CCO
        If destinatariosCCO IsNot Nothing Then
            For Each correo In destinatariosCCO
                mensaje.Bcc.Add(correo.Trim())
            Next
        End If

        mensaje.Attachments.Add(New Attachment(rutaPdf))

        Dim smtp As New SmtpClient("smtp.gmail.com", 587) With {
            .Credentials = New NetworkCredential(remitente, claveApp),
            .EnableSsl = True
        }

        smtp.Send(mensaje)

    End Sub
End Class
