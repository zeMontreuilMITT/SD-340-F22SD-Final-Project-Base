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
            _context.Users.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public ApplicationUser Delete(ApplicationUser entity)
        {
            _context.Users.Remove(entity);
            _context.SaveChanges();
            return entity;
        }

        public ApplicationUser? Get(int id)
        {
            return _context.Users.Find(id);
        }

        public ICollection<ApplicationUser> GetAll()
        {
            return 
        }

        public ApplicationUser Update(ApplicationUser entity)
        {
            _context.Users.Update(entity);
            _context.SaveChanges();
            return entity;
        }
    }
}
