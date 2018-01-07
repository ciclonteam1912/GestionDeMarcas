using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcApp4.Shared
{
    public class GestionDataConnect
    {

        public static string BusquedaApoderado()
        {
            string cadena = "SELECT * FROM GMC_APODERADO";
            return cadena;
        }
        public static string INSERTAR_GMC_APODERADO(string titularEmpresa , string rucEmpresa , string direccionEmp, string productoEmpresa , string telefonoEmpresa , int paisEmpresa , string correoEmpresa)
        {
           string cadena = " INSERT INTO GMC_APODERADO(APO_CODIGO,"
            +"APO_RAZON_SOCIAL,"
            +"APO_RUC,"
            +"APO_DIRECCION,"
            +"APO_PRODUCTO_SERVICIO,"
            +"APO_TELEFONO,"
            +"APO_PAIS, "
            +"APO_CORREO) "
            + " VALUES((SELECT NVL(MAX(APO_CODIGO), 0) + 1 FROM GMC_APODERADO),'"
            + titularEmpresa+ "','"
            + rucEmpresa+ "','"
            + direccionEmp+ "','"
            + productoEmpresa + "','"
            + telefonoEmpresa+ "','"
            + paisEmpresa+ "', '"
            + correoEmpresa+ "')";
            return cadena;
        }
        public static string Recuperar_GMC_DATOS_EMPRESA()
        {
            return "a";
        }
        //public static string Isertar_GMC_RESGISTRO_MARCA(string EmpresasList , int expedienteGeneral , string Stastus ,string registroGeneral , string registroInicialGeneral, string registroVencimiento , string registroInicio , string descripcionMarca , string claseGeneral , string edNizaGeneral, string tipoGeneral ,  string registroGeneral2 , string direccionGeneral , string paisGeneral , string poderGeneral , string servicioGeneral , string DenominacionGenera , string agente)
        //{
        //    string sqlString = "INSERT INTO GMC_REGISTRO_MARCA (REG_COD , " +
        //         " REG_COD_EMPRESA , " +
        //         " REG_EXPEDIENTE , " +
        //         " REG_FECHA_EXPEDIENTE, " +
        //         " REG_STATUS, " +
        //         " REG_REGISTRO, " +
        //         " REG_INICIO_FECHA, " +
        //         " REG_VENCIMIENTO, " +
        //         " REG_INICIO_REGISTRO, " +
        //         " REG_MARCA_DESCRPCION, " +
        //         " REG_CLASE, " +
        //         //" REG_ED_NIZA, " +
        //         " REG_TIPO, " +
        //         //" REG_TITULAR,\n" +
        //         " REG_DIRECCION, " +
        //         " REG_PODER, " +
        //         " REG_AGENTE, " +
        //         " REG_PRODUCTO_SERVICIO, " +
        //         //" REG_PRIORIDAD,\n" +
        //         " REG_DENOMINACION)" +
        //         " VALUES((SELECT NVL(MAX(REG_COD), 0) + 1 FROM GMC_REGISTRO_MARCA) , " +int.Parse(EmpresasList.ToString())+","+ int.Parse(expedienteGeneral.ToString())+" , '"+ registroInicialGeneral + "' , "+int.Parse(Stastus.ToString())+" , '"+registroGeneral+"' , '"+registroInicialGeneral+"' , '"+registroVencimiento+"' , '"+registroInicio+"' , '"+descripcionMarca+"' , "+int.Parse(claseGeneral.ToString())+ " ,' " + tipoGeneral + "' , '" +  direccionGeneral+ "' , '" + poderGeneral + "' , '" + agente + "' , '" + servicioGeneral + "' , '" + DenominacionGenera + "')";

        //    return sqlString;
        //}
        public static string Isertar_GMC_RESGISTRO_MARCA(int expedienteGeneral, string Stastus, string registroGeneral, string FechaInicioSol, string registroVencimiento, string registroInicialGeneral,  string tipoGeneral, string marca)
        {
            string sqlString = "INSERT INTO GMC_REGISTRO_MARCA (REG_COD , " +
                 " REG_EXPEDIENTE , " +
                 " REG_STATUS, " +
                 " REG_REGISTRO, " +
                 " REG_INICIO_FECHA, " +
                 " REG_VENCIMIENTO, " +
                 " REG_INICIO_REGISTRO, " +
                 " REG_MARCA, " +
                 //" REG_AGENTE, " +
                 //" REG_PRODUCTO_SERVICIO, " +
                 //" REG_PRIORIDAD,\n" +
                 " REG_TIPO)" +
                 " VALUES((SELECT NVL(MAX(REG_COD), 0) + 1 FROM GMC_REGISTRO_MARCA) , " + int.Parse(expedienteGeneral.ToString()) + " , '" + int.Parse(Stastus.ToString()) + "' , " + registroGeneral + " , '" + FechaInicioSol+ "' ,"+ registroInicialGeneral + "' , '" + registroVencimiento + "' , '" + int.Parse(marca) + "' , '" + tipoGeneral + "')";

            return sqlString;
        }

        public static string Insertar_GMC_DATOS_PUBLICACION(int expendiente, string fecha , int diarioPublicacion , int cantDias)
        {

            string sqlString = "insert into gmc_datos_publicacion(DPU_EXPEDIENTE , DPU_FECHA , DPU_PERIODICO , DPU_DIAS) VALUES("+ expendiente + "' , '" + fecha + "' , '" + diarioPublicacion + "', '" + cantDias +" )";

            return sqlString;

        }

        public static string Select_GMC_DATOS_RENOVACION(int codExp)
        {

            string sqlString = "select * from gmc_registro_marca T\n" +
            "INNER JOIN gmc_status T1 on T.REG_STATUS = T1.STA_COD\n" +
            "INNER JOIN gmc_clase T2 on T.REG_CLASE = T2.CLA_COD\n" +
            "INNER JOIN gmc_ed_niza T3 on T.REG_CLASE = T3.ED_CLASE AND T.REG_ED_NIZA = T3.ED_COD\n" +
            "where T.REG_EXPEDIENTE ="+ codExp;

            return sqlString;
        }
    }
}