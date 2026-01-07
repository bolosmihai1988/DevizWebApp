using System.ComponentModel.DataAnnotations;

namespace DevizWebApp.Models;

public class Deviz
{
    public int Id { get; set; }

    public int NrDeviz { get; set; }

    [MaxLength(200)]
    public string Firma { get; set; } = "";

    [MaxLength(50)]
    public string CUI { get; set; } = "";

    [MaxLength(300)]
    public string Adresa { get; set; } = "";

    [MaxLength(50)]
    public string Telefon { get; set; } = "";

    [MaxLength(50)]
    public string Data { get; set; } = "";

    [MaxLength(200)]
    public string Masina { get; set; } = "";

    [MaxLength(50)]
    public string NrInmat { get; set; } = "";

    [MaxLength(50)]
    public string KM { get; set; } = "";

    [MaxLength(50)]
    public string SerieCaroserie { get; set; } = "";

    [MaxLength(50)]
    public string SerieMotor { get; set; } = "";

    public string Constatare { get; set; } = "";
    public string LucrariConvenite { get; set; } = "";
    public string PieseAduseClient { get; set; } = "";
}
