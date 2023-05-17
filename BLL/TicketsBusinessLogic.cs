using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class TicketsBusinessLogic
    {
        private IRepository<Ticket> _ticketRepo;
        private ITicketRepository _ticketSpecialMethodRepo;
        private IRepository<Comment> _commentRepo;
        private IRepository<TicketWatcher> _ticketWatcherRepo;
        private UserRepository _userRepository;
         

        public TicketsBusinessLogic(IRepository<Ticket> ticketRepo, ITicketRepository ticketSpecialMethodRepo,IRepository<Comment> commentRepo, IRepository<TicketWatcher> ticketWatcherRepo, UserManager<ApplicationUser> userManager)
        {
            _ticketRepo = ticketRepo;
            _commentRepo = commentRepo;
            _ticketSpecialMethodRepo = ticketSpecialMethodRepo;
            _ticketWatcherRepo = ticketWatcherRepo;
            
            _userRepository = new UserRepository(userManager);
        }

        public async Task<ICollection<Ticket>> GetAllTickets()
        {
            ICollection<Ticket> tickets = await _ticketRepo.GetAll();
            if (tickets == null)
            {
                throw new Exception("Entity set 'ApplicationDbContext.Tickets'  is null."); 
            }
            return await _ticketRepo.GetAll();
        }
        public async Task<Ticket?> GetTicketById(int? id)
        {
            if (id == null)
            {
                throw new Exception("Id can't be null!");
            }

            Ticket? ticket = await _ticketRepo.Get(id);

            if (ticket == null)
            {
                throw new Exception("Ticket is null.");
            }

            return ticket;
        }

        public async Task<Project?> GetProjectById(int? id)
        {
            if (id == null)
            {
                throw new Exception("Id can't be null!");
            }

            Project? project = await _ticketSpecialMethodRepo.GetProectById(id);

            if (project == null)
            {
                throw new Exception("Project is null!");
            }

            return project;
        }

        public async Task<ProjectAndTicketAndUsersViewModel> GetCreateViewModel(Project? project)
        {
            if (project == null)
            {
                throw new Exception("Project is null.");
            }

            HashSet<ApplicationUser> users = new HashSet<ApplicationUser>();
			project.AssignedTo.ToList().ForEach(t =>
            {
                users.Add(t.ApplicationUser);
            });
          
            ProjectAndTicketAndUsersViewModel projectTicketAndUsersVM = new ProjectAndTicketAndUsersViewModel(project,new Ticket(), users);
            return projectTicketAndUsersVM;
        }

		public async Task<ProjectAndTicketAndUsersViewModel> GetEditViewModel(Ticket? ticket)
		{
			if (ticket == null)
			{
				throw new Exception("Ticket is null.");
			}

			HashSet<ApplicationUser> users = new HashSet<ApplicationUser>();
			List<ApplicationUser> results = _userRepository.GetAllUsers().Where(u => u != ticket.Owner).ToList();
            results.ForEach(r =>
            {
				users.Add(r);
            });           

            ProjectAndTicketAndUsersViewModel projectTicketAndUsersVM = new ProjectAndTicketAndUsersViewModel(null, ticket, users);
			return projectTicketAndUsersVM;
		}

		public async Task Create(int? projId,string userId,Ticket ticket)
        {
            if (projId == null)
            {
                throw new Exception("ProjectId can't be null!");
            }
            if (userId == null)
            {
                throw new Exception("UserId can't be null!");
            }
            if (ticket == null)
            {
                throw new Exception("Ticket can't be null!");
            }

            Project? currProject = await GetProjectById(projId);
            ticket.Project = currProject;
            ApplicationUser owner = _userRepository.Get(userId);
            ticket.Owner = owner;

            await _ticketRepo.Create(ticket);

        }
        public async Task Update(Ticket ticket,string userId)
        {
            if (userId == null)
            {
                throw new Exception("UserId can't be null!");
            }
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }
            ApplicationUser user = _userRepository.Get(userId);
            ticket.Owner = user;
            await _ticketRepo.Update(ticket);

        }
        public async Task Del(int? ticketId,int? projectId)
        {
            if (ticketId == null)
            {
                throw new Exception("TicketId can't be null!");
            }
            if (projectId == null)
            {
                throw new Exception("ProjectId can't be null!");
            }
            Ticket? ticket= await _ticketRepo.Get(ticketId);
            await _ticketRepo.Delete(ticket) ;

        }

        public async Task AddComment(int? taskId, string? taskText,string userName)
        {
            if (taskId == null)
            {
                throw new Exception("TaskId can't be null!");
            }
            if (taskText == null)
            {
                throw new Exception("TaskText can't be null!");
            }

            Comment newComment = new Comment();
            ApplicationUser user = _userRepository.Get(userName);
            newComment.CreatedBy = user;
            newComment.Description = taskText;
            newComment.TicketId = taskId;

            await _commentRepo.Create(newComment);
        }
        public async Task UpdateHrs(int? taskId,int? hours)
        {
            if (taskId == null)
            {
                throw new Exception("TaskId can't be null!");
            }
            if (hours == null)
            {
                throw new Exception("Hours can't be null!");
            }
            Ticket? ticket = await _ticketRepo.Get(taskId);

            if (ticket == null)
            {
                throw new Exception("Ticket is null!");
            }

            ticket.RequiredHours = hours;
            await _ticketRepo.Update(ticket);
        }

        public async Task AddWatcher(int? taskId, string userName)
        {
            if (taskId == null)
            {
                throw new Exception("TaskId can't be null!");
            }
            TicketWatcher newTickWatch = new TicketWatcher();
            ApplicationUser user = _userRepository.Get(userName);
            newTickWatch.Watcher = user;
            newTickWatch.TicketId = taskId;

            await _ticketWatcherRepo.Create(newTickWatch);
        }

        public async Task UnWatcher(int? taskId, string userName)
        {
            if (taskId == null)
            {
                throw new Exception("TaskId can't be null!");
            }
            ApplicationUser user = _userRepository.Get(userName);
            ICollection<TicketWatcher> ticketWatchers= await _ticketWatcherRepo.GetAll();
            TicketWatcher ticketWatcher=ticketWatchers.First(tw => tw.TicketId == taskId && tw.Watcher.Equals(user));
            await _ticketWatcherRepo.Delete(ticketWatcher);
        }

        public async Task MarkAsCompleted(int? taskId)
        {
            if (taskId == null)
            {
                throw new Exception("TaskId can't be null!");
            }
            Ticket? ticket = await _ticketRepo.Get(taskId);

            if (ticket == null)
            {
                throw new Exception("Ticket is null!");
            }


            ticket.Completed = true;
            await _ticketRepo.Update(ticket);
           
        }
        public async Task MarkAsUnCompleted(int? taskId)
        {
            if (taskId == null)
            {
                throw new Exception("TaskId can't be null!");
            }

            Ticket? ticket = await _ticketRepo.Get(taskId);

            if (ticket == null)
            {
                throw new Exception("Ticket is null!");
            }

            ticket.Completed = false;

            await _ticketRepo.Update(ticket);
        }
    }
}
