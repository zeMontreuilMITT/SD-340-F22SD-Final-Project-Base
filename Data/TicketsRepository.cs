using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Models;
using System.Net.Sockets;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class TicketsRepository : IRepository<Ticket>, ITicketRepository
    {
        private ApplicationDbContext _context;

        public TicketsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        async Task IRepository<Ticket>.Create(Ticket entity)
        {
            _context.Tickets.Add(entity);
            await _context.SaveChangesAsync();
        }

        async Task<Ticket?> IRepository<Ticket>.Get(int? id)
        {

          Ticket? ticket= await _context.Tickets.Include(t => t.Project).Include(t => t.TicketWatchers).ThenInclude(tw => tw.Watcher).Include(u => u.Owner).Include(t => t.Comments).ThenInclude(c => c.CreatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);

            return ticket;
        }

        async Task<ICollection<Ticket>> IRepository<Ticket>.GetAll()
        {
            return await _context.Tickets.Include(t => t.Project).Include(t => t.Owner).ToListAsync();
        }


        async Task IRepository<Ticket>.Update(Ticket entity)
        {
             
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        async Task IRepository<Ticket>.Delete(Ticket entity)
        {

            
            _context.Tickets.Remove(entity);
            await _context.SaveChangesAsync();
             
        }

        async Task<Project?> ITicketRepository.GetProectById(int? proId)
        {
           Project? project= await _context.Projects.Include(p => p.AssignedTo).ThenInclude(at => at.ApplicationUser).FirstOrDefaultAsync(p => p.Id == proId);
            return project;

        }
    
       
         
    }
}
