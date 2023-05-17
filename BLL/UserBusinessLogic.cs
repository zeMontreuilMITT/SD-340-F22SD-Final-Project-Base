using Microsoft.AspNetCore.Identity;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class UserBusinessLogic
    {
        private UserRepository _userRepository;
        private UserManager<ApplicationUser> _users;

        public UserBusinessLogic(UserManager<ApplicationUser> _users) 
        {
            _userRepository = new UserRepository(_users);
        }

        public ApplicationUser Get(string? userParameter)
        {
            if(userParameter == null)
            {
                throw new NullReferenceException("User parameter passed cannot be null");
            } else
            {
                ApplicationUser user = _userRepository.Get(userParameter);

                if(user != null)
                {
                    return user;
                } else
                {
                    throw new KeyNotFoundException();
                }
            }
        }
    }
}
