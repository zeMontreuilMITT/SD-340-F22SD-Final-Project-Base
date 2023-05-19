using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class TicketWatcherRepo : IRepository<TicketWatcher>
    {
        private readonly ApplicationDbContext _context;

        public TicketWatcherRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public TicketWatcher Create(TicketWatcher entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public TicketWatcher Delete(TicketWatcher entity)
        {
            _context.Remove(entity);
            _context.SaveChanges();
            return entity;
        }

        public TicketWatcher Get(int id)
        {
            return _context.TicketWatchers
                .Include(tw => tw.Ticket)
                .Include(tw => tw.Watcher)
                .First(tw => tw.Id == id);
        }

        public ICollection<TicketWatcher> GetAll()
        {
            return _context.TicketWatchers
                .Include(tw => tw.Ticket)
                .Include(tw => tw.Watcher)
                .ToList();
        }

        public TicketWatcher Update(TicketWatcher entity)
        {
            _context.TicketWatchers.Update(entity);
            _context.SaveChanges();
            return entity;
        }
    }
}
