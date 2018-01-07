using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{

    public class MntPoder : ModelBase
    {
        public MntPoder(int poder, string descPoder, string fechaCreacion, string descApoderado)
        {
            
            Poder = poder;
            DescPoder = descPoder;
            FechaCreacion = fechaCreacion;
            DescApoderado = descApoderado;
        }

        public MntPoder(string descPoder, int poder, string fechaCreacion, int codApoderado, string observacion, string firmante)
        {
            DescPoder = descPoder;
            Poder = poder;
            FechaCreacion = fechaCreacion;
            CodApoderado = codApoderado;
            Observacion = observacion;
            Firmante = firmante;
        }
        public MntPoder(int poder, string descPoder)
        {
            Poder = poder;
            DescPoder = descPoder;
        }
        public MntPoder(int poder)
        {
            Poder = poder;
        }
        public MntPoder()
        {

        }

        public MntPoder(int pod, int codAgente)
        {
            Poder = pod;
            CodAgente = codAgente;
        }

        public MntPoder(int codPoder, string descPoder, byte[] scanPoder)
        {
            Poder = codPoder;
            //Poder = nroPoder;
            DescPoder = descPoder;
            ScanPoder = scanPoder;
        }
        public string DescPoder { get; set; }
        public int Poder { get; set; }

        //public DateTime? FechaCreacion { get; set; }
        public string FechaCreacion { get; set; }
        public int CodApoderado { get; set; }
        public string DescApoderado { get; set; }
        public string Observacion { get; set; }
        public string Firmante { get; set; }
        public int CodAgente { get; set; }
        public byte[] ScanPoder { get; set; }
        public string imagen64
        {
            get
            {
                if (ScanPoder != null)
                    return Convert.ToBase64String(ScanPoder);
                return string.Empty;
            }
        }
    }

    public class ListPoder : List<MntPoder>
    {

    }
}
   