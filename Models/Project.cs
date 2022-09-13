using System.ComponentModel.DataAnnotations;

namespace SD_340_W22SD_Final_Project_Group6.Models
{
    public class Project
    {
        public int Id { get; set; }
        [Range(5,200)]
        public string ProjectName { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public ICollection<ApplicationUser>? AssignedTo = new HashSet<ApplicationUser>();
        public ICollection<Ticket>? Tickets = new HashSet<Ticket>();

    }
}
