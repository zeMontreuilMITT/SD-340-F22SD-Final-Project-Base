using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class AdminBusinessLogic
    {
        private UserManager<ApplicationUser> _users;

        public AdminBusinessLogic(UserManager<ApplicationUser> users)
        {
            _users = users;
        }   

        public async ProjectManagersAndDevelopersViewModels GetAllAsync()
        {
            // ====== Get User List ======
            HashSet<ApplicationUser> allUsers = _users.Users.ToHashSet();
            HashSet<ApplicationUser> pmUsers = (HashSet<ApplicationUser>)await _users.GetUsersInRoleAsync("ProjectManager");
            HashSet<ApplicationUser> devUsers = (HashSet<ApplicationUser>)await _users.GetUsersInRoleAsync("Developer");

            // ====== Initialize View Model ======
            ProjectManagersAndDevelopersViewModels VM = new ProjectManagersAndDevelopersViewModels
            {
                allUsers = allUsers,
                pms = pmUsers,
                devs = devUsers
            };

            return VM;
        }
    }
}
