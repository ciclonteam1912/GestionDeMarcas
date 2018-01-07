using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class TitularesCreadoresModels : ModelBase
    {      
        public int CodExpe { get; set; }
        public string FechaInicio { get; set; }
        public string Status { get; set; }
        public string TitularExp { get; set; }
        public string CreadorExp { get; set; }
        public string RegExp { get; set; }
        public string FechaCreacion { get; set; }
        public string FechaVencimiento { get; set; }
        public string DesdeFecha { get; set; }
        public string Expediente { get; set; }

        public TitularesCreadoresModels(): base(){
            FechaInicio = string.Empty;
            Status = string.Empty;
            TitularExp = string.Empty;
            CreadorExp = string.Empty;
            RegExp = string.Empty;
            FechaCreacion = string.Empty;
            FechaVencimiento = string.Empty;
            DesdeFecha = string.Empty;
            Expediente = string.Empty;
        }

    }

   
}