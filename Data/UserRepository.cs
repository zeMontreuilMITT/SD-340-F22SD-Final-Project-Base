using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class UserRepository : IRepository<ApplicationUser>
    {
        private UserManager<ApplicationUser> _users;

        public UserRepository(UserManager<ApplicationUser> users)
        {
            _users = users;
        }

        void IRepository<ApplicationUser>.Create(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }

        void IRepository<ApplicationUser>.Delete(int? id)
        {
            throw new NotImplementedException();
        }

        public ApplicationUser Get(string? id)
        {
            return _users.Users.First(u => u.Id == id);
        }

        ICollection<ApplicationUser> IRepository<ApplicationUser>.GetAll()
        {
            throw new NotImplementedException();
        }

        void IRepository<ApplicationUser>.Update(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }
    }
}
