using DevizWebApp.Data;
using DevizWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace DevizWebApp.Controllers
{
    public class FacturaController : Controller
    {
        private readonly AppDbContext _db;

        public FacturaController(AppDbContext db)
        {
            _db = db;
        }

        // =========================
        // 1) FACTURĂ DIN DEVIZE
        // =========================
        [HttpPost]
        [IgnoreAntiforgeryToken] // pe Render evităm 400
        public async Task<IActionResult> CreateFromDevize([FromForm] int[] devizIds)
        {
            if (devizIds == null || devizIds.Length == 0)
                return RedirectToAction("Istoric", "Deviz");

            await using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                var devize = await _db.Devize
                    .Include(d => d.Items)
                    .Where(d => devizIds.Contains(d.Id))
                    .ToListAsync();

                if (devize.Count == 0)
                    return RedirectToAction("Istoric", "Deviz");

                // nr factură continuu
                int nextNr = (await _db.Facturi.MaxAsync(f => (int?)f.NrFactura) ?? 0) + 1;

                // Client: luăm din primul deviz
                var first = devize.OrderBy(d => d.NrDeviz).First();

                var factura = new Factura
                {
                    NrFactura = nextNr,
                    Data = DateTime.Now.ToString("dd.MM.yyyy"),
                    ClientNume = first.Firma ?? "",
                    ClientCUI = first.CUI ?? "",
                    ClientAdresa = first.Adresa ?? ""
                };

                // Sumar (2 linii: piese + manoperă)
                var devizNrs = devize.Select(d => d.NrDeviz.ToString("D4")).ToList();
                string lista = string.Join(", ", devizNrs);

                decimal totalPiese = devize.Sum(d => d.Items
                    .Where(i => i.Tip == "piesa")
                    .Sum(i => i.TotalLinie));

                decimal totalManopera = devize.Sum(d => d.Items
                    .Where(i => i.Tip == "manopera")
                    .Sum(i => i.TotalLinie));

                if (totalPiese > 0)
                {
                    factura.Items.Add(new FacturaItem
                    {
                        Denumire = $"PIESE AUTO conform deviz nr. {lista}",
                        UM = "buc",
                        Cantitate = 1,
                        PretUnitar = totalPiese,
                        TotalLinie = totalPiese
                    });
                }

                if (totalManopera > 0)
                {
                    factura.Items.Add(new FacturaItem
                    {
                        Denumire = $"MANOPERĂ conform deviz nr. {lista}",
                        UM = "buc",
                        Cantitate = 1,
                        PretUnitar = totalManopera,
                        TotalLinie = totalManopera
                    });
                }

                factura.TotalPiese = totalPiese;
                factura.TotalManopera = totalManopera;
                factura.TotalGeneral = factura.Items.Sum(i => i.TotalLinie);

                _db.Facturi.Add(factura);
                await _db.SaveChangesAsync();

                // legăm devizele de factură (ca istoric intern)
                foreach (var d in devize)
                {
                    _db.FacturaDevize.Add(new FacturaDeviz
                    {
                        FacturaId = factura.Id,
                        DevizId = d.Id
                    });
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return RedirectToAction("Istoric", "Deviz");
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        // =========================
        // 2) FACTURĂ PIESE (FĂRĂ DEVIZ)
        // =========================

        [HttpGet]
        public IActionResult CreatePiese()
        {
            var vm = new FacturaPieseModel();

            // 5 rânduri default (poți crește)
            for (int i = 0; i < 5; i++)
                vm.Piese.Add(new FacturaItem { UM = "buc", Cantitate = 1 });

            return View(vm);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken] // pe Render evităm 400
        public async Task<IActionResult> CreatePiese(FacturaPieseModel vm)
        {
            var piese = (vm.Piese ?? new List<FacturaItem>())
                .Where(x => !string.IsNullOrWhiteSpace(x.Denumire))
                .ToList();

            if (piese.Count == 0)
                return RedirectToAction("Istoric", "Deviz");

            foreach (var it in piese)
            {
                it.Denumire = it.Denumire!.Trim();
                it.UM = string.IsNullOrWhiteSpace(it.UM) ? "buc" : it.UM.Trim();
                if (it.Cantitate <= 0) it.Cantitate = 1;
                if (it.PretUnitar < 0) it.PretUnitar = 0;

                it.TotalLinie = it.Cantitate * it.PretUnitar;
            }

            int nextNr = (await _db.Facturi.MaxAsync(f => (int?)f.NrFactura) ?? 0) + 1;

            var factura = new Factura
            {
                NrFactura = nextNr,
                Data = DateTime.Now.ToString("dd.MM.yyyy"),
                ClientNume = vm.ClientNume ?? "",
                ClientCUI = vm.ClientCUI ?? "",
                ClientAdresa = vm.ClientAdresa ?? "",

                Items = piese,
                TotalPiese = piese.Sum(x => x.TotalLinie),
                TotalManopera = 0,
                TotalGeneral = piese.Sum(x => x.TotalLinie)
            };

            _db.Facturi.Add(factura);
            await _db.SaveChangesAsync();

            return RedirectToAction("Istoric", "Deviz");
        }

        // =========================
        // 3) PDF FACTURĂ
        // =========================
        [HttpGet]
        public async Task<IActionResult> DownloadFacturaPdf(int id)
        {
            var factura = await _db.Facturi
                .Include(f => f.Items)
                .Include(f => f.FacturaDevize)
                    .ThenInclude(fd => fd.Deviz)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (factura == null) return NotFound();

            var doc = new FacturaDocument(factura);
            byte[] pdf = doc.GeneratePdf();

            return File(pdf, "application/pdf", $"Factura_{factura.NrFactura:D4}.pdf");
        }

        // =========================
        // 4) ȘTERGERE FACTURĂ
        // =========================
        [HttpPost]
        [IgnoreAntiforgeryToken] // pe Render evităm 400
        public async Task<IActionResult> DeleteFactura(int id)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                var factura = await _db.Facturi
                    .Include(f => f.Items)
                    .Include(f => f.FacturaDevize)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (factura == null) return NotFound();

                _db.FacturaDevize.RemoveRange(factura.FacturaDevize);
                _db.Facturi.Remove(factura);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return RedirectToAction("Istoric", "Deviz");
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}
