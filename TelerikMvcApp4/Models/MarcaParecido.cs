using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class MarcaParecido : ModelBase
    {
        public MarcaParecido(int codMarca, string descMarca)
        {
            CodMarca = codMarca;
            DescMarca = descMarca;
        }

        public MarcaParecido(int parecido, int codMarca, string descMarca, string descTitular, int porcentajeSimilitud, 
            int porcentajeError, int oposicion, int codTitular)
        {
            ParecidoCodigo = parecido;
            CodMarca = codMarca;
            DescMarca = descMarca;
            DescTitular = descTitular;
            PorcentajeSimilitud = porcentajeSimilitud;
            PorcentajeError = porcentajeError;
            IntencionOposicion = oposicion;
            CodTitular = codTitular;
        }

        public MarcaParecido(int parecido, int codMarca, string descMarca, string descTitular, int porcentajeSimilitud,
            int porcentajeError, int oposicion)
        {
            ParecidoCodigo = parecido;
            CodMarca = codMarca;
            DescMarca = descMarca;
            DescTitular = descTitular;
            PorcentajeSimilitud = porcentajeSimilitud;
            PorcentajeError = porcentajeError;
            IntencionOposicion = oposicion;
        }

        public MarcaParecido(int codMarca)
        {
            CodMarca = codMarca;
        }

        public MarcaParecido()
        {

        }

        public MarcaParecido(int codMarca, string descMarca, string descTitular, int nroClase, string edNizaDesc)
        {
            CodMarca = codMarca;
            DescMarca = descMarca;
            DescTitular = descTitular;
            NroClaseEdNiza = nroClase;
            EdNizaDescripcion = edNizaDesc;
        }

        public int ParecidoCodigo { get; set; }
        public int CodMarca { get; set; }
        public string DescMarca { get; set; }
        public int CodTitular { get; set; }
        public string DescTitular { get; set; }
        public int PorcentajeSimilitud { get; set; }
        public int PorcentajeError { get; set; }
        public int EnviarCorreo { get; set; }
        public int IntencionOposicion { get; set; }
        public int NroClaseEdNiza { get; set; }
        public string EdNizaDescripcion { get; set; }
    }

    public class ListMarcaParecido : List<MarcaParecido>
    {

    }
}