using GLOBALESJP.NET.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GLOBALESJP.NET.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ContactViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.FormSubmitted = true;
                return View("Index", model);
            }

            _logger.LogInformation("Nuevo contacto recibido de {FullName} ({Email}) - Servicio: {Service}", 
                model.FullName, model.Email, model.Service);

            TempData["ContactSuccess"] = $"¡Gracias {model.FullName}! Tu mensaje ha sido registrado exitosamente. Nuestro equipo de ingenieros TI se comunicará contigo a la brevedad.";
            
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
