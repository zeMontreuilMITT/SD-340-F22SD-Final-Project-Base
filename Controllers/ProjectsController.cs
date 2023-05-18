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
        private readonly ProjectBusinessLogic _projectBLL;

        public ProjectsController(ProjectBusinessLogic projectBusinessLogic)
        {
            _projectBLL = projectBusinessLogic;
        }
        // GET: Projects
        [Authorize]
        public async Task<IActionResult> Index(string? sortOrder, int? page, bool? sort, string? userId)
        {
            try
            {
                ProjectIndexVM vm = await _projectBLL.Index(sortOrder, page, sort, userId);
                return View(vm);
            } catch
            {
                return NotFound();
            }
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            try
            {
                Project project = _projectBLL.Details((int)id);

                return View(project);
            } catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> RemoveAssignedUser(string id, int projId)
        {
            if (id == null)
            {
                return BadRequest();
            }
            _projectBLL.RemoveAssignedUser(id, projId);

            return RedirectToAction("Edit", new { id = projId });
        }

        // GET: Projects/Create
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> CreateAsync()
        {
            return View(await _projectBLL.CreateAsync());
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create([Bind("Id,ProjectName")] ProjectCreateVM project, List<string> userIds)
        {
            if (ModelState.IsValid)
            {
                _projectBLL.Create(project, userIds);
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
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Projects'  is null.");
            }
            var project = await _context.Projects.Include(p => p.Tickets).FirstAsync(p => p.Id == id);
            if (project != null)
            {
                List<Ticket> tickets = project.Tickets.ToList();
                tickets.ForEach(ticket =>
                {
                    _context.Tickets.Remove(ticket);
                });
                await _context.SaveChangesAsync();
                List<UserProject> userProjects = _context.UserProjects.Where(up => up.ProjectId == project.Id).ToList();
                userProjects.ForEach(userProj =>
                {
                    _context.UserProjects.Remove(userProj);
                });

                _context.Projects.Remove(project);


            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
