using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class InventorModel : ModelBase
    {
        public InventorModel(int idInventor, string descrpcionInventor)
        {
            this.idInventor = idInventor;
            this.descrpcionInventor = descrpcionInventor;
        }

        public int idInventor  { get; set; }
        public string descrpcionInventor { get; set; }
    }

    public class InventorList : List<InventorModel>
    {

    }
}