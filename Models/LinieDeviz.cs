namespace DevizWebApp.Models
{
    public class LinieDeviz
    {
        public string? Denumire { get; set; }

        // Cantitatea (piese sau ore de manoperă)
        public double Cantitate { get; set; }

        // Preț FINAL, cu TVA inclus, introdus de utilizator
        public double PretUnitar { get; set; }

        // ====== COMPATIBILITATE PDF ======

        // Cota TVA 21%
        public double CotaTVA => 0.21;

        // Preț fără TVA pe unitate
        public double PretFaraTVA => PretUnitar / (1 + CotaTVA);

        // TVA inclus în prețul unitar
        public double TVA => PretUnitar - PretFaraTVA;

        // Preț cu TVA = exact prețul introdus de tine
        public double PretCuTVA => PretUnitar;

        // Total linie cu TVA inclus
        public double Total => Cantitate * PretUnitar;

        // Total fără TVA pentru linie
        public double TotalFaraTVA => Cantitate * PretFaraTVA;

        // TVA total pentru linie
        public double TotalTVA => Cantitate * TVA;
    }
}