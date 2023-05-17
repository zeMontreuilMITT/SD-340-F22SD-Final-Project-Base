using Microsoft.EntityFrameworkCore;
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

        public Comment Get(int id)
        {
            return _context.Comments
                .Include(c => c.CreatedBy)
                .Include(c => c.Ticket)
                .ThenInclude(c => c.Owner)
                .ThenInclude(o => o.Tickets)
                .Include(c => c.Ticket)
                .ThenInclude(t => t.Project)
                .ThenInclude(p => p.AssignedTo)
                .First( c => c.Id == id);
        }

        public ICollection<Comment> GetAll()
        {
            return _context.Comments
                .Include(c => c.CreatedBy)
                .Include(c => c.Ticket)
                .ThenInclude(c => c.Owner)
                .ThenInclude(o => o.Tickets)
                .Include(c => c.Ticket)
                .ThenInclude(t => t.Project)
                .ThenInclude(p => p.AssignedTo)
                .ToList();
        }

        public Comment Update(Comment entity)
        {
            _context.Comments.Update(entity);
            _context.SaveChanges();
            return entity;
        }
    }
}
