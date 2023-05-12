using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class TicketRepo : IRepository<Ticket>
    {
        private readonly ApplicationDbContext _context;

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

        public Ticket? Get(int id)
        {
            return _context.Tickets.Find(id);
        }

        public ICollection<Ticket> GetAll()
        {
            return _context.Tickets.ToList();
        }

        public Ticket Update(Ticket entity)
        {
            _context.Update(entity);
            _context.SaveChanges();
            return entity;
        }
    }
}
