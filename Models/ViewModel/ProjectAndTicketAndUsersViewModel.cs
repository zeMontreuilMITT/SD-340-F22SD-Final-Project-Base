using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class ProjectAndTicketAndUsersViewModel
    {
		public Project? curProject { get; set; }
		public Ticket? curTicket { get; set; }
      
        public List<SelectListItem> curUsers { get; } = new List<SelectListItem>();

        //public string userId { get; set; }
        //public string ticketId { get; set; }


        public ProjectAndTicketAndUsersViewModel(Project? project ,Ticket? ticket,HashSet<ApplicationUser> users)
        {
            curProject = project;
            curTicket = ticket;
            foreach (ApplicationUser user in users)
            {
                curUsers.Add(new SelectListItem(user.UserName, user.Id.ToString()));
            }           
        }
        public ProjectAndTicketAndUsersViewModel() { }
    }
}
