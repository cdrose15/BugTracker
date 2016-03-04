using BugTracker.Models;
using BugTracker.Models.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [Authorize(Roles ="Administrator")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Users/Users
        [HttpGet]
        public ActionResult Users()
        {
            return View(db.Users.ToList());
        }


        // GET: Users/UserRoles
        [HttpGet]
        public ActionResult UserRoles(string id)
        {
            var user = db.Users.Find(id);
            AssignRolesViewModel ViewModel = new AssignRolesViewModel();
            UserRolesHelper helper = new UserRolesHelper();
            var selected = helper.ListUserRoles(id);
            ViewModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
            ViewModel.FirstName = user.FirstName;
            ViewModel.LastName = user.LastName;
            ViewModel.Id = user;

            return View(ViewModel);
        }

        // POST: Users/UserRoles
        [HttpPost]
        public ActionResult UserRoles (AssignRolesViewModel ViewModel, string id)
        {
            UserRolesHelper helper = new UserRolesHelper();

            if(ViewModel.SelectedRoles == null)
            {
                ViewModel.SelectedRoles = new string[]{""};                    
            }

            foreach(var role in db.Roles.Select(r => r.Name))
            {
                if (ViewModel.SelectedRoles.Contains(role))
                {
                    helper.AddUserToRole(id,role);
                }
                else
                {
                    helper.RemoveUserFromRole(id,role);
                }
            }
            return RedirectToAction("Users");           
        }
    }
}