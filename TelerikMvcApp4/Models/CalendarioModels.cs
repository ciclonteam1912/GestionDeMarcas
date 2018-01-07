using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class CalendarioModels
    {
      

        public int CodFecha { get; set; }
        public string FechaRegistroExpediente { get; set; }
        public string FechaVencimiento { get; set; }
        public string FechaInicioRegistro { get; set; }
        public string MarcaDescrupcion { get; set; }

        public CalendarioModels(int codFecha, string fechaRegistroExpediente, string fechaVencimiento, string fechaInicioRegistro, string marcaDescrupcion)
        {
            CodFecha = codFecha;
            FechaRegistroExpediente = fechaRegistroExpediente;
            FechaVencimiento = fechaVencimiento;
            FechaInicioRegistro = fechaInicioRegistro;
            MarcaDescrupcion = marcaDescrupcion;
        }
    }

    public class ListCalendarModels : List<CalendarioModels>
    {

    }
}