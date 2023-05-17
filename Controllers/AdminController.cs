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
        private readonly AdminBusinessLogic _adminBusinessLogic;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _adminBusinessLogic = new AdminBusinessLogic(userManager);
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _adminBusinessLogic.Index());
            } catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> ReassignRoleAsync()
        {
            try
            {
                return View(_adminBusinessLogic.ReassignRoleAsync());
            } catch
            {
                return BadRequest();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReassignRole(string role, string userId)
        {
            try
            {
                bool tf = await _adminBusinessLogic.ReassignRole(role, userId);
                if (tf)
                {
                    return RedirectToAction("Index", "Admin", new { area = "" });
                }
                else
                {
                    return RedirectToAction("Index", "Admin", new { area = "" });
                }
            }
            catch
            {
                return NotFound();
            }
        }
    }
}

