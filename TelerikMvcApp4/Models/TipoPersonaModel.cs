using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class TipoPersonaModel : ModelBase
    {
        public TipoPersonaModel(int codTipoPersona, string descTipoPersona)
        {
            CodTipoPersona = codTipoPersona;
            DescTipoPersona = descTipoPersona;
        }

        public int CodTipoPersona { get; set; }
        public string DescTipoPersona { get; set; }
    }

    public class ListTipoPers : List<TipoPersonaModel>
    {

    }
}