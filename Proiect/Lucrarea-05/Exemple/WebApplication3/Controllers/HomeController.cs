using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Report()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenReport()
        {
            //var reportUrl = Url.Action("Report", "Home");  // Schimbă cu numele și controllerul raportului
            //var script = $"<script>window.open('{reportUrl}', '_blank');</script>";
            //return Content(script, "text/html");
            // Procesează evenimentul și generează raportul (dacă este necesar)

            //return RedirectToAction("Report");

            //string requestBody;
            //using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            //{
            //    requestBody = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();
            //}

            //// Afișează mesajul pe pagina Report
            //ViewBag.Message = requestBody;
            return View("Report");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}