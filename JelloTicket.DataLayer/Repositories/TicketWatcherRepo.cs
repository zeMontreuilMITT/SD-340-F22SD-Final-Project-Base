using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JelloTicket.DataLayer.Repositories
{
    public class TicketWatcherRepo : IRepository<TicketWatcher>
    {
        private ApplicationDbContext _context;

        public TicketWatcherRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public TicketWatcher Get(int? id)
        {
            return (TicketWatcher)_context.Tickets.Where(t => t.Id == id);
        }

        public ICollection<TicketWatcher> GetAll()
        {
            return _context.TicketWatchers.ToHashSet();
        }

        public void Create(TicketWatcher tw)
        {
            _context.Add(tw);
            _context.SaveChanges();
        }

        public void Update(TicketWatcher tw)
        {
            _context.Update(tw);
            _context.SaveChanges();
        }

        public void Delete(int? id)
        {
            _context.TicketWatchers.Remove(_context.TicketWatchers.First(t => t.Id == id));
            _context.SaveChanges();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public bool Exists(int id)
        {
            return _context.TicketWatchers.Any(tw => tw.Id == id);
        }
    }
}
