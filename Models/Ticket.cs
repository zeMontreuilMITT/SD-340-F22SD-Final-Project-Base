using System.ComponentModel.DataAnnotations;

namespace SD_340_W22SD_Final_Project_Group6.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        [StringLength(200, ErrorMessage ="Title should be from 5 upto 200 characters only")]
        [MinLength(5)]
        public string Title { get; set; }
        public string Body { get; set; }

        [Range(1,999)]
        public int RequiredHours { get; set; }  

        public ICollection<ApplicationUser>? AssignedUsers { get; set; } = new HashSet<ApplicationUser>();

        public Project? Project { get; set; }

        public Priority? TicketPriority { get; set; }

        public enum Priority
        {
            [Display(Name = "Low")]
            Low,
            [Display(Name = "Medium")]
            Medium,
            [Display(Name = "High")]
            High,
        }

    }
}
