using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class AdminBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminBusinessLogic(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ProjectManagersAndDevelopersViewModels> Index()
        {
            List<ApplicationUser> allUsers = await _userManager.Users.ToListAsync();
            List<ApplicationUser> pmUsers = await _userManager.

            return users;
        }
    }
}
