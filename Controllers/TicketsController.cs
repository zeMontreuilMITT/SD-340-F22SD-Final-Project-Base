using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
              return _context.Tickets != null ? 
                          View(await _context.Tickets.Include(t => t.Project).Include(t => t.AssignedUsers).ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Tickets'  is null.");
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.Include(u => u.AssignedUsers).ThenInclude(c => c.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            List<SelectListItem> currProjects = new List<SelectListItem>();
            _context.Projects.ToList().ForEach(t =>
            {
                currProjects.Add(new SelectListItem(t.ProjectName, t.Id.ToString()));
            });

            List<SelectListItem> currUsers = new List<SelectListItem>();
            _context.Users.ToList().ForEach(t =>
            {
                currUsers.Add(new SelectListItem(t.UserName, t.Id.ToString()));
            });

            ViewBag.Projects = currProjects;
            ViewBag.Users = currUsers;

            return View();

        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Body,RequiredHours,TicketPriority")] Ticket ticket, int projId, List<string> userIds)
        {
            if (ModelState.IsValid)
            { 
                ticket.Project = await _context.Projects.FirstAsync(p => p.Id == projId);
                userIds.ForEach((user) =>
                {
                    ApplicationUser currUser =  _context.Users.FirstOrDefault(u => u.Id == user);
                    ticket.AssignedUsers.Add(currUser);
                });
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.Include(t => t.AssignedUsers).FirstAsync(t => t.Id == id);
      
            if (ticket == null)
            {
                return NotFound();
            }

            List<ApplicationUser> results = _context.Users.Where(u => !ticket.AssignedUsers.Contains(u)).ToList();

            List<SelectListItem> currUsers = new List<SelectListItem>();
            results.ForEach(r =>
            {
                currUsers.Add(new SelectListItem(r.UserName, r.Id.ToString()));
            });
            ViewBag.Users = currUsers;

            return View(ticket);
        }

        
        public async Task<IActionResult> RemoveAssignedUser(string id, int ticketId)
        {
            if (id == null)
            {
                return NotFound();
            }
            Ticket currTicket = await _context.Tickets.Include(t => t.AssignedUsers).FirstAsync(t => t.Id == ticketId);
            ApplicationUser currUser = await _context.Users.FirstAsync(u => u.Id == id);
            currTicket.AssignedUsers.Remove(currUser);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Edit", new { id = ticketId });
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,List<string> userIds, [Bind("Id,Title,Body,RequiredHours")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    userIds.ForEach((user) =>
                    {
                        ApplicationUser currUser = _context.Users.FirstOrDefault(u => u.Id == user);
                        ticket.AssignedUsers.Add(currUser);
                    });
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Edit), new {id = ticket.Id});
            }
            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> CommentTask(int TaskId, string? TaskText)
        {
            if (TaskId != null || TaskText != null)
            {
                try
                {
                    Comment newComment = new Comment();
                    string userName = User.Identity.Name;
                    ApplicationUser user = _context.Users.First(u => u.UserName == userName);
                    Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Id == TaskId);

                    newComment.CreatedBy = user;
                    newComment.Description = TaskText;
                    newComment.Ticket = ticket;
                    user.Comments.Add(newComment);
                    _context.Comments.Add(newComment);
                    ticket.Comments.Add(newComment);

                    int Id = TaskId;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new {Id});

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tickets == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tickets'  is null.");
            }
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
          return (_context.Tickets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

