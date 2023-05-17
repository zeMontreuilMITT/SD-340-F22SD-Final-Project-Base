using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class TicketVM
    {
        public Project CurrProject { get; set; }
        public List<SelectListItem> CurrUsers { get; set; }
        public Ticket TicketViewModel { get; set; }

        public TicketVM(int projId, List<ApplicationUser> users)
        {
            CurrProject = new Project { Id = projId };
            CurrUsers = new List<SelectListItem>();
            users.ForEach(user =>
            {
                CurrUsers.Add(new SelectListItem(user.UserName, user.Id));
            });
            TicketViewModel = new Ticket();
        }


    }
}
