using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class AssignRolesViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ApplicationUser Id { get; set; }
        public MultiSelectList Roles { get; set; }
        public string[] SelectedRoles { get; set; }
    }
}