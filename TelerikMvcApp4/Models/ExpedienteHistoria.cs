using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class ExpedienteHistoria : ModelBase
    {
        public ExpedienteHistoria(int codExpediente, DateTime fechaExpediente, int estado, string observacion)
        {
            CodExpediente = codExpediente;
            FechaExpediente = fechaExpediente;
            //TipoMovimiento = tipoMovimiento;
            Estado = estado;
            Observacion = observacion;
        }

        public int CodHistoriaExpediente { get; set; }
        public int CodExpediente { get; set; }
        public DateTime FechaExpediente { get; set; }
        public int TipoMovimiento { get; set; }
        public int Estado { get; set; }
        public string Observacion { get; set; }
        public int CodExpRel { get; set; }
    }

    public class ListExpHistoria : List<ExpedienteHistoria>
    {

    }
}