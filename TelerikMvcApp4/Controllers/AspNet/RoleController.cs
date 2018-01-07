using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using TelerikMvcApp4.Models;

namespace TelerikMvcApp4.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
		ApplicationDbContext context;

		public RoleController()
		{
			context = new ApplicationDbContext();
		}



		/// <summary>
		/// Get All Roles
		/// </summary>
		/// <returns></returns>
		public ActionResult Index()
		{

            //if (User.Identity.IsAuthenticated)
            //{


            //    if (!isAdminUser())
            //    {
            //        return RedirectToAction("Index", "Home");
            //    }
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            if (User.Identity.IsAuthenticated)
            {
                var Roles = context.Roles.ToList();
                return View(Roles);
            }

            return RedirectToAction("Login", "Account");

        }
		
		/// <summary>
		/// Create  a New role
		/// </summary>
		/// <returns></returns>
		public ActionResult Create()
		{
			if (User.Identity.IsAuthenticated)
			{


				if (!Helpers.UserContext.isAdminUser(User))
				{
					return RedirectToAction("Login", "Account");
				}
			}
			else
			{
				return RedirectToAction("Login", "Account");
			}

			var Role = new IdentityRole();
			return View(Role);
		}

		/// <summary>
		/// Create a New Role
		/// </summary>
		/// <param name="Role"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Create(IdentityRole Role)
		{
            if (User.Identity.IsAuthenticated)
            {


                if (!Helpers.UserContext.isAdminUser(User))
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            context.Roles.Add(Role);
			context.SaveChanges();
			return RedirectToAction("Index");
		}

        //
        // GET: /Roles/Edit/5
        public ActionResult Edit(string roleName)
        {
            var thisRole = context.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            return View(thisRole);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Microsoft.AspNet.Identity.EntityFramework.IdentityRole role)
        {
            try
            {
                context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(string RoleName)
        {
            var thisRole = context.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            context.Roles.Remove(thisRole);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ManageUserRoles()
        {
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string UserName, string RoleName)
        {
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var usrMgr = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var account = new AccountController(usrMgr);
            account.UserManager.AddToRole(user.Id, RoleName);

            ViewBag.ResultMessage = "Role created successfully !";

            // prepopulat roles for the view dropdown
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;

            return View("ManageUserRoles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string UserName)
        {
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                var usrMgr = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));

                var account = new AccountController(usrMgr);

                ViewBag.RolesForThisUser = account.UserManager.GetRoles(user.Id);

                // prepopulat roles for the view dropdown
                var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;
            }

            return View("ManageUserRoles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string UserName, string RoleName)
        {
            if(string.IsNullOrEmpty(RoleName))
            {
                ViewBag.ResultMessage = "Debe seleccionar un rol";
                return View("ManageUserRoles");
            }
            if (string.IsNullOrEmpty(UserName))
            {
                ViewBag.ResultMessage = "Debe seleccionar un usuario";
                return View("ManageUserRoles");
            }
            var usrMgr = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var account = new AccountController(usrMgr);
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (account.UserManager.IsInRole(user.Id, RoleName))
            {
                account.UserManager.RemoveFromRole(user.Id, RoleName);
                ViewBag.ResultMessage = "Role removed from this user successfully !";
            }
            else
            {
                ViewBag.ResultMessage = "This user doesn't belong to selected role.";
            }
            // prepopulat roles for the view dropdown
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;

            return View("ManageUserRoles");
        }
    }
}