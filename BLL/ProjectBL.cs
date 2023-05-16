

using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class ProjectBL
    {
        private readonly IRepository<Project> _projectRepo;
        private readonly IRepository<Ticket> _ticketRepo;
        private readonly IRepository<Comment> _commentRepo;
        private readonly IRepository<TicketWatcher> _ticketWatcherRepo;
        private readonly IRepository<UserProject> _userProjectRepo;
        private readonly UserRepo _userRepo;
        private readonly UserManager<ApplicationUser> _users;

        public ProjectBL(IRepository<Project> projectRepo, IRepository<Ticket> ticketRepo, IRepository<Comment> commentRepo, IRepository<TicketWatcher> ticketWatcherRepo, IRepository<UserProject> userProjectRepo, UserRepo userRepo, UserManager<ApplicationUser> users)
        {
            _projectRepo = projectRepo;
            _ticketRepo = ticketRepo;
            _commentRepo = commentRepo;
            _ticketWatcherRepo = ticketWatcherRepo;
            _userProjectRepo = userProjectRepo;
            _userRepo = userRepo;
            _users = users;
        }


        public Project? GetProject(int? id)
        {
            if (id == null) { return null; }

            Project? project = _projectRepo.Get((int)id);

            return project;
        }

        public Project DeleteProject(int id)
        {
            // Get project
            Project project = _projectRepo.Get(id);

            List<Ticket> ticketsOnProject = _ticketRepo.GetAll().Where(t => t.ProjectId == id).ToList();

            List<UserProject> userProjectsOnProject = _userProjectRepo.GetAll().Where(up => up.ProjectId == id).ToList();

            foreach(Ticket ticket in ticketsOnProject)
            {
                _ticketRepo.Delete(ticket);
            }

            foreach(UserProject userProject in userProjectsOnProject)
            {
                _userProjectRepo.Delete(userProject);
            }

            return _projectRepo.Delete(project);
        }


        public async Task<List<SelectListItem>> GetDevelopersAsSelectList()
        {
            List<ApplicationUser> allDevelopers = (List<ApplicationUser>)await _users.GetUsersInRoleAsync("Developer");
            
            
            List<SelectListItem> developers = new();

            allDevelopers.ForEach(developer =>
            {
                developers.Add(new SelectListItem(developer.UserName, developer.Id.ToString()));
            });

            return developers;
        }

    }
}
