using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class TicketCreateVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int RequiredHours { get; set; }
        public Ticket.Priority TicketPriority { get; set; }

        public ApplicationUser Owner { get; set; }

        public Project Project { get; set; }
        public List<SelectListItem> Users { get; set; } = new List<SelectListItem>();

        public TicketCreateVM(string title, string body, int requiredHours, Ticket.Priority ticketPriority, Project currProject, List<SelectListItem> users)
        {
            Title = title;
            Body = body;
            RequiredHours = requiredHours;
            TicketPriority = ticketPriority;
            Project = currProject;
            Users = users;
        }
        public TicketCreateVM() { }
    }
}
