using GLOBALESJP.NET.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace GLOBALESJP.NET.Services
{
    /// <summary>
    /// Implementación del servicio de correo usando MailKit + Gmail SMTP con App Password.
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
                var smtpHost     = settings["SmtpHost"]       ?? "smtp.gmail.com";
                var smtpPort     = int.Parse(settings["SmtpPort"] ?? "587");
                var senderEmail  = settings["SenderEmail"]    ?? "";
                var senderName   = settings["SenderName"]     ?? "GLOBALES JP";
                var appPassword  = settings["AppPassword"]    ?? "";
                var recipientEmail = settings["RecipientEmail"] ?? "";
                var recipientName  = settings["RecipientName"]  ?? "Soporte";

                var whatsappPhone = model.Phone.Replace("+", "").Replace(" ", "").Replace("-", "");
                // Asegura prefijo Venezuela si no trae código de país
                if (whatsappPhone.StartsWith("04") && whatsappPhone.Length <= 11)
                    whatsappPhone = "58" + whatsappPhone.Substring(1);

                var waLink = $"https://wa.me/{whatsappPhone}?text=Hola%20{Uri.EscapeDataString(model.FullName)},%20soy%20del%20equipo%20de%20GLOBALES%20JP%20y%20recib%C3%AD%20tu%20solicitud.%20%C2%BFEn%20qu%C3%A9%20puedo%20ayudarte%3F";
                var replyLink = $"mailto:{model.Email}?subject=Re:%20Tu%20solicitud%20en%20GLOBALES%20JP%20-%20{Uri.EscapeDataString(model.Service)}";

                var htmlBody = $@"
<!DOCTYPE html>
<html lang='es'>
<head><meta charset='UTF-8'/><meta name='viewport' content='width=device-width, initial-scale=1.0'/></head>
<body style='margin:0;padding:0;background:#0a0f1e;font-family:""Segoe UI"",Arial,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='background:#0a0f1e;padding:32px 16px;'>
    <tr><td align='center'>
      <table width='640' cellpadding='0' cellspacing='0' style='max-width:640px;width:100%;background:#111827;border-radius:16px;overflow:hidden;border:1px solid rgba(255,255,255,0.08);'>

        <!-- HEADER -->
        <tr>
          <td style='background:linear-gradient(135deg,#0f2044 0%,#0a1628 100%);padding:28px 32px;border-bottom:2px solid #06b6d4;'>
            <table width='100%' cellpadding='0' cellspacing='0'>
              <tr>
                <td>
                  <div style='font-size:11px;font-family:monospace;color:#06b6d4;letter-spacing:2px;margin-bottom:4px;'>GLOBALES JP | SISTEMA DE NOTIFICACIONES</div>
                  <h1 style='margin:0;color:#ffffff;font-size:22px;font-weight:700;'>📩 Nueva Solicitud de Contacto</h1>
                  <div style='margin-top:6px;font-size:13px;color:#94a3b8;'>Recibida el {DateTime.Now:dd/MM/yyyy} a las {DateTime.Now:HH:mm} hrs</div>
                </td>
                <td align='right' style='vertical-align:top;'>
                  <div style='background:rgba(16,185,129,0.15);border:1px solid rgba(16,185,129,0.4);border-radius:8px;padding:6px 14px;display:inline-block;'>
                    <span style='color:#10b981;font-size:12px;font-weight:600;'>● NUEVO LEAD</span>
                  </div>
                </td>
              </tr>
            </table>
          </td>
        </tr>

        <!-- DATOS DEL CLIENTE -->
        <tr>
          <td style='padding:28px 32px;'>
            <div style='background:rgba(6,182,212,0.05);border:1px solid rgba(6,182,212,0.15);border-radius:12px;padding:20px 24px;margin-bottom:20px;'>
              <div style='font-size:11px;font-family:monospace;color:#06b6d4;letter-spacing:1.5px;margin-bottom:16px;'>DATOS DEL CLIENTE</div>
              <table width='100%' cellpadding='0' cellspacing='0'>
                <tr>
                  <td style='padding-bottom:12px;'>
                    <div style='font-size:11px;color:#64748b;margin-bottom:2px;'>NOMBRE COMPLETO</div>
                    <div style='font-size:16px;color:#f1f5f9;font-weight:600;'>{System.Net.WebUtility.HtmlEncode(model.FullName)}</div>
                  </td>
                </tr>
                <tr>
                  <td style='padding-bottom:12px;'>
                    <div style='font-size:11px;color:#64748b;margin-bottom:2px;'>EMPRESA / ORGANIZACIÓN</div>
                    <div style='font-size:15px;color:#e2e8f0;'>{System.Net.WebUtility.HtmlEncode(model.Company ?? "Particular / No especificado")}</div>
                  </td>
                </tr>
                <tr>
                  <td style='padding-bottom:12px;'>
                    <div style='font-size:11px;color:#64748b;margin-bottom:2px;'>CORREO ELECTRÓNICO</div>
                    <div style='font-size:15px;'><a href='mailto:{model.Email}' style='color:#38bdf8;text-decoration:none;'>{System.Net.WebUtility.HtmlEncode(model.Email)}</a></div>
                  </td>
                </tr>
                <tr>
                  <td style='padding-bottom:4px;'>
                    <div style='font-size:11px;color:#64748b;margin-bottom:2px;'>TELÉFONO / WHATSAPP</div>
                    <div style='font-size:15px;color:#e2e8f0;'>{System.Net.WebUtility.HtmlEncode(model.Phone)}</div>
                  </td>
                </tr>
              </table>
            </div>

            <!-- SERVICIO SELECCIONADO -->
            <div style='background:rgba(139,92,246,0.08);border:1px solid rgba(139,92,246,0.2);border-radius:12px;padding:16px 24px;margin-bottom:20px;'>
              <div style='font-size:11px;font-family:monospace;color:#a78bfa;letter-spacing:1.5px;margin-bottom:8px;'>SERVICIO DE INTERÉS</div>
              <div style='font-size:16px;color:#c4b5fd;font-weight:600;'>🔧 {System.Net.WebUtility.HtmlEncode(model.Service)}</div>
            </div>

            <!-- MENSAJE -->
            <div style='background:rgba(15,23,42,0.7);border:1px solid rgba(255,255,255,0.06);border-radius:12px;padding:20px 24px;margin-bottom:24px;'>
              <div style='font-size:11px;font-family:monospace;color:#94a3b8;letter-spacing:1.5px;margin-bottom:12px;'>MENSAJE / REQUERIMIENTO</div>
              <div style='font-size:14px;color:#cbd5e1;line-height:1.7;white-space:pre-wrap;'>{System.Net.WebUtility.HtmlEncode(model.Message)}</div>
            </div>

            <!-- BOTONES DE ACCIÓN RÁPIDA -->
            <table width='100%' cellpadding='0' cellspacing='0'>
              <tr>
                <td style='padding-right:8px;width:50%;'>
                  <a href='{replyLink}' style='display:block;text-align:center;background:linear-gradient(135deg,#0284c7,#0369a1);color:#ffffff;padding:14px 20px;border-radius:10px;text-decoration:none;font-weight:600;font-size:14px;'>
                    ✉️ Responder al Cliente
                  </a>
                </td>
                <td style='padding-left:8px;width:50%;'>
                  <a href='{waLink}' style='display:block;text-align:center;background:linear-gradient(135deg,#16a34a,#15803d);color:#ffffff;padding:14px 20px;border-radius:10px;text-decoration:none;font-weight:600;font-size:14px;'>
                    📱 Abrir WhatsApp del Cliente
                  </a>
                </td>
              </tr>
            </table>
          </td>
        </tr>

        <!-- FOOTER -->
        <tr>
          <td style='background:#0d1117;padding:18px 32px;border-top:1px solid rgba(255,255,255,0.05);'>
            <div style='font-size:11px;color:#475569;text-align:center;font-family:monospace;'>
              GLOBALESJP.NET · Sistema de Notificaciones Automáticas · Este correo fue generado automáticamente desde el formulario de contacto del sitio web.
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
                message.Subject = $"[GLOBALES JP] Nueva solicitud: {model.Service} — {model.FullName}";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody,
                    TextBody = $"Nueva solicitud de contacto recibida:\n\n" +
                               $"Nombre: {model.FullName}\n" +
                               $"Empresa: {model.Company ?? "No especificada"}\n" +
                               $"Correo: {model.Email}\n" +
                               $"Teléfono: {model.Phone}\n" +
                               $"Servicio: {model.Service}\n\n" +
                               $"Mensaje:\n{model.Message}"
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(senderEmail, appPassword.Replace(" ", ""));
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Correo de contacto enviado correctamente desde {From} para {Client} ({Email})",
                    senderEmail, model.FullName, model.Email);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo de contacto para {Client} ({Email})",
                    model.FullName, model.Email);
                return false;
            }
        }
    }
}
