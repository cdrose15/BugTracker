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
using System.IO;

namespace BugTracker.Controllers
{
    [RequireHttps]
    [Authorize]
    public class TicketCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketId,Comment,Attachment")] TicketComment ticketComment, HttpPostedFileBase image)
        {
            var user = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                ticketComment.CreatedDate = DateTime.UtcNow;
                ticketComment.UserId = user;
                if(ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/img/attachments/"), fileName));
                    ticketComment.Attachment = "~/img/attachments/" + fileName;
                }
                db.TicketComments.Add(ticketComment);
                db.SaveChanges();
                db.TicketComments.Include(c => c.Ticket).Include(c => c.Ticket.AssigneeUser).FirstOrDefault(t => t.Id == ticketComment.Id);
                if (ticketComment.UserId != ticketComment.Ticket.AssigneeUserId)
                {
                    if (ticketComment.Ticket.AssigneeUserId != null)
                    {
                        var assignedDeveloper = ticketComment.Ticket.AssigneeUser.Email;
                        var website = "https://crose-bugtracker.azurewebsites.net/Tickets/Details/" + ticketComment.Ticket.Id;
                        var email = new EmailService();
                        var mail = new IdentityMessage
                        {
                            Subject = "BugTracker Notification",
                            Destination = assignedDeveloper,
                            Body = $"Hello {ticketComment.Ticket.AssigneeUser.DisplayName}, {ticketComment.Ticket.Title} has been updated. Click here to view changes: {website}"
                        };
                        email.SendAsync(mail);
                    }
                }

                return RedirectToAction("Details","Tickets", new { id = ticketComment.TicketId });
            }

            //ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            return View(ticketComment);
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
