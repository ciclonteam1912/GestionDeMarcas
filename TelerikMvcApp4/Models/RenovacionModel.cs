using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class RenovacionModel : ModelBase
    {
        public int CodRegistro { get; set; }

        public string  FechaCreacion { get; set; }

        public string  StatusRenovacion { get; set; }

        public string RegistroRenovacion { get; set; }

        public string FechaInicioRenovacion { get; set; }

        public string  VencimientoRenovacion { get; set; }

        public string DesdeFechaRenovacion { get; set; }

        public string MarcaRenovacion { get; set; }

        public string ClaseRenovacion { get; set; }

        public string EdNizaRenovacion { get; set; }

        public string  TipoRenovacion { get; set; }

        public string PrioridadRenovacion { get; set; }

        public string RenovacionExp { get; set; }

        public RenovacionModel() : base()
        {
            CodRegistro = 0;
            FechaCreacion = string.Empty;
            StatusRenovacion = string.Empty;
            RegistroRenovacion = string.Empty;
            FechaInicioRenovacion = string.Empty;
            VencimientoRenovacion = string.Empty;
            DesdeFechaRenovacion = string.Empty;
            MarcaRenovacion = string.Empty;
            ClaseRenovacion = string.Empty;
            EdNizaRenovacion = string.Empty;
            TipoRenovacion = string.Empty;
            PrioridadRenovacion = string.Empty;
            RenovacionExp = string.Empty;
        }

    }
}