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

        public ApplicationDbContext(): base() { }

        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<UserProject> UserProjects { get; set; }
        public virtual DbSet<TicketWatcher> TicketWatchers { get; set; }

    }
}