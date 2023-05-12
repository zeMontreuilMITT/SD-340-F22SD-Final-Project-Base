using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class CommentRepo : IRepository<Comment>
    {
        private readonly ApplicationDbContext _context;

        public CommentRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public Comment Create(Comment entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public Comment Delete(Comment entity)
        {
            _context.Remove(entity);
            _context.SaveChanges();
            return entity;
        }

        public Comment? Get(int id)
        {
            return _context.Comments.Find(id);
        }

        public ICollection<Comment> GetAll()
        {
            return _context.Comments.ToList();
        }

        public Comment Update(Comment entity)
        {
            _context.Update(entity);
            _context.SaveChanges();
            return entity;
        }
    }

}
