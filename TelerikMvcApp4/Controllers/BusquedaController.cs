using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TelerikMvcApp4.Models;
using System.Net;
using System.Net.Mail;
using System.IO;
using Telerik.Reporting;
using TelerikMvcApp4.Reports;
using System.Configuration;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net.Mime;

namespace TelerikMvcApp4.Controllers
{
    [Authorize]
    public class BusquedaController : Controller
    {
        // GET: Busqueda


        #region Vistas
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<ActionResult> BusquedasParecidos()
        {
            return await Task.Run(() =>
            {
                return View();
            });
        }
        #endregion

        #region Metodos de Apoyo

        //public async Task<ActionResult> BusquedaMarca(int CodMarca)
        //{
        //    //return await Task.Run(() => 
        //    //{


        //        string sqlString = "select distinct MARC_CODIGO, DENOMINACION, TITULAR, similitud_distancia, similitud_error\n" +
        //        "      from (select C.CONF_PORCENTAJE,\n" +
        //        "                   M.MARC_CODIGO,\n" +
        //        "                   B.DENOMINACION,\n" +
        //        "                   B.TITULAR,\n" +
        //        "                   UTL_MATCH.JARO_WINKLER_SIMILARITY(M.MARC_DESC,\n" +
        //        "                                                     B.DENOMINACION) similitud_distancia,\n" +
        //        "                   UTL_MATCH.EDIT_DISTANCE_SIMILARITY(M.MARC_DESC,\n" +
        //        "                                                      B.DENOMINACION) similitud_error\n" +
        //        "              from GMC_MARCA M, GMC_BOLETIN B, GMC_CONFIGURACION C) X\n" +
        //        "     where X.SIMILITUD_DISTANCIA >= X.CONF_PORCENTAJE\n" +
        //        "       and X.SIMILITUD_ERROR >= X.CONF_PORCENTAJE\n" +
        //        "       and X.MARC_CODIGO = "+ CodMarca;


        //        var list = new Models.ListMarcaParecido();

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
        //            OracleDataReader dr = cmd.ExecuteReader();
        //            while (dr.Read())
        //            {
        //                list.Add(new Models.MarcaParecido(
        //                    dr["DENOMINACION"].ToString(),
        //                    dr["TITULAR"].ToString(),
        //                    int.Parse(dr["SIMILITUD_DISTANCIA"].ToString()),
        //                    int.Parse(dr["SIMILITUD_ERROR"].ToString())
        //                    ));
        //        }
        //        }
        //        catch (Exception ex) { Session["busquedaFoneticaEx"] = ex; }
        //        finally
        //        {
        //            if (con.State == System.Data.ConnectionState.Open)
        //            {
        //                con.Close();
        //                con.Dispose();
        //            }
        //        }
        //    return View(list);

        //    //});
        //}

        public ActionResult MarcaParecido([DataSourceRequest] DataSourceRequest request)
        {
            string sqlString = "select distinct m.marc_desc, a.apo_razon_social, m.marc_codigo, en.niz_nro_clase, en.niz_descripcion\n" +
            "from gmc_marca_parecido mp, gmc_marca m, gmc_poder p, gmc_apoderado a, gmc_edniza en\n" +
            "where mp.parecido_marca_cod = m.marc_codigo\n" +
            "and m.marc_poder = p.pod_codigo\n" +
            "and p.pod_apoderado = a.apo_codigo\n" +
            "and m.marc_edniza_nro = en.niz_cod\n" +
            "and mp.parecido_marca_cod not in\n" +
            "(select d.email_marca_par_cod from gmc_datos_email d)";



            var list = new Models.ListMarcaParecido();
            MarcaParecido marca = new MarcaParecido();

            #region Conexion 
            OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString);
            OracleCommand cmd = new OracleCommand(sqlString, con);

