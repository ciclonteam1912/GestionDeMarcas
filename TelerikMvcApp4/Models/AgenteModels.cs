using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class AgenteModels : ModelBase
    {
        public AgenteModels(int codAgente, string descAgente, int poder)
        {
            CodAgente = codAgente;
            DescAgente = descAgente;
            CodPoder = poder;
        }

        public int CodAgente { get; set; }
        public string DescAgente { get; set; }
        public string MatriculaAgente { get; set; }
        public int CodPoder { get; set; }
        public AgenteModels() : base()
        {
            
        }

        public AgenteModels(int codAgente, string descAgente, string matriculaAgente)
        {
            CodAgente = codAgente;
            DescAgente = descAgente;
            MatriculaAgente = matriculaAgente;
        }

        public AgenteModels(int codAgente, string descAgente)
        {
            CodAgente = codAgente;
            DescAgente = descAgente;
        }
    }

    public class AgenteList : List<AgenteModels>
    {

    }
}