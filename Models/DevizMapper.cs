using System.Linq;
using System.Collections.Generic;

namespace DevizWebApp.Models;

public static class DevizMapper
{
    public static Deviz ToDevizEntity(this DevizDocumentModel m)
    {
        return new Deviz
        {
            NrDeviz = m.NrDeviz,
            Firma = m.Firma,
            CUI = m.CUI,
            Adresa = m.Adresa,
            Telefon = m.Telefon,
            Data = m.Data,
            Masina = m.Masina,
            NrInmat = m.NrInmat,
            KM = m.KM,
            SerieCaroserie = m.SerieCaroserie,
            SerieMotor = m.SerieMotor,
            Constatare = m.Constatare,
            LucrariConvenite = m.LucrariConvenite,
            PieseAduseClient = m.PieseAduseClient
        };
    }
}
