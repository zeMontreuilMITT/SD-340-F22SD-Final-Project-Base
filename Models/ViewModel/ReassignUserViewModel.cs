using Microsoft.AspNetCore.Mvc.Rendering;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class ReassignUserViewModel
    {
        public List<SelectListItem> UserList{ get; set; } = new List<SelectListItem>();

        public ReassignUserViewModel(List<ApplicationUser> users) 
        {
            foreach(ApplicationUser user in users)
            {
                UserList.Add(new SelectListItem(user.UserName, user.Id));
            }
        }

        public ReassignUserViewModel() { }

    }
}
