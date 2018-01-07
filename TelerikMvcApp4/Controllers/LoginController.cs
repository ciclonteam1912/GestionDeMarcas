using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Web.Security;
using TelerikMvcApp4.Models;

namespace TelerikMvcApp4.Controllers
{
    [Authorize]
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            Response.Cookies.Clear();
            return View();
        }

        [HttpPost]
        public ActionResult GetLogin(FormCollection frm)
        {
            Session["Error"] = false;
            bool ok = false;
            Models.LoginModel login = new Models.LoginModel();
            login.NombreUsuario = Request.Form["txtUsuario"].ToString();
            login.ContraseñaUsuario = Request.Form["txtContraseña"].ToString();
            login.BaseUsuario = Request.Form["txtBaseDato"].ToString();

            string conString = Shared.DALConexion.CrearConexion(login.NombreUsuario, login.ContraseñaUsuario, login.BaseUsuario);
            string QueryLogin = "SELECT * FROM GEN_OPERADOR";
            OracleConnection con = new OracleConnection(conString);
            OracleCommand cmd = new OracleCommand(QueryLogin, con);
            try
            {
                con.Open();
                Permisos.OtorgarPermisos(con);
                OracleDataReader dr = cmd.ExecuteReader();
                ok = (dr.Read()) ? true : false;
                if (ok)
                {
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                        login.NombreUsuario, DateTime.Now, DateTime.Now.AddMinutes(10000000), false, conString);
                    string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    cookie.Expires = DateTime.Now.AddMinutes(10000000);
                    Session["NombreUsuario"] = login.NombreUsuario;
                    Response.Cookies.Add(cookie);
                    return RedirectToAction("Index", "Gestion");
                }
            }
            catch (Exception ex)
            {
                Session["aaa"] = ex;
                Session["Error"] = true;
                return RedirectToAction("Index", "Login", Session["Error"]);
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    cmd.Dispose();
                }
            }
            return View();
        }
    }
}