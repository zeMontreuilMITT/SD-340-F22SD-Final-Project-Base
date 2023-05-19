using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using X.PagedList;
using X.PagedList.Mvc;


namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize(Roles = "ProjectManager, Developer")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _users;
        private readonly ProjectBL _projectBL;

        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> users, ProjectBL projectBL)
        {
            _context = context;
            _users = users;
            _projectBL = projectBL;
        }
        // GET: Projects
        [Authorize]
        public async Task<IActionResult> Index(string? sortOrder, int? page, bool? sort, string? userId)
        {
            try
            {
                
               ProjectIndexVM x =  await _projectBL.ProjectIndex(sortOrder, page, sort, userId, User);
                return View(x);

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToAction("Index", "Home");
            }

            //return RedirectToAction("Index", "Home");
            /*
            List<Project> SortedProjs = new List<Project>();
            List<ApplicationUser> allUsers = (List<ApplicationUser>)await _users.GetUsersInRoleAsync("Developer");

            List<SelectListItem> users = new List<SelectListItem>();
            allUsers.ForEach(au =>
            {
                users.Add(new SelectListItem(au.UserName, au.Id.ToString()));
            });
            ViewBag.Users = users;
            switch (sortOrder)
            {
                case "Priority":
                    if (sort == true)
                    {
                        SortedProjs =
                        await _context.Projects
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.ApplicationUser)
                        .Include(p => p.Tickets.OrderByDescending(t => t.TicketPriority))
                        .ThenInclude(t => t.Owner)
                        .ToListAsync();
                    }
                    else
                    {
                        SortedProjs =
                        await _context.Projects
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.ApplicationUser)
                        .Include(p => p.Tickets.OrderBy(t => t.TicketPriority))
                        .ThenInclude(t => t.Owner)
                        .ToListAsync();
                    }

                    break;
                case "RequiredHrs":
                    if (sort == true)
                    {
                        SortedProjs =
                        await _context.Projects
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.ApplicationUser)
                        .Include(p => p.Tickets.OrderByDescending(t => t.RequiredHours))
                        .ThenInclude(t => t.Owner)
                        .ToListAsync();
                    }
                    else
                    {
                        SortedProjs =
                        await _context.Projects
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.ApplicationUser)
                        .Include(p => p.Tickets.OrderBy(t => t.RequiredHours))
                        .ThenInclude(t => t.Owner)
                        .ToListAsync();
                    }

                    break;
                case "Completed":
                    SortedProjs =
                        await _context.Projects
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.ApplicationUser)
                        .Include(p => p.Tickets.Where(t => t.Completed == true))
                        .ThenInclude(t => t.Owner)
                        .ToListAsync();
                    break;
                default:
                    if (userId != null)
                    {
                        SortedProjs =
                        await _context.Projects
                        .OrderBy(p => p.ProjectName)
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.ApplicationUser)
                        .Include(p => p.Tickets.Where(t => t.Owner.Id.Equals(userId)))
                        .ThenInclude(t => t.Owner)
                        .Include(p => p.Tickets).ThenInclude(t => t.TicketWatchers).ThenInclude(tw => tw.Watcher)
                        .ToListAsync();
                    }
                    else
                    {
                        SortedProjs =
                        await _context.Projects
                        .OrderBy(p => p.ProjectName)
                        .Include(p => p.CreatedBy)
                        .Include(p => p.AssignedTo)
                        .ThenInclude(at => at.ApplicationUser)
                        .Include(p => p.Tickets)
                        .ThenInclude(t => t.Owner)
                        .Include(p => p.Tickets).ThenInclude(t => t.TicketWatchers).ThenInclude(tw => tw.Watcher)
                        .ToListAsync();
                    }

                    break;
            }
            //check if User is PM or Develoer
            var LogedUserName = User.Identity.Name;  // logined user name
            var user = _context.Users.FirstOrDefault(u => u.UserName == LogedUserName);
            var rolenames = await _users.GetRolesAsync(user);
            var AssinedProject = new List<Project>();
            // geting assined project
            if (rolenames.Contains("Developer"))
            {
                AssinedProject = SortedProjs.Where(p => p.AssignedTo.Select(projectUser => projectUser.UserId).Contains(user.Id)).ToList();
            }
            else
            {
                AssinedProject = SortedProjs;
            }
            X.PagedList.IPagedList<Project> projList = AssinedProject.ToPagedList(page ?? 1, 3);
            return View(projList);
            */
        }

        // GET: Projects/Details/5 //RAG
        public async Task<IActionResult> Details(int? id)
        {
            var project = _projectBL.GetProject(id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // RAG
        public async Task<IActionResult> RemoveAssignedUser(string id, int projId)  
        {
            if (id == null)
            {
                return NotFound();
            }

            _projectBL.RemoveUserFromProject(id, projId);

            return RedirectToAction("Edit", new { id = projId });
        }

        // GET: Projects/Create  //RAG
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> CreateAsync()
        {
            List<SelectListItem> developerUsers = await _projectBL.GetDevelopersAsSelectList();

            CreateProjectVM createProjectVM = new CreateProjectVM() { DeveloperUsers = developerUsers };

            return View(createProjectVM);
        }

        // POST: Projects/Create //RAG
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create([Bind("Id,CreatedById,ProjectName")] Project project, List<string> userIds)
        {
            if (ModelState.IsValid)
            {
                _projectBL.CreateProject(project, userIds);
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.Include(p => p.AssignedTo).FirstAsync(p => p.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            List<ApplicationUser> results = _context.Users.ToList();

            List<SelectListItem> currUsers = new List<SelectListItem>();
            results.ForEach(r =>
            {
                currUsers.Add(new SelectListItem(r.UserName, r.Id.ToString()));
            });
            ViewBag.Users = currUsers;

            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(int id, List<string> userIds, [Bind("Id,CreatedById,ProjectName")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    userIds.ForEach((user) =>
                    {
                        ApplicationUser currUser = _context.Users.FirstOrDefault(u => u.Id == user);
                        UserProject newUserProj = new UserProject();
                        newUserProj.ApplicationUser = currUser;
                        newUserProj.UserId = currUser.Id;
                        newUserProj.Project = project;
                        project.AssignedTo.Add(newUserProj);
                    });
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Edit), new { id = id });
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            
            try
            {
                return View(_projectBL.GetProject(id));
            } catch (Exception ex)
            {
                return NotFound();
            }
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            try
            {
                _projectBL.DeleteProject(id);
                return RedirectToAction(nameof(Index));
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            

        }

        private bool ProjectExists(int id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
