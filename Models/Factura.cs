using System.ComponentModel.DataAnnotations;

namespace DevizWebApp.Models;

public class Factura
{
    public int Id { get; set; }

    public int NrFactura { get; set; }

    [MaxLength(50)]
    public string Data { get; set; } = "";

    [MaxLength(200)]
    public string ClientNume { get; set; } = "";

    [MaxLength(50)]
    public string ClientCUI { get; set; } = "";

    [MaxLength(300)]
    public string ClientAdresa { get; set; } = "";

    // TOTALURI
    public decimal TotalPiese { get; set; }
    public decimal TotalManopera { get; set; }
    public decimal TotalGeneral { get; set; }

    // LISTE – FOARTE IMPORTANT
    public List<FacturaItem> Items { get; set; } = new();
    public List<FacturaDeviz> FacturaDevize { get; set; } = new();
}
