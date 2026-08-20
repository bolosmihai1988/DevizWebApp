using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevizWebApp.Models;

public class FacturaItem
{
    public int Id { get; set; }

    public int FacturaId { get; set; }
    public Factura? Factura { get; set; }

    [MaxLength(500)]
    public string Denumire { get; set; } = "";

    [MaxLength(20)]
    public string UM { get; set; } = "buc";

    public decimal Cantitate { get; set; } = 1;

    // Prețul FINAL introdus de tine, cu TVA inclus
    public decimal PretUnitar { get; set; }

    // Total final linie = Cantitate × Preț final cu TVA
    public decimal TotalLinie { get; set; }

    // ==========================
    // CALCULE TVA 21%
    // ==========================

    [NotMapped]
    public decimal CotaTVA => 0.21m;

    // Preț unitar fără TVA
    [NotMapped]
    public decimal PretUnitarFaraTVA =>
        PretUnitar / (1 + CotaTVA);

    // TVA aferent unei unități
    [NotMapped]
    public decimal TVAUnitar =>
        PretUnitar - PretUnitarFaraTVA;

    // Valoare fără TVA = cantitate × preț unitar fără TVA
    [NotMapped]
    public decimal ValoareFaraTVA =>
        Cantitate * PretUnitarFaraTVA;

    // TVA total aferent liniei
    [NotMapped]
    public decimal TotalTVA =>
        Cantitate * TVAUnitar;

    // Valoare finală cu TVA
    [NotMapped]
    public decimal ValoareCuTVA =>
        Cantitate * PretUnitar;
}