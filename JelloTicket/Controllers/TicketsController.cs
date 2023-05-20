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
using JelloTicket.BusinessLayer.ViewModels;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly TicketBusinessLogic _ticketBusinessLogic;
        private readonly ProjectBusinessLogic _projectBusinessLogic;
        private readonly UserManager<ApplicationUser> _users;
        private readonly IRepository<Comment> _commentRepo;
        private readonly UserManagerBusinessLogic _userManagerBusinessLogic;
        
        public TicketsController(IRepository<Ticket> ticketRepo
            , IRepository<Project> projectRepo
            , IRepository<Comment> commentRepo
            , UserManager<ApplicationUser> users
            , UserManagerBusinessLogic userManagerBusinessLogic
            , ProjectBusinessLogic projectBusinessLogic
            , TicketBusinessLogic ticketBusinessLogic)
        {
            _ticketBusinessLogic = ticketBusinessLogic;
            _users = users;
            _userManagerBusinessLogic = userManagerBusinessLogic;
            _commentRepo = commentRepo;
            _projectBusinessLogic = projectBusinessLogic;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            return View(_ticketBusinessLogic.GetTickets().ToList());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Ticket ticket = _ticketBusinessLogic.GetTicketById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            ViewBag.Users = _userManagerBusinessLogic.AllUserSelectListItem();

            return View(ticket);
        }

        //// GET: Tickets/Create
        [Authorize(Roles = "ProjectManager")]
        public IActionResult Create(int projId)
        {

            TicketCreateVM vm = _ticketBusinessLogic.CreateGet(projId);
            return View(vm);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create([Bind("Id,Title,Body,RequiredHours,TicketPriority")] Ticket ticket, int projId, string userId)
        {
            if (ModelState.IsValid)
            {

                _ticketBusinessLogic.CreatePost(projId, userId, ticket);
                return RedirectToAction("Index", "Projects", new { area = "" });
            }
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            Ticket ticket = _ticketBusinessLogic.GetTicketById(id);
            TicketEditVM vm = _ticketBusinessLogic.EditGet(ticket);
            return View(vm);

        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int id, string userId, TicketEditVM ticketVM)
        {
            _ticketBusinessLogic.EditTicket(ticketVM, id, userId);

            return RedirectToAction("Index", "Projects");

        }

        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> RemoveAssignedUser(string? id, int ticketId)
        {
            _ticketBusinessLogic.RemoveUser(id, ticketId);

            return RedirectToAction("Edit", new { id = ticketId });
        }
                
        [HttpPost]
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

        [HttpPost]
        public async Task<IActionResult> UpdateHrs(int id, int hrs)
        {
            if (id != null || hrs != null)
            {
                try
                {
                    Ticket ticket = _ticketBusinessLogic.GetTicketById(id);
                    ticket.RequiredHours = hrs;
                    _ticketBusinessLogic.Save();
                    return RedirectToAction("Index", "Projects");

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index", "Projects");
        }

        public async Task<IActionResult> AddToWatchers(int id)
        {

            string userName = User.Identity.Name;
            _ticketBusinessLogic.AddtoWatchers(userName, id);
            return RedirectToAction("Details", new { id });

        }

        public async Task<IActionResult> UnWatch(int id)
        {
            string userName = User.Identity.Name;
            _ticketBusinessLogic.UnWatch(userName, id);
            return RedirectToAction("Details", new { id });

        }

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

