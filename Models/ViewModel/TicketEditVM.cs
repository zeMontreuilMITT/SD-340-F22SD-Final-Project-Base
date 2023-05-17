using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class TicketEditVM
    {
        public Ticket Ticket { get; set; }
        public List<SelectListItem> Users { get; set; }

        public TicketEditVM(Ticket ticket, List<ApplicationUser> users)
        {
            Ticket = ticket;
            Users = new List<SelectListItem>();
            users.ForEach(user =>
            {
                Users.Add(new SelectListItem(user.UserName, user.Id));
            });
        }

    }
}
