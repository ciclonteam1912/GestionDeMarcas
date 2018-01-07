using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class StatusModel : ModelBase
    {
        public StatusModel(int codStatus, string desStatus)
        {
            CodStatus = codStatus;
            DesStatus = desStatus;
        }

        public StatusModel(int expediente, int codStatus, string desStatus)
        {
            Expediente = expediente;
            CodStatus = codStatus;
            DesStatus = desStatus;
        }

        public StatusModel(string desStatus, string check)
        {
            DesStatus = desStatus;
            CheckStatus = check;
        }

        public StatusModel()
        {

        }

        public StatusModel(string checkStatus)
        {
            CheckStatus = checkStatus;
        }

        public int Expediente { get; set; }
        public int CodStatus { get; set; }
        public string  DesStatus { get; set; }
        public string CheckStatus { get; set; }
    }

    public class StatusList : List<StatusModel>
    {

    }
}