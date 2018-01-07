using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class TipoMarcaModel : ModelBase
    {
        public TipoMarcaModel(int tipoMarcaCod, string tipoMarcaDesc)
        {
            TipoMarcaCod = tipoMarcaCod;
            TipoMarcaDesc = tipoMarcaDesc;
        }

        public TipoMarcaModel()
        {

        }

        public TipoMarcaModel(int tipoMarcaCod, string tipoMarcaDesc, string tipoMarcaIndLogo)
        {
            TipoMarcaCod = tipoMarcaCod;
            TipoMarcaDesc = tipoMarcaDesc;
            TipoMarcaIndLogo = tipoMarcaIndLogo;
        }

        public int TipoMarcaCod { get; set; }
        public string TipoMarcaDesc { get; set; }
        public string TipoMarcaIndLogo { get; set; }

    }

    public class ListTipoMarca : List<TipoMarcaModel>
    {

    }
}