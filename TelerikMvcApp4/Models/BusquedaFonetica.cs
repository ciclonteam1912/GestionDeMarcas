using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class BusquedaFonetica : ModelBase
    {
        public BusquedaFonetica(int codBusqueda, string expedienteBusqeuda, string denominacionBusqueda, string titularBusqueda, string claseBusqueda, string parecidoFoneticoBusqueda, string paracidoEscritoBusqeuda)
        {
            CodBusqueda = codBusqueda;
            ExpedienteBusqeuda = expedienteBusqeuda;
            DenominacionBusqueda = denominacionBusqueda;
            TitularBusqueda = titularBusqueda;
            ClaseBusqueda = claseBusqueda;
            ParecidoFoneticoBusqueda = parecidoFoneticoBusqueda;
            ParacidoEscritoBusqeuda = paracidoEscritoBusqeuda;
        }

        public int CodBusqueda { get; set; }
        public string ExpedienteBusqeuda { get; set; }
        public string DenominacionBusqueda { get; set; }
        public string TitularBusqueda { get; set; }
        public string ClaseBusqueda { get; set; }
        public string ParecidoFoneticoBusqueda { get; set; }
        public string ParacidoEscritoBusqeuda { get; set; }
        
    }

    public class ListBusquedaFonetica : List<BusquedaFonetica>
    {

    }
}