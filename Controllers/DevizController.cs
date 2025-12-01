using Microsoft.AspNetCore.Mvc;
using DevizWebApp.Models;
using DevizWebApp.Data;
using System.Linq;
using System.IO;
using QuestPDF.Fluent;

namespace DevizWebApp.Controllers
{
    public class DevizController : Controller
    {
        private const double TVA_PROCENT = 0.21;
        private readonly AppDbContext _context;

        public DevizController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new DevizDocumentModel());
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult GeneratePDF(DevizDocumentModel model)
        {
            if (model == null)
                return BadRequest("Datele trimise sunt invalide.");

            // --- Calcul TVA și PretFaraTVA ---
            foreach (var piesa in model.Piese)
            {
                double pretTotal = piesa.PretCuTVA * piesa.Cantitate;
                piesa.PretFaraTVA = Math.Round(pretTotal / (1 + TVA_PROCENT), 2);
                piesa.TVA = Math.Round(pretTotal - piesa.PretFaraTVA, 2);
                piesa.PretCuTVA = Math.Round(pretTotal, 2);
            }

            foreach (var lucrare in model.Manopera)
            {
                lucrare.PretFaraTVA = Math.Round(lucrare.PretCuTVA / (1 + TVA_PROCENT), 2);
                lucrare.TVA = Math.Round(lucrare.PretCuTVA - lucrare.PretFaraTVA, 2);
            }

            // --- Obține următorul număr deviz din DB ---
            var lastDevizNr = _context.Devize.Max(d => (int?)d.NrDeviz) ?? 0;
            model.NrDeviz = lastDevizNr + 1;

            // --- Salvează devizul în DB ---
            var devizEntity = model.ToDevizEntity();
            _context.Devize.Add(devizEntity);
            _context.SaveChanges();

            // --- Generare PDF ---
            string tempFolder = Path.Combine(Path.GetTempPath(), "DevizePdf");
            Directory.CreateDirectory(tempFolder);

            string safeFirma = string.Concat(model.Firma.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
            string fileName = $"Deviz_{model.NrDeviz:D4}_{safeFirma}.pdf";
            string filePath = Path.Combine(tempFolder, fileName);

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
