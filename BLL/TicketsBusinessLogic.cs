using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using System.Data;
using System.Web.Mvc;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class TicketsBusinessLogic
    {

        private readonly ApplicationDbContext _context;

        public TicketsBusinessLogic(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<IActionResult> Index()
        //{
            
        //}

        public async Task<List<Ticket>> GetTickets()
        {
            return await _context.Tickets
                .Include(t => t.Project)
                .Include(t => t.Owner)
                .ToListAsync();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return new NotFoundResult();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Project)
                .Include(t => t.TicketWatchers).ThenInclude(tw => tw.Watcher)
                .Include(u => u.Owner)
                .Include(t => t.Comments).ThenInclude(c => c.CreatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);

            List<SelectListItem> currUsers = new List<SelectListItem>();
            ticket.Project.AssignedTo.ToList().ForEach(t =>
            {
                currUsers.Add(new SelectListItem(t.ApplicationUser.UserName, t.ApplicationUser.Id.ToString()));
            });

            if (ticket == null)
            {
                return new NotFoundResult();
            }

            // Process the ticket details

            return new Microsoft.AspNetCore.Mvc.ViewResult();
        }


        public IActionResult Create(int projId)
        {
            Project currProject = _context.Projects
                .Include(p => p.AssignedTo)
                .ThenInclude(at => at.ApplicationUser)
                .FirstOrDefault(p => p.Id == projId);

            List<SelectListItem> currUsers = new List<SelectListItem>();
            currProject.AssignedTo.ToList().ForEach(t =>
            {
                currUsers.Add(new SelectListItem(t.ApplicationUser.UserName, t.ApplicationUser.Id.ToString()));
            });

            // Process the creation logic

            return new Microsoft.AspNetCore.Mvc.ViewResult();
        }

    }
}
