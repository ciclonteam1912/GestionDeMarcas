using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using System.Web.Security;
using Microsoft.AspNet.Identity;

using Microsoft.AspNet.Identity.EntityFramework;

using TelerikMvcApp4.Models;
using TelerikMvcApp4.Helpers;

namespace TelerikMvcApp4.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        // GET: Users
        ApplicationDbContext context;
        public UsersController()
        {
            context = new ApplicationDbContext();
        }
        public ActionResult Index()
		{
			if (User.Identity.IsAuthenticated)
			{
				var user = User.Identity;
				ViewBag.Name = user.Name;
				//	ApplicationDbContext context = new ApplicationDbContext();
				//	var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

				//var s=	UserManager.GetRoles(user.GetUserId());
				ViewBag.displayMenu = "No";

				if (UserContext.isAdminUser(User))
				{
					ViewBag.displayMenu = "Yes";
				}
                var Users = context.Users.ToList();
                return View(Users);
              
			}
			else
			{
				ViewBag.Name = "Not Logged IN";
			}


			return View();


		}
	}
}