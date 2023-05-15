using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class AdminRepository : IRepository<ApplicationUser>
    {
        private ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int? id)
        {
            throw new NotImplementedException();
        }

        public ApplicationUser? Get(int? id)
        {
            throw new NotImplementedException();
        }

        public ICollection<ApplicationUser> GetAll()
        {
            return _context.Users.ToHashSet();
        }

        public void Update(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }
    }
}
