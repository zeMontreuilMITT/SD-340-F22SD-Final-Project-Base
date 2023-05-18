using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class CreateTicketViewModel
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public int RequiredHours { get; set; }
        public int SelectedProject { get; set; }
        public string ProjectName { get; set; }
        public Ticket.Priority Priority { get; set; }
        public string OwnerId { get; set; }
        public IEnumerable<SelectListItem> Projects { get; set; }
        public IEnumerable<SelectListItem> Users { get; set; }
    }
}
