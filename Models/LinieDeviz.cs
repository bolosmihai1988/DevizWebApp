namespace DevizWebApp.Models
{
    public class LinieDeviz
    {
        public string? Denumire { get; set; }

        // Cantitatea (piese sau ore de manoperă)
        public double Cantitate { get; set; }

        // Preț NET (fără TVA) pe unitate
        public double PretUnitar { get; set; }

        // ====== COMPATIBILITATE PDF ======

        // Preț net pe unitate (folosit în DevizDocument)
        public double PretFaraTVA => PretUnitar;

        // TVA = 0 pentru că tu lucrezi fără TVA
        public double TVA => 0.0;

        // Preț cu TVA = tot net (pentru că TVA=0)
        public double PretCuTVA => PretUnitar;

        // Total linie (cantitate × preț)
        public double Total => Cantitate * PretUnitar;
    }
}
