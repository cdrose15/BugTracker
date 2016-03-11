using BugTracker.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class AssignTicketsViewModel
    {
        public Ticket Ticket { get; set; }
        public SelectList Users { get; set; }
        public string SelectedUser { get; set; }
    }
}