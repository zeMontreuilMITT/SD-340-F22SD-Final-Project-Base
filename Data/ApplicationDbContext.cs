using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moq;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Ticket> Tickets { get; set; } = default!;
		public virtual DbSet<Project> Projects { get; set; } = default!;
		public virtual DbSet<Comment> Comments { get; set; } = default!;
		public virtual DbSet<UserProject> UserProjects { get; set; } = default!;
		public virtual DbSet<TicketWatcher> TicketWatchers { get; set; } = default!;
		public ApplicationDbContext()
        {
			Projects = new Mock<DbSet<Project>>().Object;
		}

    }
}