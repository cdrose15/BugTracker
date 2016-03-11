using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Helpers
{
    public class TicketUserHelper
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public void AssignTicketToUser(string userId, int ticketId)
        {
            var ticket = db.Tickets.Find(ticketId);
            var user = db.Users.Find(userId);
            
        }
    }
}