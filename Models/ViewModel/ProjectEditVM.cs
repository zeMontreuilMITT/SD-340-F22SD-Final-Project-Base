using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class ProjectEditVM
    {
        public Project Project { get; set; }
        public List<SelectListItem> CurrUsers { get; set; }

        public ProjectEditVM(Project project, List<ApplicationUser> users)
        {
            Project = project;
            CurrUsers = new List<SelectListItem>();

            foreach (var user in users)
            {
                CurrUsers.Add(new SelectListItem(user.UserName, user.Id));
            }



        }
    }
}
