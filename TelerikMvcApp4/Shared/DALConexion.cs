using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Shared
{
    public class DALConexion
    {
        public static string CrearConexion(string usuarioCon, string contraseñaCon, string baseCon)
        {
            string cadena = "User Id=" + usuarioCon + ";Password=" + contraseñaCon + ";Data Source=" + baseCon;
            return cadena;
        }
    }
}