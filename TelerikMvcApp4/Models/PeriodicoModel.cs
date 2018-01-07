using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class PeriodicoModel : ModelBase
    {
        public PeriodicoModel(int codigoPeriodico, string descPeriodico)
        {
            CodigoPeriodico = codigoPeriodico;
            DescPeriodico = descPeriodico;
        }

        public PeriodicoModel()
        {

        }
        public int CodigoPeriodico { get; set; }
        public string DescPeriodico { get; set; }

    }


    public class ListPeriodicos : List<PeriodicoModel>
    {

    }
}