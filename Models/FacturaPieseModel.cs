using System.Collections.Generic;

namespace DevizWebApp.Models
{
    public class FacturaPieseModel
    {
        // client (optional)
        public string? ClientNume { get; set; }
        public string? ClientCUI { get; set; }
        public string? ClientAdresa { get; set; }

        // piese pe factură
        public List<FacturaItem> Piese { get; set; } = new();
    }
}
