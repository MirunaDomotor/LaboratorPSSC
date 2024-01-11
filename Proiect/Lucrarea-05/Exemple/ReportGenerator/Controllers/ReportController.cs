using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Exemple.ReportGenerator.Controllers
{
    [ApiController]
    [Route("report")]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> _logger;

        public ReportController(ILogger<ReportController> logger)
        {
            _logger = logger;
        }

        [HttpPost("semester-report")]
        public IActionResult GenerateReport()
        {
            //return Ok("Report generated sucessfully");

            // Simulați generarea unui raport
            string reportContent = "Acesta este conținutul raportului generat.";

            // Adăugați conținutul raportului în ViewData
           // ViewData["ReportContent"] = reportContent;

            // Construiți URL-ul către pagina HTML pentru a afișa raportul
            var url = "/ReportView.html";

            // Deschideți automat un browser cu URL-ul generat
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });

            // Returnați un răspuns (opțional)
            return Ok(new { ReportContent = reportContent });
        }

        //[HttpGet("reportview")] // Această rută trebuie să corespundă cu numele acțiunii în Url.Action
        //public IActionResult ReportView()
        //{
        //   // return View("~/Views/Report/ReportView.cshtml");
        //}

        [HttpPost("scholarship")]
        public IActionResult ScholarshipCalculation()
        {
            return Ok("Sholarship calculated sucessfully");
        }
    }
}