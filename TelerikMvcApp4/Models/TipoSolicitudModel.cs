using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class TipoSolicitudModel : ModelBase
    {

        public TipoSolicitudModel()
        {

        }
        public TipoSolicitudModel(string descTipoSolicitud)
        {
            DescTipoSolicitud = descTipoSolicitud;
        }

        public TipoSolicitudModel(string descTipoSolicitud, string abrevTipoSolicitud)
        {
            DescTipoSolicitud = descTipoSolicitud;
            AbrevTipoSolicitud = abrevTipoSolicitud;
        }

        public TipoSolicitudModel(int codTipoSolicitud, string descTipoSolicitud, string abrevTipoSolicitud)
        {
            CodTipoSolicitud = codTipoSolicitud;
            DescTipoSolicitud = descTipoSolicitud;
            AbrevTipoSolicitud = abrevTipoSolicitud;
        }

        public int CodTipoSolicitud { get; set; }
        public string DescTipoSolicitud { get; set; }
        public string AbrevTipoSolicitud { get; set; }
    }

    public class ListTipoSol : List<TipoSolicitudModel>
    {

    }
}