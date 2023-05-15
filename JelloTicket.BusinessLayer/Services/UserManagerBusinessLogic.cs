using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
