using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class SolicitanteModel : ModelBase
    {
        public SolicitanteModel(int idSolicitnate, string descrpcionModel)
        {
            this.IdSolicitnate = idSolicitnate;
            this.DescrpcionModel = descrpcionModel;
        }

        public int IdSolicitnate { get; set; }
        public string DescrpcionModel { get; set; }
    }

    public class    SolicintateList : List<SolicitanteModel>
    {

    }
}