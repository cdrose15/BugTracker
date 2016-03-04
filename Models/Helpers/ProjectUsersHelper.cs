using BugTracker.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Helpers
{
    public class ProjectUsersHelper
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public void AddUserToProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var user = db.Users.Find(userId);
            project.Users.Add(user);

            db.SaveChanges();           
        }

        public bool IsUserInProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var user = project.Users.Any(u => u.Id == userId);

            return user;
        }

        public void RemoveUserFromProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var user = db.Users.Find(userId);
            project.Users.Remove(user);

            db.SaveChanges();
        }

        public IList<Project> ProjectsForUser(string userId)
        {
            //var resultList = new List<Project>();

            //foreach(var project in db.Users.Find(userId).Projects)
            //{
            //    resultList.Add(project);
            //}
            //return resultList;

            var user = db.Users.Find(userId);
            return user.Projects.ToList();

        }

        public IList<ApplicationUser> UsersOnProject(int projectId)
        {
            //var resultList = new List<ApplicationUser>();

            //foreach (var user in db.Projects.Find(projectId).Users)
            //{
            //    resultList.Add(user);
            //}
            //return resultList;

            var project = db.Projects.Find(projectId);
            return project.Users.ToList();
        }

        public IList<ApplicationUser> UsersNotOnProject(int projectId)
        {
            return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();
        }

    }
}