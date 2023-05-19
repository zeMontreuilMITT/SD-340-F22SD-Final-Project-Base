using AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using System.Data;
using System.Threading.Tasks;
using System.Web.Mvc;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class TicketsBusinessLogic
    {

        private readonly IRepository<Project> _projectRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRepository<UserProject> _userProjectRepo;
        private readonly IRepository<Ticket> _ticketRepo;
        private readonly IRepository<Comment> _commentRepo;
        private readonly IRepository<TicketWatcher> _ticketWatcherRepo;

        public TicketsBusinessLogic(IRepository<Project> projectRepo, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, IRepository<UserProject> userProjectRepo, IRepository<Ticket> ticketRepo, IRepository<Comment> commentRepo, IRepository<TicketWatcher> ticketWatcherRepo)
        {
            _projectRepo = projectRepo;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _userProjectRepo = userProjectRepo;
            _ticketRepo = ticketRepo;
            _commentRepo = commentRepo;
            _ticketWatcherRepo = ticketWatcherRepo;
        }

        //public async Task<IActionResult> Index()
        //{

        //}

        public async Task<List<Ticket>> Index()
        {
            return _ticketRepo.GetAll().ToList();
        }

        public async Task<TicketVM> Details(int id)
        {
            var ticket = _ticketRepo.Get(id);

            List<SelectListItem> currUsers = new List<SelectListItem>();
            ticket.Project.AssignedTo.ToList().ForEach(t =>
            {
                currUsers.Add(new SelectListItem(t.ApplicationUser.UserName, t.ApplicationUser.Id.ToString()));
            });

            TicketVM vm = new TicketVM();
            vm.CurrUsers = currUsers;
            vm.Ticket = ticket;
            vm.CurrProject = ticket.Project;

            // Process the ticket details

            return vm;
        }


        public async Task<TicketCreateVM> Create(int projId)
        {
            Project currProject = _projectRepo.Get(projId);

            List<SelectListItem> currUsers = new List<SelectListItem>();
            currProject.AssignedTo.ToList().ForEach(t =>
            {
                currUsers.Add(new SelectListItem(t.ApplicationUser.UserName, t.ApplicationUser.Id.ToString()));
            });

            TicketCreateVM vm = new TicketCreateVM();

            vm.Project = currProject;
            vm.Users = currUsers;

            return vm;
        }

        public async Task<Ticket> CreatePost(Ticket ticket, int projId, string userId)
        {
            Project currProj = _projectRepo.Get(projId);
            ticket.Project = currProj;
            ApplicationUser owner = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            ticket.Owner = owner;
            _ticketRepo.Create(ticket);
            currProj.Tickets.Add(ticket);
            _projectRepo.Update(currProj);
            return ticket;
        }

        public async void RemoveAssignedUser(string id, int ticketId)
        {
            Ticket currTicket = _ticketRepo.Get(ticketId);
            ApplicationUser currUser = await _userManager.Users.FirstAsync(u => u.Id == id);
            //To be fixed ASAP
            currTicket.Owner = currUser;
            _ticketRepo.Update(currTicket);
        }

        public async void Edit(int id, string userId, Ticket ticket)
        {
            ApplicationUser currUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            ticket.Owner = currUser;
            _ticketRepo.Update(ticket);

            return;
        }

        public async void CommentTask(int taskId, string taskText)
        {
            Comment newComment = new Comment();
            string userName = _contextAccessor.HttpContext.User.Identity.Name;
            ApplicationUser user = _userManager.Users.First(u => u.UserName == userName);
            Ticket ticket = _ticketRepo.Get(taskId);

            newComment.CreatedBy = user;
            newComment.Description = taskText;
            newComment.Ticket = ticket;
            user.Comments.Add(newComment);
            _commentRepo.Create(newComment);
            ticket.Comments.Add(newComment);
            _ticketRepo.Update(ticket);
        }

        public async void UpdateHrs(int id, int hrs)
        {
            Ticket ticket = _ticketRepo.Get(id);
            ticket.RequiredHours = hrs;

            _ticketRepo.Update(ticket);
        }

        public async void AddToWatchers(int id)
        {
            TicketWatcher newTickWatch = new TicketWatcher();
            string userName = _contextAccessor.HttpContext.User.Identity.Name;
            ApplicationUser user = _userManager.Users.First(u => u.UserName == userName);
            Ticket ticket = _ticketRepo.Get(id);

            newTickWatch.Ticket = ticket;
            newTickWatch.Watcher = user;
            user.TicketWatching.Add(newTickWatch);
            ticket.TicketWatchers.Add(newTickWatch);

            _ticketWatcherRepo.Create(newTickWatch);
            await _userManager.UpdateAsync(user);
            _ticketRepo.Update(ticket);
        }

        public async void UnWatch(int id)
        {
            string userName = _contextAccessor.HttpContext.User.Identity.Name;
            ApplicationUser user = _userManager.Users.First(u => u.UserName == userName);
            Ticket ticket = _ticketRepo.Get(id);
            TicketWatcher currTickWatch = _ticketWatcherRepo.GetAll().First(tw => tw.Ticket.Equals(ticket) && tw.Watcher.Equals(user));
            _ticketWatcherRepo.Delete(currTickWatch);
            ticket.TicketWatchers.Remove(currTickWatch);
            _ticketRepo.Update(ticket);
            user.TicketWatching.Remove(currTickWatch);
        }

        public void MarkAsCompleted(int id)
        {
            Ticket ticket = _ticketRepo.Get(id);
            ticket.Completed = true;
            _ticketRepo.Update(ticket);
        }

        public void UnMarkAsCompleted(int id)
        {
            Ticket ticket = _ticketRepo.Get(id);
            ticket.Completed = false;
            _ticketRepo.Update(ticket);
        }

        public async Task<Ticket> Delete(int id)
        {
            Ticket ticket = _ticketRepo.Get(id);
            return ticket;
        }

        public async void DeleteConfirmed(int id, int projId)
        {
            Ticket ticket = _ticketRepo.Get(id);
            Project currProj = _projectRepo.Get(projId);
            currProj.Tickets.Remove(ticket);
            _projectRepo.Update(currProj);
            _ticketRepo.Delete(ticket);
        }
    }
}
