using System.ComponentModel.DataAnnotations;

namespace GLOBALESJP.NET.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Por favor ingrese su nombre y apellido.")]
        [Display(Name = "Nombre completo")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor ingrese su correo electrónico.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        [Display(Name = "Correo corporativo")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor ingrese su número de teléfono o WhatsApp.")]
        [Phone(ErrorMessage = "Ingrese un número telefónico válido.")]
        [Display(Name = "Teléfono / WhatsApp")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Empresa u organización (opcional)")]
        [StringLength(120)]
        public string? Company { get; set; }

        [Required(ErrorMessage = "Seleccione un servicio de su interés.")]
        [Display(Name = "Servicio de interés")]
        public string Service { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor indíquenos brevemente su requerimiento.")]
        [Display(Name = "Mensaje o requerimiento")]
        [StringLength(2000, ErrorMessage = "El mensaje no puede superar los 2000 caracteres.")]
        public string Message { get; set; } = string.Empty;
    }
}
