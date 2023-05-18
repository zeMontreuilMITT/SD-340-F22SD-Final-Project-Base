using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class ProjectIndexVM
    {
        public IPagedList<Project> Projects { get; set; }
        public List<SelectListItem> Users { get; set; } = new List<SelectListItem>();

        public ProjectIndexVM(IPagedList<Project> projects, List<ApplicationUser> users)
        {
            Projects = projects;
            users.ForEach(u =>
            {
                Users.Add(new SelectListItem(u.UserName, u.Id));
            });
        }

        public ProjectIndexVM() { }
    }
}
