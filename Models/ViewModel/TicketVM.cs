using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class TicketVM
    {
        public Project CurrProject { get; set; }
        public List<SelectListItem> CurrUsers { get; set; }
        public Ticket Ticket { get; set; }

        public TicketVM(int projId, List<ApplicationUser> users, Ticket ticket)
        {
            CurrProject = new Project { Id = projId };
            CurrUsers = new List<SelectListItem>();
            users.ForEach(user =>
            {
                CurrUsers.Add(new SelectListItem(user.UserName, user.Id));
            });
            Ticket = ticket;
        }

        public TicketVM() { }


    }
}
