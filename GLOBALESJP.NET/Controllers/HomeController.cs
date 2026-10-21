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
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                          Request.Headers.Accept.ToString().Contains("application/json");

            if (!ModelState.IsValid)
            {
                if (isAjax)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { success = false, message = "Por favor verifica los campos obligatorios.", errors });
                }

                ViewBag.FormSubmitted = true;
                return View("Index", model);
            }

            // Enviamos el correo a informaticosjp@gmail.com con un timeout suave de 4 segundos
            // Si la conexión tarda más en la nube, se completa en segundo plano para no demorar al cliente
            var sendTask = _emailService.SendContactNotificationAsync(model);
            var completedTask = await Task.WhenAny(sendTask, Task.Delay(4000));

            _logger.LogInformation("Solicitud de contacto recibida de {FullName} ({Email}) - Notificación procesada", 
                model.FullName, model.Email);

            if (isAjax)
            {
                return Json(new { success = true, message = "Enviado" });
            }

            TempData["ContactSuccess"] = "¡Enviado! Tu mensaje ha sido recibido exitosamente.";
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
