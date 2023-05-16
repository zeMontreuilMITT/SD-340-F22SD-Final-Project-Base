using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JelloTicket.DataLayer.Repositories
{
    public class ProjectRepo : IRepository<Project>
    {
        private ApplicationDbContext _context;

        public ProjectRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public Project Get(int? id)
        {
            return (Project)_context.Projects.FirstOrDefault(t => t.Id == id);
        }

        public ICollection<Project> GetAll()
        {
            ICollection<Project> result = _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.AssignedTo)
                    .ThenInclude(at => at.ApplicationUser)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Owner)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.TicketWatchers)
                    .ThenInclude(tw => tw.Watcher).ToList();

            return result;
        }

        public void Create(Project project)
        {
            _context.Add(project);
            _context.SaveChanges();
        }

        public void Update(Project project)
        {
            _context.Update(project);
            _context.SaveChanges();
        }

        public void Delete(int? id)
        {
            _context.Projects.Remove(_context.Projects.First(t => t.Id == id));
            _context.SaveChanges();
        }
    }
}
