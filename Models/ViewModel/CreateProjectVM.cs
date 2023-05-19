using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class CreateProjectVM
    {
        public Project Project { get; set; } = new Project();
        public List<SelectListItem> DeveloperUsers { get; set; } = new List<SelectListItem>();
    }
}
