using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class TipoMovimientoModel : ModelBase
    {
        public TipoMovimientoModel()
        {

        }

        public TipoMovimientoModel(string descTipoMov)
        {
            DescTipoMov = descTipoMov;
        }

        public TipoMovimientoModel(int codigoTipoMov, string descTipoMov)
        {
            CodigoTipoMov = codigoTipoMov;
            DescTipoMov = descTipoMov;
        }

        public int CodigoTipoMov { get; set; }
        public string DescTipoMov { get; set; }
    }

    public class ListTipoMov : List<TipoMovimientoModel>
    {

    }
}