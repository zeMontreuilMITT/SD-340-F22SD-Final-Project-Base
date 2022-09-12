namespace SD_340_W22SD_Final_Project_Group6.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        public ICollection<ApplicationUser> AssignedUsers { get; set; } = new HashSet<ApplicationUser>();

        public enum Priority { Low, Medium, High }

    }
}
