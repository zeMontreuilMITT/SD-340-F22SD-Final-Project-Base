using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using JelloTicket.DataLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using JelloTicket.BusinessLayer.Services;
using Microsoft.AspNetCore.Identity;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly TicketBusinessLogic _ticketBusinessLogic;
        private readonly ProjectBusinessLogic _projectBusinessLogic;
        private readonly UserManager<ApplicationUser> _users;
        private readonly CommentRepo _commentRepo;
        private readonly UserManagerBusinessLogic _userManagerBusinessLogic;
        
        public TicketsController(IRepository<Ticket> ticketRepo
            , IRepository<Project> projectRepo
            , IRepository<Comment> commentRepo
            , UserManager<ApplicationUser> users
            , UserManagerBusinessLogic userManagerBusinessLogic)
        {
            _ticketBusinessLogic = new TicketBusinessLogic(ticketRepo, projectRepo, commentRepo, users, userManagerBusinessLogic);
            _users = users;
            _userManagerBusinessLogic = userManagerBusinessLogic;
        }

        // GET: Tickets
        //public async Task<IActionResult> Index()
        //{
        //    return _context.Tickets != null ?
        //                View(await _context.Tickets.Include(t => t.Project).Include(t => t.Owner).ToListAsync()) :
        //                Problem("Entity set 'ApplicationDbContext.Tickets'  is null.");
        //}

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Ticket ticket = _ticketBusinessLogic.GetTicketById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            ViewBag.Users = _userManagerBusinessLogic.UserSelectListItem();

            return View(ticket);
        }

        //// GET: Tickets/Create
        //[Authorize(Roles = "ProjectManager")]
        //public IActionResult Create(int projId)
        //{
        //    Project currProject = _context.Projects.Include(p => p.AssignedTo).ThenInclude(at => at.ApplicationUser).FirstOrDefault(p => p.Id == projId);

        //    List<SelectListItem> currUsers = new List<SelectListItem>();
        //    currProject.AssignedTo.ToList().ForEach(t =>
        //    {
        //        currUsers.Add(new SelectListItem(t.ApplicationUser.UserName, t.ApplicationUser.Id.ToString()));
        //    });

        //    ViewBag.Projects = currProject;
        //    ViewBag.Users = currUsers;

        //    return View();

        //}

        //// POST: Tickets/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "ProjectManager")]
        //public async Task<IActionResult> Create([Bind("Id,Title,Body,RequiredHours,TicketPriority")] Ticket ticket, int projId, string userId)
        //{
        //    if (ModelState.IsValid)
        //    { 
        //        ticket.Project = await _context.Projects.FirstAsync(p => p.Id == projId);
        //        Project currProj = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projId);
        //        ApplicationUser owner = _context.Users.FirstOrDefault(u => u.Id == userId);
        //        ticket.Owner = owner;
        //        _context.Add(ticket);
        //        currProj.Tickets.Add(ticket);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction("Index","Projects", new { area = ""});
        //    }
        //    return View(ticket);
        //}

        // GET: Tickets/Edit/5
        //[Authorize(Roles = "ProjectManager")]
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    Ticket ticket = await _ticketBusinessLogic.Include(t => t.Owner).FirstAsync(t => t.Id == id);

        //    if (ticket == null)
        //    {
        //        return NotFound();
        //    }

        //    List<ApplicationUser> results = _context.Users.Where(u => u != ticket.Owner).ToList();

        //    List<SelectListItem> currUsers = new List<SelectListItem>();

        //    results.ForEach(r =>
        //    {
        //        currUsers.Add(new SelectListItem(r.UserName, r.Id.ToString()));
        //    });
        //    ViewBag.Users = currUsers;

        //    return View(ticket);
        //}

        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> RemoveAssignedUser(string? id, int ticketId)
        {
            _ticketBusinessLogic.RemoveUser(id, ticketId);

            return RedirectToAction("Edit", new { id = ticketId });
        }

        //// POST: Tickets/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "ProjectManager")]
        //public async Task<IActionResult> Edit(int id,string userId, [Bind("Id,Title,Body,RequiredHours")] Ticket ticket)
        //{
        //    if (id != ticket.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            ApplicationUser currUser = _context.Users.FirstOrDefault(u => u.Id == userId);
        //            ticket.Owner = currUser;
        //            _context.Update(ticket);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!TicketExists(ticket.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Edit), new {id = ticket.Id});
        //    }
        //    return View(ticket);
        //}

        //[HttpPost]
        public async Task<IActionResult> CommentTask(int TaskId, string? TaskText)
        {
            if (TaskId != null || TaskText != null)
            {
                try
                {
                    Comment newComment = new Comment();
                    ApplicationUser user = _userManagerBusinessLogic.GetLoggedInUser(User).Result;
                    Ticket ticket = _ticketBusinessLogic.GetTicketById(TaskId);

                    newComment.CreatedBy = user;
                    newComment.Description = TaskText;
                    newComment.Ticket = ticket;
                    user.Comments.Add(newComment);
                    _commentRepo.Create(newComment);
                    ticket.Comments.Add(newComment);

                    int Id = TaskId;
                    _commentRepo.Save();

                    return RedirectToAction("Details", new { Id });
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> UpdateHrs(int id, int hrs)
        {
            if (id != null || hrs != null)
            {
                try
                {
                    Ticket ticket = _ticketBusinessLogic.GetTicketById(id);
                    ticket.RequiredHours = hrs;
                    _ticketBusinessLogic.Save();
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        //public async Task<IActionResult> AddToWatchers(int id)
        //{
        //    if (id != null)
        //    {
        //        try
        //        {
        //            TicketWatcher newTickWatch = new TicketWatcher();
        //            string userName = User.Identity.Name;
        //            ApplicationUser user = _context.Users.First(u => u.UserName == userName);
        //            Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);

        //            newTickWatch.Ticket = ticket;
        //            newTickWatch.Watcher = user;
        //            user.TicketWatching.Add(newTickWatch);
        //            ticket.TicketWatchers.Add(newTickWatch);
        //            _context.Add(newTickWatch);

        //            await _context.SaveChangesAsync();
        //            return RedirectToAction("Details", new { id });

        //        }
        //        catch (Exception ex)
        //        {
        //            return RedirectToAction("Error", "Home");
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}

        //public async Task<IActionResult> UnWatch(int id)
        //{
        //    if (id != null)
        //    {
        //        try
        //        {

        //            string userName = User.Identity.Name;
        //            ApplicationUser user = _context.Users.First(u => u.UserName == userName);
        //            Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
        //            TicketWatcher currTickWatch = await _context.TicketWatchers.FirstAsync(tw => tw.Ticket.Equals(ticket) && tw.Watcher.Equals(user));
        //            _context.TicketWatchers.Remove(currTickWatch);
        //            ticket.TicketWatchers.Remove(currTickWatch);
        //            user.TicketWatching.Remove(currTickWatch);

        //            await _context.SaveChangesAsync();
        //            return RedirectToAction("Details", new { id });

        //        }
        //        catch (Exception ex)
        //        {
        //            return RedirectToAction("Error", "Home");
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}

        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            if (id != null)
            {
                try
                {
                    Ticket ticket = _ticketBusinessLogic.GetTicketById(id);
                    ticket.Completed = true;
                    _ticketBusinessLogic.Save();
                    return RedirectToAction("Details", new { id });
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UnMarkAsCompleted(int id)
        {
            if (id != null)
            {
                try
                {
                    Ticket ticket = _ticketBusinessLogic.GetTicketById(id);
                    ticket.Completed = false;
                    _ticketBusinessLogic.Save();
                    return RedirectToAction("Details", new { id });
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }


        //// GET: Tickets/Delete/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = _ticketBusinessLogic.GetTicketById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> DeleteConfirmed(int id, int projId)
        {
            Ticket ticket = _ticketBusinessLogic.GetTicketById(id);

            Project project = _projectBusinessLogic.GetProject(projId);

            if (ticket == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tickets'  is null.");
            }

            if (ticket != null)
            {
                project.Tickets.Remove(ticket);
                _ticketBusinessLogic.RemoveTicket(ticket);
            }

            _ticketBusinessLogic.Save();
            
            return RedirectToAction("Index", "Projects");
        }

        private bool TicketExists(int id)
        {
            return (_ticketBusinessLogic.DoesTicketExist(id));
        }
    }
}

