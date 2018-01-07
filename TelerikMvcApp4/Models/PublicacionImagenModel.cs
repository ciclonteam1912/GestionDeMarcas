using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public class PublicacionImagenModel : ModelBase
    {
        public PublicacionImagenModel(int pubExp, int puim_clave, int puim_clave_pub, string perDesc, byte[] imagenPub)
        {
            PubExpediente = pubExp;
            Puim_clave = puim_clave;
            Puim_clave_pub = puim_clave_pub;
            PeriodicoDesc = perDesc;
            ImagenPub = imagenPub;
        }

        public PublicacionImagenModel(byte[] imagenPub)
        {
            ImagenPub = imagenPub;
        }

        public PublicacionImagenModel()
        {

        }
        public int PubExpediente { get; set; }
        public int Puim_clave { get; set; }
        public int Puim_clave_pub { get; set; }
        public string PeriodicoDesc { get; set; }
        public byte[] ImagenPub { get; set; }

        public string imagen64
        {
            get
            {
                if (ImagenPub != null)
                    return Convert.ToBase64String(ImagenPub);
                return string.Empty;
            }
        }
    }

    public class ListIamgenesPub : List<PublicacionImagenModel>
    {

    }
}