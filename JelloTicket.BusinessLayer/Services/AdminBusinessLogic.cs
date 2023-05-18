using JelloTicket.DataLayer.Repositories;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JelloTicket.DataLayer.Models;
using JelloTicket.BusinessLayer.ViewModels;

namespace JelloTicket.BusinessLayer.Services
{
    public class AdminBusinessLogic
    {
        private readonly IUserRepo<ApplicationUser> _userRepository;

        public AdminBusinessLogic(IUserRepo<ApplicationUser> userRepository)
        {
            _userRepository = userRepository;
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


    }
}
