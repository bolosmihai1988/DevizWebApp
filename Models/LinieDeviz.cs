namespace DevizWebApp.Models
{
    public class LinieDeviz
    {
        public string Denumire { get; set; } = string.Empty;
        public double PretCuTVA { get; set; }
        public double PretFaraTVA { get; set; }
        public double TVA { get; set; }
        public double Cantitate { get; set; } = 1; // default 1
    }
}
