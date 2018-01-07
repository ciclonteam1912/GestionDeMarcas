using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class ExpedienteModels : ModelBase
    {
        //public ExpedienteModels(int codExpediente)
        //{
        //    CodExpediente = codExpediente;

        //}
        public ExpedienteModels()
        {

        }
        //public ExpedienteModels(int codExpediente, string descExpediente)
        //{
        //    CodExpediente = codExpediente;
        //    DescExpediente = descExpediente;
        //}

        public ExpedienteModels(int codExpediente, int codEstado, int codAgente, string fechaSolicitud,
            int tipoSolicitud, int codSolicitante, int codPais, int codPoder, int codMarca, string descPoder, string fechaVencimiento)
        {
            CodExpediente = codExpediente;
            CodEstado = codEstado;
            CodAgente = codAgente;
            FechaSolicitud = fechaSolicitud;
            //FechaEstado = fechaEstado;
            //TipoMovimiento = tipoMovimiento;
            TipoSolicitud = tipoSolicitud;
            CodSolicitante = codSolicitante;
            CodPais = codPais;
            CodPoder = codPoder;
            CodMarca = codMarca;
            DescripcionPoder = descPoder;
            FechaVencimiento = fechaVencimiento;
        }

        public ExpedienteModels(int codExpediente, string descMarca)
        {
            CodExpediente = codExpediente;
            DescMarca = descMarca;
        }

        public ExpedienteModels(string solicitante, string descPoder, string descEstado, int codEstado,
            string fechaSolicitudActual, string marcaDesc, string agenteDesc, int? registro, string fechaVenc, string fechaSol,
            int codTipoMov, string descTipoMov, string obs, string institucion)
        {
            Solicitante = solicitante;
            DescPoder = descPoder;
            DescEstado = descEstado;
            CodEstado = codEstado;
            FechaSolicitudActual = fechaSolicitudActual;
            DescMarca = marcaDesc;
            DescAgente = agenteDesc;
            CodRegistro = registro;
            FechaVencimiento = fechaVenc;
            FechaSolicitud = fechaSol;
            TipoMovimiento = codTipoMov;
            DescTipoMov = descTipoMov;
            Observacion = obs;
            Institucion = institucion;
        }

        public ExpedienteModels(string descMarca)
        {
            DescMarca = descMarca;
        }

        public int CodExpediente { get; set; }
        public string DescExpediente { get; set; }
        public int CodEstado { get; set; }
        public string DescEstado { get; set; }
        public int CodAgente { get; set; }
        public string DescAgente { get; set; }
        public string FechaSolicitudActual { get; set; }
        public string FechaSolicitud { get; set; }
        public string FechaEstado { get; set; }
        public int TipoMovimiento { get; set; }
        public string DescTipoMov { get; set; }
        public int CodSolicitante { get; set; }
        public string Solicitante { get; set; }
        public int CodPais { get; set; }
        public int CodPoder { get; set; }
        public string DescPoder { get; set; }
        public int CodMarca { get; set; }
        public string DescMarca { get; set; }
        public string DescripcionPoder { get; set; }
        public int TipoSolicitud { get; set; }
        public int? CodRegistro { get; set; }
        public string FechaVencimiento { get; set; }
        public DateTime? FechaVenc { get; set; }
        public string Observacion { get; set; }
        public string Institucion { get; set; }
    }

    public class ExpedienteList : List<ExpedienteModels>
    {

    }
}