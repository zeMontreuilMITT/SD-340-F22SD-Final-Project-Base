using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using System.Web.Mvc;
using X.PagedList;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class ProjectBusinessLogic
    {
        private readonly ProjectRepo _projectRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserProjectRepo _userProjectRepo;

        public ProjectBusinessLogic(ProjectRepo projectRepo, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, UserProjectRepo userProjectRepo)
        {
            _projectRepo = projectRepo;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _userProjectRepo = userProjectRepo;
        }


        public async Task<ProjectIndexVM> Index(string? sortOrder, int? page, bool? sort, string? userId)
        {
            ProjectIndexVM vm = new ProjectIndexVM();
            List<Project> SortedProjs = new List<Project>();
            List<ApplicationUser> allUsers = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Developer");

            List<SelectListItem> users = new List<SelectListItem>();
            allUsers.ForEach(au =>
            {
                users.Add(new SelectListItem(au.UserName, au.Id.ToString()));
            });
            vm.Users = users;
            SortedProjs = _projectRepo.GetAll().ToList();
            switch (sortOrder)
            {
                case "Priority":
                    if (sort == true)
                    {
                        foreach (Project project in SortedProjs)
                        {
                            project.Tickets = project.Tickets
                                .OrderByDescending(t => t.TicketPriority)
                                .ToList();
                        }
                    }
                    else
                    {
                        foreach (Project project in SortedProjs)
                        {
                            project.Tickets = project.Tickets
                                .OrderBy(t => t.TicketPriority)
                                .ToList();
                        }
                    }

                    break;
                case "RequiredHrs":
                    if (sort == true)
                    {
                        foreach (Project project in SortedProjs)
                        {
                            project.Tickets = project.Tickets
                                .OrderByDescending(t => t.RequiredHours)
                                .ToList();
                        }
                    }
                    else
                    {
                        foreach (Project project in SortedProjs)
                        {
                            project.Tickets = project.Tickets
                                .OrderBy(t => t.RequiredHours)
                                .ToList();
                        }
                    }

                    break;
                case "Completed":
                    foreach (Project project in SortedProjs)
                    {
                        project.Tickets = project.Tickets
                            .Where(t => t.Completed == true)
                            .ToList();
                    }
                    break;
                default:
                    if (userId != null)
                    {
                        SortedProjs = SortedProjs
                            .OrderBy(p => p.ProjectName)
                            .ToList();
                        foreach (Project project in SortedProjs)
                        {
                            project.Tickets = project.Tickets
                                .Where(t => t.Owner.Id.Equals(userId)).ToList();
                        }
                    }
                    else
                    {
                        SortedProjs = SortedProjs.OrderBy(p => p.ProjectName).ToList();
                    }

                    break;
            }
            //check if User is PM or Develoer
            var LoggedUserName = _contextAccessor.HttpContext.User.Identity.Name;  // logined user name
            var user = _userManager.Users.FirstOrDefault(u => u.UserName == LoggedUserName);
            var rolenames = await _userManager.GetRolesAsync(user);
            var AssignedProject = new List<Project>();
            // geting assined project
            if (rolenames.Contains("Developer"))
            {
                AssignedProject = SortedProjs.Where(p => p.AssignedTo.Select(projectUser => projectUser.UserId).Contains(user.Id)).ToList();
            }
            else
            {
                AssignedProject = SortedProjs;
            }
            X.PagedList.IPagedList<Project> projList = AssignedProject.ToPagedList(page ?? 1, 3);

            vm.Projects = projList;
            return vm;
        }

        public Project Details(int id)
        {
            Project project = _projectRepo.Get(id);
            return project;
        }

        public void RemoveAssignedUser(string id, int projId)
        {
            UserProject userProject = _userProjectRepo.GetAll()
                .First(up => up.UserId == id && up.ProjectId == projId);

            _userProjectRepo.Delete(userProject);
            return;
        }

        public async Task<ProjectCreateVM> CreateAsync()
        {
            List<ApplicationUser> allUsers = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Developer");

            List<SelectListItem> users = new List<SelectListItem>();
            allUsers.ForEach(au =>
            {
                users.Add(new SelectListItem(au.UserName, au.Id.ToString()));
            });

            ProjectCreateVM vm = new ProjectCreateVM();

            vm.Users = users;

            return vm;
        }

        public async void Create(ProjectCreateVM vm, List<string> userIds)
        {
            string userName = _contextAccessor.HttpContext.User.Identity.Name;

            Project project = new Project();

            project.ProjectName = vm.ProjectName;
            project.Id = vm.Id;

            ApplicationUser createdBy = _userManager.Users.First(u => u.UserName == userName);
            userIds.ForEach((user) =>
            {
                ApplicationUser currUser = _userManager.Users.FirstOrDefault(u => u.Id == user);
                UserProject newUserProj = new UserProject();
                newUserProj.ApplicationUser = currUser;
                newUserProj.UserId = currUser.Id;
                newUserProj.Project = project;
                project.AssignedTo.Add(newUserProj);
                _userProjectRepo.Create(newUserProj);
            });
            _projectRepo.Create(project);
            return;
        }

        private async Task<bool> ProjectExists(int id)
        {
            List<Project> projects = await _projectRepo.GetAll().ToListAsync();
            if (projects.Any(p => p.Id == id))
            {
                return true;
            }
            return false;
        }
    }
}
