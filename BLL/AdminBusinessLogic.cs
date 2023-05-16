using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Protocol.Core.Types;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;
using System.Web.Mvc;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class AdminBusinessLogic
    {
        private UserManager<ApplicationUser> _users;

        public AdminBusinessLogic(UserManager<ApplicationUser> users)
        {
            _users = users;
        }   

        public async Task<ProjectManagersAndDevelopersViewModels> GetAllAsync()
        {
            // ====== Get User List ======
            List<ApplicationUser> allUsers = _users.Users.ToList();
            List<ApplicationUser> pmUsers = (List<ApplicationUser>)await _users.GetUsersInRoleAsync("ProjectManager");
            List<ApplicationUser> devUsers = (List<ApplicationUser>)await _users.GetUsersInRoleAsync("Developer");

            // ====== Initialize View Model ======
            ProjectManagersAndDevelopersViewModels VM = new ProjectManagersAndDevelopersViewModels();

            VM.allUsers = allUsers;
            VM.pms = pmUsers;
            VM.devs = devUsers;

            return VM;
        }

        public ReassignUserViewModel GetReassignUserView()
        {
            // ====== Get all users and initialize view model ======
            List<ApplicationUser> allUsers = _users.Users.ToList();   
            ReassignUserViewModel VM = new ReassignUserViewModel(allUsers);

            return VM;
        }

        public async Task PostReassignUserAsync(string role, string userId)
        {
            // ====== Query user and the roles of that user ====== 
            ApplicationUser user = _users.Users.First(u => u.Id == userId);
            ICollection<string> roleUser = await _users.GetRolesAsync(user);

            // ====== Validation to see if a user has a current role or not ======
            if (roleUser.Count == 0)
            {
                await _users.AddToRoleAsync(user, role);
            } 
            else
            {
                await _users.RemoveFromRoleAsync(user, roleUser.First());
                await _users.AddToRoleAsync(user, role);
            }
        }

    }
}
