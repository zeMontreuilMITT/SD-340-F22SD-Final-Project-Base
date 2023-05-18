using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JelloTicket.DataLayer.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
        public virtual ICollection<UserProject> Projects { get; set; } = new HashSet<UserProject>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<TicketWatcher> TicketWatching { get; set; } = new HashSet<TicketWatcher>();

        // for moq.....
        public ApplicationUser() { }
    }
}
