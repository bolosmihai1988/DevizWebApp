using Microsoft.AspNetCore.Mvc;
using DevizWebApp.Models;
using System.Globalization;
using System.IO;
using QuestPDF.Fluent;
using System.Linq;

namespace DevizWebApp.Controllers
{
    public class DevizController : Controller
    {
        private const double TVA_PROCENT = 0.21;
        private readonly string baseFolder = @"C:\DevizeWebApp\DevizePdf"; // <-- Folder principal DevizePdf

        [HttpGet]
        public IActionResult Index()
        {
            return View(new DevizDocumentModel());
        }

        [HttpPost]
        public IActionResult GeneratePDF(DevizDocumentModel model)
        {
            if (model == null)
                return BadRequest("Datele trimise sunt invalide.");

            // Data devizului - completare manuală dacă nu există
            if (string.IsNullOrWhiteSpace(model.Data))
                model.Data = "_________________";

            // Calcul TVA și PretFaraTVA pentru Piese (cu cantitate)
            foreach (var piesa in model.Piese)
            {
                double pretTotal = piesa.PretCuTVA * piesa.Cantitate;
                piesa.PretFaraTVA = Math.Round(pretTotal / (1 + TVA_PROCENT), 2);
                piesa.TVA = Math.Round(pretTotal - piesa.PretFaraTVA, 2);
                piesa.PretCuTVA = Math.Round(pretTotal, 2);
            }

            // Calcul TVA și PretFaraTVA pentru Manopera
            foreach (var lucrare in model.Manopera)
            {
                lucrare.PretFaraTVA = Math.Round(lucrare.PretCuTVA / (1 + TVA_PROCENT), 2);
                lucrare.TVA = Math.Round(lucrare.PretCuTVA - lucrare.PretFaraTVA, 2);
            }

            // Organizare folder pe an/lună
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString("D2");
            string folderPath = Path.Combine(baseFolder, year, month);
            Directory.CreateDirectory(folderPath);

            // Număr ordine per folder lună
            int nrOrdine = 1;
            var allFiles = Directory.GetFiles(folderPath, "Deviz_*.pdf");
            if (allFiles.Length > 0)
            {
                nrOrdine = allFiles
                    .Select(f => Path.GetFileName(f))
                    .Select(f =>
                    {
                        var parts = f.Split('_');
                        if (parts.Length >= 2 && int.TryParse(parts[1], out int n))
                            return n;
                        return 0;
                    }).Max() + 1;
            }

            model.NrDeviz = nrOrdine;

            string safeFirma = string.Concat(model.Firma.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
            string fileName = $"Deviz_{nrOrdine:D4}_{safeFirma}.pdf";
            string filePath = Path.Combine(folderPath, fileName);

            try
            {
                model.GeneratePdf(filePath);

                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/pdf", fileName);
            }
            catch
            {
                return StatusCode(500, "Eroare la generarea PDF-ului.");
            }
        }
    }
}
