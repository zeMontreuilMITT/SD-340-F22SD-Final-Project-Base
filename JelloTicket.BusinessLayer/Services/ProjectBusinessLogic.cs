using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JelloTicket.DataLayer.Repositories;
using JelloTicket.DataLayer.Models;
using JelloTicket.BusinessLayer.HelperLibrary;
using Microsoft.AspNetCore.Identity;
using System.Collections;
using System.Security.Claims;
using System.ComponentModel;

namespace JelloTicket.BusinessLayer.Services
{
    public class ProjectBusinessLogic
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HelperMethods _helperMethods;
        private readonly UserManagerBusinessLogic _userManagerBusinessLogic;
        private readonly IRepository<UserProject> _userProjectRepository;

        public ProjectBusinessLogic(IRepository<Project> projectRepository
            , UserManager<ApplicationUser> userManager
            , UserManagerBusinessLogic userManagerBusinessLogic
            , IRepository<UserProject> userProjectRepository)
        {
            _projectRepository = projectRepository;
            _userManager = userManager;
            _helperMethods = new HelperMethods();
            _userManagerBusinessLogic = userManagerBusinessLogic;
            _userProjectRepository = userProjectRepository;
        }

        public ICollection<Project> GetProjectsWithAssociations()
        {
            ICollection<Project> projects = _projectRepository.GetAll();

            return projects;
        }

        public List<Project> SortProjects(string? sortOrder, bool? sort)
        {
            ICollection<Project> projects = (ICollection<Project>)GetProjectsWithAssociations();

            foreach (var project in projects)
            {
                switch (sortOrder)
                {
                    case "Priority":
                        project.Tickets = sort == true
                            ? project.Tickets.OrderByDescending(t => t.TicketPriority).ToList()
                            : project.Tickets.OrderBy(t => t.TicketPriority).ToList();
                        break;
                    case "RequiredHrs":
                        project.Tickets = sort == true
                            ? project.Tickets.OrderByDescending(t => t.RequiredHours).ToList()
                            : project.Tickets.OrderBy(t => t.RequiredHours).ToList();
                        break;
                    case "Completed":
                        project.Tickets = project.Tickets.Where(t => (bool)t.Completed).ToList();
                        break;
                    default:
                        // No sorting for tickets within the project
                        break;
                }
            }

            return projects.ToList();
        }

        public async Task<List<Project>> GetAssignedDeveloperProjects(ClaimsPrincipal user, List<Project> projects, ApplicationUser applicationUser, string? sortOrder, bool? sort)
        {
            IList<String> roles = await _userManager.GetRolesAsync(_userManagerBusinessLogic.GetLoggedInUser(user).Result);

            if (roles.Contains("Developer"))
            {
                return projects.Where(p => p.AssignedTo
                    .Select(projectUser => projectUser.UserId).Contains(applicationUser.Id)).ToList();
            }
            else
            {
                return SortProjects(sortOrder, sort);
            }
        }

        public Project GetProject(int? id)
        {
            return _projectRepository.Get(id);
        }

        public bool CreateProject(Project project)
        {
            try
            {
                _projectRepository.Create(project);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public async Task<bool> BuildProjectModel(List<String> userIds, Project project, ApplicationUser createdBy)
        {
            try
            {
                foreach (String userId in userIds)
                {
                    ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

                    // build the joining table model
                    UserProject newUserProject = new UserProject()
                    {
                        ApplicationUser = currentUser,
                        UserId = currentUser.Id,
                        Project = project
                    };

                    project.AssignedTo.Add(newUserProject);
                    project.CreatedBy = createdBy;
                    _userProjectRepository.Create(newUserProject);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }
    }
}
