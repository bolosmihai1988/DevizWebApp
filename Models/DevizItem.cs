using System.ComponentModel.DataAnnotations;

namespace DevizWebApp.Models;

public class DevizItem
{
    public int Id { get; set; }

    public int DevizId { get; set; }
    public Deviz? Deviz { get; set; }

    // "piesa" sau "manopera"
    [MaxLength(20)]
    public string Tip { get; set; } = "piesa";

    [MaxLength(500)]
    public string Denumire { get; set; } = "";

    public decimal Cantitate { get; set; } = 1;

    // preț unitar (poți interpreta ca fără TVA sau cu TVA, după cum vrei)
    public decimal PretUnitar { get; set; }

    public decimal TotalLinie { get; set; }
}
