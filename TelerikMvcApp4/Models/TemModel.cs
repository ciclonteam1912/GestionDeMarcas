using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class TemModel : ModelBase
    {
        

        public int Tmp { get; set; }

        public TemModel() : base()
        {
            Tmp = 0;
        }
    }
}