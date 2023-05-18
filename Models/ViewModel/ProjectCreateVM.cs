using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class ProjectCreateVM
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public List<SelectListItem> Users { get; set; } = new List<SelectListItem>();

        public ICollection<UserProject> AssignedTo { get; set; } = new HashSet<UserProject>();


        public ProjectCreateVM (string projectName, List<ApplicationUser> users)
        {
            foreach (ApplicationUser user in users)
            {
                Users.Add(new SelectListItem(user.UserName, user.Id));
            }
            ProjectName = projectName;
        }
        public ProjectCreateVM () { }
    }
}
