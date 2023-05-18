
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class UserProjectRepo: IRepository<UserProject>
    {
        private readonly ApplicationDbContext _context;

        public UserProjectRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public UserProject Create(UserProject entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public UserProject Get(int id)
        {
            return _context.UserProjects
                .Include(up => up.Project)
                .Include(up => up.ApplicationUser)
                .First(up => up.Id == id);
        }


        public UserProject Update(UserProject entity)
        {
            _context.UserProjects.Update(entity);
            _context.SaveChanges();
            return entity;
        }

        public UserProject Delete(UserProject entity)
        {
            _context.Remove(entity);
            _context.SaveChanges();
            return entity;
        }

        public ICollection<UserProject> GetAll()
        {
            return _context.UserProjects
                .Include(up => up.Project)
                .Include(up => up.ApplicationUser)
                .ToList();
        }
    }
}
