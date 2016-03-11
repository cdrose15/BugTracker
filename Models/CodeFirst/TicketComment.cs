using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.CodeFirst
{
    public class TicketComment
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string Comment { get; set; }
        public string Attachment { get; set; }
        public string UserId { get; set; }
        public int TicketId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Ticket Ticket { get; set; }

    }
}