

using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using X.PagedList;
using System.Security.Claims;

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

        public async Task<ProjectIndexVM> ProjectIndex(string? sortOrder, int? page, bool? sort, string? userId, ClaimsPrincipal User)
        {
            List<SelectListItem> developers = GetDevelopersAsSelectList().Result;
            List<Project> projects = _projectRepo.GetAll().OrderBy(p => p.ProjectName).ToList();
            List<UserProject> userProjects = _userProjectRepo.GetAll().ToList();

            ApplicationUser activeUser = GetActiveUser(User);
            /*
                Project Values
                CreatedBy
                AssignedTo (UserProject) > ApplicationUser
               
            */

            
            if(IsActiveUserDeveloper(User))
            {
                foreach(Project project in projects)
                {
                    if(userProjects.Any(up => up.ProjectId == project.Id && up.UserId == activeUser.Id))
                    {
                        projects.Remove(project);
                    }
                }
            }
            

            IPagedList<Project>pagedProjects = projects.ToPagedList(page ?? 1, 3);
            ProjectIndexVM vm = new(developers, pagedProjects );
            // create vm
            return vm;
        }

        public List<Project> RemoveProjectsActiveDeveloperNotAssignedTo(List<Project> projects, ApplicationUser activeDeveloper)
        {
            List<UserProject> userProjects = _userProjectRepo.GetAll().Where(up => up.UserId == activeDeveloper.Id).ToList();

            foreach(Project project in projects)
            {
                if(!userProjects.Any(up => up.ProjectId == project.Id))
                {
                    projects.Remove(project);
                }
            }

            return projects;
        }
        public ApplicationUser GetActiveUser(ClaimsPrincipal User)
        {
            List<ApplicationUser> users = _userRepo.GetAll().ToList();

            return users.First(u => u.UserName == User.Identity.Name);
        }
        public bool IsActiveUserDeveloper(ClaimsPrincipal User)
        {
            return User.IsInRole("Developer");
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
