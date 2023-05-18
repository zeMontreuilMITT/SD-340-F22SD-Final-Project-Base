using Microsoft.AspNetCore.Mvc.Rendering;

namespace JelloTicket.BusinessLayer.ViewModels
{
    public class CreateTicketViewModel
    {
        public int SelectedProject { get; set; }
        public IEnumerable<SelectListItem> Projects { get; set; }
    }
}
