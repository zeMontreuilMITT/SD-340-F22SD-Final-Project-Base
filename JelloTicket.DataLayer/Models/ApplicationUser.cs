using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JelloTicket.DataLayer.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
        public ICollection<UserProject> Projects { get; set; } = new HashSet<UserProject>();
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public ICollection<TicketWatcher> TicketWatching { get; set; } = new HashSet<TicketWatcher>();


    }
}
