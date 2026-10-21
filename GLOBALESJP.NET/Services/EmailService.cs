using GLOBALESJP.NET.Models;
using System.Net.Http.Json;

namespace GLOBALESJP.NET.Services
{
    /// <summary>
    /// Servicio de notificación que envía por HTTPS (Puerto 443) a informaticosjp@gmail.com
    /// Compatible con Render (evita el bloqueo de puertos SMTP 25, 465 y 587).
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };

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
                var recipientEmail = Environment.GetEnvironmentVariable("EMAIL_RECIPIENT") 
                                     ?? settings["RecipientEmail"] 
                                     ?? "informaticosjp@gmail.com";

                var payload = new Dictionary<string, string>
                {
                    { "_subject", $"[LEAD WEB GLOBALES JP] {model.Service} — {model.FullName}" },
                    { "_replyto", model.Email },
                    { "_captcha", "false" },
                    { "_template", "table" },
                    { "Nombre del Cliente", model.FullName },
                    { "Empresa", string.IsNullOrWhiteSpace(model.Company) ? "Particular / No especificado" : model.Company },
                    { "Correo Electrónico", model.Email },
                    { "Teléfono / WhatsApp", model.Phone },
                    { "Servicio Solicitado", model.Service },
                    { "Mensaje del Cliente", model.Message }
                };

                // Envío mediante HTTPS (puerto 443) garantizado sin bloqueos en Render
                var response = await _httpClient.PostAsJsonAsync($"https://formsubmit.co/ajax/{recipientEmail}", payload);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Notificación enviada con éxito a {Recipient} vía HTTPS", recipientEmail);
                    return true;
                }
                else
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Respuesta FormSubmit ({Status}): {Body}", response.StatusCode, errorMsg);
                    return true; // No fallamos para la UX del usuario
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar notificación para {Name}", model.FullName);
                return false;
            }
        }
    }
}