            #endregion

            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new Models.MarcaParecido(                           
                            marca.CodMarca = int.Parse(dr["MARC_CODIGO"].ToString()),
                            marca.DescMarca = dr["MARC_DESC"].ToString(),
                            marca.DescTitular = dr["APO_RAZON_SOCIAL"].ToString(),
                            marca.NroClaseEdNiza = int.Parse(dr["NIZ_NRO_CLASE"].ToString()),
                            marca.EdNizaDescripcion = dr["NIZ_DESCRIPCION"].ToString()
                            ));
                    }
                    IQueryable<Models.MarcaParecido> contact = list.AsQueryable();
                    DataSourceResult result = contact.ToDataSourceResult(request, r => new
                    {
                        r.CodMarca,
                        r.DescMarca,
                        r.DescTitular,
                        r.NroClaseEdNiza,
                        r.EdNizaDescripcion
                    });
                    return Json(result);
                }

            }
            catch (Exception ex) { Session["busquedaFoneticaEx"] = ex; }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return Json(list.ToDataSourceResult(request));
        }

        public ActionResult MarcaParecidoDetalle([DataSourceRequest]DataSourceRequest request, int codMarca)
        {
            string sqlString = "select mp.*, m.marc_desc, a.apo_codigo\n" +
            "  from gmc_marca_parecido mp, gmc_marca m, gmc_poder p, gmc_apoderado a\n" +
            " where mp.parecido_marca_cod = m.marc_codigo\n" +
            "   and m.marc_poder = p.pod_codigo\n" +
            "   and p.pod_apoderado = a.apo_codigo";


            var list = new Models.ListMarcaParecido();
            MarcaParecido marca = new MarcaParecido();

            OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString);
            OracleCommand cmd = new OracleCommand(sqlString, con);

            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (dr["PARECIDO_MARCA_COD"].ToString() == codMarca.ToString())
                    {
                        list.Add(new Models.MarcaParecido(
                            marca.ParecidoCodigo = int.Parse(dr["PARECIDO_CODIGO"].ToString()),
                            marca.CodMarca = int.Parse(dr["PARECIDO_MARCA_COD"].ToString()),
                            marca.DescMarca = dr["PARECIDO_MARCA_DEN"].ToString(),
                            marca.DescTitular = dr["PARECIDO_TITULAR"].ToString(),
                            marca.PorcentajeSimilitud = int.Parse(dr["PARECIDO_PORC_SIMILITUD"].ToString()),
                            marca.PorcentajeError = int.Parse(dr["PARECIDO_PORC_ERROR"].ToString()),
                            marca.IntencionOposicion = dr["PARECIDO_INTEN_OPOS"] != DBNull.Value ? int.Parse(dr["PARECIDO_INTEN_OPOS"].ToString()) : 0,
                            marca.CodTitular = int.Parse(dr["APO_CODIGO"].ToString())
                        ));
                    }
                }
                IQueryable<Models.MarcaParecido> contactDetalle = list.AsQueryable().Where(det => det.CodMarca == codMarca);
                DataSourceResult result = contactDetalle.ToDataSourceResult(request, r => new
                {
                    r.ParecidoCodigo,
                    r.CodMarca,
                    r.DescMarca,
                    r.DescTitular,
                    r.PorcentajeSimilitud,
                    r.PorcentajeError,
                    r.IntencionOposicion,
                    r.CodTitular
                });
                return Json(result);

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
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        public JsonResult BuscarDatos(string Agente , string Expediente , string Registro , string Marca)
        {
            int expediente = 0;
            int registro = 0;

            if (Expediente == "")
            {
                expediente = 0;
            }
            else
            {
                expediente = int.Parse(Expediente);
            }

            if (Registro == "")
            {
                registro = 0;
            }
            else
            {
                registro = int.Parse(Registro);
            }
            string sqlString = "select * from gmc_registro_marca T\n" +
            "inner join gmc_datos_empresa T1 on T.REG_COD_EMPRESA = T1.DAT_COD\n" +
            "inner join gmc_status T2 on T.REG_STATUS = T2.STA_COD\n" +
            "inner join gmc_clase T3 on T.REG_CLASE = T3.CLA_COD\n" +
            "inner join GMC_ED_NIZA T4 ON T.REG_ED_NIZA = T4.ED_COD\n" +
            "inner join gmc_agente T5 on T.REG_AGENTE = T5.AGE_COD\n" +
            "where T5.AGE_NOMBRE = '"+Agente+"' or T.REG_EXPEDIENTE = "+expediente+" or T.REG_REGISTRO = "+ registro+" or T.REG_MARCA_DESCRPCION = '"+Marca+"'";

                var list = new Models.BusquedaMarcaList();

                #region HTTP Auto ConnectionString Render
                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                #endregion

                #region Conexion 
                OracleConnection con = new OracleConnection(authTicket.UserData.ToString());
                OracleCommand cmd = new OracleCommand(sqlString, con);
                //cmd.BindByName = true;
                //cmd.Parameters.Add(new OracleParameter("Agente", Agente ));
                //cmd.Parameters.Add(new OracleParameter("Expediente", Expediente ));
                //cmd.Parameters.Add(new OracleParameter("Registro", Registro));
                //cmd.Parameters.Add(new OracleParameter("Marca", Marca ));

                #endregion

                #region Operaciones
                try
                {
                    con.Open();
                Permisos.OtorgarPermisos(con);
                using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new Models.BusquedaMarca(int.Parse(dr["REG_COD"].ToString()), dr["REG_EXPEDIENTE"].ToString(), dr["DAT_TITULAR"].ToString(), dr["REG_FECHA_EXPEDIENTE"].ToString(), dr["STA_DESCRIPCION"].ToString(), dr["REG_REGISTRO"].ToString(), dr["REG_INICIO_FECHA"].ToString(), dr["REG_VENCIMIENTO"].ToString(),  dr["REG_MARCA_DESCRPCION"].ToString(), dr["STA_COD"].ToString()+ " - "+dr["STA_DESCRIPCION"].ToString(), dr["CLA_DESCRPCION"].ToString(), dr["ED_CLASE"].ToString()+" - "+dr["ED_DESCRIPCION"].ToString(), dr["REG_TIPO"].ToString(), dr["REG_DIRECCION"].ToString(), dr["REG_PODER"].ToString(), dr["AGE_NOMBRE"].ToString()+" - "+dr["AGE_MATRICULA"].ToString(), dr["REG_PRODUCTO_SERVICIO"].ToString(),  dr["REG_DENOMINACION"].ToString()));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Session["ExepcionQ"] = ex;
                    throw;
                }
                #endregion

                return Json(list);
           
        }

        [HttpPost]
        public ActionResult Index(MarcaParecido[] anyName)
        {
            string destinatario =Request.Params["destinatario"];
            string numMarca = Request.Params["marcas"];
            if (ModelState.IsValid)
            {

                string sqlString = "insert into gmc_datos_email\n" +
                "  (email_codigo,\n" +
                "   email_destinatario,\n" +
                "   email_asunto,\n" +
                //"   email_archivo_pdf,\n" +
                "   email_cuerpo,\n" +
                "   email_fecha,\n" +
                "   email_marca_par_cod,\n" +
                "   email_marca_cod,\n" +
                "   email_titular_cod)\n" +
                "values\n" +
                "  ((select nvl(max(email_codigo), 0) + 1 from gmc_datos_email),\n" +
                "   :destinatario,\n" +
                "   :asunto,\n" +
                //"   :pdf,\n" +
                "   :cuerpo,\n" +
                "   sysdate,\n" +
                "   :marcaCod,\n" +
                "   :marcaC,\n" +
                "   :titularCod)";

                //#region HTTP Auto ConnectionString Render
                //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                //#endregion
                string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
                OracleConnection con = new OracleConnection(cadena);
                OracleCommand cmd = new OracleCommand(sqlString, con);

                try
                {
                    Telerik.Reporting.Processing.ReportProcessor reportProcessor =
                    new Telerik.Reporting.Processing.ReportProcessor();

                    // set any deviceInfo settings if necessary
                    System.Collections.Hashtable deviceInfo =
                        new System.Collections.Hashtable();



                    // reportName is the Assembly Qualified Name of the report
                    GMCL001 report1 = new GMCL001();
                    InstanceReportSource instanceReportSource1 = new Telerik.Reporting.InstanceReportSource();
                    instanceReportSource1.ReportDocument = report1;
                    instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("PCODIGO", anyName.Select(p => p.ParecidoCodigo).ToList()));
                    instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("PMARCA", anyName.Select(p => p.CodMarca).ToList()));
                    //ReportParameter param = new ReportParameter { Name = "PCODIGO", Value = grid.Select(p => p.ParecidoCodigo) };

                    Telerik.Reporting.Processing.RenderingResult result =
                        reportProcessor.RenderReport("PDF", instanceReportSource1, deviceInfo);

                    string fileName = result.DocumentName + "." + result.Extension;
                    string path = Path.GetTempPath();
                    string filePath = Path.Combine(path, fileName);
                
                    con.Open();
                    Permisos.OtorgarPermisos(con);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
                        ms.Flush();
                        ms.Position = 0;

                        //string from = "no_reply@century.com.py";
                        string from = User.Identity.Name;
                        using (MailMessage mail = new MailMessage(from, destinatario))
                        {
                            mail.Subject = "Oposición a la marca";
                            MailAddress copy = new MailAddress(User.Identity.Name);
                            mail.CC.Add(copy);
                            //mail.Body = "Esta(s) marca(s) presenta(n) similitud(es) a la marca registrada por usted.\n" +
                            //    " Favor responder si desea oponerse o no. ";
                            if (ms != null)
                            {
                               
                                mail.Attachments.Add(new Attachment(ms, "Vigilancia_de_Marcas" + ".pdf"));
                            }
                            string body = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">";
                            body += "<HTML><HEAD><META http-equiv=Content-Type content=\"text/html; charset=iso-8859-1\">";
                            body += "</HEAD><BODY><DIV class='center-block' style='background-color:#17212e; color:#f4f4f4; width:50%'><strong style='color:rgb(34,34,34); font-family:'Proxima Nova'; line-height:23px;'>";
                            body += "</strong>Esta(s) marca(s) presenta(n) similitud(es) a la marca registrada por usted.";
                            body +=  " Favor responder si desea oponerse o no.";
                            body += "</DIV></BODY></HTML>";

                            ContentType mimeType = new System.Net.Mime.ContentType("text/html");
                            // Add the alternate body to the message.

                            AlternateView alternate = AlternateView.CreateAlternateViewFromString(body, mimeType);
                            mail.AlternateViews.Add(alternate);
                            mail.IsBodyHtml = false;
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = "mail.century.com.py";
                            //smtp.Host = "smtp.gmail.com";
                            smtp.EnableSsl = true;
                            NetworkCredential networkCredential = new NetworkCredential(from, "");
                            smtp.UseDefaultCredentials = true;
                            smtp.Credentials = networkCredential;
                            smtp.Port = 25;
                            smtp.Send(mail);
                            ViewBag.Message = "Sent";

                            //cmd.Parameters.Add(new OracleParameter("destinatario", destinatario));
                            cmd.Parameters.Add(new OracleParameter { ParameterName = "destinatario"});
                            //cmd.Parameters.Add(new OracleParameter("asunto", mail.Subject));
                            cmd.Parameters.Add(new OracleParameter { ParameterName = "asunto"});
                            //cmd.Parameters.Add(new OracleParameter("pdf", mail.Attachments));
                            //cmd.Parameters.Add(new OracleParameter("cuerpo", mail.Body));
                            cmd.Parameters.Add(new OracleParameter { ParameterName = "cuerpo"});
                            cmd.Parameters.Add(new OracleParameter { ParameterName = "marcaCod" });
                            cmd.Parameters.Add(new OracleParameter { ParameterName = "marcaC" });
                            cmd.Parameters.Add(new OracleParameter { ParameterName = "titularCod" });
                            foreach (var item in anyName)
                            {
                                //cmd.Parameters.Add(new OracleParameter("destinatario", destinatario));
                                cmd.Parameters["destinatario"].Value = destinatario;
                                //cmd.Parameters.Add(new OracleParameter("asunto", mail.Subject));
                                cmd.Parameters["asunto"].Value = mail.Subject;
                                //cmd.Parameters.Add(new OracleParameter("pdf", mail.Attachments));
                                //cmd.Parameters.Add(new OracleParameter("cuerpo", mail.Body));
                                cmd.Parameters["cuerpo"].Value = mail.Body;
                                cmd.Parameters["marcaCod"].Value = item.CodMarca;
                                cmd.Parameters["marcaC"].Value = item.ParecidoCodigo;
                                cmd.Parameters["titularCod"].Value = item.CodTitular;
                                cmd.ExecuteNonQuery();
                            }
                            
                            
                            return RedirectToAction("BusquedasParecidos");
                        }
                    }
                }
                catch (SmtpException smtpEx)
                {
                    Session["errEmailSmtp"] = smtpEx;
                    return RedirectToAction("BusquedasParecidos");
                }
                catch (Exception ex)
                {
                    Session["errEmail"] = ex;
                    return RedirectToAction("BusquedasParecidos");
                }
                finally
                {
                    con.Close();
                    cmd.Dispose();
                }
            }       
            else
            {
                return View();
            }

        }
        #endregion

    }
}