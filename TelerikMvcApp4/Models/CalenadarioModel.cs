using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class CalenadarioModel : ModelBase
    {
      public int IdRegistroCalendario { get; set; }
      public Nullable<DateTime> FechaRegistroCalendario{ get; set; }
      public Nullable<DateTime> FechaPublicacionPrimeraCalendario{ get; set;  }
      public Nullable<DateTime>  FechaVencimiento{ get; set; }
      public string DescripcionRegistroCalendario{ get; set; }

        public CalenadarioModel(int idRegistroCalendario, DateTime? fechaRegistroCalendario, DateTime? fechaPublicacionPrimeraCalendario, DateTime? fechaVencimiento, string descripcionRegistroCalendario)
        {
            IdRegistroCalendario = idRegistroCalendario;
            FechaRegistroCalendario = fechaRegistroCalendario;
            FechaPublicacionPrimeraCalendario = fechaPublicacionPrimeraCalendario;
            FechaVencimiento = fechaVencimiento;
            DescripcionRegistroCalendario = descripcionRegistroCalendario;
        }

    }
    
    public class CalenadarioModelList : List<CalenadarioModel>
    {

    }
}