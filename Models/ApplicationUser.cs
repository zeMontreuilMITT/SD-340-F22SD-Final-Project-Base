using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SD_340_W22SD_Final_Project_Group6.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Ticket> tickets { get; set; } = new HashSet<Ticket>();
        public ICollection<Project> projects { get; set; } = new HashSet<Project>();

    }
}
