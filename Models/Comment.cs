namespace SD_340_W22SD_Final_Project_Group6.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public ApplicationUser CreatedBy { get; set; }

        public ICollection<Ticket> Tickets = new HashSet<Ticket>();

    }
}
