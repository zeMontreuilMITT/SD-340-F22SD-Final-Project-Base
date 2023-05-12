using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class UserRepo : IRepository<ApplicationUser>
    {
        private readonly ApplicationDbContext _context;

        public UserRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationUser Create(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }

        public ApplicationUser Delete(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }

        public ApplicationUser? Get(int id)
        {
            throw new NotImplementedException();
        }

        public ICollection<ApplicationUser> GetAll()
        {
            return _context.Users.ToList();
        }

        public ApplicationUser Update(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }
    }
}
