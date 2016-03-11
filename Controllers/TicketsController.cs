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
using BugTracker.Models.Helpers;

namespace BugTracker.Controllers
{
    [RequireHttps]
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        //public ActionResult Index()
        //{
        //    var user = db.Users.Find(User.Identity.GetUserId());

        //    if (User.IsInRole("Project Manager") || User.IsInRole("Developer"))
        //    {
        //        return View(db.Tickets.Where(t => t.AssigneeUser.Id == user.Id)
        //            .Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).ToList());
        //    }

        //    if (User.IsInRole("Submitter"))
        //    {
        //        return View(db.Tickets.Where(t => t.CreatedUser.Id == user.Id).Include(t => t.TicketPriority)
        //            .Include(t => t.TicketStatus).Include(t => t.TicketType).ToList());
        //    }
        //    var model = db.Tickets.Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus)
        //        .Include(t => t.TicketType).ToList();
        //    return View(model);
        //}

        // GET: Tickets
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            var tickets = db.Tickets.Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus)
                     .Include(t => t.TicketType);
            var ownedTickets = tickets.Where(o => o.CreatedUserId == user.Id).AsEnumerable();
            var projectTickets = user.Projects.SelectMany(p => p.Tickets);
            var projectOwnedTickets = ownedTickets.Union(projectTickets);

            if(User.IsInRole("Project Manager") || User.IsInRole("Developer"))
            {
                ViewBag.EditMessage = TempData["editMessage"];
                return View(projectOwnedTickets.ToList());
            }
            if(User.IsInRole("Submitter"))
            {
                return View(ownedTickets.ToList());
            }

            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            //ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,CreatedDate,UpdatedDate,FileSource,ProjectId,TicketPriorityId,TicketStatusId,TicketTypeId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var user = User.Identity.GetUserId();
                ticket.CreatedUserId = user;
                ticket.TicketStatusId = 1;
                ticket.CreatedDate = DateTime.UtcNow.ToLocalTime();
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/AssignUser
        [Authorize(Roles = "Administrator, Project Manager, Developer")]
        [HttpGet]
        public ActionResult AssignUser(int id)
        {
            UserRolesHelper helper = new UserRolesHelper();
            var ticket = db.Tickets.Find(id);
            AssignTicketsViewModel ViewModel = new AssignTicketsViewModel();
            var developers = helper.UsersInRole("Developer").Where(d => d.Projects.Any(p => p.Id == ticket.ProjectId));
            //var projectUsers = ticket.Project.Users;
            //var developerOnProject = new List<ApplicationUser>();
            //foreach(var dev in developers.ToList() )
            //{
            //    if(projectUsers.Any(u => u.Id == dev.Id))
            //    {
            //        developerOnProject.Add(dev);
            //    }
            //}
            var selected = ticket.AssigneeUserId;
            
            ViewModel.Users = new SelectList(developers, "Id", "DisplayName", selected);
            ViewModel.Ticket = ticket;

            return View(ViewModel);
        }

        // POST: Tickets/AssignUser
        [Authorize(Roles = "Administrator, Project Manager, Developer")]
        [HttpPost]
        public ActionResult AssignUser(AssignTicketsViewModel ViewModel, int id)
        {
            //var t = new Ticket(1);
            var user = db.Users.Find(User.Identity.GetUserId());
            var ticket = db.Tickets.Find(id);
            ticket.AssigneeUserId = ViewModel.SelectedUser;
            ticket.TicketStatusId = 2;
            db.SaveChanges();

            // Add TicketNotification to database
            TicketNotification ticketNotification = new TicketNotification();
            ticketNotification.SentDate = DateTime.Now;
            ticketNotification.AssigneeUserId = ticket.AssigneeUserId;
            ticketNotification.TicketId = ticket.Id;
            ticketNotification.Description = ticket.AssigneeUser.DisplayName;
            db.TicketNotifications.Add(ticketNotification);
            db.SaveChanges();

            // Send email notification when a user is assigned to a ticket
            var assigned = ticket.AssigneeUser.Email;
            var website = "https://crose-bugtracker.azurewebsites.net/Tickets";
            var email = new EmailService();
            var mail = new IdentityMessage
            {
                Subject = "BugTracker Notification",
                Destination = assigned,
                Body = $"Attention {ticket.AssigneeUser.DisplayName}, you have been assigned a new ticket from {user.DisplayName}. To view click here: {website}"
            };
            email.SendAsync(mail);

            return RedirectToAction("Index");
        }

         // Controller action that will change ticket status to resolved. Not being used at this time
        [HttpPost]
        public ActionResult ResolveTicket(int id)
        {
            var ticket = db.Tickets.Find(id);
            ticket.TicketStatusId = 3;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Administrator, Project Manager, Developer")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            //var user = db.Users.Find(User.Identity.GetUserId());
            //var developerEdit = ticket.Id.Where(d => d.AssigneeUserId == user.Id);
            //if(ticket != developerEdit)
            //{
            //    TempData["editMessage"] = "You can only edit tickets that you are assigned to you.";
            //    return RedirectToAction("Index");
            //}
                
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Project Manager, Developer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,CreatedDate,UpdatedDate,FileSource,ProjectId,TicketPriorityId,TicketStatusId,TicketTypeId,CreatedUserId,AssigneeUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var user = User.Identity.GetUserId();       
                ticket.UpdatedDate = DateTime.UtcNow.ToLocalTime();
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                db.Tickets.Include(t => t.AssigneeUser).FirstOrDefault(t => t.Id == ticket.Id);
                if (user != ticket.AssigneeUserId)
                {
                    if (ticket.AssigneeUserId != null)
                    {
                        var assignedDeveloper = ticket.AssigneeUser.Email;
                        var website = "https://crose-bugtracker.azurewebsites.net/Tickets/Details/" + ticket.Id;
                        var email = new EmailService();
                        var mail = new IdentityMessage
                        {
                            Subject = "BugTracker Notification",
                            Destination = assignedDeveloper,
                            Body = $"Hello {ticket.AssigneeUser.DisplayName}, {ticket.Title} has been updated. Click here to view changes: {website}"
                        };
                        email.SendAsync(mail);
                    }
                }
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);

            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
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
