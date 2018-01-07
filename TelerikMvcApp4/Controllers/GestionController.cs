using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Web.Security;
using TelerikMvcApp4.Models;
using System.IO;
using Newtonsoft.Json;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Configuration;

namespace TelerikMvcApp4.Controllers
{
	[Authorize]
	public class GestionController : Controller
	{
		public byte[] bytes;
		// GET: Gestion
		#region Vistas       
		public async Task<ActionResult> Index()
		{
			return await Task.Run(() =>
			 {
				return View();
			});

		}

		[Authorize]
		public ActionResult RegistroMarca()
		{
			return View();
		}

		[Authorize]
		public ActionResult MovimientoExpediente()
		{
			return View();
		}
		//[Authorize]
		//public ActionResult ImagenesPublicacion(PublicacionImagenModel model)
		//{
		//    return View(model);
		//}

		[Authorize]
		public ActionResult ImagenesPublicacion(string fecha, int id=0)
		{
			if (id != 0)
			{
				ViewBag.Id = id.ToString();
				ViewBag.Fecha = fecha.ToString();
			}           
			return View();
		}

		[Authorize]
		public ActionResult RegistroOposicion()
		{
			return View();
		}

		[Authorize]
		public async Task<ActionResult> PatenteInvencion()
		{
			return await Task.Run(() =>
			{
				return View();
			});

		}
		[Authorize]
		public async Task<ActionResult> DibujoIndustriales()
		{
			return await Task.Run(() =>
			{
				return View();
			});
		}
		#endregion
		
		#region Acciones de Vista
		//Acción que permite confirmar si una marca fue publicada o no en un periódico.
		public async Task<ActionResult> ConfirmarPublicacion(FormCollection frm, IEnumerable<HttpPostedFileBase> files)
		{
			//return await Task.Run(() =>
			//{
				int ExpedienteNro = int.Parse(Request.Form["ExpedienteNro"]);
				string Publicado = Request.Form["fuePublicado"];
				string observacion = Request.Form["observacionPub"];
				string fechaPub = Request.Form["fechaPub"];
				byte[] bytes = null;
			//string queryRoles = "update gmc_publicacion set " +
			//" PUB_PUBLICADO = :updatePublicado, " +
			//" PUB_IMAGEN = :updateImagen, " +
			//" PUB_OBS = :updateObs," +
			//" PUB_MODIFICADO = " + 1  +
			//" where PUB_EXPEDIENTE = " + ExpedienteNro;

			string queryRoles = "update gmc_publicacion_detalle set " +
				" PUB_PUBLICADO = :updatePublicado, " +
				" PUB_IMAGEN = :updateImagen, " +
				" PUB_OBS = :updateObs" +
				" where PUB_CLAVE = (select p.pub_clave from gmc_publicacion p where p.pub_expediente =" + ExpedienteNro+ ")" +
				" and pub_fecha_pub = '" + fechaPub +"'";

			string sqlStringSelect = "select pub_clave from gmc_publicacion where pub_expediente = " + ExpedienteNro;
			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;


			OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand(queryRoles, con);
				OracleCommand cmdSelect = new OracleCommand(sqlStringSelect, con);
				try
				{
					con.Open();
					Permisos.OtorgarPermisos(con);
					OracleDataReader dr = cmdSelect.ExecuteReader();
					while (dr.Read())
					{
						if (files != null)
						{
							foreach (var item in files)
							{
								if (item != null)
								{
									string sqlString = "insert into gmc_publicacion_imagen (puim_clave_pub, puim_imagen) values(:clavePub, :imagenPub)";
									OracleCommand cmd2 = new OracleCommand(sqlString, con);
									cmd2.Parameters.Add(new OracleParameter("clavePub", int.Parse(dr["PUB_CLAVE"].ToString())));
									using (Stream inputStream = item.InputStream)
									{
										MemoryStream memoryStream = inputStream as MemoryStream;
										if (memoryStream == null)
										{
											using (Image img = Image.FromStream(item.InputStream))
											using (Bitmap bmp = new Bitmap(img))
											{
												memoryStream = new MemoryStream();
												//for (int x = 0; x < img.Width; x++)
												//{
												//    for (int y = 0; y < img.Height; y++)
												//    {
												//        Color c = bmp.GetPixel(x, y);
												//        if (c.R == 255 && c.G == 255 && c.B == 255)
												//            bmp.SetPixel(x, y, Color.FromArgb(0));
												//    }
												//}
												bmp.Save(memoryStream, ImageFormat.Png);
												
												//bmp.Save(inputStream, ImageFormat.Png);
												inputStream.CopyTo(memoryStream);
												bytes = memoryStream.ToArray();
											}                                        
										}
										cmd2.Parameters.Add(new OracleParameter("imagenPub", OracleDbType.Blob, bytes, System.Data.ParameterDirection.Input));
									}
									cmd2.ExecuteNonQuery();
								}
							}
							cmd.Parameters.Add(new OracleParameter("updatePublicado", Publicado));
							cmd.Parameters.Add(new OracleParameter("updateImagen", OracleDbType.Blob, bytes, System.Data.ParameterDirection.Input));
							cmd.Parameters.Add(new OracleParameter("updateObs", observacion));
							cmd.ExecuteNonQuery();
						}
					}

				}
				catch (Exception ex) { Session["ExSessionPu"] = ex; }
				finally
				{
					if (con.State == System.Data.ConnectionState.Open)
					{
						con.Close();
						con.Dispose();
						cmd.Dispose();
					}
				}

			

			//return View("ImagenesPublicacion");
			return RedirectToAction("ImagenesPublicacion");
			//});
		}


		/*Acción que guarda el scan de la respuesta del cliente, sobre la oposicion sobre una determinada marca.
		Además de insertar tal respues en la tabla cabecera GMC_OPOSICION.*/
		public async Task<ActionResult> Save(IEnumerable<HttpPostedFileBase> files, int numMarca, string[] ids)
		{
			//string sqlInsertImagen = "insert into gmc_oposicion (OPOSICION_CODIGO, OPOSICION_MARCA_COD, OPOSICION_IMAGEN,\n" +
			//   " OPOSICION_FECHA, OPOSICION_EMAIL_COD)\n" +
			//   " values((select nvl(max(OPOSICION_CODIGO),0) + 1 from gmc_oposicion),\n" +
			//   " :marcaCod,\n" +
			//   " :imagenOposicion,\n" +
			//   " sysdate,\n" +
			//   " (select nvl(max(email_codigo), 0) from gmc_datos_email where email_marca_par_cod = " + numMarca + "\n)" +
			//   " ) RETURNING OPOSICION_CODIGO INTO :id";
			string sqlInsertImagen = "insert into gmc_oposicion (OPOSICION_CODIGO, OPOSICION_IMAGEN,\n" +
			   " OPOSICION_FECHA, OPOSICION_EMAIL_COD, OPOSICION_TITULAR_COD)\n" +
			   " values((select nvl(max(OPOSICION_CODIGO),0) + 1 from gmc_oposicion),\n" +
			   " :imagenOposicion,\n" +
			   " sysdate,\n" +
			   " (select nvl(max(email_codigo), 0) from gmc_datos_email where email_titular_cod = " + numMarca + "\n)," +
			   " :titularCod" +
			   " ) RETURNING OPOSICION_CODIGO INTO :id";

			string sqlString = "select * from gmc_marca_parecido";
			string sqlInsertOposicion = "";

			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;

			OracleConnection con = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand(sqlString, con);
			OracleCommand cmdInsertImage = new OracleCommand(sqlInsertImagen, con);
			try
			{
				con.Open();
				Permisos.OtorgarPermisos(con);
				using (OracleDataReader dr = cmd.ExecuteReader())
				{
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
						//cmdInsertImage.Parameters.Add(new OracleParameter("marcaCod", numMarca));
						
						cmdInsertImage.Parameters.Add(new OracleParameter("imagenOposicion", bytes));
						cmdInsertImage.Parameters.Add(new OracleParameter("titularCod", numMarca));
						cmdInsertImage.Parameters.Add(new OracleParameter
						{
							ParameterName = ":id",
							OracleDbType = OracleDbType.Int32,
							Direction = ParameterDirection.Output
						});
						//if (!imagenInsertada)
						//{
						cmdInsertImage.ExecuteNonQuery();
						//imagenInsertada = true;
						//}

					}
					while (dr.Read())
					{
						if (ids != null)
						{
							var filas = ids[0].Split(',');
							for (int i = 0; i < filas.Length; i++)
							{
								if (dr["PARECIDO_MARCA_DEN"].ToString() == filas[i])
								{
									sqlInsertOposicion = "insert into gmc_oposicion_detalle (opo_codigo_det, opo_den_marca,\n" +
								   " opo_titular, opo_porc_sim, opo_porc_err, opo_confirmar, opo_codigo)\n" +
								   " values((select nvl(max(ro.opo_codigo_det), 0) +1 from gmc_oposicion_detalle ro),\n" +
								   " :denominacion,\n" +
								   " :titular,\n" +
								   " :porcentajeSimilitud,\n" +
								   " :porcentajeError,\n" +
								   " :confirmar,\n" +
								   " (select nvl(max(o.oposicion_codigo), 0) from gmc_oposicion o))";

									OracleCommand cmdInsert = new OracleCommand(sqlInsertOposicion, con);
									cmdInsert.Parameters.Add(new OracleParameter("denominacion", filas[i]));
									cmdInsert.Parameters.Add(new OracleParameter("titular", dr["PARECIDO_TITULAR"].ToString()));
									cmdInsert.Parameters.Add(new OracleParameter("porcentajeSimilitud", dr["PARECIDO_PORC_SIMILITUD"].ToString()));
									cmdInsert.Parameters.Add(new OracleParameter("porcentajeError", dr["PARECIDO_PORC_ERROR"].ToString()));
									cmdInsert.Parameters.Add(new OracleParameter("confirmar", filas[i + 1]));
									cmdInsert.ExecuteNonQuery();
								}
							}
						}
					}
				}

			}
			catch (Exception ex)
			{
				Session["imgEx"] = ex;
				return View();
			}
			finally
			{
				con.Close();
				cmdInsertImage.Dispose();
			}
			return Content("");

		}

