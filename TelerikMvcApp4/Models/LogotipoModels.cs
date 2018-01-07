using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class LogotipoModels : ModelBase
    {
       
        public int CodReg { get; set; }

        public string  FechaCreada { get; set; }

        public string Status { get; set; }

        public string RegistroReg { get; set; }

        public string  VencimientoReg { get; set; }

        public string FechaDesdeReg { get; set; }


        public LogotipoModels() : base()
        {
            CodReg = 0;
            FechaCreada = string.Empty;
            Status = string.Empty;
            RegistroReg = string.Empty;
            VencimientoReg = string.Empty;
            FechaDesdeReg = string.Empty;
        }

    }
}