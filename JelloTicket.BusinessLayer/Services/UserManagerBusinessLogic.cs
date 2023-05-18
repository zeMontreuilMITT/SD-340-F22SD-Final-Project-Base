using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JelloTicket.BusinessLayer.Services
{
    public class UserManagerBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _users;

        // unsure if this is exactly how to inject
        // might have to modify
        public UserManagerBusinessLogic(UserManager<ApplicationUser> users)
        {
            _users = users;
        }

        public async Task<ApplicationUser> GetLoggedInUser(ClaimsPrincipal user)
        {
            return await _users.GetUserAsync(user);
        }

        public async Task<List<ApplicationUser>> GetAllDeveloperUsers()
        {
            return (List<ApplicationUser>)await _users.GetUsersInRoleAsync("Developer");
        }

        public List<SelectListItem> UserSelectListItem()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            List<ApplicationUser> users = GetAllDeveloperUsers().Result;

            foreach (ApplicationUser user in users)
            {
                selectListItems.Add(new SelectListItem(user.UserName, user.Id.ToString()));
            }

            return selectListItems;
        }

        public UserManager<ApplicationUser> Get_users()
        {
            return _users;
        }

        //public ApplicationUser GetUserByUserName(string userName)
        //{
        //    List<SelectListItem> selectListItems = new List<SelectListItem>();
        //    List<ApplicationUser> users = GetAllDeveloperUsers().Result;

        //    foreach (ApplicationUser user in users)
        //    {
        //        selectListItems.Add(new SelectListItem(user.UserName, user.Id.ToString()));
        //    }

        //    return _users.Get(u => u.UserName == userName);
        //}
    }
}