		public async Task<ActionResult> GetSolictudMarca(FormCollection frm)
		{
			//return await Task.Run(() =>
			//{
			Session["error"] = false;
			bool existe = false;
			string empresa = Request.Form["EmpresasList"];
			string ExpedienteGeneral =  Request.Form["expedienteNro"];
			string poder = Request.Form["poder"];
			//string FechaInicioSol =Request.Form["fechaIncioGeneral"];
			string Status = Request.Form["Status"];
			string fechaSolicitud = Request.Form["fechaSolicitud"];
			//string fechaEstado = Request.Form["fechaEstado"];
			string marca = Request.Form["Marca"];
			string agente = Request.Form["Agente"];
			string pais = Request.Form["pais2"];
			string TipoSol = Request.Form["select"];
			//string fechaVencimiento = Request.Form["fechaVencimiento"];

			string nuevoExpediente = Request.Form["nuevoExpediente"];

			
			string sqlString = "select * from gmc_expediente where exp_codigo = :selectExpNro";

			string sqlSelect = "select * from gmc_expediente where exp_marca = " + int.Parse(marca);

			string queryRoles = "insert into gmc_expediente(EXP_CODIGO,\n" +
				"EXP_ESTADO,\n" +
				"EXP_AGENTE,\n" +
				"EXP_FECHA_SOLICITUD,\n" +
				//"EXP_FECHA_ESTADO,\n" +
				"EXP_TIPO_SOL,\n" +
				"EXP_SOLICITANTE,\n" +
				"EXP_PAIS,\n" +
				"EXP_PODER,\n" +
				"EXP_MARCA,\n" +
				"EXP_COD_ANT)\n" +
				"values(:ExpCodigo,\n" +
				":estado ,\n" +
				" :agente ,\n" +
				" :fechaSolicitud ,\n" +
				//" :fechaEstado,\n" +
				" :tipoSolicitud ,\n" +
				" :solicitante ,\n" +
				" :pais,\n" +
				" :poder,\n" +
				" :marca,\n" +
				" :expAnterior)";

			string sqlInsertHist = "insert into gmc_historia_exp(HIST_COD,\n" +
				" HIST_EXP_COD,\n" +
				" HIST_EXP_FECHA,\n" +
				//"HIST_TMOV,\n" +
				" HIST_ESTADO,\n"+
				" HIST_FEC_GRAB,\n"+
				" HIST_LOGIN_GRAB)\n" +
				"values((select nvl(max(hist_cod), 0) + 1 from gmc_historia_exp),\n" +
				":ExpCod ,\n" +
				" :FechaExp ,\n" +
				//" :TipoMov ,\n" +
				" :est,\n"+
				" sysdate,\n"+
				" user)";

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand(queryRoles, con);
				
				cmd.Parameters.Add(new OracleParameter("ExpCodigo", string.IsNullOrEmpty(nuevoExpediente) ? int.Parse(ExpedienteGeneral) : int.Parse(nuevoExpediente)));
				cmd.Parameters.Add(new OracleParameter("estado", int.Parse(Status)));
				cmd.Parameters.Add(new OracleParameter("agente", int.Parse(agente)));
				cmd.Parameters.Add(new OracleParameter("fechaSolicitud", DateTime.Parse(fechaSolicitud)));
				//cmd.Parameters.Add(new OracleParameter("fechaEstado", DateTime.Parse(fechaEstado)));
				cmd.Parameters.Add(new OracleParameter("tipoSolicitud", int.Parse(TipoSol)));
				cmd.Parameters.Add(new OracleParameter("solicitante", int.Parse(empresa)));
				cmd.Parameters.Add(new OracleParameter("pais", int.Parse(pais)));
				cmd.Parameters.Add(new OracleParameter("poder", int.Parse(poder)));
				cmd.Parameters.Add(new OracleParameter("marca", int.Parse(marca)));
				cmd.Parameters.Add(new OracleParameter("expAnterior", string.IsNullOrEmpty(nuevoExpediente) ? (int?)null : int.Parse(ExpedienteGeneral)));
				//cmd.Parameters.Add(new OracleParameter("fechaVencimiento", DateTime.Parse(fechaVencimiento)));



				#region Insert en la tabla GMC_HISTORIA_EXP
				OracleCommand cmdInsertHist = new OracleCommand(sqlInsertHist, con);
				cmdInsertHist.Parameters.Add(new OracleParameter("ExpCod", string.IsNullOrEmpty(nuevoExpediente) ? int.Parse(ExpedienteGeneral) : int.Parse(nuevoExpediente)));
				cmdInsertHist.Parameters.Add(new OracleParameter("FechaExp", DateTime.Parse(fechaSolicitud)));
				//cmdInsertHist.Parameters.Add(new OracleParameter("TipoMov", TipoMov));
				cmdInsertHist.Parameters.Add(new OracleParameter("est", int.Parse(Status)));
				//cmdInsertHist.Parameters.Add(new OracleParameter("observacion", int.Parse(ExpedienteGeneral)));
				#endregion

				OracleCommand cmd2 = new OracleCommand(sqlString, con);
				cmd2.Parameters.Add(new OracleParameter("selectExpNro", string.IsNullOrEmpty(nuevoExpediente) ? int.Parse(ExpedienteGeneral) : int.Parse(nuevoExpediente)));
				//OracleCommand cmdHistoria = new OracleCommand(sqlStringHistoria, con);
				#endregion

				#region Procedimiento
				try
				{
					con.Open();
				Permisos.OtorgarPermisos(con);
				using (OracleDataReader dr = cmd2.ExecuteReader())
					{
						while (dr.Read())
						{
							existe = true;
							string sqlStringUpdate = "update gmc_expediente set exp_estado = :updateEstado,\n" +
							"exp_agente = :updateAgente,\n" +
							"exp_fecha_solicitud = :updateFechaSolicitud,\n" +
							//"exp_fecha_estado = :updateFechaEstado,\n" +
							"exp_tipo_sol = :updateTipoSol,\n" +
							"exp_solicitante = :updateSolicitante,\n" +
							"exp_pais = :updatePais,\n" +
							"exp_poder = :updatePoder,\n" +
							"exp_marca = :updateMarca\n" +
							"where exp_codigo = "+ ExpedienteGeneral;

							OracleCommand cmdUpdate = new OracleCommand(sqlStringUpdate, con);
							cmdUpdate.Parameters.Add(new OracleParameter("updateEstado", int.Parse(Status)));
							cmdUpdate.Parameters.Add(new OracleParameter("updateAgente", int.Parse(agente)));
							cmdUpdate.Parameters.Add(new OracleParameter("updateFechaSolicitud", DateTime.Parse(fechaSolicitud)));
							//cmdUpdate.Parameters.Add(new OracleParameter("updateFechaEstado", DateTime.Parse(fechaEstado)));
							cmdUpdate.Parameters.Add(new OracleParameter("updateTipoSol", int.Parse(TipoSol)));
							cmdUpdate.Parameters.Add(new OracleParameter("updateSolicitante", int.Parse(empresa)));
							cmdUpdate.Parameters.Add(new OracleParameter("updatePais", int.Parse(pais)));
							cmdUpdate.Parameters.Add(new OracleParameter("updatePoder", int.Parse(poder)));
							cmdUpdate.Parameters.Add(new OracleParameter("updateMarca", int.Parse(marca)));
							//cmdUpdate.Parameters.Add(new OracleParameter("updateFechaVencimiento", DateTime.Parse(fechaVencimiento)));
							cmdUpdate.ExecuteNonQuery();
						}
					}
					if (!existe)
					{
						cmd.ExecuteNonQuery();
					}

					cmdInsertHist.ExecuteNonQuery();
					ViewBag.Message = "Success";
				}
				catch (Exception ex)
				{
					Session["ex"] = ex;
					Session["error"] = true;
					ViewBag.Message = "Success";
				//if (e.Message.Contains("GMCEXP_GMCDATPUB"))
				// FK_dbo.Vehiculos_dbo.Clientes_ClienteCodigo
		//    return Json(new JsonResponse { Success = false, Message = "No se puede borrar el expediente. Elimine primero la publicación de ese expediente" });
			}
				finally
				{
					if (con.State == System.Data.ConnectionState.Open)
					{
						con.Close();
						con.Dispose();
						cmd.Dispose();
						cmd2.Dispose();
						//cmdHistoria.Dispose();
						cmdInsertHist.Dispose();
					}
				}
			#endregion
			//return View("RegistroMarca");
			return RedirectToAction("RegistroMarca");
			//});
		}

		public async Task<ActionResult> GetPublicacion(FormCollection frm)
		{
			//return await Task.Run(() =>
			//{
				string ExpedienteList = Request.Form["PoderExpediente2"];
				string fechaPrimeraPublicacion = Request.Form["fechaPrimeraPublicacion"];
				string diarioPublicacion = Request.Form["ListaPeriodicos"];
				string cantDias = Request.Form["diasPublicacion"];
				//string fechaSegundoPublicacion = Request.Form["fechaSegundoPublicacion"];
				//string fechaTerceroPublicacion = Request.Form["fechaTerceroPublicacion"];
				//byte[] bytes = new byte[int.Parse(Request.Form["archivosPublicacion"])];
				//string archivosPublicacion = Request.Form["archivosPublicacion"];
				// string queryRoles = Shared.GestionDataConnect.Insertar_GMC_DATOS_PUBLICACION(int.Parse(ExpedienteList), fechaPrimeraPublicacion, int.Parse(diarioPublicacion), int.Parse(cantDias));
				string queryRoles = "insert into gmc_publicacion(PUB_EXPEDIENTE,\n" +
				"PUB_FECHA,\n" +
				"PUB_PERIODICO,\n" +
				"PUB_DIAS)\n" +
				"values(:Exp ,\n" +
				" :Fecha ,\n" +
				" :Periodico ,\n" +
				" :Dias)";

			//#region HTTP Auto ConnectionString Render
			//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			//#endregion

			#region Conexion 
			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand(queryRoles, con);

				cmd.Parameters.Add(new OracleParameter("Exp", int.Parse(ExpedienteList)));
				cmd.Parameters.Add(new OracleParameter("Fecha", DateTime.Parse(fechaPrimeraPublicacion)));
				cmd.Parameters.Add(new OracleParameter("Periodico", int.Parse(diarioPublicacion)));
				cmd.Parameters.Add(new OracleParameter("Dias", int.Parse(cantDias)));
				#endregion

				#region Procedimiento
				try
					{
						con.Open();
					Permisos.OtorgarPermisos(con);
					cmd.ExecuteNonQuery();

				}
				catch (Exception ex) { Session["ExSessionPu"] = ex; }
				finally
				{
					if (con.State == System.Data.ConnectionState.Open)
					{
						con.Close();
						con.Dispose();
						cmd.Dispose();
					}
				}

				#endregion
				return RedirectToAction("RegistroMarca");
				//return View("RegistroMarca");
			//});
		}

		//public async Task<ActionResult> GetPub(FormCollection frm)
		//{
		//    string exp = Request.Form["PoderExpediente3"];
		//    string empresa = Request.Form["EmpresasList3"];
		//    string sqlString = "select * from GMC_PUBLICACION_V";
		//    var listPublicaciones = new Models.ListPublicaciones();

		//    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
		//    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

		//    OracleConnection conn = new OracleConnection(authTicket.UserData.ToString());
		//    OracleCommand cmd = new OracleCommand(sqlString, conn);

		//    try
		//    {
		//        conn.Open();
		//        Permisos.OtorgarPermisos(conn);
		//        //OtorgarPermisos(conn);
		//        using (OracleDataReader dr = cmd.ExecuteReader())
		//        {
		//            while (dr.Read())
		//            {
		//                if (dr["EXP_CODIGO"].ToString() == exp)
		//                {
		//                    listPublicaciones.Add(new Models.PublicacionModel(
		//                dr["PUB_FECHA"] != DBNull.Value ? dr["PUB_FECHA"].ToString() : string.Empty,
		//                dr["PER_DESCRIPCION"] != DBNull.Value ? dr["PER_DESCRIPCION"].ToString() : string.Empty,
		//                dr["PUB_DIAS"] != DBNull.Value ? int.Parse(dr["PUB_DIAS"].ToString()) : 0,
		//                dr["PUBLICADO"] != DBNull.Value ? dr["PUBLICADO"].ToString() : string.Empty,
		//                dr["PUB_OBS"] != DBNull.Value ? dr["PUB_OBS"].ToString() : string.Empty));
		//                }

		//            }
		//        }
		//    }
		//    catch (Exception e)
		//    {
		//        throw e;
		//    }
		//    finally
		//    {
		//        if (conn.State != System.Data.ConnectionState.Closed)
		//        {
		//            conn.Close();
		//            conn.Dispose();
		//        }
		//    }
		//    return View("RegistroMarca", listPublicaciones);
		//}

