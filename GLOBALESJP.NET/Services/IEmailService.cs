using GLOBALESJP.NET.Models;

namespace GLOBALESJP.NET.Services
{
    /// <summary>
    /// Contrato para el servicio de envío de correos del formulario de contacto.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envía la notificación de nuevo contacto al correo corporativo.
        /// </summary>
        Task<bool> SendContactNotificationAsync(ContactViewModel model);
    }
}
