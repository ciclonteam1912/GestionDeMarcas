using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TelerikMvcApp4.Models;

namespace TelerikMvcApp4.Controllers
{
    //[Authorize(Roles="Admin")]
    public class MantenimientoController : Controller
    {
        // GET: Mantenimiento
        #region Vistas
        public async Task<ActionResult> Index()
        {
            return await Task.Run(() =>
            {
                return View();
            });

        }

        public async Task<ActionResult> MantenimientoDiseño()
        {
            return await Task.Run(() =>
            {
                return View();
            });

        }

        public async Task<ActionResult> MantenimientoPoderes()
        {
            return await Task.Run(() =>
            {
                return View();
            });
        }

        public async Task<ActionResult> EstadoMarca()
        {
            return await Task.Run(() =>
            {
                return View();
            });
        }
        [Authorize]
        public async Task<ActionResult> MantenimientoGeneral()
        {
           
            var listPoderes = new Models.ListPoder();
            return await Task.Run(() =>
            {
                string sqlString = "select p.*, a. apo_razon_social from gmc_poder p, gmc_apoderado a where p.pod_apoderado = a.apo_codigo";

                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);


                #region Conexion 
                OracleCommand cmd2 = new OracleCommand(sqlString, con);
                #endregion
                try
                {
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    using (OracleDataReader dr = cmd2.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listPoderes.Add(new Models.MntPoder(
                            int.Parse(dr["POD_CODIGO"].ToString()),
                            dr["POD_DESCRIPCION"].ToString(),
                            //dr["POD_FECHA_CREACION"] != DBNull.Value ? DateTime.Parse(dr["POD_FECHA_CREACION"].ToString()) : (DateTime?)null));
                            dr["POD_FECHA_CREACION"].ToString(),
                            dr["APO_RAZON_SOCIAL"].ToString()));
                    }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (con.State != System.Data.ConnectionState.Closed)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                return View(listPoderes);
            });
        }


        #endregion

        #region Metodos Interactuan con las Vistas

        public JsonResult BuscarMaxCodApoderado()
        {
            string sqlString = "SELECT NVL(MAX(APO_CODIGO), 0) + 1 MAX_COD_APODERADO FROM GMC_APODERADO";

            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);

            OracleCommand cmd = new OracleCommand(sqlString, con);

