using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.CodeFirst
{
    public class Ticket
    {
        public Ticket()
        {
            this.TicketComments = new HashSet<TicketComment>();
            this.TicketLogs = new HashSet<TicketLog>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public string FileSource { get; set; }
        public int CreatedUserId { get; set; }
        public int AssigneeUserId { get; set; }
        public int ProjectId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }
        public int TicketTypeId { get; set; }

        public virtual ApplicationUser CreatedUser { get; set; }
        public virtual ApplicationUser AssigneeUser { get; set; }
        public virtual Project Project { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketLog> TicketLogs { get; set;}

    }
}