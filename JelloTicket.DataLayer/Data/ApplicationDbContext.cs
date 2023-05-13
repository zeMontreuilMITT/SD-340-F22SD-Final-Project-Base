using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using JelloTicket.DataLayer.Models;

namespace JelloTicket.DataLayer.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<JelloTicket.DataLayer.Models.Ticket> Tickets { get; set; }
        public DbSet<JelloTicket.DataLayer.Models.Project> Projects { get; set; }
        public DbSet<JelloTicket.DataLayer.Models.Comment> Comments { get; set; }
        public DbSet<JelloTicket.DataLayer.Models.UserProject> UserProjects { get; set; }
        public DbSet<JelloTicket.DataLayer.Models.TicketWatcher> TicketWatchers { get; set; }

    }
}