            int max = 0;
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        max = int.Parse(dr["MAX_COD_APODERADO"].ToString());

                    }
                }
            }
            catch (Exception ex) { Session["ExRegistroEmpresa"] = ex; }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();

                }
            }
            return Json(max);

        }
        public JsonResult BuscarMaxCodAgente()
        {
            string sqlString = "SELECT NVL(MAX(AGE_COD), 0) + 1 MAX_COD_AGENTE FROM GMC_AGENTE";

            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, con);

            int max = 0;
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        max = int.Parse(dr["MAX_COD_AGENTE"].ToString());

                    }
                }
            }
            catch (Exception ex) { Session["ExRegistroEmpresa"] = ex; }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();

                }
            }
            return Json(max);
        }

        public JsonResult BuscarMaxCodPeriodico()
        {
            string sqlString = "SELECT NVL(MAX(PER_CODIGO), 0) + 1 MAX_COD_PERIODICO FROM GMC_PERIODICO";

            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, con);

            int max = 0;
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        max = int.Parse(dr["MAX_COD_PERIODICO"].ToString());

                    }
                }
            }
            catch (Exception ex) { Session["ExRegistroEmpresa"] = ex; }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();

                }
            }
            return Json(max);
        }
        public JsonResult BuscarMaxCodMarca()
        {
            string sqlString = "SELECT NVL(MAX(MARC_CODIGO), 0) + 1 MAX_COD_MARCA FROM GMC_MARCA";

            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, con);

            int max = 0;
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        max = int.Parse(dr["MAX_COD_MARCA"].ToString());

                    }
                }
            }
            catch (Exception ex) { Session["ExRegistroEmpresa"] = ex; }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();

                }
            }
            return Json(max);
        }
        public JsonResult BuscarMaxCodTipoMarca()
        {
            string sqlString = "SELECT NVL(MAX(TIPO_MARCA_COD), 0) + 1 MAX_COD_TIPO_MARCA FROM GMC_TIPO_MARCA";

            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, con);

            int max = 0;
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        max = int.Parse(dr["MAX_COD_TIPO_MARCA"].ToString());

                    }
                }
            }
            catch (Exception ex) { Session["ExRegistroEmpresa"] = ex; }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();

                }
            }
            return Json(max);
        }
        public JsonResult BuscarMaxCodClase()
        {
            string sqlString = "SELECT NVL(MAX(TICL_COD), 0) + 1 MAX_COD_CLASE FROM GMC_TIPO_CLASE";

            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, con);

            int max = 0;
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        max = int.Parse(dr["MAX_COD_CLASE"].ToString());

                    }
                }
            }
            catch (Exception ex) { Session["ExRegistroEmpresa"] = ex; }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();

                }
            }
            return Json(max);
        }
        public JsonResult BuscarMaxCodEdNiza()
        {
            string sqlString = "SELECT NVL(MAX(NIZ_COD), 0) + 1 MAX_COD_EDNIZA FROM GMC_EDNIZA";

            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, con);

            int max = 0;
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        max = int.Parse(dr["MAX_COD_EDNIZA"].ToString());

                    }
                }
            }
            catch (Exception ex) { Session["ExRegistroEmpresa"] = ex; }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();

                }
            }
            return Json(max);
        }

        public JsonResult BuscarMaxCodPais()
        {
            string sqlString = "select NVL(MAX(pais_codigo), 0) + 1 MAX_COD_PAIS from gen_pais where pais_codigo <> 99";

            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, con);

            int max = 0;
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        max = int.Parse(dr["MAX_COD_PAIS"].ToString());

                    }
                }
            }
            catch (Exception ex) { Session["ExRegistroEmpresa"] = ex; }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();

                }
            }
            return Json(max);
        }
        public JsonResult BuscarMaxCodTipoMov()
        {
            string sqlString = "select NVL(MAX(tmov_codigo), 0) + 1 MAX_COD_TMOV from gmc_tipo_movimiento";

            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, con);

            int max = 0;
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        max = int.Parse(dr["MAX_COD_TMOV"].ToString());

                    }
                }
            }
            catch (Exception ex) { Session["ExRegistroEmpresa"] = ex; }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();

                }
            }
            return Json(max);
        }
        public JsonResult BuscarMaxCodTipoSol()
        {
            string sqlString = "select NVL(MAX(tsol_codigo), 0) + 1 MAX_COD_TSOL from gmc_tipo_solicitud";

            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, con);

            int max = 0;
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        max = int.Parse(dr["MAX_COD_TSOL"].ToString());

                    }
                }
            }
            catch (Exception ex) { Session["ExRegistroEmpresa"] = ex; }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();

                }
            }
            return Json(max);
        }
        public JsonResult BuscarMaxCodEstado()
        {
            string sqlString = "SELECT NVL(MAX(EST_CODIGO), 0) + 1 MAX_COD_ESTADO FROM GMC_ESTADO";

            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, con);

            int max = 0;
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        max = int.Parse(dr["MAX_COD_ESTADO"].ToString());

                    }
                }
            }
            catch (Exception ex) { Session["ExRegistroEmpresa"] = ex; }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();

                }
            }
            return Json(max);
        }
        public ActionResult GetConfiguracion(FormCollection frm)
        {
            bool existe = false;
            string porcentajeParecido = Request.Form["porcentajeParecido"];
            string diasEspera = Request.Form["diasEspera"];

            string sqlStringSelect = "select * from gmc_configuracion";
            string sqString = "insert into gmc_configuracion (conf_porcentaje, conf_dias_espera) values(:porcentaje, :dias)";
            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqString, con);
            cmd.Parameters.Add(new OracleParameter("porcentaje", int.Parse(porcentajeParecido)));
            cmd.Parameters.Add(new OracleParameter("dias", int.Parse(diasEspera)));

            OracleCommand cmd2 = new OracleCommand(sqlStringSelect, con);
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd2.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        existe = true;
                        string sqlStringUpdate = "update gmc_configuracion set conf_porcentaje = :updatePorc,"
                        + " conf_dias_espera = :updateDiasEspera";
                        OracleCommand cmdUpdate = new OracleCommand(sqlStringUpdate, con);
                        cmdUpdate.Parameters.Add(new OracleParameter("updatePorc", int.Parse(porcentajeParecido)));
                        cmdUpdate.Parameters.Add(new OracleParameter("updateDiasEspera", int.Parse(diasEspera)));
                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                if (!existe)
                {
                    cmd.ExecuteNonQuery();
                }
            }catch(Exception ex)
            {
                Session["Configuracion"] = ex;
            }
            finally
            {
                con.Close();
                cmd.Dispose();
            }
            return RedirectToAction("MantenimientoGeneral");
        }   
            

        [HttpPost]
        public ActionResult GetRegistroApoderado(FormCollection frm)
        {
            
            bool existe = false;
            string tipoPersona = Request.Form["comboPersona"];
            string codigoApoderado = Request.Form["codigoApoderado"];
            string nombreEmpresa = Request.Form["nombreEmpresa"];
            string rucEmpresa = Request.Form["rucEmpresa"];
            string direccionEmpresa = Request.Form["direccionEmpresa"];
            //string productoEmpresa = Request.Form["rubroEmpresa"];
            string telefonoEmpresa = Request.Form["telefonoEmpresa"];
            string paisEmpresa = Request.Form["pais2"];
            string correoEmpresa = Request.Form["correoEmpresa"];
            //string titulares = Request.Form["elTitular"];

            
            
            string sqlString = "select * from GMC_APODERADO A, GEN_PAIS P where A.APO_PAIS = P.PAIS_CODIGO AND A.APO_CODIGO = " + codigoApoderado;

            string sqlInsertTitulares = "";
            //string queryString = Shared.GestionDataConnect.INSERTAR_GMC_APODERADO(nombreEmpresa, rucEmpresa, direccionEmpresa, productoEmpresa, telefonoEmpresa, int.Parse(paisEmpresa), correoEmpresa);
            string queryString = " INSERT INTO GMC_APODERADO(APO_CODIGO,"
            + "APO_RAZON_SOCIAL,"
            + "APO_RUC,"
            + "APO_DIRECCION,"
            //+ "APO_PRODUCTO_SERVICIO,"
            + "APO_TELEFONO,"
            + "APO_PAIS, "
            + "APO_CORREO, "
            //+ "APO_TITULARES, "
            + "APO_TIPO_PERSONA) "
            + " VALUES((SELECT NVL(MAX(APO_CODIGO), 0) + 1 FROM GMC_APODERADO),\n "
            + ":razonSocial, \n "
            + ":ruc, \n "
            + ":direccion, \n"
            //+ ":producto, \n"
            + ":telefono, \n"
            + ":pais, \n"
            + ":correo, \n"
            //+ ":titulares, \n"
            + ":tipoPersona)";
            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);

            #region Conexion 
            OracleCommand cmd = new OracleCommand(queryString, con);
            cmd.Parameters.Add(new OracleParameter("razonSocial", nombreEmpresa.ToUpper()));
            cmd.Parameters.Add(new OracleParameter("ruc", rucEmpresa));
            cmd.Parameters.Add(new OracleParameter("direccion", direccionEmpresa));
            //cmd.Parameters.Add(new OracleParameter("producto", productoEmpresa));
            cmd.Parameters.Add(new OracleParameter("telefono", telefonoEmpresa));
            cmd.Parameters.Add(new OracleParameter("pais", int.Parse(paisEmpresa)));
            cmd.Parameters.Add(new OracleParameter("correo", correoEmpresa));
            //cmd.Parameters.Add(new OracleParameter("titulares", titulares));
            cmd.Parameters.Add(new OracleParameter("tipoPersona", tipoPersona));

            OracleCommand cmd2 = new OracleCommand(sqlString, con);
            #endregion
            
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd2.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        existe = true;
                        string sqlStringUpdate = "update gmc_apoderado set APO_RAZON_SOCIAL = :updateRazon,"
                        + " APO_RUC = :updateRuc,"
                        + " APO_DIRECCION = :updateDireccion,"
                        //+ "APO_PRODUCTO_SERVICIO = :updateProducto,"
                        + " APO_TELEFONO = :updateTelefono,"
                        + " APO_PAIS = :updatePais, "
                        + " APO_CORREO = :updateCorreo, "
                        //+ "APO_TITULARES = :updateTitulares"
                        + " APO_TIPO_PERSONA = :updateTipoPers"
                        + " where apo_codigo = " + codigoApoderado;
                        OracleCommand cmdUpdate = new OracleCommand(sqlStringUpdate, con);
                        cmdUpdate.Parameters.Add(new OracleParameter("updateRazon", nombreEmpresa.ToUpper()));
                        cmdUpdate.Parameters.Add(new OracleParameter("updateRuc", rucEmpresa));
                        cmdUpdate.Parameters.Add(new OracleParameter("updateDireccion", direccionEmpresa));
                        //cmdUpdate.Parameters.Add(new OracleParameter("updateProducto", productoEmpresa));
                        cmdUpdate.Parameters.Add(new OracleParameter("updateTelefono", telefonoEmpresa));
                        cmdUpdate.Parameters.Add(new OracleParameter("updatePais", int.Parse(paisEmpresa)));
                        cmdUpdate.Parameters.Add(new OracleParameter("updateCorreo", correoEmpresa));
                        //cmdUpdate.Parameters.Add(new OracleParameter("updateTitulares", titulares));
                        cmdUpdate.Parameters.Add(new OracleParameter("updateTipoPers", tipoPersona));
                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                if (!existe)
                {
                    cmd.ExecuteNonQuery();
                    //if (int.Parse(tipoPersona) == 2)
                    //{
                    //    var etiq = titulares.Split(',');
                    //    for (int i = 0; i < etiq.Length; i++)
                    //    {
                    //        sqlInsertTitulares = "insert into GMC_APODERADO_TITULAR(APTI_EMPRESA, APTI_TITULAR) VALUES(:codEmpresa, :codTitular)";
                    //        OracleCommand cmdInsertTitulares = new OracleCommand(sqlInsertTitulares, con);
                    //        cmdInsertTitulares.Parameters.Add(new OracleParameter("codEmpresa", codigoApoderado));
                    //        cmdInsertTitulares.Parameters.Add(new OracleParameter("codTitular", int.Parse(etiq[i])));
                    //        cmdInsertTitulares.ExecuteNonQuery();
                    //    }
                    //}
                    
                }
                
            }
            catch (Exception ex) { Session["ExRegistroEmpresa"] = ex; }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();
                    cmd2.Dispose();

                }
            }
            return RedirectToAction("MantenimientoGeneral");

        }
        public async Task<ActionResult> GetRegistroPoder(FormCollection frm, IEnumerable<HttpPostedFileBase> scanPoder)
        {
            int docEmpresa = int.Parse(Request.Form["EmprLst"].ToString());
            int poder = int.Parse(Request.Form["poderEmpresa"].ToString());
            string poderDesc = Request.Form["poderDescripcion"];
            string fecha = Request.Form["FechaGen"];
            string observacion = Request.Form["poderObservacion"];
            string firmante = Request.Form["firmantePoder"];
            bool existe = false;
            byte[] bytes = null;

            if (scanPoder != null)
            {
                foreach (var item in scanPoder)
                {
                    if (item != null)
                    {
                        using (Stream inputStream = item.InputStream)
                        {
                            MemoryStream memoryStream = inputStream as MemoryStream;
                            if (memoryStream == null)
                            {
                                memoryStream = new MemoryStream();
                                inputStream.CopyTo(memoryStream);
                            }
                            bytes = memoryStream.ToArray();
                        }
                    }
                }
            }
            //Los firmantes mediantes un Split crea un array de String, luego cada elemento del array se añade a una lista de enteros.
            //Para su posterior uso con linQ.
            var agentes = firmante.Split(',');
            int nuevosFirmantes = agentes.Length;
            List<int> val = new List<int>();
            foreach (var item in agentes)
                val.Add(int.Parse(item));

            MntPoder pod = new MntPoder();
            var listPoderes = new List<MntPoder>();
            //return await Task.Run(() =>
            //{
            string sqlString = "select * from gmc_poder where pod_codigo = " + poder;

                string sqlInsertAgentes = "";
                string queryRoles = "insert into gmc_poder(POD_CODIGO,\n" +
                "POD_DESCRIPCION,\n" +
                "POD_FECHA_CREACION, \n" +
                "POD_APODERADO, \n" +
                "POD_OBS,\n" +
                "POD_FIRMANTE,\n" +
                "POD_ESCANEO)\n" +
                "values(:PodCodigo ,\n" +
                " :PodDesc ,\n" +
                " :PodFecCreacion ,\n" +
                " :PodApoderado, \n" +
                " :PodObservacion, \n" +
                " :PodFirmante,\n" +
                " :PodEscaneo)";

                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                //Obtener la cantidad de firmantes o agentes de la base de datos.
                string sqlStringSelect = "select count(*) from gmc_poder_agente where podage_poder = " + poder;
                OracleCommand cmdAgentePoder = new OracleCommand(sqlStringSelect, con);

                string select = "select * from gmc_poder_agente where podage_poder = " +poder;
                OracleCommand cmdSelect = new OracleCommand(select ,con);


                OracleCommand cmd = new OracleCommand(queryRoles, con);
                cmd.Parameters.Add(new OracleParameter("PodCodigo", poder));
                cmd.Parameters.Add(new OracleParameter("PodDesc", poderDesc));
                cmd.Parameters.Add(new OracleParameter("PodFecCreacion", fecha));
                cmd.Parameters.Add(new OracleParameter("PodApoderado", docEmpresa));
                cmd.Parameters.Add(new OracleParameter("PodObservacion", observacion));
                cmd.Parameters.Add(new OracleParameter("PodFirmante", firmante));
                cmd.Parameters.Add(new OracleParameter("PodEscaneo", OracleDbType.Blob, bytes, System.Data.ParameterDirection.Input));

                OracleCommand cmd2 = new OracleCommand(sqlString, con);



                #region Procedimiento
                try
                {
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    cmdAgentePoder.ExecuteScalar();
                    
                    using (OracleDataReader dr = cmd2.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            existe = true;
                        StringBuilder sqlStringUpdate = new StringBuilder();
                        sqlStringUpdate.AppendLine("update gmc_poder set pod_descripcion = :updatePoderDescripcion, ");
                        sqlStringUpdate.AppendLine(" pod_fecha_creacion = :updateFechaCreacion, ");
                        sqlStringUpdate.AppendLine(" pod_apoderado = :updateApoderado, ");
                        sqlStringUpdate.AppendLine(" pod_obs = :updateObs, ");
                        sqlStringUpdate.AppendLine(" pod_firmante = :updateFirmante \n");
                        if(bytes != null)
                        {
                            sqlStringUpdate.AppendLine(" ,pod_escaneo = :updateEscaneo");
                        }                           
                        sqlStringUpdate.AppendLine(" where pod_codigo = " + poder);

                            OracleCommand cmdUpdate = new OracleCommand(sqlStringUpdate.ToString(), con);
                            cmdUpdate.Parameters.Add(new OracleParameter("updatePoderDescripcion", poderDesc.ToUpper()));
                            cmdUpdate.Parameters.Add(new OracleParameter("updateFechaCreacion", fecha));
                            cmdUpdate.Parameters.Add(new OracleParameter("updateApoderado", docEmpresa));
                            cmdUpdate.Parameters.Add(new OracleParameter("updateObs", observacion));
                            cmdUpdate.Parameters.Add(new OracleParameter("updateFirmante", firmante));
                            if(bytes != null)
                            {
                                cmdUpdate.Parameters.Add(new OracleParameter("updateEscaneo", OracleDbType.Blob, bytes, System.Data.ParameterDirection.Input));
                            }                           
                            cmdUpdate.ExecuteNonQuery();

                        }
                    }
                    if (!existe) {
                        cmd.ExecuteNonQuery();
                    }
                    using (OracleDataReader dr2 = cmdSelect.ExecuteReader())
                    {
                        while (dr2.Read())
                        {
                            listPoderes.Add(new MntPoder(
                                pod.Poder = int.Parse(dr2["PODAGE_PODER"].ToString()),
                                pod.CodAgente = int.Parse(dr2["PODAGE_AGENTE"].ToString())
                                ));
                        }

                        //si existen mas detalles en la operacion actual, insertamos el resto en la base de datos
                        int firmantesActuales = Convert.ToInt32(cmdAgentePoder.ExecuteScalar());
                        if (nuevosFirmantes > firmantesActuales)
                        {
                            var query =
                                       from c in val
                                       where !(from o in listPoderes
                                               select o.CodAgente)
                                                                   .Contains(c)
                                       select c;

                            foreach (var i in query)
                            {
                                //for (int i = 0; i < agentes.Length; i++)
                                //{
                                sqlInsertAgentes = "insert into GMC_PODER_AGENTE(PODAGE_PODER, PODAGE_AGENTE) VALUES(:codPoder, :codAgente)";
                                OracleCommand cmdInsertAgentes = new OracleCommand(sqlInsertAgentes, con);
                                cmdInsertAgentes.Parameters.Add(new OracleParameter("codPoder", poder));
                                cmdInsertAgentes.Parameters.Add(new OracleParameter("codAgente", i));
                                cmdInsertAgentes.ExecuteNonQuery();
                                //}
                            }
                        }
                        else
                        {
                            if (nuevosFirmantes < firmantesActuales)
                            {
                                var query =
                                       from c in listPoderes
                                       where !(from o in val
                                               select o)
                                                                   .Contains(c.CodAgente)
                                       select c;

                                foreach (var i in query)
                                {
                                    string sqlDeleteAgentes = "delete from GMC_PODER_AGENTE where podage_poder = :poderAgenteCod\n" +
                                    " and podage_agente = "+ i.CodAgente;
                                    OracleCommand cmdInsertAgentes = new OracleCommand(sqlDeleteAgentes, con);
                                    cmdInsertAgentes.Parameters.Add(new OracleParameter("poderAgenteCod", poder));
                                    cmdInsertAgentes.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                
                                
                                
                                string sqlDeleteAgente = "delete from gmc_poder_agente where podage_poder = " +poder;
                                OracleCommand cmdDeleteAgente = new OracleCommand(sqlDeleteAgente, con);
                                cmdDeleteAgente.ExecuteNonQuery();

                                foreach (var elemento in val)
                                {
                                    sqlInsertAgentes = "insert into GMC_PODER_AGENTE(PODAGE_PODER, PODAGE_AGENTE) VALUES(:codPoder, :codAgente)";
                                    OracleCommand cmdInsertAgentes = new OracleCommand(sqlInsertAgentes, con);
                                    cmdInsertAgentes.Parameters.Add(new OracleParameter("codPoder", poder));
                                    cmdInsertAgentes.Parameters.Add(new OracleParameter("codAgente", elemento));
                                    cmdInsertAgentes.ExecuteNonQuery();
                                }
                                
                            }

                    }

                }
            }
                catch (Exception ex)
                {
                    Session["warning"] = ex;

                }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                #endregion
                return RedirectToAction("MantenimientoGeneral");
            //});
        }
        public async Task<ActionResult> GetMantenimienoGeneral(FormCollection frm)
        {
            #region Variables del Form
            string expendGeneralMantenimiento = Request.Form["expendGeneralMantenimiento"];
            string FechaIncioGeneralMant = Request.Form["fechaIncioGeneralMant"];
            string statusGeneralMant = Request.Form["statusGeneralMant"];
            string registroGeneralMant = Request.Form["registroGeneralMant"];
            string registroInicioMant = Request.Form["registroInicioMant"];
            string titutloMant = Request.Form["titutloMant"];
            string ClaseGeneralMant = Request.Form["ClaseGeneralMant"];
            string RegistroInicioMant = Request.Form["registroInicioMant"];
            string PaisesMant = Request.Form["paisesMant"];
            string RegistroVencimientoGeneralMant = Request.Form["registroVencimientoGeneralMant"];
            string TitutloMant = Request.Form["titutloMant"];
            string EdNizaMant = Request.Form["edNizaMant"];
            string RegistroRadioMant = Request.Form["registroRadioMant"];
            string RenovacionMant = Request.Form["renovacionMant"];
            string segundaRenovacionMant = Request.Form["segundaRenovacionMant"];
            string PoderMant = Request.Form["poderMant"];
            string AgenteMant = Request.Form["agenteMant"];
            string numeroSolicitudMant = Request.Form["numeroSolicitudMant"];
            string FechaSolicitudMant = Request.Form["fechaSolicitudMant"];
            string renovacionExpMant = Request.Form["renovacionExpMant"];
            #endregion

            return await Task.Run(() =>
            {
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand("", con);
                cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("RucEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("DireccionEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("ProductoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("TelefonoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("PaisEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("CorreoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("TelefonoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("PaisEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("CorreoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("TelefonoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("PaisEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("CorreoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("CorreoEmpresa", ));
                #endregion
                try
                {
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { Session["manetinimientoEx"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                return View();

            });
        }

        public async Task<ActionResult> GetTitularesMant(FormCollection frm)
        {
            string ExptList22 = Request.Form["ExptList22"];
            string StatusQ = Request.Form["StatusQ"];
            string TipoSlt2 = Request.Form["TipoSlt2"];
            string registroInicioCreadoresMant = Request.Form["registroInicioCreadoresMant"];
            string registroCreadoresMant = Request.Form["registroCreadoresMant"];
            string registroVencimientoCreadoresMant = Request.Form["registroVencimientoCreadoresMant"];
            string ReginiCreadores = Request.Form["ReginiCreadores"];
            string titularCreadoresMnt = Request.Form["titularCreadoresMnt"];



            string sqlString = "insert into gmc_mantenmiento_titulo (til_cod,\n" +
            "                                     til_expediente,\n" +
            "                                     til_status,\n" +
            "                                     til_tipo,\n" +
            "                                     til_titular,\n" +
            "                                     til_creador,\n" +
            "                                     til_registro,\n" +
            "                                     til_fecha_inicio,\n" +
            "                                     til_vencimiento,\n" +
            "                                     til_fecha_desde)\n" +
            "values((SELECT NVL(MAX(til_cod), 0) + 1 FROM gmc_mantenmiento_titulo)," + int.Parse(ExptList22) + "," + int.Parse(StatusQ) + "," + "'" + TipoSlt2 + "'," + "'" + titularCreadoresMnt + "'," + "'" + titularCreadoresMnt + "'," + int.Parse(registroCreadoresMant) + ",'" + registroCreadoresMant + "','" + registroVencimientoCreadoresMant + "','" + ReginiCreadores + "'" + ")";



            return await Task.Run(() =>
            {
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);
                //cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("RucEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("DireccionEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("ProductoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("TelefonoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("PaisEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("CorreoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("TelefonoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("PaisEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("CorreoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("TelefonoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("PaisEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("CorreoEmpresa", ));                                                                                                                                 
                //cmd.Parameters.Add(new OracleParameter("CorreoEmpresa", ));
                #endregion
                try
                {
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { Session["manetinimientoEx"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                return View("MantenimientoDiseño");
            });
        }

        public async Task<ActionResult> GetFiguraMant(FormCollection frm)
        {
            string ExptList222 = Request.Form["ExptList222"];
            string fechaCreacionRenovacion2 = Request.Form["fechaCreacionRenovacion2"];
            string StatusQR = Request.Form["StatusQR"];
            string files = Request.Form["files"];
            string fechaCreacion2 = Request.Form["fechaCreacion2"];
            string tpDiseño = Request.Form["tpDiseño"];
            string variacionesMnt = Request.Form["variacionesMnt"];



            string sqlString = "insert into gmc_mantenimiento_figura(fig_cod,\n" +
            "                                     fig_expediente,\n" +
            "                                     fig_fecha_inicio,\n" +
            "                                     fig_status,\n" +
            //"                                     fig_figure,\n" +
            "                                     fig_fecha_creacion,\n" +
            "                                     fig_tipo_disenho,\n" +
            "                                     fig_variaciones)\n" +
            "values((SELECT NVL(MAX(fig_cod), 0) + 1 FROM gmc_mantenimiento_figura)," + int.Parse(ExptList222) + ",'" + fechaCreacionRenovacion2 + "'," + int.Parse(StatusQR) + ",'" + fechaCreacion2 + "','" + tpDiseño + "','" + variacionesMnt + "'" + ")";


            return await Task.Run(() =>
            {
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);
                //cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("RucEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("DireccionEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("ProductoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("TelefonoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("PaisEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("CorreoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("TelefonoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("PaisEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("CorreoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("TelefonoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("PaisEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("CorreoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("CorreoEmpresa", ));
                #endregion

                try
                {
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { Session["manetinimientoEx"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                return View("MantenimientoDiseño");
            });
        }

        public async Task<ActionResult> GetAgente(FormCollection frm)
        {
            bool existe = false;
            //nombreAgente apellidoAgente
            int codigoAgente = int.Parse(Request.Form["codigoAgente"]);
            string NombreCompleto = Request.Form["nombreAgente"];
            string Matricula = Request.Form["matriculaAgente"];

            string sqlString2 = "select * from gmc_agente where age_cod = " + codigoAgente;
            string sqlString = "insert into gmc_agente(age_cod, age_nombre, age_matricula)values((SELECT NVL(MAX(age_cod), 0) + 1 FROM gmc_agente),'" + NombreCompleto + "','"+Matricula+"')";

            return await Task.Run(() =>
            {
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);

            OracleCommand cmd2 = new OracleCommand(sqlString2, con);

            #endregion
            try
                {
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    using (OracleDataReader dr = cmd2.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            existe = true;
                            string sqlStringUpdate = "update gmc_agente set age_nombre = :nombreAge, age_matricula = :age_mat where age_cod = "+ codigoAgente;
                            OracleCommand cmdUpdate = new OracleCommand(sqlStringUpdate, con);
                            cmdUpdate.Parameters.Add(new OracleParameter("nombreAge", NombreCompleto.ToUpper()));
                            cmdUpdate.Parameters.Add(new OracleParameter("age_mat", Matricula.ToUpper()));
                            cmdUpdate.ExecuteNonQuery();
                        }
                    }
                    if (!existe)
                    {
                        cmd.ExecuteNonQuery();
                    }                   
                }
                catch (Exception ex) { Session["manetinimientoEx"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                return RedirectToAction("MantenimientoGeneral"); ;
            });
        }
        public async Task<ActionResult> GetPeriodico(FormCollection frm)
        {
            
            //nombreAgente apellidoAgente
            bool existe = false;
            int CodigoPeriodico = int.Parse(Request.Form["codigoPeriodico"]);
            string NombrePeriodico = Request.Form["PeriodicoNombre"];

            string sqlString2 = "select * from gmc_periodico where per_codigo = "+ CodigoPeriodico;
            string sqlString = "insert into gmc_periodico(per_codigo, per_descripcion)values((SELECT NVL(MAX(per_codigo), 0) + 1 FROM gmc_periodico),'" + NombrePeriodico + "')";

            return await Task.Run(() =>
            {
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);

            OracleCommand cmd2 = new OracleCommand(sqlString2, con);

                #endregion
                try
                {
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    using (OracleDataReader dr = cmd2.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        existe = true;
                        string sqlStringUpdate = "update gmc_periodico set per_descripcion = :updatePerDesc where per_codigo =" + CodigoPeriodico;

                        OracleCommand cmdUpdate = new OracleCommand(sqlStringUpdate, con);
                        cmdUpdate.Parameters.Add(new OracleParameter("updatePerDesc", NombrePeriodico));
                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                if (!existe)
                {
                    cmd.ExecuteNonQuery();
                }
                    
                }
                catch (Exception ex) { Session["manetinimientoEx"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                return RedirectToAction("MantenimientoGeneral"); ;
            });
        }
        public async Task<ActionResult> GetMarca(FormCollection frm, IEnumerable<HttpPostedFileBase> files)
        {
            //nombreAgente apellidoAgente
            bool existe = false;
            byte[] bytes = null;
            int CodigoMarca = int.Parse(Request.Form["codigoMarca"]);
            string NombreMarca = Request.Form["MarcaNombre"];
            string Clase = Request.Form["clase2"];
            string EdNizaNro = Request.Form["edCla"];
            string TipoMarca = Request.Form["tipoMarca"];
            string TipoMarcaObs = Request.Form["EspecificarTipo"];
            string Reivindicacion = Request.Form["reivindicacion"];
            string ReivindicacionObs = Request.Form["EspecificarReiv"];
            string DescripcionProducto = Request.Form["descripcionProd"];
            int NroPrioridad = 0;
            int.TryParse(Request.Form["nroPrioridad"], out NroPrioridad);
            //int pais = int.Parse(Request.Form["pais"]);
            int pais = 99   ;
            if (!int.TryParse(Request.Form["pais"], out pais))
                pais = 99;
            string FechaPrioridad = Request.Form["FechaPrioridad"];
            int Poder = 0;
            int.TryParse(Request.Form["poderMarca"], out Poder);
            //string Poder = Request.Form["poderMarca"];

            string sqlString = "select * from gmc_marca where marc_codigo = " +CodigoMarca;
            //string sqlString = "insert into gmc_marca(marc_codigo, marc_desc,marc_clase,marc_edniza_nro,marc_tipo,marc_tipo_obs, marc_reivindicacion, marc_reiv_obs, marc_desc_prod,marc_prioridad,marc_pais, marc_fecha, marc_poder)values((SELECT NVL(MAX(marc_codigo), 0) + 1 FROM gmc_marca),'" + NombreMarca + "')";
            string queryRoles = "insert into gmc_marca(MARC_CODIGO,\n" +
                "MARC_DESC,\n" +
                "MARC_CLASE,\n" +
                "MARC_EDNIZA_NRO,\n" +
                "MARC_TIPO,\n" +
                "MARC_TIPO_OBS,\n" +
                "MARC_REIVINDICACION,\n" +
                "MARC_REIV_OBS,\n" +
                "MARC_DESC_PROD,\n" +
                "MARC_PRIORIDAD,\n" +
                "MARC_PAIS,\n" +
                "MARC_FECHA,\n" +
                "MARC_PODER,\n" +
                "MARC_LOGOTIPO)\n" +
                "values((SELECT NVL(MAX(MARC_CODIGO), 0) + 1 FROM gmc_marca),\n" +
                ":MarcaDesc ,\n" +
                " :MarcaClase ,\n" +
                " :EdNizaNro ,\n" +
                " :MarcaTipo,\n" +
                " :TipoObs ,\n" +
                " :Reivindicacion ,\n" +
                " :ReivObs,\n" +
                " :Producto,\n" +
                " :Prioridad,\n" +
                " :Pais,\n" +
                " :Fecha,\n" +
                " :Poder,\n" +
                " :Logotipo)";


            //return await Task.Run(() =>
            //{
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);
                #region Conexion 
                OracleCommand cmd = new OracleCommand(queryRoles, con);

            if (files != null)
            {
                foreach (var item in files)
                {
                    if (item != null)
                    {
                        using (Stream inputStream = item.InputStream)
                        {
                            MemoryStream memoryStream = inputStream as MemoryStream;
                            if (memoryStream == null)
                            {
                                memoryStream = new MemoryStream();
                                inputStream.CopyTo(memoryStream);
                            }
                            bytes = memoryStream.ToArray();
                        }
                    }
                }
            }
            cmd.Parameters.Add(new OracleParameter("MarcaDesc", NombreMarca.ToUpper()));
            cmd.Parameters.Add(new OracleParameter("MarcaClase", int.Parse(Clase)));
            cmd.Parameters.Add(new OracleParameter("EdNizaNro", int.Parse(EdNizaNro)));
            cmd.Parameters.Add(new OracleParameter("MarcaTipo", int.Parse(TipoMarca)));
            cmd.Parameters.Add(new OracleParameter("TipoObs", TipoMarcaObs));
            cmd.Parameters.Add(new OracleParameter("Reivindicacion", Reivindicacion));
            cmd.Parameters.Add(new OracleParameter("ReivObs", ReivindicacionObs));
            cmd.Parameters.Add(new OracleParameter("Producto", DescripcionProducto));
            cmd.Parameters.Add(new OracleParameter("Prioridad", NroPrioridad));
            cmd.Parameters.Add(new OracleParameter("Pais", pais));
            cmd.Parameters.Add(new OracleParameter("Fecha", FechaPrioridad));
            cmd.Parameters.Add(new OracleParameter("Poder", Poder==0 ? (int?)null:Poder));
            cmd.Parameters.Add(new OracleParameter("Logotipo", OracleDbType.Blob, bytes, System.Data.ParameterDirection.Input));

            OracleCommand cmd2 = new OracleCommand(sqlString, con);
            #endregion
            try
                {
                    con.Open();
                Permisos.OtorgarPermisos(con);
                
                using (OracleDataReader dr = cmd2.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        existe = true;
                        StringBuilder sqlStringUpdate = new StringBuilder();
                        sqlStringUpdate.AppendLine("update gmc_marca set marc_desc = :updateMarcDesc, ");
                        sqlStringUpdate.AppendLine("marc_clase = :updateMarcClase, ");
                        sqlStringUpdate.AppendLine("marc_edniza_nro = :updateEdNiza, ");
                        sqlStringUpdate.AppendLine("marc_tipo = :updateTipo, ");
                        sqlStringUpdate.AppendLine("marc_tipo_obs = :updateTipoOBS, ");
                        sqlStringUpdate.AppendLine("marc_reivindicacion = :updateReiv, ");
                        sqlStringUpdate.AppendLine("marc_reiv_obs = :updateReivObs, ");
                        sqlStringUpdate.AppendLine("marc_desc_prod = :updateProd, ");
                        sqlStringUpdate.AppendLine("marc_prioridad = :updatePrioridad, ");
                        sqlStringUpdate.AppendLine("marc_pais = :pais, ");
                        sqlStringUpdate.AppendLine("marc_fecha = :fecha, ");
                        sqlStringUpdate.AppendLine("marc_poder = :poder ");
                        if (bytes!= null)
                            {
                            sqlStringUpdate.AppendLine(",marc_logotipo = :logotipo");
                        }

                        sqlStringUpdate.AppendLine(" where marc_codigo =" + CodigoMarca);

                        OracleCommand cmdUpdate = new OracleCommand(sqlStringUpdate.ToString(), con);
                        cmdUpdate.Parameters.Add(new OracleParameter("updateMarcDesc", NombreMarca.ToUpper()));
                        cmdUpdate.Parameters.Add(new OracleParameter("updateMarcClase", int.Parse(Clase)));
                        cmdUpdate.Parameters.Add(new OracleParameter("updateEdNiza", int.Parse(EdNizaNro)));
                        cmdUpdate.Parameters.Add(new OracleParameter("updateTipo", int.Parse(TipoMarca)));
                        cmdUpdate.Parameters.Add(new OracleParameter("updateTipoOBS", TipoMarcaObs));
                        cmdUpdate.Parameters.Add(new OracleParameter("updateReiv", Reivindicacion));
                        cmdUpdate.Parameters.Add(new OracleParameter("updateReivObs", ReivindicacionObs));
                        cmdUpdate.Parameters.Add(new OracleParameter("updateProd", DescripcionProducto));
                        cmdUpdate.Parameters.Add(new OracleParameter("updatePrioridad", NroPrioridad));
                        cmdUpdate.Parameters.Add(new OracleParameter("pais", pais));
                        cmdUpdate.Parameters.Add(new OracleParameter("fecha", FechaPrioridad));
                        cmdUpdate.Parameters.Add(new OracleParameter("poder", Poder == 0 ? (int?)null : Poder));
                        if (bytes != null)
                            cmdUpdate.Parameters.Add(new OracleParameter("logotipo", OracleDbType.Blob, bytes, System.Data.ParameterDirection.Input));
                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                if (!existe)
                {
                    cmd.ExecuteNonQuery();
                }
                   
            }
                catch (Exception ex) { Session["manetinimientoEx"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                return RedirectToAction("MantenimientoGeneral"); ;
            //});
        }
        public async Task<ActionResult> GetTipoMarca(FormCollection frm)
        {
            bool existe = false;
            int codigoTipoMarca = int.Parse(Request.Form["codigoTipoMarca"]);
            string tipoMarca = Request.Form["tipoMarcaDesc"];
            string logotipo = Request.Form["IndLogotipo"];

            string sqlString2 = "select * from gmc_tipo_marca where tipo_marca_cod = "+ codigoTipoMarca;

            string sqlString = "insert into gmc_tipo_marca(TIPO_MARCA_COD, TIPO_MARCA_DESC, TIPO_MARCA_IND_LOGO)\n"+
                "values((select MAX(NVL(TIPO_MARCA_COD,0)) + 1 from gmc_tipo_marca),\n"+
                ":TipoMarcaDesc,\n" +
                " :IndicadorLogotipo)";
            return await Task.Run(() =>
            {
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);
                #endregion

                OracleCommand cmd = new OracleCommand(sqlString, con);
                cmd.Parameters.Add(new OracleParameter("TipoMarcaDesc", tipoMarca));
                cmd.Parameters.Add(new OracleParameter("IndicadorLogotipo", logotipo));

                OracleCommand cmd2 = new OracleCommand(sqlString2, con);

                try
                {
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    using (OracleDataReader dr = cmd2.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            existe = true;
                            string sqlStringUpdate = "update gmc_tipo_marca set tipo_marca_desc = :updateTipoMarcaDesc,\n" +
                                " tipo_marca_ind_logo = :updateIndLogo where tipo_marca_cod = " + codigoTipoMarca;
                            OracleCommand cmdUpdate = new OracleCommand(sqlStringUpdate, con);
                            cmdUpdate.Parameters.Add(new OracleParameter("updateTipoMarcaDesc", tipoMarca));
                            cmdUpdate.Parameters.Add(new OracleParameter("updateIndLogo", logotipo));
                            cmdUpdate.ExecuteNonQuery();
                        }
                    }
                    if (!existe)
                    {
                        cmd.ExecuteNonQuery();
                    }
                
                }
                catch (Exception ex) { Session["manetinimientoEx"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                return RedirectToAction("MantenimientoGeneral");
            });
        }
        public async Task<ActionResult> GetPais(FormCollection frm)
        {
            bool existe = false;
            int paisCodigo = int.Parse(Request.Form["codigoPais"]);
            string pais = Request.Form["PaisDescripcion"];

            string sqlString2 = "select * from gen_pais where pais_codigo = "+ paisCodigo;
            string sqlString = "insert into gen_pais(PAIS_CODIGO,PAIS_DESC)\n" +
                "values((select NVL(MAX(pais_codigo), 0) + 1 from gen_pais where pais_codigo <> 99),\n" +
                ":paisDesc)";

            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);


            #region Conexion 
            OracleCommand cmd = new OracleCommand(sqlString, con);
            cmd.Parameters.Add(new OracleParameter("paisDesc", pais));

            OracleCommand cmd2 = new OracleCommand(sqlString2, con);
            #endregion
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd2.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        existe = true;
                        string sqlStringUpdate = "update gen_pais set pais_desc = :updatePaisDesc where pais_codigo = " + paisCodigo;
                        OracleCommand cmdUpdate = new OracleCommand(sqlStringUpdate, con);
                        cmdUpdate.Parameters.Add(new OracleParameter("updatePaisDesc", pais));
                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                if (!existe)
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { Session["manetinimientoEx"] = ex; }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return RedirectToAction("MantenimientoGeneral");
        }
        public async Task<ActionResult> GetTipoMov(FormCollection frm)
        {
            bool existe = false;
            int tipoMovCodigo = int.Parse(Request.Form["codigoTipoMov"]);
            string TipoMovDescripcion = Request.Form["TipoMovDescripcion"];

            string sqlString2 = "select * from gmc_tipo_movimiento where tmov_codigo = " + tipoMovCodigo;
            string sqlString = "insert into gmc_tipo_movimiento(TMOV_CODIGO,TMOV_DESC)\n" +
                "values((select NVL(MAX(TMOV_CODIGO), 0) + 1 from gmc_tipo_movimiento),\n" +
                ":tipoMovDesc)";

            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);


            #region Conexion 
            OracleCommand cmd = new OracleCommand(sqlString, con);
            cmd.Parameters.Add(new OracleParameter("tipoMovDesc", TipoMovDescripcion));

            OracleCommand cmd2 = new OracleCommand(sqlString2, con);
            #endregion
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd2.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        existe = true;
                        string sqlStringUpdate = "update gmc_tipo_movimiento set TMOV_DESC = :updateTipoMovDesc where tmov_codigo = " + tipoMovCodigo;
                        OracleCommand cmdUpdate = new OracleCommand(sqlStringUpdate, con);
                        cmdUpdate.Parameters.Add(new OracleParameter("updateTipoMovDesc", TipoMovDescripcion));
                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                if (!existe)
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { Session["manetinimientoEx"] = ex; }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return RedirectToAction("MantenimientoGeneral");
        }

        public async Task<ActionResult> GetTipoSol(FormCollection frm)
        {
            bool existe = false;
            int tipoSolCodigo = int.Parse(Request.Form["codigoTipoSol"]);
            string TipoSolDescripcion = Request.Form["TipoSolDescripcion"];
            string TipoSolAbrev = Request.Form["TipoSolAbrev"];

            string sqlString2 = "select * from gmc_tipo_solicitud where tsol_codigo = " + tipoSolCodigo;
            string sqlString = "insert into gmc_tipo_solicitud(TSOL_CODIGO,TSOL_DESC,TSOL_ABREV)\n" +
                "values((select NVL(MAX(TSOL_CODIGO), 0) + 1 from gmc_tipo_solicitud),\n" +
                ":tipoSolDesc,\n"+
                ":tipoSolAbrev)";

            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);


            #region Conexion 
            OracleCommand cmd = new OracleCommand(sqlString, con);
            cmd.Parameters.Add(new OracleParameter("tipoSolDesc", TipoSolDescripcion));
            cmd.Parameters.Add(new OracleParameter("tipoSolAbrev", tipoSolCodigo));

            OracleCommand cmd2 = new OracleCommand(sqlString2, con);
            #endregion
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd2.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        existe = true;
                        string sqlStringUpdate = "update gmc_tipo_solicitud set TSOL_DESC = :updateTipoSolDesc, TSOL_ABREV = :updateTipoSolAbrev where TSOL_CODIGO = " + tipoSolCodigo;
                        OracleCommand cmdUpdate = new OracleCommand(sqlStringUpdate, con);
                        cmdUpdate.Parameters.Add(new OracleParameter("updateTipoSolDesc", TipoSolDescripcion));
                        cmdUpdate.Parameters.Add(new OracleParameter("updateTipoSolAbrev", TipoSolAbrev));
                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                if (!existe)
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { Session["manetinimientoEx"] = ex; }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return RedirectToAction("MantenimientoGeneral");
        }

        public async Task<ActionResult> GetClaseMnt(FormCollection frm)
        {
            //codigoClase  descrpcionClase
            bool existe = false;
            //int codClase = int.Parse(Request.Form["codigoClase"]);
            int codigoClase = int.Parse(Request.Form["codigoClase"]);
            string descripcionClase = Request.Form["descripcionClase"];


            string sqlString2 = "select * from gmc_tipo_clase where ticl_cod = " + codigoClase;
            //string sqlString = "insert into gmc_clase(cla_cod, CLA_DESCRIPCION,cla_cod_clase)values((SELECT NVL(MAX(age_cod), 0) + 1 FROM gmc_agente),'" + descrpcionClase + "'," + codClase + ")";
            string sqlString = "insert into gmc_tipo_clase(ticl_cod, TICL_DESC)values((SELECT NVL(MAX(ticl_cod), 0) + 1 FROM gmc_tipo_clase),'" + descripcionClase +"')";

            return await Task.Run(() =>
            {
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);

            OracleCommand cmd2 = new OracleCommand(sqlString2, con);

                #endregion
                try
                {
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    using (OracleDataReader dr = cmd2.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        existe = true;
                        string sqlStringUpdate = "update gmc_tipo_clase set TICL_DESC = :updateClaseDesc where ticl_cod = " + codigoClase;

                        OracleCommand cmdUpdate = new OracleCommand(sqlStringUpdate, con);
                        cmdUpdate.Parameters.Add(new OracleParameter("updateClaseDesc", descripcionClase));
                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                if (!existe)
                {
                    cmd.ExecuteNonQuery();
                }
                    
                }
                catch (Exception ex) { Session["manetinimientoEx"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                return RedirectToAction("MantenimientoGeneral");
            });

        }

        //public async Task<ActionResult> GetEdNiza(FormCollection frm)
        //{
        //    //codigoClase  descrpcionClase

        //    int codClase = int.Parse(Request.Form["codigoClase2"]);
        //    string descrpcionClase = Request.Form["descrpcionClase2"];



        //    string sqlString = "insert into GMC_EDNIZA(NIZ_COD, NIZ_DESCRPCION,NIZ_COD_DESC)values((SELECT NVL(MAX(NIZ_COD), 0) + 1 FROM GMC_EDNIZA),'" + descrpcionClase + "'," + codClase + ")";

        //    return await Task.Run(() =>
        //    {
        //        #region HTTP Auto ConnectionString Render
        //        HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
        //        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
        //        #endregion

        //        #region Conexion 
        //        OracleConnection con = new OracleConnection(authTicket.UserData.ToString());
        //        OracleCommand cmd = new OracleCommand(sqlString, con);

        //        #endregion
        //        try
        //        {
        //            con.Open();
        //            cmd.ExecuteNonQuery();
        //        }
        //        catch (Exception ex) { Session["manetinimientoEx"] = ex; }
        //        finally
        //        {
        //            if (con.State == System.Data.ConnectionState.Open)
        //            {
        //                con.Close();
        //                con.Dispose();
        //            }
        //        }
        //        return View("MantenimientoGeneral");
        //    });

        //}

        public async Task<ActionResult> GetEdNiza(FormCollection frm)
        {
            bool existe = false;
            //ClaseList edNizaMnt descripcionEd
            int codigoEdniza = int.Parse(Request.Form["codigoEdNiza"]);
            int claseCod = int.Parse(Request.Form["ClaseList"]);
            int edNiza = int.Parse(Request.Form["edNizaMnt"]);
            string descripcion = Request.Form["descripcionEd"];

            string sqlString2 = "select * from gmc_edniza where niz_cod = "+ codigoEdniza;
            string sqlString = "insert into gmc_edniza(niz_cod,\n" +
            "                        niz_descripcion,\n" +
            "                        niz_tipo,niz_nro_clase)values((SELECT NVL(MAX(niz_cod), 0) + 1 FROM gmc_edniza) ,  '" + descripcion + "'," + claseCod + "," + edNiza + ")";

            return await Task.Run(() =>
            {
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);

                OracleCommand cmd2 = new OracleCommand(sqlString2, con);
                #endregion
                try
                {
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    using (OracleDataReader dr = cmd2.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            existe = true;
                            string sqlStringUpdate = "update gmc_edniza set niz_tipo = :updateNizTipo, niz_nro_clase = :updateNizNroClase, niz_descripcion = :updateNizDesc where niz_cod = "+ codigoEdniza;
                            OracleCommand cmdUpdate = new OracleCommand(sqlStringUpdate, con);
                            cmdUpdate.Parameters.Add(new OracleParameter("updateNizTipo", claseCod));
                            cmdUpdate.Parameters.Add(new OracleParameter("updateNizNroClase", edNiza));
                            cmdUpdate.Parameters.Add(new OracleParameter("updateNizDesc", descripcion));
                            cmdUpdate.ExecuteNonQuery();
                        }
                    }
                    if (!existe)
                    {
                        cmd.ExecuteNonQuery();
                    }                                          
                }
                catch (Exception ex) { Session["manetinimientoEx"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                return RedirectToAction("MantenimientoGeneral");
            });
        }

        public async Task<ActionResult> GetEstado()
        {
            bool existe = false;
            int codigoEstado = int.Parse(Request.Form["codigoEstado"]);
            string estadoNombre = Request.Form["estadoNombre"];
            string checkConcedido = Request.Form["cbox1"];

            string sqlString2 = "select * from gmc_estado where est_codigo = "+ codigoEstado;
            string sqlString = "insert into gmc_estado (est_codigo, est_desc, est_ind_marc_conced)values((SELECT NVL(MAX(est_codigo), 0) + 1 FROM gmc_estado) , '" + estadoNombre + "', '"+ checkConcedido + "')";

            return await Task.Run(() =>
            {
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);
                OracleCommand cmd2 = new OracleCommand(sqlString2, con);
                #endregion

                try
                {
                    con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd2.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            existe = true;
                        string sqlStringUpdate = "update gmc_estado set est_desc = :estadoDesc , est_ind_marc_conced = :checkEstado where est_codigo = " + codigoEstado;
                        OracleCommand cmdUpdate = new OracleCommand(sqlStringUpdate, con);
                        cmdUpdate.Parameters.Add(new OracleParameter("estadoDesc", estadoNombre));
                        cmdUpdate.Parameters.Add(new OracleParameter("checkEstado", checkConcedido));
                        cmdUpdate.ExecuteNonQuery();
                        }
                    }
                if (!existe)
                {
                    cmd.ExecuteNonQuery();
                }
                    
                }
                catch (Exception ex) { Session["manetinimientoEx"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
            return RedirectToAction("MantenimientoGeneral");
            });
        }

        public async Task<JsonResult> GetAgenteEdit()
        {
            return await Task.Run(() =>
            {
                var list = new Models.AgenteList();

                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand("select * from gmc_agente", con);
                //cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));

                #endregion

                #region Procedimiento
                try
                {
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    OracleDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        list.Add(new Models.AgenteModels(int.Parse(dr["AGE_COD"].ToString()), dr["AGE_NOMBRE"].ToString()));
                    }
                }
                catch (Exception) { }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                #endregion
                return Json(list);
            });
        }

        public async Task<ActionResult> GetSolicitante()
        {
            string solicitanteNombre = Request.Form["solicitanteNombre"];
            return await Task.Run(() =>
            {
                string sqlString = "insert into  gmc_solicitante(sol_cod, sol_descripcion)values((SELECT NVL(MAX(sol_cod), 0) + 1 FROM gmc_solicitante),'" + solicitanteNombre + "'" + ")";
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);
                #endregion
                try
                {
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { Session["manetinimientoEx"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                return View("MantenimientoGeneral");
            });

        }

        public async Task<ActionResult> GetInventor()
        {
            return await Task.Run(() =>
            {

                string sqlString = "insert into gmc_inventor(inv_cod, inv_descripcion)values((SELECT NVL(MAX(inv_cod), 0) + 1 FROM gmc_inventor) , '" + Request.Form["InventorNombre"] + "'" + ")";

                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);
                #endregion
                try
                {
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { Session["manetinimientoEx"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                return View("MantenimientoGeneral");
            });
        }

        public async Task<ActionResult> GetSoicitante()
        {
            return await Task.Run(() => {
                var list = new Models.SolicintateList();
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand("select * from gmc_solicitante", con);
                //cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));

                #endregion

                #region Procedimiento
                try
                {
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    OracleDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        list.Add(new Models.SolicitanteModel(int.Parse(dr["SOL_COD"].ToString()), dr["SOL_DESCRIPCION"].ToString()));
                    }
                }
                catch (Exception) { }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                #endregion
                return Json(list, JsonRequestBehavior.AllowGet);
            });
        }

        public async Task<ActionResult> GetInvension()
        {
            return await Task.Run(() => {
                var list = new Models.InventorList();
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand("select * from gmc_inventor", con);
                //cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));

                #endregion

                #region Procedimiento
                try
                {
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    OracleDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        list.Add(new Models.InventorModel(int.Parse(dr["INV_COD"].ToString()), dr["INV_DESCRIPCION"].ToString()));
                    }
                }
                catch (Exception) { }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                #endregion
                return Json(list, JsonRequestBehavior.AllowGet);
            });
        }

        
        public async Task<JsonResult> EditarAgenda(string value, string valueString, string matricula)
        {

            //string sqlString = "UPDATE gmc_agente " +
            //"SET AGE_NOMBRE = '"+ valueString + "' " +
            //"WHERE AGE_COD ="+value;


            string sqlString2 = "UPDATE gmc_agente\n" +
            "SET AGE_NOMBRE = '" + valueString + "',\n" +
            "AGE_MATRICULA ='" + matricula + "'" +
            "WHERE AGE_NOMBRE = '" + value + "'";


            return await Task.Run(() =>
            {
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString2, con);
                //cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));

                #endregion

                #region Procedimiento
                try
                {
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { Session["SessionEditarAgenda"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                #endregion
                return Json("");
            });
        }

        public async Task<JsonResult> EditarPeriodicoMnt(string value2, string valueString2)
        {
            string sqlString = "UPDATE GMC_PERIODICO\n" +
            "SET PER_DESCRIPCION = '" + valueString2 + "'\n" +
            "WHERE PER_DESCRIPCION = '" + value2 + "'";

            return await Task.Run(() =>
            {
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);
                //cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));

                #endregion

                #region Procedimiento
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { Session["SessionEditarAgenda"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                #endregion

                return Json(View());
            });
        }
        public async Task<JsonResult> EditarMarcaMnt(string value2, string valueString2)
        {
            string sqlString = "UPDATE GMC_MARCA\n" +
            "SET MARC_DESC = '" + valueString2 + "'\n" +
            "WHERE MARC_DESC = '" + value2 + "'";

            return await Task.Run(() =>
            {
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);
                //cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));

                #endregion

                #region Procedimiento
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { Session["SessionEditarAgenda"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                #endregion

                return Json(View());
            });
        }

        public async Task<JsonResult> EditarTipoMarcaMnt(string valor, string nuevoValor)
        {
            string sqlString = "UPDATE GMC_TIPO_MARCA\n" +
            "SET TIPO_MARCA_DESC = '" + nuevoValor + "'\n" +
            "WHERE TIPO_MARCA_DESC = '" + valor + "'";

            return await Task.Run(() =>
            {
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);
                //cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));

                #endregion

                #region Procedimiento
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { Session["SessionEditarAgenda"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                #endregion

                return Json(View());
            });
        }
        public async Task<JsonResult> EditarPaisMnt(string valor, string nuevoValor)
        {
            string sqlString = "UPDATE GEN_PAIS\n" +
           "SET PAIS_DESC = '" + nuevoValor + "'\n" +
           "WHERE PAIS_DESC = '" + valor + "'";

            return await Task.Run(() =>
            {
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);
                //cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));

                #endregion

                #region Procedimiento
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { Session["SessionEditarAgenda"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                #endregion

                return Json(View());
            });
        }

        public async Task<JsonResult> EditarClaseMnt(string value2, string valueString2)
        {

            string sqlString = "UPDATE GMC_CLASE\n" +
            "SET CLA_DESCRPCION = '"+valueString2+"'\n" +
            "WHERE CLA_DESCRPCION = '"+value2+"'";

            return await Task.Run(() => 
            {
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);
                //cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));
                
                #endregion

                #region Procedimiento
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { Session["SessionEditarAgenda"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                #endregion

                return Json(View());
            });
        }

        public async Task<JsonResult> EdNizaMnt(string valor, string nuevoValor)
        {
            return await Task.Run(() =>
            {
                string sqlString = "UPDATE GMC_EDNIZA\n" +
                "SET NIZ_DESCRIPCION = '" + nuevoValor + "'\n" +
                "WHERE NIZ_COD = " + int.Parse(valor);

                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);
                //cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));

                #endregion

                #region Procedimiento
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { Session["SessionEditarAgenda"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                #endregion
                return Json(View());
            });
        }

        public async Task<JsonResult> EstadoMnt(string value4, string valueString4)
        {
            return await Task.Run(() => 
            {

                string sqlString = "UPDATE GMC_STATUS\n" +
                "SET STA_DESCRIPCION = '"+value4+"'\n" +
                "WHERE STA_DESCRIPCION = '"+valueString4+"'";

                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);
                //cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));

                #endregion

                #region Procedimiento
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { Session["SessionEditarAgenda"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                #endregion
                return Json(View());
            });
        }

        public async Task<JsonResult> GetMntGeneral(string expedienteCod)
        {
            
                return await Task.Run(() =>
                {
                    bool ok;
                    string sqlString = "select t.reg_cod , t.reg_fecha_expediente , " +
                    " t.reg_inicio_registro , t.reg_registro ,\n" +
                    " t1.sta_descripcion , t.reg_vencimiento from gmc_registro_marca T " +
                    "inner join gmc_status T1 on T.REG_STATUS = T1.STA_COD " +
                    "WHERE T.REG_STATUS = " + int.Parse(expedienteCod.ToString());



                    Models.LogotipoModels lg = new Models.LogotipoModels();
                    //#region HTTP Auto ConnectionString Render
                    //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                    //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    //#endregion
                    string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                    OracleConnection con = new OracleConnection(cadena);

                    #region Conexion 
                    OracleCommand cmd = new OracleCommand(sqlString, con);
                    //cmd.BindByName = true;
                    //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", nombreEmpresa));
                    //cmd.Parameters.Add(new OracleParameter("RucEmpresa", rucEmpresa));
                    //cmd.Parameters.Add(new OracleParameter("DireccionEmpresa", direccionEmpresa));
                    //cmd.Parameters.Add(new OracleParameter("ProductoEmpresa", productoEmpresa));
                    //cmd.Parameters.Add(new OracleParameter("TelefonoEmpresa", telefonoEmpresa));
                    //cmd.Parameters.Add(new OracleParameter("PaisEmpresa", paisEmpresa));
                    //cmd.Parameters.Add(new OracleParameter("CorreoEmpresa", correoEmpresa));
                    #endregion

                    try
                    {
                        con.Open();
                        OracleDataReader dr = cmd.ExecuteReader();
                        ok = (dr.Read()) ? true : false;
                        if (ok)
                        {
                            CargarEntidades(dr, lg);
                        }
                    }
                    catch (Exception ex) { Session["ExRegistroEmpresa"] = ex; }
                    finally
                    {
                        if (con.State == System.Data.ConnectionState.Open)
                        {
                            con.Close();
                            con.Dispose();
                            cmd.Dispose();
                        }
                    }
                    return Json(View(lg));
                });
          
        }

        public async Task<ActionResult> GetMntDiseño(FormCollection frm)
        {
            int ExptList2 = int.Parse(Request.Form["ExptList2"].ToString());
            string fechaIncioRenovacion = Request["fechaIncioRenovacion"];
            int Status = int.Parse(Request.Form["Status"].ToString());
            string registroRenovacion = Request.Form["registroRenovacion"];
            //string registroInicioRenovacion = Request.Form["registroInicioRenovacion"];
            //string registroVencimientoRenovacion = Request.Form["registroVencimientoRenovacion"];
            //string registroInicioRenovacion2 = Request.Form["registroInicioRenovacion2"];
            string fechaCreacionRenovacion = Request.Form["fechaCreacionRenovacion"];
            string fechaVencimientoRenovacion = Request.Form["fechaVencimientoRenovacion"];
            string fechaRenovacion = Request.Form["fechaRenovacion"];
            string marcaRenovacion = Request.Form["marcaRenovacion"].ToString();
            string ClaseGeneralRenovacion = Request.Form["ClaseGeneralRenovacion"].ToString();
            string edNizaRenovacion = Request.Form["edNizaRenovacion"];
            string selectTipo = Request.Form["selectTipo"];
            //int expRenovacionId = int.Parse(Request.Form["expRenovacionId"].ToString());
            //int regRenovacion = int.Parse(Request.Form["regRenovacion"].ToString());

            return await Task.Run(() => 
            {

                string sqlString = "INSERT INTO GMC_MANTENIMIENTO_DISENHO (MAN_COD,\n" +
                "                                       MAN_EXPEDIENTE,\n" +
                "                                       MAN_FECHA_CREACION,\n" +
                "                                       MAN_ESTADO,\n" +
                "                                       MAN_REGISTRO,\n" +
                "                                       MAN_FECHA_INICIO,\n" +
                "                                       MAN_VENCIMIENTO,\n" +
                "                                       MAN_RENOVACION,\n" +
                "                                       MAN_MARCA,\n" +
                "                                       MAN_CLASE,\n" +
                "                                       MAN_ED_NIZA,\n" +
                "                                       MAN_TIPO,\n" +
                "                                       MAN_RENOVACION_EXP,\n" +
                "                                       MAN_RENOVACION_REG)\n" +
                "VALUES((SELECT NVL(MAX(MAN_COD), 0) + 1 FROM GMC_MANTENIMIENTO_DISENHO),"+ExptList2+",'"+fechaIncioRenovacion+"'" + "," + Status +  "," + registroRenovacion  + ",'" + fechaCreacionRenovacion + "'" + ",'" + fechaVencimientoRenovacion + "'" + ",'" + fechaRenovacion + "'" + ",'" + fechaIncioRenovacion + "'" + ",'" + marcaRenovacion + "'" +",'" + ClaseGeneralRenovacion + "'" + ",'" + selectTipo + "," + 0 + "," +  0 + ")";
                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);
                //cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));

                #endregion

                #region Procedimiento
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { Session["SessionEditarAgenda"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                #endregion

                return View();
            });
        }

        public async Task<JsonResult> ObtenerTitularesRenovacion(int expedienteCod)
        {
            return await Task.Run(() =>
             {
                 //var list = new Models.TitularesCreadorList();
                 Models.TitularesCreadoresModels tc = new TitularesCreadoresModels();
                 //string sqlString = "select * from gmc_registro_marca T\n" +
                 //"inner join gmc_datos_empresa T1 on T.REG_COD_EMPRESA = T1.DAT_COD\n" +
                 //"WHERE REG_COD =" + expedienteCod;

                 string sqlString = "select * from gmc_registro_marca T\n" +
                 "inner join gmc_datos_empresa T1 on T.REG_COD_EMPRESA = T1.DAT_COD\n" +
                 "inner join gmc_status T2  on T.REG_STATUS = T2.STA_COD\n" +
                 "WHERE REG_EXPEDIENTE ="+expedienteCod;


                 //#region HTTP Auto ConnectionString Render
                 //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                 //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                 //#endregion
                 string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                 OracleConnection con = new OracleConnection(cadena);

                 #region Conexion 
                 OracleCommand cmd = new OracleCommand(sqlString, con);
                 //cmd.BindByName = true;
                 //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));

                 #endregion

                 #region Procedimiento
                 try
                 {
                     con.Open();
                     OracleDataReader dr = cmd.ExecuteReader();
                     while (dr.Read())
                     {
                         tc.CodExpe = int.Parse(dr["REG_COD"].ToString());
                         tc.FechaCreacion = dr["REG_FECHA_EXPEDIENTE"].ToString();
                         tc.FechaInicio = dr["REG_INICIO_FECHA"].ToString();
                         tc.CreadorExp = dr["DAT_TITULAR"].ToString();
                         tc.Status = dr["STA_DESCRIPCION"].ToString();
                         tc.TitularExp = dr["DAT_TITULAR"].ToString();
                         tc.FechaVencimiento = dr["REG_VENCIMIENTO"].ToString();
                         tc.RegExp = dr["REG_REGISTRO"].ToString();
                         tc.Expediente = dr["REG_EXPEDIENTE"].ToString();
                     }
                 }
                 catch (Exception ex) { Session["SessionEditarAgenda"] = ex; }
                 finally
                 {
                     if (con.State == System.Data.ConnectionState.Open)
                     {
                         con.Close();
                         con.Dispose();
                     }
                 }
                 #endregion
                 return Json(View(tc));
             });
        }

        public async Task<JsonResult> Nizahange(string value4, string valueString4)
        {
            return await Task.Run(() => 
            {
                string sqlString = "UPDATE GMC_EDNIZA\n" +
              "SET NIZ_DESCRPCION = '" + value4 + "'\n" +
              "WHERE STA_DESCRIPCION = '" + valueString4 + "'";

                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);
                //cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));

                #endregion

                #region Procedimiento
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { Session["SessionEditarAgenda"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                #endregion
                return Json(View());
            });
        }

        //public async  Task<JsonResult> ObtenerMntFigura(int expedienteCod)
        //{

        //    return await Task.Run(() => 
        //    {
        //        return Json(View());
        //    });
        //}
        
        public JsonResult RetornarApoderadoEditar(int buscar)
        {
            string sqlString = "select * from GMC_APODERADO A, GEN_PAIS P, GEN_TIPO_PERSONA TP where A.APO_PAIS = P.PAIS_CODIGO "+
                "AND A.APO_TIPO_PERSONA = TP.GEN_TIPER_CODIGO AND A.APO_CODIGO = '" + buscar+ "'";

            Models.ListEmpresaBusqueda listApoderado = new Models.ListEmpresaBusqueda();


            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection conn = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, conn);
            Models.EmpresaBusquedaModel apoderado = new Models.EmpresaBusquedaModel();

            try
            {
                conn.Open();
                Permisos.OtorgarPermisos(conn);
                //OtorgarPermisos(conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    listApoderado.Add(new Models.EmpresaBusquedaModel(
                         apoderado.DescripcionEmpresa = dr["APO_RAZON_SOCIAL"].ToString(),
                         apoderado.RUC = dr["APO_RUC"].ToString(),
                         apoderado.Direccion = dr["APO_DIRECCION"].ToString(),
                         //apoderado.ProductoServicio = dr["APO_PRODUCTO_SERVICIO"].ToString(),
                         apoderado.Telefono = dr["APO_TELEFONO"].ToString(),
                         apoderado.PaisCodigo = int.Parse(dr["APO_PAIS"].ToString()),
                         apoderado.PaisDesc = dr["PAIS_DESC"].ToString(),
                         apoderado.Correo = dr["APO_CORREO"].ToString(),
                         //apoderado.Titulares = dr["APO_TITULARES"].ToString(),
                         apoderado.TipoPersona = int.Parse(dr["APO_TIPO_PERSONA"].ToString())));
                }


                
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return Json(listApoderado);

        }

        public JsonResult RetornarPoderEditar(int buscar)
        {

            string sqlString = "SELECT p.pod_codigo, p.pod_descripcion, p.pod_fecha_creacion, p.pod_apoderado, p.pod_obs, p.pod_firmante\n" +
            "FROM gmc_poder p, gmc_apoderado a\n" +
            "where p.pod_apoderado = a.apo_codigo and p.pod_codigo = "+buscar;

            Models.ListPoder listPoderes = new Models.ListPoder();


            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection conn = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, conn);
            Models.MntPoder poder = new Models.MntPoder();

            try
            {
                conn.Open();
                Permisos.OtorgarPermisos(conn);
                //OtorgarPermisos(conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    listPoderes.Add(new Models.MntPoder(
                         poder.DescPoder = dr["POD_DESCRIPCION"].ToString(),
                         poder.Poder = int.Parse(dr["POD_CODIGO"].ToString()),
                         //poder.FechaCreacion = DateTime.Parse(dr["POD_FECHA_CREACION"].ToString()),
                         poder.FechaCreacion = dr["POD_FECHA_CREACION"].ToString(),
                         poder.CodApoderado = int.Parse(dr["POD_APODERADO"].ToString()),
                         poder.Observacion = dr["POD_OBS"].ToString(),
                         poder.Firmante = dr["POD_FIRMANTE"].ToString()
                         ));
                }



            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return Json(listPoderes);

        }

        public JsonResult RetornarAgenteEditar(int buscar)
        {
            string sqlString = "select * from gmc_agente where age_cod = " + buscar;

            Models.AgenteList listAgentes = new Models.AgenteList();


            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection conn = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, conn);
            Models.AgenteModels agente = new Models.AgenteModels();

            try
            {
                conn.Open();
                Permisos.OtorgarPermisos(conn);
                //OtorgarPermisos(conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    listAgentes.Add(new Models.AgenteModels(
                         agente.CodAgente = int.Parse(dr["AGE_COD"].ToString()),
                         agente.DescAgente = dr["AGE_NOMBRE"].ToString(),
                         agente.MatriculaAgente = dr["AGE_MATRICULA"].ToString()));
                }



            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return Json(listAgentes);
        }

        public JsonResult RetornarPeriodicoEditar(int buscar)
        {
            string sqlString = "select * from gmc_periodico where per_codigo = " + buscar;

            Models.ListPeriodicos listPeriodicos = new Models.ListPeriodicos();


            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection conn = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, conn);
            Models.PeriodicoModel periodico = new Models.PeriodicoModel();

            try
            {
                conn.Open();
                Permisos.OtorgarPermisos(conn);
                //OtorgarPermisos(conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    listPeriodicos.Add(new Models.PeriodicoModel(
                         periodico.CodigoPeriodico = int.Parse(dr["PER_CODIGO"].ToString()),
                         periodico.DescPeriodico = dr["PER_DESCRIPCION"].ToString()));
                }



            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return Json(listPeriodicos);
        }
        public JsonResult RetornarMarcaEditar(int buscar)
        {
            //string sqlString = "select * from gmc_marca where marc_codigo = " + buscar;

            string sqlString = "select m.*, tm.tipo_marca_ind_logo\n" +
            "  from gmc_marca m, gmc_tipo_marca tm\n" +
            " where m.marc_tipo = tm.tipo_marca_cod\n" +
            "   and m.marc_codigo = " + buscar;


            Models.ListMarca listMarcas = new Models.ListMarca();


            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection conn = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, conn);
            Models.MarcaModel marca = new Models.MarcaModel();

            try
            {
                conn.Open();
                Permisos.OtorgarPermisos(conn);
                //OtorgarPermisos(conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    //listMarcas.Add(new Models.MarcaModel(
                    marca.MarcaDescripcion = dr["MARC_DESC"].ToString();
                    marca.MarcaClase = int.Parse(dr["MARC_CLASE"].ToString());
                    marca.MarcaEdniza = int.Parse(dr["MARC_EDNIZA_NRO"].ToString());
                    marca.Tipo = int.Parse(dr["MARC_TIPO"].ToString());
                    marca.TipoObs = dr["MARC_TIPO_OBS"].ToString();
                    marca.Reivindicacion = dr["MARC_REIVINDICACION"].ToString();
                    marca.ReivindicacionObs = dr["MARC_REIV_OBS"].ToString();
                         marca.DescProducto = dr["MARC_DESC_PROD"].ToString();
                    marca.Prioridad = int.Parse(dr["MARC_PRIORIDAD"].ToString());
                    marca.PaisCodigo = int.Parse(dr["MARC_PAIS"].ToString());
                    marca.Poder = dr["MARC_PODER"] != DBNull.Value ? int.Parse(dr["MARC_PODER"].ToString()) : 0;
                    marca.FechaPrioridad = dr["MARC_FECHA"].ToString();
                         marca.Logotipo = dr["MARC_LOGOTIPO"] != DBNull.Value ? Convert.ToBase64String((byte[])(dr["MARC_LOGOTIPO"])) : null;
                    marca.IndicadorLogotipo = dr["TIPO_MARCA_IND_LOGO"].ToString();
                    //));
                    listMarcas.Add(marca);
                }



            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return Json(listMarcas);
        }

        public JsonResult RetornarTipoMarcaEditar(int buscar)
        {
            string sqlString = "select * from gmc_tipo_marca where tipo_marca_cod = " + buscar;

            Models.ListTipoMarca listTipoMarcas = new Models.ListTipoMarca();


            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection conn = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, conn);
            Models.TipoMarcaModel tipoMarca = new Models.TipoMarcaModel();

            try
            {
                conn.Open();
                Permisos.OtorgarPermisos(conn);
                //OtorgarPermisos(conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    listTipoMarcas.Add(new Models.TipoMarcaModel(
                         tipoMarca.TipoMarcaCod = int.Parse(dr["TIPO_MARCA_COD"].ToString()),
                         tipoMarca.TipoMarcaDesc = dr["TIPO_MARCA_DESC"].ToString(),
                         tipoMarca.TipoMarcaIndLogo = dr["TIPO_MARCA_IND_LOGO"].ToString()));
                }



            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return Json(listTipoMarcas);
        }
        public JsonResult RetornarClaseEditar(int buscar)
        {
            string sqlString = "select * from gmc_tipo_clase where ticl_cod = " + buscar;

            Models.ClaseList listClases = new Models.ClaseList();


            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection conn = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, conn);
            Models.ClaseModels clase = new Models.ClaseModels();

            try
            {
                conn.Open();
                Permisos.OtorgarPermisos(conn);
                //OtorgarPermisos(conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    listClases.Add(new Models.ClaseModels(
                         clase.CodClase = int.Parse(dr["TICL_COD"].ToString()),
                         clase.DescClase = dr["TICL_DESC"].ToString()));
                }



            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return Json(listClases);
        }

        public JsonResult RetornarEdNizaEditar(int buscar)
        {
            string sqlString = "select * from gmc_edniza where niz_cod = " + buscar;

            Models.EdNizaList listEdNizas = new Models.EdNizaList();


            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection conn = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, conn);
            Models.EdNizaModels edNiza = new Models.EdNizaModels();

            try
            {
                conn.Open();
                Permisos.OtorgarPermisos(conn);
                //OtorgarPermisos(conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    listEdNizas.Add(new Models.EdNizaModels(                       
                         edNiza.CodClase = int.Parse(dr["NIZ_TIPO"].ToString()),
                         edNiza.NizNroClase = int.Parse(dr["NIZ_NRO_CLASE"].ToString()),
                         edNiza.DescEdNiza = dr["NIZ_DESCRIPCION"].ToString()));
                }



            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return Json(listEdNizas);
        }
        public JsonResult RetornarEstadoEditar(int buscar)
        {
            string sqlString = "select * from gmc_estado where est_codigo = " + buscar;

            Models.StatusList listEstados = new Models.StatusList();


            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection conn = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, conn);
            Models.StatusModel estado = new Models.StatusModel();

            try
            {
                conn.Open();
                Permisos.OtorgarPermisos(conn);
                //OtorgarPermisos(conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    listEstados.Add(new Models.StatusModel(

                         estado.DesStatus = dr["EST_DESC"].ToString(),
                         estado.CheckStatus = dr["EST_IND_MARC_CONCED"].ToString()
                         ));
                }



            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return Json(listEstados);
        }
        public JsonResult RetornarPaisEditar(int buscar)
        {
            string sqlString = "select * from gen_pais where pais_codigo = " + buscar;

            Models.ListPaises listPaises = new Models.ListPaises();


            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection conn = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, conn);
            Models.PaisModel pais = new Models.PaisModel();

            try
            {
                conn.Open();
                Permisos.OtorgarPermisos(conn);
                //OtorgarPermisos(conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    listPaises.Add(new Models.PaisModel(

                         pais.PaisDescripcion = dr["PAIS_DESC"].ToString()));
                }



            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return Json(listPaises);
        }
        public JsonResult RetornarTipoMovEditar(int buscar)
        {
            string sqlString = "select * from gmc_tipo_movimiento where tmov_codigo = " + buscar;

            Models.ListTipoMov listTipoMov = new Models.ListTipoMov();


            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection conn = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, conn);
            Models.TipoMovimientoModel tipoMov = new Models.TipoMovimientoModel();

            try
            {
                conn.Open();
                Permisos.OtorgarPermisos(conn);
                //OtorgarPermisos(conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    listTipoMov.Add(new Models.TipoMovimientoModel(

                         tipoMov.DescTipoMov= dr["TMOV_DESC"].ToString()));
                }



            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return Json(listTipoMov);
        }
        public JsonResult RetornarTipoSolEditar(int buscar)
        {
            string sqlString = "select * from gmc_tipo_solicitud where tsol_codigo = " + buscar;

            Models.ListTipoSol listTipoSol = new Models.ListTipoSol();


            //#region HTTP Auto ConnectionString Render
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //#endregion
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection conn = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand(sqlString, conn);
            Models.TipoSolicitudModel tipoSol = new Models.TipoSolicitudModel();

            try
            {
                conn.Open();
                Permisos.OtorgarPermisos(conn);
                //OtorgarPermisos(conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    listTipoSol.Add(new Models.TipoSolicitudModel(

                         tipoSol.DescTipoSolicitud = dr["TSOL_DESC"].ToString(),
                         tipoSol.AbrevTipoSolicitud = dr["TSOL_ABREV"].ToString()));
                }



            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            return Json(listTipoSol);
        }
        #region Controles
        public async Task<ActionResult> GetNiza()
        {
            return await Task.Run(() => 
            {
                var list = new NizaModelsList();

                string sqlString = "select * from gmc_edniza";

                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);

                #region Conexion 
                OracleCommand cmd = new OracleCommand(sqlString, con);
                //cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("RucEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("DireccionEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("ProductoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("TelefonoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("PaisEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("CorreoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("TelefonoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("PaisEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("CorreoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("TelefonoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("PaisEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("CorreoEmpresa", ));
                //cmd.Parameters.Add(new OracleParameter("CorreoEmpresa", ));
                #endregion
                try
                {
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new NizaClaseModels(int.Parse(dr["NIZ_COD_DESC"].ToString()),dr["NIZ_DESCRIPCION"].ToString()));
                        }
                    }
                }
                catch (Exception ex) { Session["manetinimientoEx"] = ex; }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                        con.Dispose();
                    }
                }
                return Json(list , JsonRequestBehavior.AllowGet);
            }); 
        }
        #endregion

        #region Metodos de Apoyo

        public ActionResult GrillaLogotipo([DataSourceRequest]DataSourceRequest request, string param1)
        {


            string sqlString = "select * from gmc_marca";


            var listMarcasLogotipos = new ListMarca();

            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand();
            if(!string.IsNullOrEmpty(param1))
                cmd = new OracleCommand("select * from gmc_marca where marc_codigo = " + param1, con);
            else
            {
                cmd = new OracleCommand("select * from gmc_marca", con);
            }
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {

                        listMarcasLogotipos.Add(new MarcaModel(
                            int.Parse(dr["MARC_CODIGO"].ToString()),
                            dr["MARC_DESC"].ToString(),
                            dr["MARC_LOGOTIPO"] != DBNull.Value ? (byte[])(dr["MARC_LOGOTIPO"]) : null
                            ));
                    }
                }
            }
            catch (Exception ex)
            {
                Session["ex"] = ex;
            }
            finally
            {
                con.Close();
                con.Dispose();
                cmd.Dispose();
            }

            return Json(listMarcasLogotipos.ToDataSourceResult(request));
        }
        public ActionResult GrillaPoder([DataSourceRequest]DataSourceRequest request, string param1)
        {

            var listScanPoderes = new ListPoder();

            string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
            OracleConnection con = new OracleConnection(cadena);
            OracleCommand cmd = new OracleCommand();
            if (!string.IsNullOrEmpty(param1))
                cmd = new OracleCommand("select * from gmc_poder where pod_codigo = " + param1, con);
            else
            {
                cmd = new OracleCommand("select * from gmc_poder", con);
            }
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {

                        listScanPoderes.Add(new MntPoder(
                            int.Parse(dr["POD_CODIGO"].ToString()),
                            dr["POD_DESCRIPCION"].ToString(),
                            dr["POD_ESCANEO"] != DBNull.Value ? (byte[])(dr["POD_ESCANEO"]) : null
                            ));
                    }
                }
            }
            catch (Exception ex)
            {
                Session["ex"] = ex;
            }
            finally
            {
                con.Close();
                con.Dispose();
                cmd.Dispose();
            }

            return Json(listScanPoderes.ToDataSourceResult(request));
        }
        private void CargarEntidades(OracleDataReader dr, LogotipoModels lg)
        {
            lg.CodReg = int.Parse(dr["REG_COD"].ToString());
            lg.FechaCreada = dr["REG_FECHA_EXPEDIENTE"].ToString();
            lg.FechaDesdeReg = dr["REG_INICIO_REGISTRO"].ToString();
            lg.RegistroReg = dr["REG_REGISTRO"].ToString();
            lg.Status = dr["STA_DESCRIPCION"].ToString();
            lg.VencimientoReg = dr["REG_VENCIMIENTO"].ToString();
        }
        #endregion
    }
}