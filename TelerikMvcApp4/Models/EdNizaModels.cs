using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class EdNizaModels : ModelBase
    {
        public EdNizaModels(int codEdNiza, string descEdNiza, int codClase, int nizNroClase)
        {
            CodEdNiza = codEdNiza;
            DescEdNiza = descEdNiza;
            CodClase = codClase;
            NizNroClase = nizNroClase;
        }

        public EdNizaModels(int codClase, int nizNroClase, string descEdNiza)
        {
            CodClase = codClase;
            NizNroClase = nizNroClase;
            DescEdNiza = descEdNiza;
        }

        public EdNizaModels(int nizNroClase)
        {
            NizNroClase = nizNroClase;
        }

        public EdNizaModels(string descEdNiza)
        {
            DescEdNiza = descEdNiza;
        }

        public EdNizaModels()
        {

        }
        public int CodEdNiza { get; set; }

        public string DescEdNiza { get; set; }

        public int CodClase { get; set; }
        public int NizNroClase { get; set; }
    }

    public class EdNizaList : List<EdNizaModels>
    {

    }
}