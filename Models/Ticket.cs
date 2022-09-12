using System.ComponentModel.DataAnnotations;

namespace SD_340_W22SD_Final_Project_Group6.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        [Range(5,200)]
        public string Title { get; set; }
        public string Body { get; set; }
        [Range(1,999)]
        public int RequiredHours { get; set; }  

        public ICollection<ApplicationUser> AssignedUsers { get; set; } = new HashSet<ApplicationUser>();

        public Priority Priority { get; set; }

    }
}
