using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class ProjectIndexVM
    {
        public List<SelectListItem> developers= new List<SelectListItem>();
        public IPagedList<Project> pagedListProjects;
      
        public ProjectIndexVM(List<SelectListItem> developers, IPagedList<Project> pagedListProjects)
        {
            this.developers = developers;
            this.pagedListProjects = pagedListProjects;
        }
    }
}
