using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.CodeFirst
{
    public class TicketNotification
    {
        public int Id { get; set; }
        public DateTimeOffset SentDate { get; set; }
        public string Description { get; set; }
        public int AssigneeUserId { get; set; }
        public int TicketId { get; set; }

        public virtual ApplicationUser AssigneeUser { get; set; }
        public virtual Ticket Ticket { get; set; }

    }
}