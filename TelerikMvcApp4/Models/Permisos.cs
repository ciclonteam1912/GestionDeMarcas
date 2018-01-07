using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace TelerikMvcApp4.Models
{
    public static class Permisos
    {
        public static void OtorgarPermisos(OracleConnection conn)
        {
            //string consulta = "BEGIN DBMS_SESSION.SET_ROLE('operador identified by xzh1w');BEGIN SYS.GEN_AUD_USR_MAQ;EXCEPTION WHEN OTHERS THEN NULL;END;COMMIT;END;";
            string clave = "LLotykKKk7NO95GyZkvporbIcYckNvyldrDJJbS1v+g=";

            //clave = !string.IsNullOrEmpty(clave) ? Desencriptar(clave) : "xzh1w";
            clave = "xzh1w";
            string consulta = "BEGIN DBMS_SESSION.SET_ROLE('operador identified by " + clave + "');COMMIT;END;";
            using (OracleCommand cmd = new OracleCommand(consulta, conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                try
                {
                    cmd.ExecuteScalar();
                    cmd.Dispose();
                }
                catch (OracleException ex)
                {
                    //WriteToEventLog(ex.Message, EventLogEntryType.Error);
                    //WriteToEventLog(consulta, EventLogEntryType.Information);
                }

            }

        }
        public static string Desencriptar(string encryptedString)
        {
            //Variable que almacena la cadena encriptada

            string resultado = encryptedString;

            if (!string.IsNullOrEmpty(encryptedString))
            {
                // Instanciamos la clase que administra el algoritmo de encriptación
                AesManaged decryptor = new AesManaged();
                byte[] encryptedData = null;
                if (!string.IsNullOrEmpty(encryptedString) && !encryptedString.StartsWith("Data") && !encryptedString.StartsWith("USER") && !encryptedString.StartsWith("Provider"))
                {
                    encryptedData = Convert.FromBase64String(encryptedString);
                }
                else
                {
                    encryptedData = null;
                    decryptor.Dispose();
                    return encryptedString;
                }

                // Obtenemos el salto de cadena, en este caso, pasamos el salto directamente. Luego, creamos el objeto byte[]
                string salt = "PLUS+v2.0";
                byte[] saltBytes = new UTF8Encoding().GetBytes(salt);
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(salt, saltBytes);

                decryptor.Key = rfc.GetBytes(16);
                decryptor.IV = rfc.GetBytes(16);
                decryptor.BlockSize = 128;

                // Creamos el objeto Stream en Memoria
                using (MemoryStream decryptionStream = new MemoryStream())
                {
                    // Creamos el objeto CryptoStream
                    using (CryptoStream decrypt = new CryptoStream(decryptionStream, decryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        try
                        {
                            // Procedemos a desencriptar la cadena
                            decrypt.Write(encryptedData, 0, encryptedData.Length);
                            decrypt.Flush();
                            decrypt.Close();
                        }
                        catch { }
                        finally { }

                        // Retornamos los datos encriptados
                        byte[] decryptedData = decryptionStream.ToArray();
                        decryptionStream.Dispose();
                        resultado = UTF8Encoding.UTF8.GetString(decryptedData, 0, decryptedData.Length);
                    }
                }
                decryptor.Dispose();
                rfc.Dispose();
                saltBytes = null;
                encryptedData = null;
            }
            return resultado;
        }
    }
}