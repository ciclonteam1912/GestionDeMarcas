using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class DescripcionMarcaModel : ModelBase
    {
        public int IdMarca { get; set; }

        public int DescripcionMarca { get; set; }
        public DescripcionMarcaModel(int idMarca, int descripcionMarca)
        {
            IdMarca = idMarca;
            DescripcionMarca = descripcionMarca;
        }
     
    }

    public class DescripcionMarcaList : List<DescripcionMarcaModel>
    {

    }
}