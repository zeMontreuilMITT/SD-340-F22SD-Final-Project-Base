using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class TicketRepo : IRepository<Ticket>
    {
        public readonly ApplicationDbContext _context;

        public TicketRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public Ticket Create(Ticket entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public Ticket Delete(Ticket entity)
        {
            _context.Remove(entity);
            _context.SaveChanges();
            return entity;
        }

        public Ticket Get(int id)
        {
            return _context.Tickets
                .Include(t => t.Project)
                .ThenInclude(p => p.AssignedTo)
                .Include(t => t.Owner)
                .Include(t => t.TicketWatchers)
                .ThenInclude(tw => tw.Watcher)
                .Include(t => t.Comments)
                .ThenInclude(c => c.CreatedBy)
                .First(t => t.Id == id);
        }

        public ICollection<Ticket> GetAll()
        {
            return _context.Tickets
                .Include(t => t.Project)
                .ThenInclude(p => p.AssignedTo)
                .Include(t => t.Owner)
                .Include(t => t.TicketWatchers)
                .ThenInclude(tw => tw.Watcher)
                .ToList();
        }

        public Ticket Update(Ticket entity)
        {
            _context.Tickets.Update(entity);
            _context.SaveChanges();
            return entity;
        }
    }
}
