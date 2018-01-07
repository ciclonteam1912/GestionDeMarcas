using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class ExpedientePoder : ModelBase
    {
        public ExpedientePoder(int expediente, int poder, int codTitular, string fecha)
        {
            //this.codPoder = codPoder;
            Expediente = expediente;
            Poder = poder;
            this.codTitular = codTitular;            
            this.fecha = fecha;
        }

        //public int codPoder { get; set; }
        public int Poder { get; set; }
        public int codTitular { get; set; }
        public int Expediente { get; set; }
        public string fecha { get; set; }
    }

    public class ExpedientePoderList : List<ExpedientePoder>
    {

    }
}