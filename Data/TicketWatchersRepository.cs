using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Models;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class TicketWatchersRepository : IRepository<TicketWatcher> 
    {
        private ApplicationDbContext _context;

        public TicketWatchersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public Task<TicketWatcher?> Get(int? id)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<TicketWatcher>> GetAll()
        {
            return await _context.TicketWatchers.ToListAsync();
        }

        public Task Update(TicketWatcher entity)
        {
            throw new NotImplementedException();
        }

        async Task IRepository<TicketWatcher>.Create(TicketWatcher entity)
        {
            _context.TicketWatchers.Add(entity);

            await _context.SaveChangesAsync();
        }

        async Task IRepository<TicketWatcher>.Delete(TicketWatcher entity)
        {
            _context.TicketWatchers.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
