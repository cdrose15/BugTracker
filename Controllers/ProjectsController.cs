using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using BugTracker.Models.Helpers;

namespace BugTracker.Controllers
{
    [RequireHttps]
    [Authorize]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        [Authorize(Roles ="Administrator, Project Manager, Developer")]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            if(!User.IsInRole("Administrator"))
            {
                return View(user.Projects.ToList());
            }
           // ViewBag.SelectedUsers = new SelectList(db.Users, "Id", "DisplayName");
           
            return View(db.Projects.ToList());          
        }

        // GET: Projects/ProjectUsers
        [Authorize(Roles ="Administrator, Project Manager")]
        [HttpGet]
        public ActionResult ProjectUsers(int id)
        {
            var project = db.Projects.Find(id);
            AssignProjectsViewModel ViewModel = new AssignProjectsViewModel();
            ProjectUsersHelper helper = new ProjectUsersHelper();
            UserRolesHelper roles = new UserRolesHelper();
            var user = roles.UsersNotInRole("Submitter");
            var selected = helper.UsersOnProject(id).Select(a => a.Id) ;
            //ViewBag.SelectedUsers = new SelectList(db.Users, "Id", "DisplayName", project.Users.Select(u => u.Id));
            ViewModel.Users= new MultiSelectList(user, "Id", "DisplayName", selected);
            ViewModel.Project = project;

            return View(ViewModel);
        }

        // POST: Project/ProjectUsers
        [Authorize(Roles = "Administrator, Project Manager")]
        [HttpPost]
        public ActionResult ProjectUsers(AssignProjectsViewModel ViewModel)
        {
            ProjectUsersHelper helper = new ProjectUsersHelper();

            if (ViewModel.SelectedUsers == null)
            {
                ViewModel.SelectedUsers = new string[] { "" };
            }

            foreach (var user in db.Users.Select(u => u.Id))
            {
                if (ViewModel.SelectedUsers.Contains(user))
                {
                    helper.AddUserToProject(user, ViewModel.Project.Id);
                }
                else
                {
                    helper.RemoveUserFromProject(user, ViewModel.Project.Id);
                }
            }

            return RedirectToAction("Index");
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles ="Administrator, Project Manager")]
        public PartialViewResult _Create()
        {
            return PartialView();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles ="Administrator, Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize (Roles ="Administrator, Project Manager")]
        public PartialViewResult _Edit(int? id)
        {
            Project project = db.Projects.Find(id);

            return PartialView(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles ="Administrator, Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
