using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SD_340_W22SD_Final_Project_Group6.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        [StringLength(200, ErrorMessage ="Title should be from 5 upto 200 characters only")]
        [MinLength(5)]
        [Required]
        public string Title { get; set; }
        public string Body { get; set; }

        [Range(1,999)]
        public int RequiredHours { get; set; }

        [ForeignKey("ApplicationUser")]
        public ApplicationUser? Owner { get; set; } 

        public Project? Project { get; set; }

        public Priority? TicketPriority { get; set; }
        public bool? Completed { get; set; } = false;
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public ICollection<TicketWatcher>? TicketWatchers { get; set; } = new HashSet<TicketWatcher>();

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
