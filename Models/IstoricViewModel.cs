using System.Collections.Generic;

namespace DevizWebApp.Models
{
    public class IstoricViewModel
    {
        public List<Deviz> Devize { get; set; } = new();
        public List<Factura> Facturi { get; set; } = new();
    }
}
