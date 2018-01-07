using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class PublicacionModel : ModelBase
    {
        public PublicacionModel()
        {

        }
        public PublicacionModel(int pub, int codExpediente, DateTime fecha, string periodico, int dias,  string observacion, int pubDetCod)
        {
            PubClave = pub;
            CodExpediente = codExpediente;
            Fecha = fecha;
            Periodico = periodico;
            Dias = dias;
            //Publicado = publicado;
            Observacion = observacion;
            PubDetCodigo = pubDetCod;
        }
        public int PubDetCodigo { get; set; }
        public int PubClave { get; set; }
        public int Expediente { get; set; }
        public DateTime Fecha { get; set; }
        public string Periodico { get; set; }
        public int Dias { get; set; }
        public string Publicado { get; set; }
        public byte[] Imagen { get; set; }
        public string Observacion { get; set; }

        #region Modelo para grilla de Historia de Marcas

        public PublicacionModel(int codHistoria, int codExpediente, DateTime fechaExpediente, string tmovDesc, string estadoDesc, 
            string observacion, string institucion)
        {
            CodHistoriaExpediente = codHistoria;
            CodExpediente = codExpediente;
            FechaExpediente = fechaExpediente;
            TmovDesc = tmovDesc;
            EstadoDesc = estadoDesc;
            Observacion = observacion;
            Institucion = institucion;
        }


        public int CodHistoriaExpediente { get; set; }
        public int CodExpediente { get; set; }
        public DateTime FechaExpediente { get; set; }
        public int TipoMovimiento { get; set; }
        public string TmovDesc { get; set; }
        public int Estado { get; set; }
        public string EstadoDesc { get; set; }
        public string ObservacionHist { get; set; }
        public int CodExpRel { get; set; }
        public string Institucion { get; set; }
        public bool isDeletable { get; set; }
        #endregion

    }

    public class ListPublicaciones : List<PublicacionModel>
    {

    }
}