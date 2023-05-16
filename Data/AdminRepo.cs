using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class AdminRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminRepo (ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ProjectManagersAndDevelopersViewModels> GetUsersInRoles()
        {
            ProjectManagersAndDevelopersViewModels vm = new ProjectManagersAndDevelopersViewModels();

            List<ApplicationUser> pmUsers = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("ProjectManager");
            List<ApplicationUser> devUsers = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Developer");
            List<ApplicationUser> allUsers = _context.Users.ToList();

            vm.pms = pmUsers;
            vm.devs = devUsers;
            vm.allUsers = allUsers;

            return vm;
        }

        public async Task<List<ApplicationUser>> GetUsers()
        {

        }
    }
}
