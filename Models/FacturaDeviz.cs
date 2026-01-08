namespace DevizWebApp.Models;

public class FacturaDeviz
{
    public int Id { get; set; }

    public int FacturaId { get; set; }
    public Factura? Factura { get; set; }

    public int DevizId { get; set; }
    public Deviz? Deviz { get; set; }
}
