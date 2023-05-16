using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AdminBusinessLogic _adminBusinessLogic;

        public AdminController(UserManager<ApplicationUser> users)
        {
            _adminBusinessLogic = new AdminBusinessLogic(users);
        }
        public async Task<IActionResult> Index()
        {
            ProjectManagersAndDevelopersViewModels VM = await _adminBusinessLogic.GetAllAsync();
            return View(VM);
        }

        public async Task<IActionResult> ReassignRoleAsync()
        {
            return View(_adminBusinessLogic.GetReassignUserView());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReassignRole(string role, string userId)
        {
            await _adminBusinessLogic.PostReassignUserAsync(role, userId);
            return RedirectToAction("Index", "Admin", new { area = "" });
        }
    }
}

