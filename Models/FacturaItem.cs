using System.ComponentModel.DataAnnotations;

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

    public decimal PretUnitar { get; set; }
    public decimal TotalLinie { get; set; }
}
