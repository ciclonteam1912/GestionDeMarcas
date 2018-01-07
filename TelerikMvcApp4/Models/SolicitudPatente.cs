using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class SolicitudPatente : ModelBase
    {
        public SolicitudPatente(int patenteCodigo, int patenteSolicitud, int solicitanteCodigo, string fechaSolicitud, string fechaVencimiento, string patenteTitulo, int agenteCodigo, int tipoSolicitud)
        {
            PatenteCodigo = patenteCodigo;
            PatenteSolicitud = patenteSolicitud;
            SolicitanteCodigo = solicitanteCodigo;
            FechaSolicitud = fechaSolicitud;
            FechaVencimiento = fechaVencimiento;
            PatenteTitulo = patenteTitulo;
            AgenteCodigo = agenteCodigo;
            TipoSolicitud = tipoSolicitud;
        }

        public SolicitudPatente()
        {

        }
        public int PatenteCodigo { get; set; }
        public int PatenteSolicitud { get; set; }
        public int SolicitanteCodigo { get; set; }
        public string FechaSolicitud { get; set; }
        public string FechaVencimiento { get; set; }
        public string PatenteTitulo { get; set; }
        public int AgenteCodigo { get; set; }
        public int TipoSolicitud { get; set; }

    }

    public class ListPatentes : List<SolicitudPatente>
    {

    }
}