using System.ComponentModel.DataAnnotations.Schema;

namespace SD_340_W22SD_Final_Project_Group6.Models
{
    public class UserProject
    {
        public int Id { get; set; }
        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public int? ProjectId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
        public Project Project { get; set; }
    }
}
