using DevizWebApp.Data;
using DevizWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

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
                return BadRequest("Date invalide.");

            // Data dacă e goală
            if (string.IsNullOrWhiteSpace(model.Data))
                model.Data = DateTime.Now.ToString("dd.MM.yyyy");

            // ✅ Nr deviz continuu din DB
            int nrDeviz = (_db.Devize.Any() ? _db.Devize.Max(d => d.NrDeviz) : 0) + 1;
            model.NrDeviz = nrDeviz;

            // ✅ Construim entitatea Deviz (fără TVA)
            var deviz = model.ToDevizEntity();
            deviz.NrDeviz = nrDeviz;

            // Salvăm devizul ca să primească Id
            _db.Devize.Add(deviz);
            _db.SaveChanges();

            // ✅ Salvăm liniile (piese + manoperă) în DevizItems
            var items = new List<DevizItem>();

            // PIESE
            if (model.Piese != null)
            {
                foreach (var p in model.Piese)
                {
                    if (p == null) continue;
                    if (string.IsNullOrWhiteSpace(p.Denumire)) continue;

                    var cant = (decimal)p.Cantitate;
                    var pret = (decimal)p.PretUnitar;
                    var total = cant * pret;

                    items.Add(new DevizItem
                    {
                        DevizId = deviz.Id,
                        Tip = "piesa",
                        Denumire = p.Denumire.Trim(),
                        Cantitate = cant,
                        PretUnitar = pret,
                        TotalLinie = total
                    });
                }
            }

            // MANOPERĂ
            if (model.Manopera != null)
            {
                foreach (var m in model.Manopera)
                {
                    if (m == null) continue;
                    if (string.IsNullOrWhiteSpace(m.Denumire)) continue;

                    var cant = (decimal)m.Cantitate;   // ore
                    var pret = (decimal)m.PretUnitar;  // preț/oră
                    var total = cant * pret;

                    items.Add(new DevizItem
                    {
                        DevizId = deviz.Id,
                        Tip = "manopera",
                        Denumire = m.Denumire.Trim(),
                        Cantitate = cant,
                        PretUnitar = pret,
                        TotalLinie = total
                    });
                }
            }

            if (items.Count > 0)
            {
                _db.DevizItems.AddRange(items);
                _db.SaveChanges();
            }

            // ✅ Generăm PDF din model (care are listele Piese/Manopera)
            try
            {
                var document = (IDocument)model;
                byte[] pdf = document.GeneratePdf();

                string fileName = $"Deviz_{nrDeviz:D4}.pdf";
                return File(pdf, "application/pdf", fileName);
            }
            catch
            {
                return StatusCode(500, "Eroare la generarea PDF-ului.");
            }
        }

        [HttpGet]
        public IActionResult Istoric()
        {
            var devize = _db.Devize
                .OrderByDescending(d => d.NrDeviz)
                .ToList();

            var facturi = _db.Facturi
                .Include(f => f.Items)
                .OrderByDescending(f => f.NrFactura)
                .ToList();

            var vm = new IstoricViewModel
            {
                Devize = devize,
                Facturi = facturi
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult DownloadDevizPdf(int id)
        {
            var deviz = _db.Devize.FirstOrDefault(d => d.Id == id);
            if (deviz == null) return NotFound();

            var items = _db.DevizItems
                .Where(i => i.DevizId == deviz.Id)
                .OrderBy(i => i.Id)
                .ToList();

            var model = new DevizDocumentModel();
            model.LoadFromDeviz(deviz, items);

            var document = (IDocument)model;
            byte[] pdf = document.GeneratePdf();

            var fileName = $"Deviz_{deviz.NrDeviz:D4}.pdf";
            return File(pdf, "application/pdf", fileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDeviz(int id)
        {
            // ștergem și liniile
            var items = _db.DevizItems.Where(i => i.DevizId == id).ToList();
            if (items.Count > 0)
                _db.DevizItems.RemoveRange(items);

            var deviz = _db.Devize.FirstOrDefault(x => x.Id == id);
            if (deviz == null) return NotFound();

            _db.Devize.Remove(deviz);
            _db.SaveChanges();

            return RedirectToAction("Istoric");
        }
    }

    public class IstoricViewModel
    {
        public List<Deviz> Devize { get; set; } = new();
        public List<Factura> Facturi { get; set; } = new();
    }
}
