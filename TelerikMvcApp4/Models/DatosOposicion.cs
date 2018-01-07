using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class DatosOposicion : ModelBase
    {
        public DatosOposicion(int codigoEmail, string titular)
        {
            CodigoEmail = codigoEmail;
            Titular = titular;
        }

        public DatosOposicion(int codigoEmail, string denominacionMarca, string titular)
        {
            CodigoEmail = codigoEmail;
            DenominacionMarca = denominacionMarca;
            Titular = titular;
        }

        public int CodigoEmail { get; set; }
        public string DenominacionMarca { get; set; }
        public string Titular { get; set; }
    }

    public class listOposiciones : List<DatosOposicion>
    {

    }
}