using JelloTicket.BusinessLayer.Services;
using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using JelloTicket.DataLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
//using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _users;
        private readonly AdminBusinessLogic _adminBusinessLogic;
        private readonly UserManagerBusinessLogic _userManagerBusinessLogic;

        public AdminController(ApplicationDbContext context
            , UserManager<ApplicationUser> users
            , AdminBusinessLogic adminBusinessLogic
            , UserManagerBusinessLogic userManagerBusinessLogic)
        {
            _context = context;
            _users = users;
            _adminBusinessLogic = adminBusinessLogic;
            _userManagerBusinessLogic = userManagerBusinessLogic;
        }
        public async Task<IActionResult> Index()
        {
            return View(_adminBusinessLogic.BuildPMAndDeveloperViewModel());
        }

        public async Task<IActionResult> ReassignRoleAsync()
        {
            ViewBag.Users = _userManagerBusinessLogic.AllUserSelectListItem();
            return View(_users.Users.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReassignRole(string role, string userId)
        {
            _adminBusinessLogic.ReassignRole(role, userId);
            return RedirectToAction("Index", "Admin", new { area = "" });
        }
    }
}

