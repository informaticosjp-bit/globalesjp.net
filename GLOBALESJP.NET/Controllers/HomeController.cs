using GLOBALESJP.NET.Models;
using GLOBALESJP.NET.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GLOBALESJP.NET.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ContactViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.FormSubmitted = true;
                return View("Index", model);
            }

            var sent = await _emailService.SendContactNotificationAsync(model);

            if (sent)
            {
                TempData["ContactSuccess"] = $"¡Gracias {model.FullName}! Tu mensaje ha sido enviado correctamente. Nuestro equipo de ingenieros TI se comunicará contigo a la brevedad.";
            }
            else
            {
                TempData["ContactError"] = "Hubo un problema al enviar tu mensaje. Por favor contáctanos directamente por WhatsApp o al correo soportes@globalesjp.net";
            }

            return RedirectToAction("Index", null, "contacto");
        }

        public IActionResult Nosotros()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
