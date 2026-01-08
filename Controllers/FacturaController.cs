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

        // Creează factură din devize bifate în Istoric
        [HttpPost]
        [IgnoreAntiForgeryToken]
        public async Task<IActionResult> CreateFromDevize([FromForm] int[] devizIds)
        {
            if (devizIds == null || devizIds.Length == 0)
                return RedirectToAction("Istoric", "Deviz");

            // Tranzacție = rollback dacă apare orice eroare
            await using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                var devize = await _db.Devize
                    .Include(d => d.Items)
                    .Where(d => devizIds.Contains(d.Id))
                    .ToListAsync();

                if (devize.Count == 0)
                    return RedirectToAction("Istoric", "Deviz");

                // OPTIONAL: prevenim refacturarea acelorași devize (dacă vrei)
                // Dacă vrei să PERMIȚI refacturarea, scoate blocul de mai jos.
                var alreadyLinked = await _db.FacturaDevize
                    .Where(fd => devizIds.Contains(fd.DevizId))
                    .AnyAsync();

                if (alreadyLinked)
                {
                    // devizele sunt deja prinse într-o factură
                    // Poți schimba mesajul / redirect cum vrei
                    return RedirectToAction("Istoric", "Deviz");
                }

                // nr factură continuu
                int nextNr = (await _db.Facturi.MaxAsync(f => (int?)f.NrFactura) ?? 0) + 1;

                // Client: luăm din primul deviz (poți rafina ulterior)
                var first = devize.OrderBy(d => d.NrDeviz).First();

                var factura = new Factura
                {
                    NrFactura = nextNr,
                    Data = DateTime.Now.ToString("dd.MM.yyyy"),

                    // dacă ai câmpuri în Deviz pentru client, le mapăm aici
                    ClientNume = first.Firma,
                    ClientCUI = first.CUI ?? "",
                    ClientAdresa = first.Adresa ?? ""
                };

                // Construim sumarul (2 linii)
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

                factura.TotalGeneral = factura.Items.Sum(i => i.TotalLinie);

                // Salvăm factura + items (cascade)
                _db.Facturi.Add(factura);
                await _db.SaveChangesAsync();

                // Legăm devizele de factură
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

        // Ștergere factură (hard delete) - curată + tranzacție
        [HttpPost]
        [IgnoreAntiForgeryToken]
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

                // explict remove links (deși ai Cascade pe FacturaDevize, e ok și explicit)
                _db.FacturaDevize.RemoveRange(factura.FacturaDevize);

                // Items sunt Cascade din Facturi -> Items, dar și aici e ok să lași doar Remove(Factura)
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
