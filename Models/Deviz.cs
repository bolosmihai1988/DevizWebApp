using System.ComponentModel.DataAnnotations;

namespace DevizWebApp.Models
{
    public class Deviz
    {
        [Key]
        public int Id { get; set; }

        public int NrDeviz { get; set; }

        public string Firma { get; set; } = string.Empty;
        public string CUI { get; set; } = string.Empty;
        public string Adresa { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;

        public string Masina { get; set; } = string.Empty;
        public string NrInmat { get; set; } = string.Empty;
        public string KM { get; set; } = string.Empty;
        public string SerieCaroserie { get; set; } = string.Empty;
        public string SerieMotor { get; set; } = string.Empty;

        public string Constatare { get; set; } = string.Empty;
        public string LucrariConvenite { get; set; } = string.Empty;
        public string PieseAduseClient { get; set; } = string.Empty;

        // Optional: poți adăuga câmpuri JSON pentru Piese și Manopera dacă vrei să le salvezi în DB
    }
}
