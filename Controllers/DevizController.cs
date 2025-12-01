using Microsoft.AspNetCore.Mvc;
using DevizWebApp.Models;
using DevizWebApp.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DevizWebApp.Controllers
{
    public class DevizController : Controller
    {
        private const double TVA_PROCENT = 0.21;
        private readonly AppDbContext _dbContext;

        public DevizController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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

            // --- Număr ordine deviz din DB ---
            int nrOrdine = 1;
            var lastDeviz = _dbContext.Devize.OrderByDescending(d => d.Id).FirstOrDefault();
            if (lastDeviz != null)
                nrOrdine = lastDeviz.NrDeviz + 1;

            model.NrDeviz = nrOrdine;

            // --- Salvează devizul în DB ---
            var devizEntity = model.ToDevizEntity();
            _dbContext.Devize.Add(devizEntity);
            _dbContext.SaveChanges();

            // --- Folder temporar pentru PDF ---
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString("D2");
            string folderPath = Path.Combine(Path.GetTempPath(), year, month);
            Directory.CreateDirectory(folderPath);

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
