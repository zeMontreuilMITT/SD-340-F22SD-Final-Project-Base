using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class ProjectRepo : IRepository<Project>
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public Project Create(Project entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public Project Delete(Project entity)
        {
            _context.Remove(entity);
            _context.SaveChanges();
            return entity;
        }

        public Project? Get(int id)
        {
            return _context.Projects.Find(id);
        }

        public ICollection<Project> GetAll()
        {
           return _context.Projects.ToList();
        }

        public Project Update(Project entity)
        {
            _context.Projects.Update(entity);
            _context.SaveChanges();
            return entity;
        }
    }
}
