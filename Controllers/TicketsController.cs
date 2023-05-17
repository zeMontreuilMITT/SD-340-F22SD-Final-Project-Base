using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly TicketsBusinessLogic _ticketsBusinessLogic;
        


        public TicketsController(ApplicationDbContext context, IRepository<Ticket> ticketRepo,ITicketRepository ticketSpecialMethodRepo, IRepository<Comment> commentRepo, IRepository<TicketWatcher> ticketWatcherRepo, UserManager<ApplicationUser> userManager)
        {
            _context = context;
             
            _ticketsBusinessLogic = new TicketsBusinessLogic(ticketRepo,ticketSpecialMethodRepo, commentRepo,ticketWatcherRepo, userManager);

        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            try
            {
                ICollection<Ticket> allTickets = await _ticketsBusinessLogic.GetAllTickets();
                return View(allTickets);
            }
            catch (Exception ex)
            {
               return Problem(ex.Message);
            }
           
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                Ticket? ticket = await _ticketsBusinessLogic.GetTicketById(id);
               
                return View(ticket);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
         }

        // GET: Tickets/Create
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create(int projId)
        {
            try
            {
                Project? currProject = await _ticketsBusinessLogic.GetProjectById(projId);

               ProjectAndTicketAndUsersViewModel projectAndTicketAndUsersViewModel= await _ticketsBusinessLogic.GetCreateViewModel(currProject);
 
                return View(projectAndTicketAndUsersViewModel);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create(ProjectAndTicketAndUsersViewModel vm, int projId, string userId)
        {
            try
            {
                if (ModelState.IsValid)
                { 
                    await _ticketsBusinessLogic.Create(projId, userId, vm.curTicket);
                    return RedirectToAction("Index", "Projects", new { area = "" });
                }
                return View(vm);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var ticket = await _ticketsBusinessLogic.GetTicketById(id);
 
                ProjectAndTicketAndUsersViewModel projectAndTicketAndUsersViewModel = await _ticketsBusinessLogic.GetEditViewModel(ticket);

				return View(projectAndTicketAndUsersViewModel);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }
         
        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int id,string userId, ProjectAndTicketAndUsersViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                 {                   
                   await _ticketsBusinessLogic.Update(vm.curTicket, userId);             
                   return RedirectToAction(nameof(Edit), new {id = vm.curTicket.Id});
                 }
                 return View(vm.curTicket);
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CommentTask(int TaskId, string? TaskText)
        {

            try
            {
                string userName = User.Identity.Name;
                int Id = TaskId;
                await _ticketsBusinessLogic.AddComment(TaskId, TaskText, userName);
                return RedirectToAction("Details", new { Id });

            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        public async Task<IActionResult> UpdateHrs(int id, int hrs)
        {
          
                try
                {
                    await _ticketsBusinessLogic.UpdateHrs(id, hrs);
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                   return Problem(ex.ToString());
                 }
           
        }

        public async Task<IActionResult> AddToWatchers(int id)
        {
          
                try
                {
                    await _ticketsBusinessLogic.AddWatcher(id, User.Identity.Name);
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return Problem(ex.ToString());
            }
            
        }

        public async Task<IActionResult> UnWatch(int id)
        {
            
                try
                {

                    string userName = User.Identity.Name;
                    await  _ticketsBusinessLogic.UnWatcher(id, userName);

                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return Problem(ex.ToString());
                }
           
        }

        public async Task<IActionResult> MarkAsCompleted(int id)
        {          
                try
                {
                    await _ticketsBusinessLogic.MarkAsCompleted(id);
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return Problem(ex.ToString());
                 }
            
        }

        public async Task<IActionResult> UnMarkAsCompleted(int id)
        {
           
                try
                {
  
                   await _ticketsBusinessLogic.MarkAsUnCompleted(id);
                    return RedirectToAction("Details", new { id });

                }
                catch (Exception ex)
                {
                    return Problem(ex.ToString());
                }
            
        }


        // GET: Tickets/Delete/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            try 
            { 
                Ticket? ticket = await _ticketsBusinessLogic.GetTicketById(id);
 
                return View(ticket);
            }
            catch(Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> DeleteConfirmed(int id, int projId)
        {
           
            try
            {
                await _ticketsBusinessLogic.Del(id, projId);

                return RedirectToAction("Index", "Projects");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
 
    }
}

