using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using JelloTicket.DataLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Mvc;
using JelloTicket.BusinessLayer.Services;
using System.Security.Claims;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize(Roles = "ProjectManager, Developer")]
    public class ProjectsController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _users;
        private readonly ProjectBusinessLogic _projectBusinessLogic;
        private readonly UserManagerBusinessLogic _userManagerBusinessLogic;
        private readonly IRepository<UserProject> _userProjectRepository;
        private readonly IRepository<Ticket> _ticketRepository;

        public ProjectsController(IRepository<Project> projectRepo
            , UserManager<ApplicationUser> users
            , UserManagerBusinessLogic userManagerBusinessLogic
            , IRepository<UserProject> userProjectRepository
            , IRepository<Ticket> ticketRepository)
        {
            _projectBusinessLogic = new ProjectBusinessLogic(projectRepo, users, userManagerBusinessLogic, userProjectRepository, ticketRepository);
            _users = users;
            _userManagerBusinessLogic = userManagerBusinessLogic;
            _userProjectRepository = userProjectRepository;
            _ticketRepository = ticketRepository;
        }
        // GET: Projects
        [Authorize]
        public async Task<IActionResult> Index(string? sortOrder, int? page, bool? sort, string? userId)
        {
            ViewBag.Users = _userManagerBusinessLogic.UserSelectListItem();

            List<Project> projects = _projectBusinessLogic.GetProjectsWithAssociations().ToList();

            ApplicationUser user = _userManagerBusinessLogic.GetLoggedInUser(User).Result;

            List<Project> AssignedProject = _projectBusinessLogic.GetAssignedDeveloperProjects(User, projects, user, sortOrder, sort).Result;

            X.PagedList.IPagedList<Project> projList = AssignedProject.ToPagedList(page ?? 1, 3);
            return View(projList);
        }

        //// GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Project project = _projectBusinessLogic.GetProject(id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        //public async Task<IActionResult> RemoveAssignedUser(string id, int projId)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    UserProject currUserProj = await _context.UserProjects.FirstAsync(up => up.ProjectId == projId && up.UserId == id);
        //    _context.UserProjects.Remove(currUserProj);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Edit", new { id = projId });
        //}

        //// GET: Projects/Create
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> CreateAsync()
        {
            List<SelectListItem> users = _userManagerBusinessLogic.UserSelectListItem();
            ViewBag.Users = users;

            return View();
        }

        //// POST: Projects/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create([Bind("Id,ProjectName")] Project project, List<string> userIds)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser createdBy = _userManagerBusinessLogic.GetLoggedInUser(User).Result;
                
                if (_projectBusinessLogic.BuildProjectModel(userIds, project, createdBy).Result)
                {
                    return RedirectToAction(nameof(Index));
                } else
                {
                    return BadRequest();
                };
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project project = _projectBusinessLogic.GetProject(id);

            if (project == null)
            {
                return NotFound();
            }
            
            // populate the select list
            ViewBag.Users = _userManagerBusinessLogic.AllUserSelectListItem();

            return View(project);
        }

        //// POST: Projects/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int id, List<string> userIds, [Bind("Id,ProjectName")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _projectBusinessLogic.EditProjectModel(userIds, project);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return RedirectToAction("Index");
            }
            return View(project);
        }

        //// GET: Projects/Delete/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project project = _projectBusinessLogic.GetProject(id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        //// POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_projectBusinessLogic.DeleteProjectAndAssociations(id).Result)
            {
                return RedirectToAction("Index");
            } else
            {
                return BadRequest("ERROR: Project not found");
            }
        }

        //private bool ProjectExists(int id)
        //{
        //    return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        //}
    }
}