		public ActionResult GetPublicacionHistoria([DataSourceRequest] DataSourceRequest request)
		{
			var listPublicaciones = new Models.ListPublicaciones();

			//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection con = new OracleConnection(cadena);

			string sqlStringHistoria = "select * from gmc_publicacion_v";
			OracleCommand cmdHistoria = new OracleCommand(sqlStringHistoria, con);

			try
			{
				con.Open();
				Permisos.OtorgarPermisos(con);
				using (OracleDataReader dr = cmdHistoria.ExecuteReader())
				{
					while (dr.Read())
					{
						listPublicaciones.Add(new Models.PublicacionModel(
							dr["PUB_CLAVE"] != DBNull.Value ? int.Parse(dr["PUB_CLAVE"].ToString()) : 0,
							dr["PUB_EXPEDIENTE"] != DBNull.Value ? int.Parse(dr["PUB_EXPEDIENTE"].ToString()) : 0,
							DateTime.Parse(dr["PUB_FECHA_PUB"].ToString()),
							dr["PER_DESCRIPCION"] != DBNull.Value ? dr["PER_DESCRIPCION"].ToString() : string.Empty,
							dr["PUB_DIAS"] != DBNull.Value ? int.Parse(dr["PUB_DIAS"].ToString()) : 0,
							//dr["PUBLICADO"] != DBNull.Value ? dr["PUBLICADO"].ToString() : string.Empty,
							dr["PUB_OBS"] != DBNull.Value ? dr["PUB_OBS"].ToString() : string.Empty,
							dr["PUB_DET_CODIGO"] != DBNull.Value ? int.Parse(dr["PUB_DET_CODIGO"].ToString()) : 0
							));
					}

					var query = from c in listPublicaciones
								group c by c.CodExpediente into g
								select new
								{
									g.Key,
									MaxCodigo = g.Max(p => p.PubClave)
								};

					foreach (var item in listPublicaciones)
					{
						if (query.Select(p => p.MaxCodigo).Contains(item.PubClave))
						{
							item.isDeletable = false;
						}
						else
						{
							item.isDeletable = true;
						}
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
				cmdHistoria.Dispose();
			}

			return Json(listPublicaciones.ToDataSourceResult(request));
		}
		//public async Task<ActionResult> GetLogotipo(FormCollection frm , IEnumerable<HttpPostedFileBase> files)
		//{
		//    string ExptList = Request.Form["ExptList"];
		//    string fechaIncioLogotipo = Request.Form["fechaIncioLogotipo"];
		//    string statusLogotipo = Request.Form["statusLogotipo"];
		//    string registroLogotipo = Request.Form["registroLogotipo"];
		//    string registroInicioLogotipo = Request.Form["registroInicioLogotipo"];
		//    string registroVencimientoLogotipo = Request.Form["registroVencimientoLogotipo"];
		//    string desdeLogotipo = Request.Form["desdeLogotipo"];
		//    string descripcionLogotipo = Request.Form["descripcionLogotipo"];
		//    if (files != null)
		//    {
		//        TempData["UploadedFiles"] = GetFileInfo(files);

		//        byte[] bytes = new byte[TempData["UploadedFiles"].ToString().Length * sizeof(char)];
		//        System.Buffer.BlockCopy(TempData["UploadedFiles"].ToString().ToCharArray(), 0, bytes, 0, bytes.Length);
		//        Session["image"] = bytes;
		//    }
		//    //string files = Request.Form["files"];

		//    //string sqlString = "insert into gmc_logotipo_registro(log_cod , log_expediente , " +
		//    //" log_fecha_inicio , log_status , " +
		//    //"  log_registro , log_registro , " +
		//    //"  log_fecha_creacion ,log_fecha_vencimiento , " +
		//    //"   log_fecha_desde , log_archivos ,log_archivos, log_descrpcion) " +
		//    //"   values((SELECT NVL(MAX(log_cod), 0) + 1 FROM gmc_logotipo_registro) ," + int.Parse(ExptList) +","+"'"+fechaIncioLogotipo+"'," + "'"+statusLogotipo + "'," + "'" +registroLogotipo + "'," + "'" +registroInicioLogotipo + "'," + "'" +registroVencimientoLogotipo + "'," + "'" +desdeLogotipo + "'," + Session["image"]+","+"'"+descripcionLogotipo+"'"+")";


		//    //string sqlString = "insert into gmc_logotipo_registro(log_cod,\n" +
		//    //"                                  log_expediente,\n" +
		//    //"                                  log_fecha_inicio,\n" +
		//    //"                                  log_status,\n" +
		//    //"                                  log_registro,\n" +
		//    //"                                  log_fecha_creacion,\n" +
		//    //"                                  log_fecha_vencimiento,\n" +
		//    //"                                  log_fecha_desde,\n" +
		//    //"                                  log_archivos,\n" +
		//    //"                                  log_descrpcion)\n" +
		//    //"values((SELECT NVL(MAX(log_cod), 0) + 1 FROM gmc_logotipo_registro),"+int.Parse(ExptList)+",'"+fechaIncioLogotipo+"','"+statusLogotipo+"','"+registroLogotipo+"','"+
		//    // registroInicioLogotipo+"','"+registroVencimientoLogotipo+"','"+desdeLogotipo+"',"+Session["image"]+",'"+descripcionLogotipo+"')";


		//    string sqlString = "insert into gmc_logotipo_registro(log_cod,\n" +
		//    "log_expediente,\n" +
		//    "log_fecha_inicio,\n" +
		//    "log_status,\n" +
		//    "log_registro,\n" +
		//    "log_fecha_creacion,\n" +
		//    "log_fecha_vencimiento,\n" +
		//    "log_fecha_desde,\n" +
		//    "log_archivos,\n" +
		//    "log_descrpcion)\n" +
		//    "values((SELECT NVL(MAX(log_cod), 0) + 1 FROM gmc_logotipo_registro),\n" +
		//    ":Exp ,\n" +
		//    " :FechaInicio ,\n" +
		//    " :Status ,\n" +
		//    " :Reg,\n" +
		//    " :FechaCreacion ,\n" +
		//    " :FechaVencimiento ,\n" +
		//    " :FechaDesde,\n" +
		//    " :Dato ,\n" +
		//    " :Descripcion)";



		//    //#region HTTP Auto ConnectionString Render
		//    //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
		//    //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
		//    //#endregion

		//    return await Task.Run(() => 
		//    {
		//        #region Conexion 
		//        string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
		//        OracleConnection con = new OracleConnection(cadena);
		//        OracleCommand cmd = new OracleCommand(sqlString, con);
		//        cmd.BindByName = true;
		//        cmd.Parameters.Add(new OracleParameter("Exp", int.Parse(ExptList)));
		//        cmd.Parameters.Add(new OracleParameter("FechaInicio", fechaIncioLogotipo));
		//        cmd.Parameters.Add(new OracleParameter("Status", statusLogotipo));
		//        cmd.Parameters.Add(new OracleParameter("Reg", registroLogotipo));
		//        cmd.Parameters.Add(new OracleParameter("FechaCreacion", registroInicioLogotipo));
		//        cmd.Parameters.Add(new OracleParameter("FechaVencimiento", registroVencimientoLogotipo));
		//        cmd.Parameters.Add(new OracleParameter("FechaDesde", desdeLogotipo));
		//        cmd.Parameters.Add(new OracleParameter("Dato", Session["image"]));
		//        cmd.Parameters.Add(new OracleParameter("Descripcion", descripcionLogotipo));
		//        #endregion

		//        #region Procedimiento
		//        try
		//        {
		//            con.Open();
		//            Permisos.OtorgarPermisos(con);
		//            cmd.ExecuteNonQuery();
		//        }
		//        catch (Exception ex) { Session["qweEx"] = ex; }
		//        finally
		//        {
		//            if (con.State == System.Data.ConnectionState.Open)
		//            {
		//                con.Close();
		//                con.Dispose();
		//            }
		//        }
		//        #endregion

		//        return View("RegistroMarca");
		//    });
		   
		//}
   
		public async Task<ActionResult> GetPatenteInvencion(FormCollection frm , IEnumerable<HttpPostedFileBase> files)
		{
			bool existe = false;
			string numeroSolictudPatente = Request.Form["numeroSolicitudPatente"];
			string qwe = Request.Form["qwe"];
			string datepickerPatente = Request.Form["datepickerPatente"];
			string FechaVencimiento = Request.Form["FechaVencimiento"];
			string tituloPatente = Request.Form["tituloPatente"];
			string AgenteList = Request.Form["AgenteList"];
			string nuevaSolicitudPatente = Request.Form["nuevaSolicitud"];
			string tipoSolicitud = Request.Form["select"];

			string sqlStringSelect = "select * from gmc_patente_invencion where pat_numero_solicitud = :patSolicitud";
			string sqlString = "insert into gmc_patente_invencion(pat_codigo,\n" +
			"pat_numero_solicitud,\n" +
			"pat_solicitante,\n" +
			"pat_fecha_solicitud,\n" +
			"pat_titulo,\n" +
			"pat_agente,\n" +
			"pat_fecha_vencimiento,\n" +
			"pat_image,\n" +
			"pat_tipo_solicitud)\n" +
			"values((SELECT NVL(MAX(pat_codigo), 0) + 1 FROM gmc_patente_invencion),\n" +
			":NumeroSolicitud,:Solicitante,:FechaSolicitud,:Titulo,:Agente,\n" +
			":Vencimiento,:Images,:tipoSolicitud)";

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
			//return await Task.Run(() =>
			//{
			//#region HTTP Auto ConnectionString Render
			//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			//    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			//    #endregion
			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;

			#region Conexion 
			OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand(sqlString, con);
				cmd.BindByName = true;
				cmd.Parameters.Add(new OracleParameter("NumeroSolicitud", string.IsNullOrEmpty(nuevaSolicitudPatente) ? int.Parse(numeroSolictudPatente) : int.Parse(nuevaSolicitudPatente)));
				cmd.Parameters.Add(new OracleParameter("Solicitante", int.Parse(qwe)));
				cmd.Parameters.Add(new OracleParameter("FechaSolicitud",DateTime.Parse(datepickerPatente)));
				cmd.Parameters.Add(new OracleParameter("Titulo",tituloPatente));
				cmd.Parameters.Add(new OracleParameter("Agente", int.Parse(AgenteList)));
				cmd.Parameters.Add(new OracleParameter("Vencimiento", DateTime.Parse(FechaVencimiento)));
				cmd.Parameters.Add(new OracleParameter("Images",OracleDbType.Blob , bytes, System.Data.ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("tipoSolicitud", int.Parse(tipoSolicitud)));

			OracleCommand cmd2 = new OracleCommand(sqlStringSelect, con);
			cmd2.Parameters.Add(new OracleParameter("patSolicitud", string.IsNullOrEmpty(nuevaSolicitudPatente) ? int.Parse(numeroSolictudPatente) : int.Parse(nuevaSolicitudPatente)));
			#endregion

			#region Procedimiento
			try
				{
					con.Open();
					Permisos.OtorgarPermisos(con);
					using (OracleDataReader dr = cmd2.ExecuteReader())
					{
						while (dr.Read())
						{
							existe = true;
							string sqlStringUpdate = "update gmc_patente_invencion set pat_solicitante = :solicitante,\n" +
							"pat_fecha_solicitud = :fechaSolicitud,\n" +
							"pat_titulo = :titulo,\n" +
							"pat_agente = :agente,\n" +
							"pat_fecha_vencimiento = :fechaVencimiento,\n" +
							"pat_image = :image,\n" +
							"pat_tipo_solicitud = :tipo_solicitud\n" +
							"where pat_numero_solicitud = " + numeroSolictudPatente;

							OracleCommand cmdUpdate = new OracleCommand(sqlStringUpdate, con);
							cmdUpdate.Parameters.Add(new OracleParameter("solicitante", int.Parse(qwe)));
							cmdUpdate.Parameters.Add(new OracleParameter("fechaSolicitud", DateTime.Parse(datepickerPatente)));
							cmdUpdate.Parameters.Add(new OracleParameter("titulo", tituloPatente));
							cmdUpdate.Parameters.Add(new OracleParameter("agente", int.Parse(AgenteList)));
							cmdUpdate.Parameters.Add(new OracleParameter("fechaVencimiento", DateTime.Parse(FechaVencimiento)));
							cmdUpdate.Parameters.Add(new OracleParameter("image", OracleDbType.Blob, bytes, System.Data.ParameterDirection.Input));
							cmdUpdate.Parameters.Add(new OracleParameter("tipo_solicitud", int.Parse(tipoSolicitud)));
							cmdUpdate.ExecuteNonQuery();
						}
					}
					if (!existe)
					{
						cmd.ExecuteNonQuery();
					}                   
				}
				catch (Exception ex) { Session["error"] = ex; }
				finally
				{
					if (con.State == System.Data.ConnectionState.Open)
					{
						con.Close();
						con.Dispose();
					}
				}
				#endregion

				return RedirectToAction("PatenteInvencion");
			//});
		}

		public async Task<ActionResult> GetApoderado()
		{
			return await Task.Run(() => 
			{
				string queryRoles = Shared.GestionDataConnect.BusquedaApoderado();

				var list = new Models.ListEmpresaBusqueda();

				//#region HTTP Auto ConnectionString Render
				//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
				//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
				//#endregion

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand(queryRoles, con);
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
						list.Add(new Models.EmpresaBusquedaModel(int.Parse(dr["APO_CODIGO"].ToString()), dr["APO_RAZON_SOCIAL"].ToString()));
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

				//var filteredItems = list.Where(
				//t => t.DescripcionEmpresa == term);
				return Json(list, JsonRequestBehavior.AllowGet);
			});
				
		   
		}

		public async Task<ActionResult> GetApoderadoExp()
		{
			return await Task.Run(() =>
			{
				string sqlString = "select distinct a.apo_codigo, a.apo_razon_social\n" +
				"  from gmc_apoderado a, gmc_expediente e\n" +
				" where e.exp_solicitante = a.apo_codigo";

				var list = new Models.ListEmpresaBusqueda();

				//#region HTTP Auto ConnectionString Render
				//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
				//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
				//#endregion

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand(sqlString, con);
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
						list.Add(new Models.EmpresaBusquedaModel(int.Parse(dr["APO_CODIGO"].ToString()), dr["APO_RAZON_SOCIAL"].ToString()));
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

				//var filteredItems = list.Where(
				//t => t.DescripcionEmpresa == term);
				return Json(list, JsonRequestBehavior.AllowGet);
			});
		}

		public async Task<ActionResult> GetDibujoIndustriales(FormCollection frm)
		{
			string EmpresasList = Request.Form["EmpresasList"];
			string vencimientoRegGen = Request.Form["vencimientoRegGen"];
			string domicilioDibujo = Request.Form["domicilioDibujo"];
			string entradaNumero = Request.Form["entradaNumero"];
			string fechaDibujo = Request.Form["fechaDibujo"];
			string renovacionDibujo = Request.Form["renovacionDibujo"];
			string creadorDibujo = Request.Form["creadorDibujo"];
			string domicilioCreador = Request.Form["domicilioCreador"];
			string tituloCreador = Request.Form["tituloCreador"];
			string ClaseList = Request.Form["ClaseList"];
			string SubClaseList = Request.Form["SubClaseList"];
			string AgenteList = Request.Form["AgenteList"];
			string files = Request.Form["files"];
			string idDescripcion = Request.Form["idDescripcion"];
			string Vencimiento = Request.Form["Vencimiento"];
			//byte files = byte.Parse(Request.Form["files"]);
			return await Task.Run(() => 
			{
				#region query
				string sqlString = "insert into gmc_dibujos_modelos(dib_cod,\n" +
			   "dib_solicitante,\n" +
			   "dib_fecha_orden,\n" +
			   "dib_domicilio,\n" +
			   "dib_entrada_num,\n" +
			   "dib_fecha,\n" +
			   "dib_renovacion_reg,\n" +
			   "dib_creador,\n" +
			   "dib_domicilio_creador,\n" +
			   "dib_titulo,\n" +
			   "dib_clase,\n" +
			   "dib_ed_niza,\n" +
			   "dib_agente,\n" +
			   //"dib_figure,\n" +
			   "dib_vencimiento,\n" +
			   "dib_descrpcion)\n" +
			   "values((SELECT NVL(MAX(dib_cod), 0) + 1 FROM gmc_dibujos_modelos),:Solicitante, :FechaOrden , :Domicilio , :Entrada , :Fecha , :Renovacion,\n" +
			   "  :Creador, :Domicilio , :Titulo , :Clase , :EdNiza , :Agente  ,:Vencimiento , :Descrpcion )";




				#endregion

				//#region HTTP Auto ConnectionString Render
				//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
				//    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
				//#endregion

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
					OracleCommand cmd = new OracleCommand(sqlString, con);
					cmd.BindByName = true;
					cmd.Parameters.Add(new OracleParameter("Solicitante", int.Parse(EmpresasList)));
					cmd.Parameters.Add(new OracleParameter("FechaOrden", vencimientoRegGen));
					cmd.Parameters.Add(new OracleParameter("Domicilio", domicilioDibujo));
					cmd.Parameters.Add(new OracleParameter("Entrada", entradaNumero));
					cmd.Parameters.Add(new OracleParameter("Fecha", fechaDibujo));
					cmd.Parameters.Add(new OracleParameter("Renovacion", renovacionDibujo));
					cmd.Parameters.Add(new OracleParameter("Creador", creadorDibujo));
					cmd.Parameters.Add(new OracleParameter("Domicilio", domicilioCreador));
					cmd.Parameters.Add(new OracleParameter("Titulo", tituloCreador));
					cmd.Parameters.Add(new OracleParameter("Clase", int.Parse(ClaseList)));
					cmd.Parameters.Add(new OracleParameter("EdNiza", int.Parse(SubClaseList)));
					cmd.Parameters.Add(new OracleParameter("Agente", int.Parse(AgenteList)));                 
					//cmd.Parameters.Add(new OracleParameter("Figure", files));
					cmd.Parameters.Add(new OracleParameter("Vencimiento", Vencimiento));
					cmd.Parameters.Add(new OracleParameter("Descrpcion", idDescripcion));


				#endregion

				#region Procedimiento
				try
					{
						con.Open();
					Permisos.OtorgarPermisos(con);
					cmd.ExecuteNonQuery();
					}
					catch (Exception ex ) { Session["ex"] = ex; }
					finally
					{
						if (con.State == System.Data.ConnectionState.Open)
						{
							con.Close();
							con.Dispose();
						}
					}
					#endregion

				return View("DibujoIndustriales");
			   
			});
		}
		
		public async Task<ActionResult> GetClase()
		{
		   return await Task.Run(() => 
			{
				var list = new Models.ClaseList();
				// #region HTTP Auto ConnectionString Render
				//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
				//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
				//#endregion

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand("select * from gmc_tipo_clase", con);
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
						list.Add(new Models.ClaseModels(int.Parse(dr["TICL_COD"].ToString()),dr["TICL_DESC"].ToString()));
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
				return Json(list , JsonRequestBehavior.AllowGet);
			});
			
		}

		//public async Task<ActionResult> GetSubClase(int CodClase)
		//{
		//    return await Task.Run(() =>
		//    {
		//        var list = new Models.EdNizaList();
		//        #region HTTP Auto ConnectionString Render
		//        HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
		//        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
		//        #endregion

		//        #region Conexion 
		//        OracleConnection con = new OracleConnection(authTicket.UserData.ToString());
		//        OracleCommand cmd = new OracleCommand("select * from gmc_ed_niza", con);
		//        //cmd.BindByName = true;
		//        //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));

		//        #endregion

		//        #region Procedimiento
		//        try
		//        {
		//            con.Open();
		//            OracleDataReader dr = cmd.ExecuteReader();
		//            while (dr.Read())
		//            {
		//                list.Add(new Models.EdNizaModels(int.Parse(dr["ED_COD"].ToString()), dr["ED_DESCRIPCION"].ToString(), int.Parse(dr["ED_CLASE"].ToString())));
		//            }
		//        }
		//        catch (Exception) { }
		//        finally
		//        {
		//            if (con.State == System.Data.ConnectionState.Open)
		//            {
		//                con.Close();
		//                con.Dispose();
		//            }
		//        }
		//        #endregion

		//        var TT = list.Where(p => p.CodClase == CodClase);

		//        return Json(TT.Select(Z => new { CodigoRdNiza = Z.CodEdNiza, EdNiza = Z.DescEdNiza }), JsonRequestBehavior.AllowGet);
		//    });
		//}
		//public async Task<ActionResult> GetSubClase2()
		//{
		//    return await Task.Run(() =>
		//    {
		//        var list = new Models.EdNizaList();
		//        #region HTTP Auto ConnectionString Render
		//        HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
		//        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
		//        #endregion

		//        #region Conexion 
		//        OracleConnection con = new OracleConnection(authTicket.UserData.ToString());
		//        OracleCommand cmd = new OracleCommand("select * from gmc_ed_niza", con);
		//        //cmd.BindByName = true;
		//        //cmd.Parameters.Add(new OracleParameter("NombreEmpresa", ));

		//        #endregion

		//        #region Procedimiento
		//        try
		//        {
		//            con.Open();
		//            OracleDataReader dr = cmd.ExecuteReader();
		//            while (dr.Read())
		//            {
		//                list.Add(new Models.EdNizaModels(int.Parse(dr["ED_COD"].ToString()), dr["ED_DESCRIPCION"].ToString(), int.Parse(dr["ED_CLASE"].ToString())));
		//            }
		//        }
		//        catch (Exception) { }
		//        finally
		//        {
		//            if (con.State == System.Data.ConnectionState.Open)
		//            {
		//                con.Close();
		//                con.Dispose();
		//            }
		//        }
		//        #endregion

		//        //var TT = list.Where(p => p.CodClase == CodClase);

		//        return Json(list, JsonRequestBehavior.AllowGet);
		//    });
		//}

		public async Task<ActionResult> GetPeriodico()
		{
			return await Task.Run(() =>
			{
				var list = new Models.ListPeriodicos();

				//#region HTTP Auto ConnectionString Render
				//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
				//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
				//#endregion

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand("select * from gmc_periodico", con);
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
						list.Add(new Models.PeriodicoModel(int.Parse(dr["PER_CODIGO"].ToString()), dr["PER_DESCRIPCION"].ToString()));
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
		public async Task<ActionResult> GetMarcas()
		{
			return await Task.Run(() =>
			{
				var listMarcas = new Models.ListMarca();

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand("select * from gmc_marca m where m.marc_logotipo is not null", con);


				#endregion

				#region Procedimiento
				try
				{
					con.Open();
					Permisos.OtorgarPermisos(con);
					OracleDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						listMarcas.Add(new Models.MarcaModel(int.Parse(dr["MARC_CODIGO"].ToString()), dr["MARC_DESC"].ToString()));
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
				return Json(listMarcas, JsonRequestBehavior.AllowGet);
			});
		}

		//Acción que muestra la marca de acuerdo al poder con el cual está relacionado.
		public async Task<ActionResult> GetMarca(int CodPod)
		{
			//return await Task.Run(() =>
			//{
				var list = new Models.ListMarca();

			//string sqlString = "select *\n" +
			//"  from gmc_marca m\n" +
			//" where m.marc_codigo not in\n" +
			//"       (select e.exp_marca\n" +
			//"          from gmc_expediente e\n" +
			//"         where e.exp_marca = m.marc_codigo)";

			string sqlString = "select * from gmc_marca m";

				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand(sqlString, con);

				#region Procedimiento
				try
				{
					con.Open();
					Permisos.OtorgarPermisos(con);
					OracleDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						list.Add(new Models.MarcaModel(
							int.Parse(dr["MARC_CODIGO"].ToString()),
							dr["MARC_DESC"].ToString(),
							int.Parse(dr["MARC_PODER"].ToString())));
							//int.Parse(dr["MARC_CLASE"].ToString()), 
							//int.Parse(dr["MARC_EDNIZA"].ToString())));
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
				var TT = list.Where(p => p.Poder == CodPod);
				return Json(TT.Select(Z => new { MarcaCodigo = Z.MarcaCodigo , MarcaDescripcion = Z.MarcaDescripcion}), JsonRequestBehavior.AllowGet);
			//});
		}

		
		public async Task<ActionResult> GetMarcaParecido()
		{
			return await Task.Run(() =>
			{
				var list = new Models.ListMarcaParecido();

				//string sqlString = "select distinct mp.parecido_marca_cod, m.marc_desc from gmc_marca_parecido mp, gmc_marca m\n" +
				//"where mp.parecido_marca_cod = m.marc_codigo";


				string sqlString = "select distinct mp.parecido_marca_cod, m.marc_desc from gmc_marca_parecido mp, gmc_marca m\n" +
				"                where mp.parecido_marca_cod = m.marc_codigo\n" +
				"                and mp.parecido_marca_cod not in(select e.email_marca_par_cod from gmc_datos_email e)\n" +
				"";

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand(sqlString, con);

				#endregion

				#region Procedimiento
				try
				{
					con.Open();
					Permisos.OtorgarPermisos(con);
					OracleDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						list.Add(new Models.MarcaParecido(
							int.Parse(dr["PARECIDO_MARCA_COD"].ToString()),
							dr["MARC_DESC"].ToString()));
					}
				}
				catch (Exception ex) { Session["ex"] = ex; }
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

		//Acción que muestra en el DropDownList las marcas parecidas que fueron enviadas por email al cliente.
		public async Task<ActionResult> GetMarcaParecidoEmail()
		{
			return await Task.Run(() =>
			{
				var list = new Models.ListMarcaParecido();

				//string sqlString = "select distinct de.email_marca_par_cod, m.marc_desc\n" +
				//"  from gmc_datos_email de, gmc_marca m\n" +
				//" where de.email_marca_par_cod = m.marc_codigo\n" +
				//"   and de.email_marca_par_cod not in\n" +
				//"       (select o.oposicion_marca_cod from gmc_oposicion o)";

				/*Select para obtener todos los titulares, excepto aquellos que se encuentran en la tabla GMC_OPOSICION.
				Eso quiere decir que si se encuentra algún titular en esa tabla, entonces ya fue registrada la oposicion o no 
				a ciertas marcas por parte de ese titular, para así no volver a registrar la oposición del mismo.*/
				string sqlString = "select distinct de.email_titular_cod, a.apo_razon_social\n" +
				"  from gmc_datos_email de, gmc_apoderado a\n" +
				" where de.email_titular_cod = a.apo_codigo\n" +
				"   and de.email_titular_cod not in\n" +
				"       (select o.oposicion_titular_cod from gmc_oposicion o)";


				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand(sqlString, con);
				#endregion

				#region Procedimiento
				try
				{
					con.Open();
					Permisos.OtorgarPermisos(con);
					OracleDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						list.Add(new Models.MarcaParecido(
							//int.Parse(dr["EMAIL_MARCA_PAR_COD"].ToString()),
							//dr["MARC_DESC"].ToString()));
					int.Parse(dr["EMAIL_TITULAR_COD"].ToString()),
						dr["APO_RAZON_SOCIAL"].ToString()));
		}
				}
				catch (Exception ex) { Session["ex"] = ex; }
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

		//Acción que muestra en un DropDownList los expedientes relacionados a una empresa específica.
		public async Task<ActionResult> GetExpPoder(int CodEmpresa)
		{
			return await Task.Run(() =>
			{
				var list = new Models.ExpedienteList();

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand("select e.*, p.pod_descripcion from gmc_expediente e, gmc_poder p where e.exp_poder = p.pod_codigo", con);
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
						list.Add(new ExpedienteModels(int.Parse(dr["EXP_CODIGO"].ToString()),
							int.Parse(dr["EXP_ESTADO"].ToString()),
							int.Parse(dr["EXP_AGENTE"].ToString()),
							dr["EXP_FECHA_SOLICITUD"].ToString(),
							//dr["EXP_FECHA_ESTADO"].ToString(),
							int.Parse(dr["EXP_TIPO_SOL"].ToString()),
							int.Parse(dr["EXP_SOLICITANTE"].ToString()),
							int.Parse(dr["EXP_PAIS"].ToString()),
							int.Parse(dr["EXP_PODER"].ToString()),
							int.Parse(dr["EXP_MARCA"].ToString()),
							dr["POD_DESCRIPCION"].ToString(),
							dr["EXP_FECHA_VENCIMIENTO"].ToString()
							));
					}
				}
				catch (Exception ex) { Session["gestionEx"] = ex; }
				finally
				{
					if (con.State == System.Data.ConnectionState.Open)
					{
						con.Close();
						con.Dispose();
					}
				}
				#endregion

				var TT = list.Where(p => p.CodSolicitante == CodEmpresa);

				return Json(TT.Select(Z => new { CodExp = Z.CodExpediente, DescExp = (Z.CodExpediente +" - "+ Z.FechaSolicitud).ToString()}), JsonRequestBehavior.AllowGet);
			});
		}
		public async Task<ActionResult> GetFechaExp(int CodEmp)
		{
			return await Task.Run(() =>
			{
				var list = new Models.ExpedienteList();

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand("select e.*, p.pod_descripcion from gmc_expediente e, gmc_poder p where e.exp_poder = p.pod_codigo", con);
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
						list.Add(new ExpedienteModels(int.Parse(dr["EXP_CODIGO"].ToString()),
							int.Parse(dr["EXP_ESTADO"].ToString()),
							int.Parse(dr["EXP_AGENTE"].ToString()),
							dr["EXP_FECHA_SOLICITUD"].ToString(),
							//dr["EXP_FECHA_ESTADO"].ToString(),
							int.Parse(dr["EXP_TIPO_SOL"].ToString()),
							int.Parse(dr["EXP_SOLICITANTE"].ToString()),
							int.Parse(dr["EXP_PAIS"].ToString()),
							int.Parse(dr["EXP_PODER"].ToString()),
							int.Parse(dr["EXP_MARCA"].ToString()),
							dr["POD_DESCRIPCION"].ToString(),
							dr["EXP_FECHA_VENCIMIENTO"].ToString()
							));
					}
				}
				catch (Exception ex) { Session["gestionEx"] = ex; }
				finally
				{
					if (con.State == System.Data.ConnectionState.Open)
					{
						con.Close();
						con.Dispose();
					}
				}
				#endregion

				var TT = list.Where(p => p.CodSolicitante == CodEmp);

				return Json(TT.Select(Z => new { CodExp = Z.CodExpediente, fechaExp = Z.FechaSolicitud }), JsonRequestBehavior.AllowGet);
			});
		}

		//Acción que muestra los tipos de marcas en un ComboBox.
		public async Task<ActionResult> GetTipoMarca()
		{
			return await Task.Run(() =>
			{
				var list = new Models.ListTipoMarca();

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand("select * from gmc_tipo_marca", con);
				#endregion

				#region Procedimiento
				try
				{
					con.Open();
					Permisos.OtorgarPermisos(con);
					OracleDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						list.Add(new Models.TipoMarcaModel(
							int.Parse(dr["TIPO_MARCA_COD"].ToString()),
							dr["TIPO_MARCA_DESC"].ToString()));
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

		//Acción que muestra los países en un ComboBox.
		public async Task<ActionResult> GetPais()
		{
			return await Task.Run(() =>
			{
				var list = new Models.ListPaises();

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand("select * from gen_pais", con);
				#endregion

				#region Procedimiento
				try
				{
					con.Open();
					Permisos.OtorgarPermisos(con);
					OracleDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						list.Add(new Models.PaisModel(
							int.Parse(dr["PAIS_CODIGO"].ToString()),
							dr["PAIS_DESC"].ToString()));
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

		//Acción que muestra los tipos de solicitudes en un ComboBox.
		public async Task<ActionResult> GetTipoSolicitud()
		{
			return await Task.Run(() =>
			{
				var list = new Models.ListTipoSol();

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand("select * from gmc_tipo_solicitud", con);
				#endregion

				#region Procedimiento
				try
				{
					con.Open();
				Permisos.OtorgarPermisos(con);
				OracleDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						list.Add(new Models.TipoSolicitudModel(
							int.Parse(dr["TSOL_CODIGO"].ToString()),
							dr["TSOL_DESC"].ToString(),
							dr["TSOL_ABREV"].ToString()
							));
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

		//Acción que muestra los tipos de personas(Física o Jurídica) en un ComboBox.
		public async Task<ActionResult> GetTipoPersona()
		{
			return await Task.Run(() =>
			{
				var list = new Models.ListTipoPers();

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand("select * from gen_tipo_persona", con);
				#endregion

				#region Procedimiento
				try
				{
					con.Open();
					Permisos.OtorgarPermisos(con);
					OracleDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						list.Add(new Models.TipoPersonaModel(
							int.Parse(dr["GEN_TIPER_CODIGO"].ToString()),
							dr["GEN_TIPER_DESC"].ToString()));
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


		public async Task<ActionResult> GetEdNizaMnt()
			{
			return await Task.Run(() =>
			{
				var list = new Models.EdNizaList();
				//#region HTTP Auto ConnectionString Render
				//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
				//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
				//#endregion

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand("select * from gmc_edniza", con);
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
						list.Add(new Models.EdNizaModels(
							int.Parse(dr["NIZ_COD"].ToString()), 
							dr["NIZ_DESCRIPCION"].ToString(), 
							int.Parse(dr["NIZ_TIPO"].ToString()),
							int.Parse(dr["NIZ_NRO_CLASE"].ToString())));
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

			  

				return Json(list , JsonRequestBehavior.AllowGet);
			});



		}
		public async Task<ActionResult> GetEdNizaMnt2(int? clases, string edNizaFilter)
		{
			var list = new Models.EdNizaList();
			var lista = new List<EdNizaModels>();
			//#region HTTP Auto ConnectionString Render
			//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			//#endregion

			#region Conexion 
			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection con = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand("select * from gmc_edniza", con);
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
					list.Add(new Models.EdNizaModels(
						int.Parse(dr["NIZ_COD"].ToString()),
						dr["NIZ_DESCRIPCION"].ToString(),
						int.Parse(dr["NIZ_TIPO"].ToString()),
						int.Parse(dr["NIZ_NRO_CLASE"].ToString())));
				}
				
				if (clases != null)
				{
					lista = list.Where(l => l.CodClase == clases).ToList();
				}

				if (!string.IsNullOrEmpty(edNizaFilter))
				{
					lista = list.Where(l => l.DescEdNiza.Contains(edNizaFilter)).ToList();
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



			return Json(lista.Select(p => new { CodEdNiza = p.CodEdNiza, DescEdNiza = p.DescEdNiza }), JsonRequestBehavior.AllowGet);

		}
		public async Task<ActionResult> GetEdNizaMnt3(int? clases, string edClaseFilter)
		{
			var list = new Models.EdNizaList();
			var lista = new List<EdNizaModels>();
			//#region HTTP Auto ConnectionString Render
			//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			//#endregion

			#region Conexion 
			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			string sqlString = "select distinct niz_nro_clase from gmc_edniza where niz_tipo = " + clases + " order by 1";
			OracleConnection con = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand(sqlString, con);
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
					list.Add(new Models.EdNizaModels(
						int.Parse(dr["NIZ_NRO_CLASE"].ToString())));
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
		}

		//Acción que obtiene la descripción de EdNiza para mostrar en un comboBox.
		public async Task<ActionResult> GetEdNizaDescMnt(int? clases, string edClaseFilter)
		{
			return await Task.Run(() =>
			{
				var list = new Models.EdNizaList();
				var lista = new List<EdNizaModels>();

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				//string sqlString = "select distinct niz_nro_clase from gmc_edniza where niz_tipo = " + clases + " order by 1";
				string sqlString = "select g.niz_descripcion from gmc_edniza  g where g.niz_nro_clase = " + clases;

				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand(sqlString, con);
				#endregion

				#region Procedimiento
				try
				{
					con.Open();
					Permisos.OtorgarPermisos(con);
					OracleDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						list.Add(new Models.EdNizaModels(
							dr["NIZ_DESCRIPCION"].ToString()));
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

		//Acción que obtiene los Agentes.
		public async Task<ActionResult> GetAgente()
		{
			return await Task.Run(() => 
			{
				var list = new Models.AgenteList();

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand("select * from gmc_agente", con);
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
				return Json(list , JsonRequestBehavior.AllowGet);
			});
		}

		//Acción que obtiene los Agentes en un DropDownList relacionados con un poder.
		public async Task<ActionResult> GetAgentePoder(int CodPod)
		{
			return await Task.Run(() =>
			{
				var list = new Models.AgenteList();

				string sqlString = "select a.age_cod, a.age_nombre, p.pod_codigo\n" +
				"  from gmc_agente a, gmc_poder_agente pa, gmc_poder p\n" +
				" where a.age_cod = pa.podage_agente\n" +
				"   and p.pod_codigo = pa.podage_poder";

				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand(sqlString, con);

				try
				{
					con.Open();
					Permisos.OtorgarPermisos(con);
					OracleDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						list.Add(new Models.AgenteModels(
							int.Parse(dr["AGE_COD"].ToString()), 
							dr["AGE_NOMBRE"].ToString(),
							int.Parse(dr["POD_CODIGO"].ToString())));
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
				var TT = list.Where(p => p.CodPoder == CodPod);
				return Json(TT.Select(Z => new { CodAgente = Z.CodAgente, DescAgente = Z.DescAgente }), JsonRequestBehavior.AllowGet);
			});
		}

		//Acción que obtiene los poderes en un DropDownList.
		public async Task<ActionResult> GetPoder()
		{
			return await Task.Run(() =>
			{
				var list = new Models.ListPoder();

				//#region HTTP Auto ConnectionString Render
				//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
				//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
				//#endregion

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand("select * from gmc_poder", con);
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
						list.Add(new Models.MntPoder(
							dr["POD_DESCRIPCION"].ToString(),
							int.Parse(dr["POD_CODIGO"].ToString()),
							dr["POD_FECHA_CREACION"].ToString(),
							int.Parse(dr["POD_APODERADO"].ToString()),
							dr["POD_OBS"].ToString(),
							dr["POD_FIRMANTE"].ToString()
							));
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
				return Json(list.Select(Z => new { Poder = Z.Poder, DescPoder = (Z.Poder + " - " + Z.DescPoder).ToString() }), JsonRequestBehavior.AllowGet);
			});
		}

		//Acción que obtiene los poderes en un DropDownList relacionados con un titular.
		public async Task<ActionResult> GetPoderTitular(int CodEmp)
		{
			return await Task.Run(() =>
			{
				var list = new Models.ListPoder();

				//#region HTTP Auto ConnectionString Render
				//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
				//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
				//#endregion

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand("select * from gmc_poder", con);
				#endregion

				#region Procedimiento
				try
				{
					con.Open();
				Permisos.OtorgarPermisos(con);
				OracleDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						list.Add(new Models.MntPoder(
							dr["POD_DESCRIPCION"].ToString(),
							int.Parse(dr["POD_CODIGO"].ToString()),                            
							dr["POD_FECHA_CREACION"].ToString(),
							int.Parse(dr["POD_APODERADO"].ToString()),
							dr["POD_OBS"].ToString(),
							dr["POD_FIRMANTE"].ToString()
							));
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
				var TT = list.Where(p => p.CodApoderado == CodEmp);
				#endregion
				return Json(TT.Select(Z => new { Poder = Z.Poder, DescPoder = (Z.Poder +" - "+ Z.DescPoder).ToString()}), JsonRequestBehavior.AllowGet);
			});
		}
		public async Task<ActionResult> GetTitularAOponerse(int CodTit)
		{
			return await Task.Run(() =>
			{
				var list = new Models.ListTitulares();

				//#region HTTP Auto ConnectionString Render
				//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
				//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
				//#endregion

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand("select * from gmc_marca_parecido", con);
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
						list.Add(new Models.TitularAOponerse(
							int.Parse(dr["PARECIDO_CODIGO"].ToString()),
							dr["PARECIDO_TITULAR"].ToString(),
							int.Parse(dr["PARECIDO_MARCA_COD"].ToString())
							));
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
				var TT = list.Where(p => p.CodMarcaParecido == CodTit);
				#endregion
				return Json(TT.Select(Z => new { NombreTitular = Z.NombreTitular }), JsonRequestBehavior.AllowGet);
			});
		}
		public async Task<ActionResult> GetTitular()
		{
			return await Task.Run(() =>
			{
				var list = new Models.ListEmpresaBusqueda();

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				//string sqlString = "select distinct t.apti_titular, e.apo_razon_social\n" +
				//"from GMC_APODERADO e inner join GMC_APODERADO_TITULAR t\n" +
				//"     on e.apo_codigo = t.apti_titular";

				string sqlString = "select a.apo_codigo, a.apo_razon_social from gmc_apoderado a where a.apo_titulares is null";

				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand(sqlString, con);

				#endregion

				#region Procedimiento
				try
				{
					con.Open();
					Permisos.OtorgarPermisos(con);
					OracleDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						list.Add(new Models.EmpresaBusquedaModel(
							int.Parse(dr["APO_CODIGO"].ToString()), 
							dr["APO_RAZON_SOCIAL"].ToString()));
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

		//Acción que obtiene los estados de un expediente.
		public async Task<ActionResult> GetStatus()
		{
			return await Task.Run(() =>
			{
				var list = new Models.StatusList();

				#region Conexion 
				string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
				OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand("select * from gmc_estado", con);
				#endregion

				#region Procedimiento
				try
				{
					con.Open();
					Permisos.OtorgarPermisos(con);
					OracleDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						list.Add(new Models.StatusModel(int.Parse(dr["EST_CODIGO"].ToString()), dr["EST_DESC"].ToString()));
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

		//Acción que que obtiene y muestra en una grilla la historia y movimiento de una marca o expediente.
		public ActionResult ToolbarTemplate_Read([DataSourceRequest] DataSourceRequest request, string param1)
		{
			var listExpHistoria = new Models.ListPublicaciones();
			Models.PublicacionModel expHistoria = new Models.PublicacionModel();

			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection con = new OracleConnection(cadena);
			
			OracleCommand cmdHistoria = new OracleCommand();

			//Busca en la consulta el valor de param1, ingresado en el campo de texto, para filtrar la grilla solo por ese valor.
			//Si no muestra la grilla completa.
			if (!string.IsNullOrEmpty(param1))
				cmdHistoria = new OracleCommand("select * from gmc_hist_exp_v where hist_exp_cod = " + param1, con);
			else
				cmdHistoria = new OracleCommand("select * from gmc_hist_exp_v", con);
			try
			{
				con.Open();
				Permisos.OtorgarPermisos(con);
				using (OracleDataReader dr2 = cmdHistoria.ExecuteReader())
				{
					while (dr2.Read())
					{

						listExpHistoria.Add(new Models.PublicacionModel(
							int.Parse(dr2["HIST_COD"].ToString()),
							int.Parse(dr2["HIST_EXP_COD"].ToString()),
							DateTime.Parse(dr2["HIST_EXP_FECHA"].ToString()),
							dr2["TMOV_DESC"].ToString(),
							dr2["EST_DESC"].ToString(),
							dr2["HIST_OBSERVACION"].ToString(),
							dr2["HIST_INSTITUCION"].ToString()
							));


					}
					var query = from c in listExpHistoria
								group c by c.CodExpediente into g
								select new
								{
									g.Key,
									MaxCodigo = g.Max(p => p.CodHistoriaExpediente)
								};

					foreach(var item in listExpHistoria)
					{
						if (query.Select(p => p.MaxCodigo).Contains(item.CodHistoriaExpediente))
						{
							item.isDeletable = false;
						}
						else
						{
							item.isDeletable = true;
						}
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
				cmdHistoria.Dispose();
			}
			
			return Json(listExpHistoria.ToDataSourceResult(request));
		}

		//Acción que mustra en una grilla las marcas parecidas a las cuales el titular desea oponerse.
		public ActionResult MarcaParecido_Read([DataSourceRequest] DataSourceRequest request)
		{

			//string sqlString = "select * from gmc_marca_parecido";

			//string sqlString = "select distinct mp.parecido_codigo,\n" +
			//"       mp.parecido_marca_cod,\n" +
			//"       mp.parecido_marca_den,\n" +
			//"       mp.parecido_titular,\n" +
			//"       mp.parecido_porc_similitud,\n" +
			//"       mp.parecido_porc_error,\n" +
			//"       mp.parecido_inten_opos\n" +
			//"  from gmc_datos_email de, gmc_marca m, gmc_marca_parecido mp\n" +
			//" where de.email_marca_par_cod = m.marc_codigo\n" +
			//"   and mp.parecido_marca_cod = de.email_marca_par_cod\n" +
			//"   and de.email_marca_par_cod not in\n" +
			//"       (select o.oposicion_marca_cod from gmc_oposicion o)";

			string sqlString = "select distinct mp.parecido_codigo,\n" +
			"                mp.parecido_marca_cod,\n" +
			"                mp.parecido_marca_den,\n" +
			"                mp.parecido_titular,\n" +
			"                mp.parecido_porc_similitud,\n" +
			"                mp.parecido_porc_error,\n" +
			"                mp.parecido_inten_opos,\n" +
			"                de.email_titular_cod\n" +
			"  from gmc_datos_email de, gmc_marca m, gmc_marca_parecido mp\n" +
			" where de.email_marca_par_cod = m.marc_codigo\n" +
			"   and mp.parecido_marca_cod = de.email_marca_par_cod\n" +
			"   and de.email_marca_par_cod not in\n" +
			"       (select o.oposicion_marca_cod from gmc_oposicion o)\n" +
			"   and mp.parecido_codigo in\n" +
			"       (select demail.email_marca_cod from gmc_datos_email demail)";



			var list = new Models.ListMarcaParecido();
			MarcaParecido marca = new MarcaParecido();

			//#region HTTP Auto ConnectionString Render
			//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			//#endregion

			#region Conexion 
			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection con = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand(sqlString, con);

			#endregion

			try
			{
				con.Open();
				OracleDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					list.Add(new Models.MarcaParecido(
						marca.ParecidoCodigo = int.Parse(dr["PARECIDO_CODIGO"].ToString()),
						marca.CodMarca = int.Parse(dr["PARECIDO_MARCA_COD"].ToString()),
						marca.DescMarca = dr["PARECIDO_MARCA_DEN"].ToString(),
						marca.DescTitular = dr["PARECIDO_TITULAR"].ToString(),
						marca.PorcentajeSimilitud = int.Parse(dr["PARECIDO_PORC_SIMILITUD"].ToString()),
						marca.PorcentajeError = int.Parse(dr["PARECIDO_PORC_ERROR"].ToString()),
						marca.IntencionOposicion = dr["PARECIDO_INTEN_OPOS"] != DBNull.Value ? int.Parse(dr["PARECIDO_INTEN_OPOS"].ToString()) : 0,
						marca.CodTitular = int.Parse(dr["EMAIL_TITULAR_COD"].ToString())
						));
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

		//Acción que obtiene el expediente en un DropDownList para de acuerdo a ese expediente poder filtrar en la grilla.
		public async Task<ActionResult> GetExpediente()
		{
			var list = new Models.ExpedienteList();

			//#region HTTP Auto ConnectionString Render
			//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			//#endregion



			string sqlString = "select e.exp_codigo||' - '||m.marc_desc expediente_marca from gmc_expediente e, gmc_marca m\n" +
			"where e.exp_marca = m.marc_codigo";


			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection con = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand(sqlString, con);
			//OracleCommand cmd = new OracleCommand("select * from gmc_expediente", con);


			#region Procedimiento
			try
			{
				con.Open();
				Permisos.OtorgarPermisos(con);
				OracleDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					list.Add(new Models.ExpedienteModels(
						dr["EXPEDIENTE_MARCA"].ToString()
						));
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
		}


		//Acción que obtiene los datos de una publicación(en caso de que haya una) de un expediente y retorna a la vista Index.
		[HttpPost]
		public JsonResult GetInfoCalendario()
		{

			//string sqlString = "select * from gmc_publicacion";

			string sqlString = "select p.pub_expediente,\n" +
			"       trunc(det.pub_fecha_pub) fecha,\n" +
			"       p.pub_periodico,\n" +
			"       p.pub_dias,\n" +
			"       det.pub_obs,\n" +
			"       det.pub_publicado\n" +
			"  from gmc_publicacion_detalle det, gmc_publicacion p\n" +
			" where det.pub_clave = p.pub_clave\n" +
			"   and (trunc(det.pub_fecha_pub) <= trunc(sysdate))\n" +
			"   and (det.pub_publicado is null and\n" +
			"       trunc(det.pub_fecha_pub) <= trunc(sysdate))";

			var list = new Models.InfoCalendarioList();

			#region Conexion 
			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection con = new OracleConnection(cadena);
				OracleCommand cmd = new OracleCommand(sqlString, con);


				#endregion

				#region Procedimiento
				try
				{
					con.Open();
					Permisos.OtorgarPermisos(con);
					OracleDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						if(string.IsNullOrEmpty(dr["PUB_PUBLICADO"].ToString()))
						{
							list.Add(new InfoCalendario(
							int.Parse(dr["PUB_EXPEDIENTE"].ToString()),
							String.Format("{0:dd/MM/yyyy}",dr["FECHA"]),
							int.Parse(dr["PUB_PERIODICO"].ToString()),
							dr["PUB_DIAS"] != DBNull.Value ? int.Parse(dr["PUB_DIAS"].ToString()) : 0,
							dr["PUB_PUBLICADO"] != DBNull.Value ? int.Parse(dr["PUB_PUBLICADO"].ToString()) : 0,
							dr["PUB_OBS"] != DBNull.Value ? dr["PUB_OBS"].ToString() : string.Empty));
						}

					}
				}
				catch (Exception ex) { Session["exepcion"] = ex; }
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
		}

		//Acción que obtiene los email sin retorno para mostrar como una alerta en la vista Index.
		[HttpPost]
		public JsonResult ObtenerEmailSinRetorno()
		{
			var list = new Models.listOposiciones();

			//string sqlString = "select distinct a.apo_razon_social, m.marc_desc, de.email_codigo\n" +
			//"from gmc_apoderado a, gmc_poder p, gmc_marca m, gmc_datos_email de\n" +
			//"where a.apo_codigo = p.pod_apoderado\n" +
			//"and p.pod_codigo = m.marc_poder\n" +
			//"and de.email_marca_par_cod = m.marc_codigo";



			string sqlString = "select a.apo_razon_social, MAX(de.email_codigo) email_codigo\n" +
			"  from gmc_apoderado a, gmc_poder p, gmc_marca m, gmc_datos_email de\n" +
			" where a.apo_codigo = p.pod_apoderado\n" +
			"   and p.pod_codigo = m.marc_poder\n" +
			"   and de.email_marca_par_cod = m.marc_codigo\n" +
			" group by a.apo_razon_social";


			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection con = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand(sqlString, con);
			try
			{
				con.Open();
				Permisos.OtorgarPermisos(con);
				using (OracleDataReader dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						OracleCommand cmdProcedure = new OracleCommand();
						cmdProcedure.Connection = con;
						cmdProcedure.CommandType = System.Data.CommandType.StoredProcedure;
						cmdProcedure.CommandText = "GMC_ALERTA_OPOSICION";
						cmdProcedure.Parameters.Add("v_number", OracleDbType.Int32).Value = int.Parse(dr["EMAIL_CODIGO"].ToString());
						cmdProcedure.Parameters.Add("v_out", OracleDbType.Int32, ParameterDirection.Output);
						cmdProcedure.ExecuteNonQuery();
						var valor = cmdProcedure.Parameters["v_out"].Value;
						//Indica que si el valor es igual a 1 entonces significa que no hubo respuesta del cliente para oponerse de acuerdo al día máximo configurado.
						if(int.Parse(valor.ToString()) == 1)
						{
							list.Add(new DatosOposicion(
								int.Parse(dr["EMAIL_CODIGO"].ToString()),
								//dr["MARC_DESC"].ToString(),
								dr["APO_RAZON_SOCIAL"].ToString()
								));
						}      
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
				cmd.Dispose();
			}
			return Json(list);           
		}


		//Acción que retorna el indicador de si el estado está en estado concedido o no
		public JsonResult NuevoMetodo(string Estado)
		{
			var listEstados = new StatusList();
			string sqlString = "select e.est_ind_marc_conced from gmc_estado e where e.est_codigo = "+ Estado;
			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection con = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand(sqlString, con);

			try
			{
				con.Open();
				Permisos.OtorgarPermisos(con);
				using (OracleDataReader dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{

						listEstados.Add(new StatusModel(
								dr["EST_IND_MARC_CONCED"].ToString()
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
				cmd.Dispose();
			}
			return Json(listEstados);
		}

		/*Acción que retorna los datos de un expediente en la vista MovimientoExpediente, de acuerdo al código de expediente
		recibido como parámetro.*/
		public JsonResult CargarExpMov(string Expediente)
		{
			int exp = (Expediente != "") ? int.Parse(Expediente) : 0;

			string sqlString = "select apo.apo_razon_social,\n" +
			"       e.exp_poder || ' - ' || p.pod_descripcion poder,\n" +
			"       est.est_desc,\n" +
			"       e.exp_estado,\n" +
			"       e.exp_fecha_solicitud,\n" +
			"       m.marc_desc,\n" +
			"       a.age_nombre,\n" +
			"       e.exp_nro_registro,\n" +
			"       e.exp_fecha_vencimiento,\n" +
			"       e.exp_fecha_solicitud,\n" +
			"       tmov.tmov_codigo,\n" +
			"       tmov.tmov_desc,\n" +
			"       hist.hist_observacion,\n" +
			"       hist.hist_institucion" +
			"  from gmc_expediente e,\n" +
			"       gmc_estado est,\n" +
			"       gmc_agente a,\n" +
			"       gmc_marca m,\n" +
			"       gmc_apoderado apo,\n" +
			"       gmc_poder p,\n" +
			"       gmc_tipo_movimiento tmov,\n" +
			"       (select hist_cod, he.hist_observacion, he.hist_institucion\n" +
			"  from gmc_historia_exp he\n" +
			" where he.hist_exp_cod =\n" + exp +
			"   and he.hist_cod =\n" +
			"       (select max(hist_cod)\n" +
			"          from gmc_historia_exp a\n" +
			"         where he.hist_exp_cod = a.hist_exp_cod)) hist\n" +
			" where e.exp_estado = est.est_codigo\n" +
			"   and e.exp_agente = a.age_cod\n" +
			"   and e.exp_marca = m.marc_codigo\n" +
			"   and e.exp_solicitante = apo.apo_codigo\n" +
			"   and e.exp_poder = p.pod_codigo\n" +
			"   and e.exp_tipo_mov = tmov.tmov_codigo(+)\n" +
			"   and e.exp_codigo = "+ exp;

			var list = new Models.ExpedienteList();

			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection con = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand(sqlString, con);

			Models.ExpedienteModels exped = new Models.ExpedienteModels();

			#region Procedimiento
			try
			{
				con.Open();
				Permisos.OtorgarPermisos(con);
				OracleDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					//list.Add(new Models.ExpedienteModels(
					exped.Solicitante = dr["APO_RAZON_SOCIAL"].ToString();
					exped.DescPoder = dr["PODER"].ToString();
					exped.DescEstado = dr["EST_DESC"].ToString();
					exped.CodEstado = int.Parse(dr["EXP_ESTADO"].ToString());
					exped.FechaSolicitudActual = dr["EXP_FECHA_SOLICITUD"].ToString();
					exped.DescMarca = dr["MARC_DESC"].ToString();
					exped.DescAgente = dr["AGE_NOMBRE"].ToString();
					exped.CodRegistro = dr["EXP_NRO_REGISTRO"] != DBNull.Value ? int.Parse(dr["EXP_NRO_REGISTRO"].ToString()) : (int?)null;
					exped.FechaVencimiento = dr["EXP_FECHA_VENCIMIENTO"].ToString();
					exped.FechaSolicitud = dr["EXP_FECHA_SOLICITUD"].ToString();
					exped.TipoMovimiento = dr["TMOV_CODIGO"] != DBNull.Value ? int.Parse(dr["TMOV_CODIGO"].ToString()) : 0;
					exped.DescTipoMov = dr["TMOV_DESC"] != DBNull.Value ? dr["TMOV_DESC"].ToString() : string.Empty;
					exped.Observacion = dr["HIST_OBSERVACION"].ToString();
					exped.Institucion = dr["HIST_INSTITUCION"].ToString();
					list.Add(exped);
					//));
				}
				if (list != null)
				{
					Session["frontMessage"] = 1;
				}
				else
				{
					Session["frontMessage"] = 2;
				}
			}
			catch (Exception ex) { Session["exepcion"] = ex; }
			finally
			{
				if (con.State == System.Data.ConnectionState.Open)
				{
					con.Close();
					con.Dispose();
				}
			}
			#endregion
			Session["expediente"] = exp;
			//Session["fechaExpediente"] = list[0].FechaSolicitud;
			return Json(list);
		}

		//Acción que actualiza los datos de un expediente de acuerdo a los datos recibidos como parámetros.
		public JsonResult ActualizarEstadoExp(string Expediente, string Tmov, string Estado, string Observacion, 
			string Fecha, string registro, string fechaVen, string FechaSol, string FechaSolAgencia, string ObservacionAgencia, string Institucion)
		{

			int exp = (Expediente != "") ? int.Parse(Expediente) : 0;
			int tipoMov = (Tmov != "") ? int.Parse(Tmov) : 0;
			int? est = string.IsNullOrEmpty(Estado) ? (int?)null : int.Parse(Estado);
			//int est = (Estado != "") ? int.Parse(Estado) : 0;
			int? reg = string.IsNullOrEmpty(registro) ? (int?)null : int.Parse(registro);
			//int reg = (registro != "") ? int.Parse(registro) : 0;
			string fechaVencimiento = fechaVen;
			string fechaSolicitud = FechaSol;
			string fechaSolicitudAgencia = FechaSolAgencia;
			string institucion = Institucion;

			var listMovimientos = new Models.ListTipoMov();
			string sqlInsert = "insert into gmc_historia_exp(hist_cod, hist_exp_cod, hist_exp_fecha, hist_tmov, hist_estado, hist_observacion, hist_fec_grab, hist_login_grab, hist_institucion)\n"+
				" values((select nvl(max(hist_cod), 0) +1 from gmc_historia_exp),\n"+
				" :codExp,\n"+
				" :fechaExp,\n"+
				" :tMov,\n" +
				" :estado,\n" +
				" :observacion,\n" +
				" sysdate,\n" +
				" user,\n" +
				" :institucion)";
			string sqlSelect = "select * from gmc_expediente e, gmc_historia_exp h, gmc_tipo_movimiento tm where e.exp_codigo = h.hist_exp_cod and "+
				"e.exp_tipo_mov = tm.tmov_codigo and e.exp_codigo = "+int.Parse(Expediente);
			
			
			string sqlUpdate1 = "update gmc_expediente set exp_estado = :updateEstado, exp_nro_registro = :updateRegistro, "+
				" exp_tipo_mov = :updateTmov, exp_fecha_vencimiento = :updateFechaVencimiento , exp_fecha_solicitud = :updateFechaSol\n" +
				" where exp_codigo = " + exp;

			//Si el registro es igual a cero solo actualiza estos campos
			string sqlUpdate2 = "update gmc_expediente set exp_estado = :updateEstado, exp_tipo_mov = :updateTmov,\n"  +
				" exp_fecha_solicitud = :updateFechaSol where exp_codigo = " + exp;


			//Inserta en la tabla GMC_HISTORIA_EXP
			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection con = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand(sqlInsert, con);
			cmd.Parameters.Add(new OracleParameter("codExp", exp));
			cmd.Parameters.Add(new OracleParameter("fechaExp", string.IsNullOrEmpty(fechaSolicitud) ? DateTime.Parse(fechaSolicitudAgencia) : DateTime.Parse(fechaSolicitud)));
			//cmd.Parameters.Add(new OracleParameter("fechaExp", DateTime.Parse(fechaSolicitud)));
			cmd.Parameters.Add(new OracleParameter("tMov", tipoMov));
			cmd.Parameters.Add(new OracleParameter("estado", est));
			cmd.Parameters.Add(new OracleParameter("observacion", string.IsNullOrEmpty(Observacion) ? ObservacionAgencia : Observacion));
			//cmd.Parameters.Add(new OracleParameter("observacion", Observacion));
			cmd.Parameters.Add(new OracleParameter("institucion", institucion));

			//Actualiza la tabla GMC_EXPEDIENTE si posee un registro
			OracleCommand cmdUpdate = new OracleCommand(sqlUpdate1, con);            
			cmdUpdate.Parameters.Add(new OracleParameter("updateEstado", est));
			cmdUpdate.Parameters.Add(new OracleParameter("updateRegistro", reg));
			cmdUpdate.Parameters.Add(new OracleParameter("updateTmov", tipoMov));
			cmdUpdate.Parameters.Add(new OracleParameter("updateFechaVencimiento", string.IsNullOrEmpty(fechaVencimiento) ? (DateTime?)null : DateTime.Parse(fechaVencimiento)));
			cmdUpdate.Parameters.Add(new OracleParameter("updateFechaSol", string.IsNullOrEmpty(fechaSolicitud) ? DateTime.Parse(fechaSolicitudAgencia) : DateTime.Parse(fechaSolicitud)));
			//cmdUpdate.Parameters.Add(new OracleParameter("updateFechaSol", DateTime.Parse(fechaSolicitud)));

			//Actualiza la tabla GMC_EXPEDIENTE si no posee un registro
			OracleCommand cmdUpdate2 = new OracleCommand(sqlUpdate2, con);
			cmdUpdate2.Parameters.Add(new OracleParameter("updateEstado", est));
			cmdUpdate2.Parameters.Add(new OracleParameter("updateTmov", tipoMov));
			cmdUpdate2.Parameters.Add(new OracleParameter("updateFechaSol", string.IsNullOrEmpty(fechaSolicitud) ? DateTime.Parse(fechaSolicitudAgencia) : DateTime.Parse(fechaSolicitud)));
			//cmdUpdate2.Parameters.Add(new OracleParameter("updateFechaSol", DateTime.Parse(fechaSolicitud)));

			//Hace solo una simple consulta para retornar una lista por JSON (que no hace nada), para que una vez que se cumpla 
			//la instrucción vacíe los campos en la página, una forma de evitar postback.
			OracleCommand cmdSelect = new OracleCommand(sqlSelect, con);
			OracleTransaction transaction = null;
			try
			{
				con.Open();
				transaction = con.BeginTransaction();
				Permisos.OtorgarPermisos(con);
				cmd.ExecuteNonQuery();
				using (OracleDataReader dr = cmdSelect.ExecuteReader())
				{
					while (dr.Read())
					{
						listMovimientos.Add(new Models.TipoMovimientoModel(
							dr["TMOV_DESC"].ToString()));
					}
				}
				if (reg == 0)
				{
					cmdUpdate2.ExecuteNonQuery();
				}
				else
				{
					if(est != null)
					{
						cmdUpdate.ExecuteNonQuery();
					}                   
				}
				transaction.Commit();
			}
			catch (Exception ex) {
				transaction.Rollback();
				Session["manetinimientoEx"] = ex;

			}
			finally
			{
				if (con.State == System.Data.ConnectionState.Open)
				{
					con.Close();
					con.Dispose();
				}
			}
			return Json(listMovimientos);
		}

		/*Acción que retorna los datos de un expediente en la vista de Solicitudes, para luego poder insertar otros valores y así modificar
		dicho expediente.*/
		public JsonResult RetornarExpedienteEditar(int buscar)
		{
			string sqlString = "select e.*, p.pod_descripcion from gmc_expediente e, gmc_poder p where e.exp_poder = p.pod_codigo and exp_codigo = " + buscar;

			Models.ExpedienteList listExpedientes = new Models.ExpedienteList();

			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection conn = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand(sqlString, conn);
			Models.ExpedienteModels expedientes = new Models.ExpedienteModels();

			try
			{
				conn.Open();
				Permisos.OtorgarPermisos(conn);
				OracleDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					listExpedientes.Add(new Models.ExpedienteModels(
						expedientes.CodExpediente = int.Parse(dr["EXP_CODIGO"].ToString()),
						expedientes.CodEstado = int.Parse(dr["EXP_ESTADO"].ToString()),
						expedientes.CodAgente = int.Parse(dr["EXP_AGENTE"].ToString()),
						expedientes.FechaSolicitud = dr["EXP_FECHA_SOLICITUD"].ToString(),
						//expedientes.FechaEstado = dr["EXP_FECHA_ESTADO"].ToString(),
						expedientes.TipoSolicitud = int.Parse(dr["EXP_TIPO_SOL"].ToString()),
						expedientes.CodSolicitante = int.Parse(dr["EXP_SOLICITANTE"].ToString()),
						expedientes.CodPais = int.Parse(dr["EXP_PAIS"].ToString()),
						expedientes.CodPoder = int.Parse(dr["EXP_PODER"].ToString()),
						expedientes.CodMarca = int.Parse(dr["EXP_MARCA"].ToString()),
						expedientes.DescripcionPoder = dr["POD_DESCRIPCION"].ToString(),
						expedientes.FechaVencimiento = dr["EXP_FECHA_VENCIMIENTO"].ToString()));
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
					cmd.Dispose();
				}
			}
			return Json(listExpedientes);
		}

		//Acción que retorna los datos de una patente de acuerdo al número de solicitud en la vista Patente de Invención.
		public JsonResult RetornarPatenteEditar(int buscar)
		{
			string sqlString = "select * from gmc_patente_invencion where pat_numero_solicitud = " + buscar;
			var listPatentes = new ListPatentes();

			//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection conn = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand(sqlString, conn);
			SolicitudPatente patentes = new SolicitudPatente();

			try
			{
				conn.Open();
				Permisos.OtorgarPermisos(conn);
				OracleDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{

					listPatentes.Add(new Models.SolicitudPatente(
					patentes.PatenteCodigo = int.Parse(dr["PAT_CODIGO"].ToString()),
					patentes.PatenteSolicitud = int.Parse(dr["PAT_NUMERO_SOLICITUD"].ToString()),
					patentes.SolicitanteCodigo = int.Parse(dr["PAT_SOLICITANTE"].ToString()),
					patentes.FechaSolicitud = dr["PAT_FECHA_SOLICITUD"].ToString(),
					patentes.FechaVencimiento = dr["PAT_FECHA_VENCIMIENTO"].ToString(),
					patentes.PatenteTitulo = dr["PAT_TITULO"].ToString(),
					patentes.AgenteCodigo = int.Parse(dr["PAT_AGENTE"].ToString()),
					patentes.TipoSolicitud = int.Parse(dr["PAT_TIPO_SOLICITUD"].ToString())
					//listPatentes.Add(patentes);
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
					cmd.Dispose();
				}
			}
			return Json(listPatentes);
		}

		//Acción que retorna el tipo de movimiento.
		public JsonResult RetornarTipoMov(string tipoMovimiento)
		{
			int tMov = (tipoMovimiento != "") ? int.Parse(tipoMovimiento) : 0;
			string sqlString = "select * from gmc_tipo_movimiento where tmov_codigo = "+ tMov;

			Models.ListTipoMov listTmov = new Models.ListTipoMov();

			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection conn = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand(sqlString, conn);
			Models.TipoMovimientoModel tipoMov = new Models.TipoMovimientoModel();

			try
			{
				conn.Open();
				Permisos.OtorgarPermisos(conn);
				OracleDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					listTmov.Add(new Models.TipoMovimientoModel(
						 tipoMov.CodigoTipoMov = int.Parse(dr["TMOV_CODIGO"].ToString()),
						 tipoMov.DescTipoMov = (dr["TMOV_DESC"].ToString())));
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
			return Json(listTmov);
		}

		//Acción que anula el expediente recibido como parámetro.
		public JsonResult AnularExpediente(int buscar)
		{
		   
			Models.ExpedienteList listExpedientes = new Models.ExpedienteList();

			string sqlString = "select e.*, p.pod_descripcion from gmc_expediente e, gmc_poder p "+
				"where e.exp_poder = p.pod_codigo and exp_codigo = " + buscar;
			string sqlDelete = "delete from gmc_expediente where exp_codigo = "+ buscar;

			//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection conn = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand(sqlDelete, conn);
			OracleCommand cmd2 = new OracleCommand(sqlString, conn);
			try
			{
				conn.Open();
				Permisos.OtorgarPermisos(conn);
				OracleDataReader dr = cmd2.ExecuteReader();
				while (dr.Read())
				{
					listExpedientes.Add(new Models.ExpedienteModels(
						 int.Parse(dr["EXP_CODIGO"].ToString()),
						 int.Parse(dr["EXP_ESTADO"].ToString()),
						 int.Parse(dr["EXP_AGENTE"].ToString()),
						dr["EXP_FECHA_SOLICITUD"].ToString(),
						//dr["EXP_FECHA_ESTADO"].ToString(),
						int.Parse(dr["EXP_TIPO_SOL"].ToString()),
						int.Parse(dr["EXP_SOLICITANTE"].ToString()),
						int.Parse(dr["EXP_PAIS"].ToString()),
						int.Parse(dr["EXP_PODER"].ToString()),
						int.Parse(dr["EXP_MARCA"].ToString()),
						dr["POD_DESCRIPCION"].ToString(),
						dr["EXP_FECHA_VENCIMIENTO"].ToString()));
				}
				cmd.ExecuteNonQuery();
			}
			catch (OracleException e)
			{
				Session["errorDelete"] = e;
				if(e.Message.Contains("GMCEXP_GMCDATPUB"))
					return Json(new JsonResponse { Success = false, Message = "No se puede borrar el expediente. Elimine primero la publicación de ese expediente" }); 
				if(e.Message.Contains("GMCEXP_GMCHISTEXP"))
					return Json(new JsonResponse { Success = false, Message = "No se puede borrar el expediente. Elimine primero la historia de ese expediente" });
			}
			catch (Exception e)
			{

				Session["errorDelete"] = e;               
				return Json(new JsonResponse { Success = false, Message = "Exepción general" });
			   
			}
			finally
			{
				if (conn.State != System.Data.ConnectionState.Closed)
				{
					conn.Close();
					conn.Dispose();
				}
			}
			var errors = Session["errorDelete"];
			Session["errorOracle"] = errors;
			return Json(listExpedientes, (new JsonResponse { Success = true}).ToString());
		}
		#endregion

		#region GRID BATCH EDITING
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Editing_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<TelerikMvcApp4.Models.PublicacionModel> expedientes)
		{
			string sqlUpdate = "update gmc_historia_exp set hist_observacion = :updateObservacion where hist_cod = :updateWhere";
			//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection con = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand(sqlUpdate, con);
			cmd.Parameters.Add(new OracleParameter("updateObservacion",OracleDbType.NVarchar2));
			cmd.Parameters.Add(new OracleParameter("updateWhere", OracleDbType.NVarchar2));
			try
			{
				con.Open();
				Permisos.OtorgarPermisos(con);
				if (expedientes != null && ModelState.IsValid)
				{
					foreach (var exp in expedientes)
					{
						cmd.Parameters["updateObservacion"].Value = exp.Observacion;
						cmd.Parameters["updateWhere"].Value = exp.CodHistoriaExpediente;
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch(Exception ex)
			{
				Session["excepcionGestion"] = ex;
			}
			finally
			{
				con.Close();
				con.Dispose();
				cmd.Dispose();
			}
			
			return Json(expedientes.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Editing_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<TelerikMvcApp4.Models.PublicacionModel> expedientes)
		{
			string sqlDelete = "delete from gmc_historia_exp where hist_cod = :deleteHistExp";
			//string login = Session["NombreUsuario"].ToString();
			string sqlString = "insert into gmc_aud_hist_exp\n" +
			"  (aud_hist_cod,\n" +
			"   aud_hist_exp_cod,\n" +
			"   aud_hist_exp_fecha,\n" +
			"   aud_hist_tmov,\n" +
			"   aud_hist_estado,\n" +
			"   aud_hist_obs,\n" +
			"   aud_hist_exp_cod_rel,\n" +
			"   aud_hist_login,\n" +
			"   aud_hist_fecha_grab)\n" +
			"\n" +
			"  select hist_cod,\n" +
			"          hist_exp_cod,\n" +
			"          hist_exp_fecha,\n" +
			"          hist_tmov,\n" +
			"          hist_estado,\n" +
			"          hist_observacion,\n" +
			"          hist_exp_cod_rel,\n" +
			"          user,\n" +
			"          sysdate\n" +
			"     from gmc_historia_exp\n" +
			"    where hist_cod = :insertAuxHIstExp";

			//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection con = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand(sqlDelete, con);
			cmd.Parameters.Add(new OracleParameter("deleteHistExp", OracleDbType.NVarchar2));

			OracleCommand cmd2 = new OracleCommand(sqlString, con);
			cmd2.BindByName = true;
			cmd2.Parameters.Add(new OracleParameter("insertAuxHIstExp", OracleDbType.NVarchar2));
			//cmd2.Parameters.Add(new OracleParameter("insertLogin", login.ToUpper()));
			try
			{
				con.Open();
				Permisos.OtorgarPermisos(con);
				//if (expedientes != null && ModelState.IsValid)
				//{
					foreach (var exp in expedientes)
					{
						cmd.Parameters["deleteHistExp"].Value = exp.CodHistoriaExpediente;                        
						cmd2.Parameters["insertAuxHIstExp"].Value = exp.CodHistoriaExpediente;
						cmd2.ExecuteNonQuery();
						cmd.ExecuteNonQuery();
						ViewBag.SuccessMessage = "<p>Success!</p>";
					}
				//}
			}
			catch (Exception ex)
			{
				Session["excepcionGestion"] = ex;
			}
			finally
			{
				con.Close();
				con.Dispose();
				cmd.Dispose();
			}

			return Json(expedientes.ToDataSourceResult(request, ModelState));
		}


		//Acción que elimina las publicaciones de la grilla.
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Editing_Destroy_Pub([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<TelerikMvcApp4.Models.PublicacionModel> expedientes)
		{
			//string sqlDelete = "delete from gmc_publicacion where pub_clave = :deletePub";
			string sqlDelete = "delete from gmc_publicacion_detalle where pub_det_codigo = :deletePub";
			//string login = Session["NombreUsuario"].ToString();
			//string sqlString = "insert into gmc_aud_publicacion\n" +
			//"  (aud_pub_clave,\n" +
			//"   aud_pub_expediente,\n" +
			//"   aud_pub_fecha,\n" +
			//"   aud_pub_periodico,\n" +
			//"   aud_pub_dias,\n" +
			////"   aud_pub_publicado,\n" +
			////"   aud_pub_obs,\n" +
			//"   aud_fec_grab,\n" +
			//"   aud_login_grab)\n" +
			//"\n" +
			//"  select pub_clave,\n" +
			//"          pub_expediente,\n" +
			//"          pub_fecha,\n" +
			//"          pub_periodico,\n" +
			//"          pub_dias,\n" +
			////"          pub_publicado,\n" +
			////"          pub_obs,\n" +
			//"          sysdate,\n" +
			//"          user\n" +
			//"     from gmc_publicacion\n" +
			//"    where pub_clave = :insertAudPub";

			//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection con = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand(sqlDelete, con);
			cmd.Parameters.Add(new OracleParameter("deletePub", OracleDbType.NVarchar2));

			//OracleCommand cmd2 = new OracleCommand(sqlString, con);
			//cmd2.BindByName = true;
			//cmd2.Parameters.Add(new OracleParameter("insertAudPub", OracleDbType.NVarchar2));
			OracleTransaction transaction = null;
			try
			{
				con.Open();               
				transaction = con.BeginTransaction();
				Permisos.OtorgarPermisos(con);
				//if (expedientes != null && ModelState.IsValid)
				//{
				foreach (var exp in expedientes)
				{
					//cmd.Parameters["deletePub"].Value = exp.PubClave;
					cmd.Parameters["deletePub"].Value = exp.PubDetCodigo;
					//cmd2.Parameters["insertAudPub"].Value = exp.PubClave;
					//cmd2.ExecuteNonQuery();
					cmd.ExecuteNonQuery();

				}
				transaction.Commit();
				//}
			}
			catch (Exception ex)
			{
			   transaction.Rollback();
				Session["excepcionGestion"] = ex;
				if (ex.Message.Contains("GMCPUBLICACION_GMCPUBIMAGEN"))
					return Json(new JsonResponse { Success = false, Message = "No se puede borrar la publicación."+
						"Elimine primero la publicación de la imagen." });
				if(ex.Message.Contains("GMCPUB_GMCPUBDET"))
					return Json(new JsonResponse
					{
						Success = false,
						Message = "No se puede borrar la publicación." +
						"Elimine primero los detales de la publicación."
					});
			}
			finally
			{
				con.Close();
				con.Dispose();
				cmd.Dispose();
			}

			return Json(expedientes.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Editing_Destroy_Img([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<TelerikMvcApp4.Models.PublicacionImagenModel> expedientes)
		{
			string sqlDelete = "delete from gmc_publicacion_imagen where puim_clave = :deletePub";
			//string login = Session["NombreUsuario"].ToString();
			//string sqlString = "insert into gmc_aud_publicacion\n" +
			//"  (aud_pub_clave,\n" +
			//"   aud_pub_expediente,\n" +
			//"   aud_pub_fecha,\n" +
			//"   aud_pub_periodico,\n" +
			//"   aud_pub_dias,\n" +
			//"   aud_pub_publicado,\n" +
			//"   aud_pub_obs,\n" +
			//"   aud_fec_grab,\n" +
			//"   aud_login_grab)\n" +
			//"\n" +
			//"  select pub_clave,\n" +
			//"          pub_expediente,\n" +
			//"          pub_fecha,\n" +
			//"          pub_periodico,\n" +
			//"          pub_dias,\n" +
			//"          pub_publicado,\n" +
			//"          pub_obs,\n" +
			//"          sysdate,\n" +
			//"          user\n" +
			//"     from gmc_publicacion\n" +
			//"    where pub_clave = :insertAudPub";

			//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection con = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand(sqlDelete, con);
			cmd.Parameters.Add(new OracleParameter("deletePub", OracleDbType.NVarchar2));

			//OracleCommand cmd2 = new OracleCommand(sqlString, con);
			//cmd2.BindByName = true;
			//cmd2.Parameters.Add(new OracleParameter("insertAudPub", OracleDbType.NVarchar2));

			try
			{
				con.Open();
				Permisos.OtorgarPermisos(con);
				//if (expedientes != null && ModelState.IsValid)
				//{
				foreach (var exp in expedientes)
				{
					cmd.Parameters["deletePub"].Value = exp.Puim_clave;
					//cmd2.Parameters["insertAudPub"].Value = exp.PubClave;
					//cmd2.ExecuteNonQuery();
					cmd.ExecuteNonQuery();

				}
				//}
			}
			catch (Exception ex)
			{
				Session["excepcionGestion"] = ex;
				if (ex.Message.Contains("GMCEXP_GMCDATPUB"))
					return Json(new JsonResponse { Success = false, Message = "No se pudo borrar la imagen de la publicación" });
			}
			finally
			{
				con.Close();
				con.Dispose();
				cmd.Dispose();
			}

			return Json(expedientes.ToDataSourceResult(request, ModelState));
		}
		#endregion

		#region ROW TEMPLATE
		//Acción que retorna registros a una grilla, con una imagen.
		public ActionResult RowTemplate_Read([DataSourceRequest]DataSourceRequest request)
		{


			string sqlString = "select * from gmc_publicacion_imagen pi, gmc_publicacion p, gmc_periodico pe\n" +
			"where pi.puim_clave_pub = p.pub_clave\n" +
			"and p.pub_periodico = pe.per_codigo";


			var listImagenesPub = new Models.ListIamgenesPub();

			//HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			//FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			string cadena = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
			OracleConnection con = new OracleConnection(cadena);
			OracleCommand cmd = new OracleCommand(sqlString, con);

			try
			{
				con.Open();
				Permisos.OtorgarPermisos(con);
				using (OracleDataReader dr = cmd.ExecuteReader())
				{
					while (dr.Read())
					{
						
						listImagenesPub.Add(new PublicacionImagenModel(
							int.Parse(dr["PUB_EXPEDIENTE"].ToString()),
							int.Parse(dr["PUIM_CLAVE"].ToString()),
							int.Parse(dr["PUIM_CLAVE_PUB"].ToString()),
							dr["PER_DESCRIPCION"].ToString(),
							dr["PUIM_IMAGEN"] != DBNull.Value ? (byte[])(dr["PUIM_IMAGEN"]) : null
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

			return Json(listImagenesPub.ToDataSourceResult(request));
		}
		#endregion
	}
}