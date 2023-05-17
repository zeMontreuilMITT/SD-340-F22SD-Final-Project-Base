using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using System.Data;
using System.Web.Mvc;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class AdminBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminBusinessLogic(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ProjectManagersAndDevelopersViewModels> Index()
        {
            List<ApplicationUser> allUsers = await _userManager.Users.ToListAsync();
            List<ApplicationUser> pmUsers = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("ProjectManager");
            List<ApplicationUser> devUsers = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Developer");

            ProjectManagersAndDevelopersViewModels vm = new ProjectManagersAndDevelopersViewModels();

            vm.allUsers = allUsers;
            vm.pms = pmUsers;
            vm.devs = devUsers;

            return vm;
        }

        public async Task<List<SelectListItem>> ReassignRoleAsync()
        {
            List<ApplicationUser> allUsers = await _userManager.Users.ToListAsync();

            List<SelectListItem> users = new List<SelectListItem>();
            allUsers.ForEach(u =>
            {
                users.Add(new SelectListItem(u.UserName, u.Id.ToString()));
            });

            return users;
        }

        public async Task<bool> ReassignRole(string role, string userId)
        {
            ApplicationUser user = _userManager.Users.First(u => u.Id == userId);
            ICollection<string> roleUser = await _userManager.GetRolesAsync(user);
            if (roleUser.Count == 0)
            {
                await _userManager.AddToRoleAsync(user, role);
                return true;
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, roleUser.First());
                await _userManager.AddToRoleAsync(user, role);
                return false;
            }
        }
    }
}
