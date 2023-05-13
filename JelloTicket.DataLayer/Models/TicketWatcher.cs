namespace JelloTicket.DataLayer.Models
{
    public class TicketWatcher
    {
        public int Id { get; set; }
        public ApplicationUser Watcher { get; set; }
        public Ticket Ticket { get; set; }
    }
}
