using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class InfoCalendario : ModelBase
    {
        //public InfoCalendario(int codPublicacion, string primeraFechaPublicacion, string segundaFechaPublicacion, string teceraFechaPublicacion, string diarioPublicacion, int expedientePublicacion)
        //{
        //    CodPublicacion = codPublicacion;
        //    PrimeraFechaPublicacion = primeraFechaPublicacion;
        //    SegundaFechaPublicacion = segundaFechaPublicacion;
        //    TeceraFechaPublicacion = teceraFechaPublicacion;
        //    DiarioPublicacion = diarioPublicacion;
        //    ExpedientePublicacion = expedientePublicacion;
        //}

        public InfoCalendario(int expediente, string fecha, int periodico, int dias, int publicado, string observacion)
        {
            this.expediente = expediente;
            this.fecha = fecha;
            this.periodico = periodico;
            this.dias = dias;
            this.publicado = publicado;
            this.observacion = observacion;
        }

        public int expediente { get; set; }
        public string fecha { get; set; }
        public int periodico { get; set; }
        public int dias { get; set; }
        public int publicado { get; set; }
        public string observacion { get; set; }
        //public int CodPublicacion { get; set; }
        //public string  PrimeraFechaPublicacion { get; set; }
        //public string  SegundaFechaPublicacion { get; set; }
        //public string  TeceraFechaPublicacion { get; set; }
        //public string DiarioPublicacion { get; set; }
        //public int ExpedientePublicacion { get; set; }
    }

    public class InfoCalendarioList : List<InfoCalendario>
    {

    }
}