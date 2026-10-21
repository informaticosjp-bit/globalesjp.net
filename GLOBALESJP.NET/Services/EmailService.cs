using GLOBALESJP.NET.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace GLOBALESJP.NET.Services
{
    /// <summary>
    /// Servicio de envío de notificaciones internas por correo a informaticosjp@gmail.com
    /// No envía ningún correo al cliente; solo notifica al equipo de GLOBALES JP.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<bool> SendContactNotificationAsync(ContactViewModel model)
        {
            try
            {
                var settings = _config.GetSection("EmailSettings");
                var smtpHost       = settings["SmtpHost"]       ?? "smtp.gmail.com";
                var smtpPort       = int.Parse(settings["SmtpPort"] ?? "587");
                var senderEmail    = Environment.GetEnvironmentVariable("EMAIL_SENDER")    ?? settings["SenderEmail"]    ?? "informaticosjp@gmail.com";
                var senderName     = settings["SenderName"]     ?? "GLOBALES JP | Web";
                var appPassword    = Environment.GetEnvironmentVariable("EMAIL_APP_PASSWORD") ?? settings["AppPassword"] ?? "xoxi grmg gyaw ypnv";
                var recipientEmail = Environment.GetEnvironmentVariable("EMAIL_RECIPIENT") ?? settings["RecipientEmail"] ?? "informaticosjp@gmail.com";
                var recipientName  = settings["RecipientName"]  ?? "GLOBALES JP";

                var cleanPassword = appPassword.Replace(" ", "").Trim();

                var whatsappPhone = model.Phone.Replace("+", "").Replace(" ", "").Replace("-", "");
                if (whatsappPhone.StartsWith("04") && whatsappPhone.Length <= 11)
                    whatsappPhone = "58" + whatsappPhone.Substring(1);

                var waLink = $"https://wa.me/{whatsappPhone}?text=Hola%20{Uri.EscapeDataString(model.FullName)},%20te%20escribimos%20de%20GLOBALES%20JP%20en%20respuesta%20a%20tu%20solicitud%20de%20{Uri.EscapeDataString(model.Service)}.";
                var replyLink = $"mailto:{model.Email}?subject=Re:%20Tu%20solicitud%20en%20GLOBALES%20JP%20-%20{Uri.EscapeDataString(model.Service)}";

                var htmlBody = $@"
<!DOCTYPE html>
<html lang='es'>
<head><meta charset='UTF-8'/></head>
<body style='margin:0;padding:0;background:#0a0f1e;font-family:""Segoe UI"",Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#0a0f1e;padding:32px 16px;'>
    <tr><td align='center'>
      <table width='620' cellpadding='0' cellspacing='0' style='max-width:620px;width:100%;background:#111827;border-radius:16px;overflow:hidden;border:1px solid rgba(255,255,255,0.08);'>
        <!-- HEADER -->
        <tr>
          <td style='background:linear-gradient(135deg,#0f2044 0%,#0a1628 100%);padding:24px 28px;border-bottom:2px solid #06b6d4;'>
            <table width='100%' cellpadding='0' cellspacing='0'>
              <tr>
                <td>
                  <div style='font-size:11px;font-family:monospace;color:#06b6d4;letter-spacing:2px;margin-bottom:4px;'>GLOBALES JP | NOTIFICACIÓN WEB</div>
                  <h1 style='margin:0;color:#ffffff;font-size:20px;font-weight:700;'>📩 Nueva Solicitud de Cliente</h1>
                  <div style='margin-top:4px;font-size:12px;color:#94a3b8;'>Recibido el {DateTime.Now:dd/MM/yyyy HH:mm}</div>
                </td>
                <td align='right' style='vertical-align:top;'>
                  <span style='background:rgba(16,185,129,0.15);border:1px solid rgba(16,185,129,0.4);border-radius:8px;padding:6px 12px;color:#10b981;font-size:12px;font-weight:600;'>
                    ● NUEVO LEAD
                  </span>
                </td>
              </tr>
            </table>
          </td>
        </tr>

        <!-- CONTENIDO -->
        <tr>
          <td style='padding:24px 28px;'>
            <table width='100%' cellpadding='0' cellspacing='0' style='background:rgba(6,182,212,0.05);border:1px solid rgba(6,182,212,0.15);border-radius:12px;padding:16px 20px;margin-bottom:18px;'>
              <tr>
                <td style='padding-bottom:10px;'>
                  <div style='font-size:11px;color:#64748b;'>NOMBRE DEL CLIENTE</div>
                  <div style='font-size:16px;color:#f1f5f9;font-weight:600;'>{System.Net.WebUtility.HtmlEncode(model.FullName)}</div>
                </td>
              </tr>
              <tr>
                <td style='padding-bottom:10px;'>
                  <div style='font-size:11px;color:#64748b;'>EMPRESA</div>
                  <div style='font-size:14px;color:#e2e8f0;'>{System.Net.WebUtility.HtmlEncode(model.Company ?? "Particular / No especificado")}</div>
                </td>
              </tr>
              <tr>
                <td style='padding-bottom:10px;'>
                  <div style='font-size:11px;color:#64748b;'>CORREO</div>
                  <div style='font-size:14px;'><a href='mailto:{model.Email}' style='color:#38bdf8;text-decoration:none;'>{System.Net.WebUtility.HtmlEncode(model.Email)}</a></div>
                </td>
              </tr>
              <tr>
                <td>
                  <div style='font-size:11px;color:#64748b;'>TELÉFONO / WHATSAPP</div>
                  <div style='font-size:14px;color:#e2e8f0;'>{System.Net.WebUtility.HtmlEncode(model.Phone)}</div>
                </td>
              </tr>
            </table>

            <!-- SERVICIO -->
            <div style='background:rgba(139,92,246,0.08);border:1px solid rgba(139,92,246,0.25);border-radius:12px;padding:14px 20px;margin-bottom:18px;'>
              <div style='font-size:11px;color:#a78bfa;font-family:monospace;'>SERVICIO SOLICITADO</div>
              <div style='font-size:15px;color:#c4b5fd;font-weight:600;'>🔧 {System.Net.WebUtility.HtmlEncode(model.Service)}</div>
            </div>

            <!-- MENSAJE -->
            <div style='background:rgba(15,23,42,0.8);border:1px solid rgba(255,255,255,0.08);border-radius:12px;padding:16px 20px;margin-bottom:22px;'>
              <div style='font-size:11px;color:#94a3b8;font-family:monospace;margin-bottom:8px;'>MENSAJE / REQUERIMIENTO</div>
              <div style='font-size:14px;color:#cbd5e1;line-height:1.6;white-space:pre-wrap;'>{System.Net.WebUtility.HtmlEncode(model.Message)}</div>
            </div>

            <!-- BOTONES DE ACCIÓN RÁPIDA -->
            <table width='100%' cellpadding='0' cellspacing='0'>
              <tr>
                <td style='padding-right:6px;width:50%;'>
                  <a href='{replyLink}' style='display:block;text-align:center;background:#0284c7;color:#ffffff;padding:12px 16px;border-radius:8px;text-decoration:none;font-weight:600;font-size:13px;'>
                    ✉️ Responder por Correo
                  </a>
                </td>
                <td style='padding-left:6px;width:50%;'>
                  <a href='{waLink}' style='display:block;text-align:center;background:#16a34a;color:#ffffff;padding:12px 16px;border-radius:8px;text-decoration:none;font-weight:600;font-size:13px;'>
                    📱 Abrir WhatsApp
                  </a>
                </td>
              </tr>
            </table>
          </td>
        </tr>

        <!-- FOOTER -->
        <tr>
          <td style='background:#0d1117;padding:14px 28px;border-top:1px solid rgba(255,255,255,0.05);text-align:center;'>
            <div style='font-size:11px;color:#475569;font-family:monospace;'>
              GLOBALESJP.NET · Notificación interna · Destino: {recipientEmail}
            </div>
          </td>
        </tr>
      </table>
    </td></tr>
  </table>
</body>
</html>";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress(recipientName, recipientEmail));
                message.ReplyTo.Add(new MailboxAddress(model.FullName, model.Email));
                message.Subject = $"[LEAD WEB] {model.Service} — {model.FullName}";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody,
                    TextBody = $"Nueva solicitud recibida:\nNombre: {model.FullName}\nEmpresa: {model.Company ?? "N/A"}\nCorreo: {model.Email}\nTel: {model.Phone}\nServicio: {model.Service}\n\nMensaje:\n{model.Message}"
                };
                message.Body = bodyBuilder.ToMessageBody();

                // Intento 1: Puerto configurado (normalmente 587 STARTTLS)
                try
                {
                    using var smtp = new SmtpClient();
                    smtp.Timeout = 8000;
                    var options = smtpPort == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
                    await smtp.ConnectAsync(smtpHost, smtpPort, options);
                    await smtp.AuthenticateAsync(senderEmail, cleanPassword);
                    await smtp.SendAsync(message);
                    await smtp.DisconnectAsync(true);

                    _logger.LogInformation("Correo enviado con éxito a {Recipient} vía puerto {Port}", recipientEmail, smtpPort);
                    return true;
                }
                catch (Exception ex587)
                {
                    _logger.LogWarning("Fallo en puerto {Port}: {Msg}. Probando fallback puerto 465 SSL...", smtpPort, ex587.Message);

                    // Fallback automático al puerto 465 SSL
                    using var smtp465 = new SmtpClient();
                    smtp465.Timeout = 8000;
                    await smtp465.ConnectAsync(smtpHost, 465, SecureSocketOptions.SslOnConnect);
                    await smtp465.AuthenticateAsync(senderEmail, cleanPassword);
                    await smtp465.SendAsync(message);
                    await smtp465.DisconnectAsync(true);

                    _logger.LogInformation("Correo enviado con éxito a {Recipient} vía fallback 465 SSL", recipientEmail);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al enviar correo de contacto para {Name}", model.FullName);
                return false;
            }
        }
    }
}
