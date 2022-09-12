using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SD_340_W22SD_Final_Project_Group6.Models.Ticket> Tickets { get; set; }
        public DbSet<SD_340_W22SD_Final_Project_Group6.Models.Project> Projects { get; set; }
        public DbSet<SD_340_W22SD_Final_Project_Group6.Models.Comment> Comments { get; set; }

    }
}