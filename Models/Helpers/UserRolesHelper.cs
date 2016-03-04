using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BugTracker.Models.Helpers
{
    public class UserRolesHelper
    {
        // User Manager Object
        private UserManager<ApplicationUser> manager =
            new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(
                    new ApplicationDbContext()));

        // Does user have a specific role
        public bool IsUserInRole(string userId,string roleName)
        {
            return manager.IsInRole(userId, roleName);
        }

        // List users in role
        public IList<string> ListUserRoles(string userId)
        {
            return manager.GetRoles(userId);
        }

        // Add user to a specific role
        public bool AddUserToRole(string userId, string roleName)
        {
            var result = manager.AddToRole(userId, roleName);
            return result.Succeeded;
        }

        // Remove user from a role
        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = manager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }

        // List of users in specific role
        public IList<ApplicationUser> UsersInRole(string roleName)
        {
            var db = new ApplicationDbContext();
            var resultList = new List<ApplicationUser>();

            foreach(var user in db.Users)
            {
                if(IsUserInRole(user.Id,roleName))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }

        internal bool IsUserInRole(string user, IList<string> selected)
        {
            throw new NotImplementedException();
        }

        // List of users not in a specific role
        public IList<ApplicationUser> UsersNotInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();

            foreach (var user in manager.Users)
            {
                if (!IsUserInRole (user.Id, roleName))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }
    }
}