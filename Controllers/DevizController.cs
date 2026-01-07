using Microsoft.AspNetCore.Mvc;
using DevizWebApp.Models;
using DevizWebApp.Data;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.IO;
using System.Linq;

namespace DevizWebApp.Controllers
{
    public class DevizController : Controller
    {
        private readonly AppDbContext _db;

        public DevizController(AppDbContext db)
        {
            _db = db;
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
                return BadRequest();

            // Nr deviz continuu din DB
            int nrDeviz = (_db.Devize.Any() ? _db.Devize.Max(x => x.NrDeviz) : 0) + 1;
            model.NrDeviz = nrDeviz;

            // SALVARE IN DB (VARIANTA A)
            var deviz = model.ToDevizEntity();
            deviz.NrDeviz = nrDeviz;

            _db.Devize.Add(deviz);
            _db.SaveChanges();

            // PDF temporar
            string fileName = $"Deviz_{nrDeviz:D4}.pdf";
            string tempPath = Path.Combine(Path.GetTempPath(), fileName);

            (model as IDocument).GeneratePdf(tempPath);

            byte[] fileBytes = System.IO.File.ReadAllBytes(tempPath);
            return File(fileBytes, "application/pdf", fileName);
        }

        // ISTORIC
        public IActionResult Istoric()
        {
            var lista = _db.Devize
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(lista);
        }

        // DETALII
        public IActionResult Detalii(int id)
        {
            var deviz = _db.Devize.FirstOrDefault(x => x.Id == id);
            if (deviz == null)
                return NotFound();

            return View(deviz);
        }
    }
}
