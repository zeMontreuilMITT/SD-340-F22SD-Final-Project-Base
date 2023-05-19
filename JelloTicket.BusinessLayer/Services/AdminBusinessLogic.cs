using JelloTicket.DataLayer.Repositories;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JelloTicket.DataLayer.Models;
using JelloTicket.BusinessLayer.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace JelloTicket.BusinessLayer.Services
{
    public class AdminBusinessLogic
    {
        private readonly IUserRepo<ApplicationUser> _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminBusinessLogic(IUserRepo<ApplicationUser> userRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public AdminBusinessLogic() { }

        public ProjectManagersAndDevelopersViewModels BuildPMAndDeveloperViewModel()
        {
            ProjectManagersAndDevelopersViewModels vm = new ProjectManagersAndDevelopersViewModels();

            List<ApplicationUser> allUsers = (List<ApplicationUser>)_userRepository.GetAll();
            List<ApplicationUser> pmUsers = (List<ApplicationUser>)_userRepository.GetUsersInRole("ProjectManager");
            List<ApplicationUser> devUsers = (List<ApplicationUser>)_userRepository.GetUsersInRole("Developer");

            vm.allUsers = allUsers;
            vm.pms = pmUsers;
            vm.devs = devUsers;

            return vm;
        }

        // *** CURRENTLY BUGGED ***
        // GetRolesAsync is just not working for some reason?
        public async Task ReassignRole(string role, string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            ICollection<string> roleUser = roles.ToList();
            if (roleUser.Count == 0)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, roleUser.First());
                await _userManager.AddToRoleAsync(user, role);
            }

            await _userManager.UpdateAsync(user);
            await _userManager.UpdateSecurityStampAsync(user);
        }

    }
}
