using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class TitularAOponerse : ModelBase
    {
        public TitularAOponerse(int codigoTitular, string nombreTitular)
        {
            CodigoTitular = codigoTitular;
            NombreTitular = nombreTitular;
        }

        public TitularAOponerse(int codigoTitular, string nombreTitular, int codMarcaParecido)
        {
            CodigoTitular = codigoTitular;
            NombreTitular = nombreTitular;
            CodMarcaParecido = codMarcaParecido;
        }

        public int CodigoTitular { get; set; }
        public string NombreTitular { get; set; }
        public int CodMarcaParecido { get; set; }
    }

    public class ListTitulares : List<TitularAOponerse>
    {

    }
}