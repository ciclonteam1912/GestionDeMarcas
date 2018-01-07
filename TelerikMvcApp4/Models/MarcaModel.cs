using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class MarcaModel : ModelBase
    {
        public MarcaModel(int marcaCodigo, string marcaDescripcion, int marcaClase, int marcaEdniza)
        {
            MarcaCodigo = marcaCodigo;
            MarcaDescripcion = marcaDescripcion.ToUpper();
            MarcaClase = marcaClase;
            MarcaEdniza = marcaEdniza;
        }

        public MarcaModel(int marcaCodigo, string marcaDescripcion, int poder)
        {
            MarcaCodigo = marcaCodigo;
            MarcaDescripcion = marcaDescripcion;
            Poder = poder;
        }

        public MarcaModel(string marcaDescripcion)
        {
            MarcaDescripcion = marcaDescripcion;
        }

        public MarcaModel()
        {

        }
        public MarcaModel(int CodMarca, string DescMarca)
        {
            MarcaCodigo = CodMarca;
            MarcaDescripcion = DescMarca;
        }

        public MarcaModel(string marcaDescripcion, int marcaClase, int marcaEdNiza, int tipo, string tipoObs, 
            string reivindicacion, string reivindicacionObs, string descProd, int prioridad, int pais, int poder, 
            string fecha, string logo)
        {
            MarcaDescripcion = marcaDescripcion;
            MarcaClase = marcaClase;
            MarcaEdniza = marcaEdNiza;
            Tipo = tipo;
            TipoObs = tipoObs;
            Reivindicacion = reivindicacion;
            ReivindicacionObs = reivindicacionObs;
            DescProducto = descProd;
            Prioridad = prioridad;
            PaisCodigo = pais;
            Poder = poder;
            FechaPrioridad = fecha;
            Logotipo = logo;
        }
        
        public MarcaModel(int CodMarca, string DescMarca, byte[] logo)
        {
            MarcaCodigo = CodMarca;
            MarcaDescripcion = DescMarca;
            LogotipoMarca = logo;
        }
        public int MarcaCodigo { get; set; }
        public string MarcaDescripcion { get; set; }
        public int MarcaClase { get; set; }
        public string ClaseDesc { get; set; }
        public int MarcaEdniza { get; set; }
        public string EdNizaDesc { get; set; }
        public int Tipo { get; set; }
        public string TipoDesc { get; set; }
        public string TipoObs { get; set; }
        public string Reivindicacion { get; set; }
        public string ReivindicacionObs { get; set; }
        public string DescProducto { get; set; }
        public int Prioridad { get; set; }
        public int PaisCodigo { get; set; }
        public string PaisDesc { get; set; }
        public string FechaPrioridad { get; set; }
        public int Poder { get; set; }
        public string Logotipo { get; set; }
        public byte[] LogotipoMarca { get; set; }
        public string imagen64
        {
            get
            {
                if (LogotipoMarca != null)
                    return Convert.ToBase64String(LogotipoMarca);
                return string.Empty;
            }
        }

        public string IndicadorLogotipo { get; set; }
    }

    public class ListMarca : List<MarcaModel>
    {

    }
}