using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.CodeFirst
{
    public class TicketLog
    {
        public int ID { get; set; }
        public DateTimeOffset ChangedDate { get; set; }
        public string Property { get; set; }
        public string OldFieldValue { get; set; }
        public string NewFieldValue { get; set; }
        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}