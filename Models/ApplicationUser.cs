using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SD_340_W22SD_Final_Project_Group6.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

        public ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
        public ICollection<Project> Projects { get; set; } = new HashSet<Project>();
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();


    }
}
