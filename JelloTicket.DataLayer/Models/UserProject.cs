namespace JelloTicket.DataLayer.Models
{
    public class UserProject
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int? ProjectId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Project Project { get; set; }
    }
}
