using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;

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

        public TicketBL(IRepository<Project> projectRepo, IRepository<Ticket> ticketRepo, IRepository<Comment> commentRepo, IRepository<TicketWatcher> ticketWatcherRepo, IRepository<UserProject> userProjectRepo, UserRepo userRepo)
        {
            _projectRepo = projectRepo;
            _ticketRepo = ticketRepo;
            _commentRepo = commentRepo;
            _ticketWatcherRepo = ticketWatcherRepo;
            _userProjectRepo = userProjectRepo;
            _userRepo = userRepo;
        }
    }
}
