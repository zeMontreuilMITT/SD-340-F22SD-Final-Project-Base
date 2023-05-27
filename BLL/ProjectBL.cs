using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using X.PagedList;
using System.Security.Claims;
using System.Linq;


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

            ApplicationUser activeUser = GetActiveUser(User);
            
            if(IsActiveUserDeveloper(User))
            {
                projects = RemoveProjectsActiveDeveloperNotAssignedTo(projects, activeUser);
            }

            List<ProjectIndexVM.BasicProjectData> formattedProjects = await FormatProjectsForIndexVM(projects, activeUser);
            
            // SortOrder sort and filter still needs to be done here

            IPagedList<ProjectIndexVM.BasicProjectData>pagedProjects = formattedProjects.ToPagedList(page ?? 1, 3);
            
            ProjectIndexVM vm = new(developers, pagedProjects );
            
            return vm;
        }
        
        public List<Project> RemoveProjectsActiveDeveloperNotAssignedTo(List<Project> projects, ApplicationUser activeDeveloper)
        {
            List<UserProject> userProjects = _userProjectRepo.GetAll().Where(up => up.UserId == activeDeveloper.Id).ToList();

            List<Project> projectsAssigned = new List<Project>();
            foreach(Project project in projects)
            {
                if(userProjects.Any(up => up.ProjectId == project.Id))
                {
                    projectsAssigned.Add(project);
                }
            }

            return projectsAssigned;
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

        public bool isUserWatchingOrOwner(Ticket ticket, ApplicationUser user)
        {
            if(ticket.OwnerId == user.Id)
            {
                return true;
            }

            TicketWatcher? ticketWatcher = _ticketWatcherRepo.GetAll().FirstOrDefault(tw => tw.WatcherId == user.Id && tw.TicketId == ticket.Id);

            if(ticketWatcher != null)
            {
                return true;
            }
            
            return false;

        }
        public async Task<List<ProjectIndexVM.BasicProjectData>> FormatProjectsForIndexVM(ICollection<Project> projects, ApplicationUser activeUser)
        {
            List<ProjectIndexVM.BasicProjectData> basicProjectData = new();
            
            foreach(Project project in projects)
            {
                List<ProjectIndexVM.BasicTicketData> basicTicketData = new();
                List<Ticket> ticketsOnProject = _ticketRepo.GetAll().Where(t => t.ProjectId == project.Id).ToList();

                foreach(Ticket ticket in ticketsOnProject)
                {
                    
                    ProjectIndexVM.BasicTicketData basicTicket = new ProjectIndexVM.BasicTicketData
                    {
                        Id = ticket.Id,
                        Title = ticket.Title,
                        OwnerName = GetUserNameFromId(ticket.OwnerId),
                        RequiredHours = ticket.RequiredHours,
                        Priority = ticket.TicketPriority.ToString(),
                        Watching = isUserWatchingOrOwner(ticket, activeUser),
                        Completed = ticket.Completed
                    };

                    basicTicketData.Add(basicTicket);
                }

                ProjectIndexVM.BasicProjectData basicProject = new ProjectIndexVM.BasicProjectData
                {
                    Id = project.Id,
                    Name = project.ProjectName,
                    CreatedBy = GetUserNameFromId(project.CreatedById),
                    AssignedTo = GetUsersOnProject(project),
                    Tickets = basicTicketData
                };
                basicProjectData.Add(basicProject);
            }

            return basicProjectData;
        }

        public string GetUserNameFromId(string id)
        {
            return _users.FindByIdAsync(id).Result.UserName;
        }

        public List<string> GetUsersOnProject(Project project)
        {
            List<UserProject> userProjects = _userProjectRepo.GetAll().Where(up => up.ProjectId == project.Id).ToList();

            return userProjects.Select(up => GetUserNameFromId(up.UserId)).ToList();
        }

        

        public Project? GetProject(int? id)
        {
            if (id == null) { return null; }

            Project? project = _projectRepo.Get((int)id);

            return project;
        }
        //Regan Positive and Fail Test
        public Project DeleteProject(int id)
        {
            // Get project
            Project project = _projectRepo.Get(id);

            if(project == null)
            {
                throw new ArgumentNullException();
            }

            List<Ticket> ticketsOnProject = _ticketRepo.GetAll().Where(t => t.ProjectId == id).ToList();

            List<UserProject> userProjectsOnProject = _userProjectRepo.GetAll().Where(up => up.ProjectId == id).ToList();

            foreach (Ticket ticket in ticketsOnProject)
            {
                _ticketRepo.Delete(ticket);
            }

            foreach (UserProject userProject in userProjectsOnProject)
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
        //RAG Positive Test and fail Test
        public void RemoveUserFromProject(string userId, int projectId)
        {
            if(userId == null)
            {
                throw new ArgumentNullException("userId");
            }

            UserProject? currentUserProject = _userProjectRepo.GetAll().FirstOrDefault(up => up.UserId == userId && up.ProjectId == projectId);
            if (currentUserProject != null)
            {
                _userProjectRepo.Delete(currentUserProject);
            }
        }
        // Paul Positive and Fail Test
        public void CreateProject(Project project, List<string> associatedUserIds)
        {
            if(associatedUserIds.Count < 1)
            {
                throw new ArgumentException("Must assign at least one user to project.");
            }

            associatedUserIds.ForEach((userId) =>
            {                
                UserProject newUserProj = new UserProject();
                newUserProj.UserId = userId;
                newUserProj.Project = project;
                project.AssignedTo.Add(newUserProj);
            });
            _projectRepo.Create(project);
        }

    }
}
