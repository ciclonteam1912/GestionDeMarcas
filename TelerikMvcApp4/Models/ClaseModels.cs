using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class ClaseModels : ModelBase
    {
        public ClaseModels(int codClase, string descClase)
        {
            CodClase = codClase;
            DescClase = descClase;
        }

        public ClaseModels()
        {

        }

        public int CodClase { get; set; }
        public string DescClase { get; set; }
    }

    public class ClaseList : List<ClaseModels>
    {

    }
}