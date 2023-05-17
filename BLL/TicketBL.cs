using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using X.PagedList;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class TicketBL
    {
        private IRepository<Project> _projectRepo; 
        private IRepository<Ticket> _ticketRepo;
        private IRepository<Comment> _commentRepo;
        private IRepository<TicketWatcher> _ticketWatcherRepo;
        private IRepository<UserProject> _userProjectRepo;
        private UserRepo _userRepo;
        private readonly UserManager<ApplicationUser> _users;

        public TicketBL(IRepository<Project> projectRepo, IRepository<Ticket> ticketRepo, IRepository<Comment> commentRepo, IRepository<TicketWatcher> ticketWatcherRepo, IRepository<UserProject> userProjectRepo, UserRepo userRepo, UserManager<ApplicationUser> users)
        {
            _projectRepo = projectRepo;
            _ticketRepo = ticketRepo;
            _commentRepo = commentRepo;
            _ticketWatcherRepo = ticketWatcherRepo;
            _userProjectRepo = userProjectRepo;
            _userRepo = userRepo;
            _users = users;
        }
        public async Task<ICollection<Ticket>> GetAllTickets()
        {
            return await _ticketRepo.GetAll().ToListAsync();
        }
        public async Task<Ticket> GetTicketDetails(int? id)
        {
            if (id == null)
            {
                return null;
            }

            Ticket ticket = _ticketRepo.Get(id.Value);

            if (ticket == null)
            {
                return null;
            }

            ticket.Project = _projectRepo.Get(ticket.ProjectId);
            ticket.TicketWatchers = _ticketWatcherRepo.GetAll().Where(x => x.TicketId == id).ToList();
            ticket.Owner = await _users.FindByIdAsync(ticket.OwnerId);
            ticket.Comments = _commentRepo.GetAll().Where(x => x.TicketId == id).ToList();

            return ticket;

        }

        
    }
}
