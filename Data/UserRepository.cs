using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class UserRepository
    {
        private UserManager<ApplicationUser> _users;

        public UserRepository(UserManager<ApplicationUser> users)
        {
            _users = users;
        }

        public ApplicationUser? GetUser(string? id)
        {
            return _users.Users.First(u => u.Id == id);
        }
    }
}
