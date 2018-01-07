using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class PaisModel : ModelBase
    {
        public PaisModel(int paisCodigo, string paisDescripcion)
        {
            PaisCodigo = paisCodigo;
            PaisDescripcion = paisDescripcion;
        }

        public PaisModel(string paisDescripcion)
        {
            PaisDescripcion = paisDescripcion;
        }

        public PaisModel()
        {

        }

        public int PaisCodigo { get; set; }
        public string PaisDescripcion { get; set; }
    }

    public class ListPaises : List<PaisModel>
    {

    }
}