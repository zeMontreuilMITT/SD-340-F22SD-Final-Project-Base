﻿using Microsoft.AspNetCore.Identity;
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

    }
}
