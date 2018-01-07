using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class NizaClaseModels
    {
        public NizaClaseModels(int codEdNiza, string descEdNiza)
        {
            CodEdNiza = codEdNiza;
            DescEdNiza = descEdNiza;
        }

        public int CodEdNiza { get; set; }
        public string DescEdNiza { get; set; }
    }

    public class NizaModelsList : List<NizaClaseModels> { }

}