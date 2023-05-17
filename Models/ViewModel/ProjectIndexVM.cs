using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace SD_340_W22SD_Final_Project_Group6.Models.ViewModel
{
    public class ProjectIndexVM
    {
        public List<SelectListItem> developers= new List<SelectListItem>();
        public IPagedList<BasicProjectData> pagedListProjects;
      
        public ProjectIndexVM(List<SelectListItem> developers, IPagedList<BasicProjectData> pagedListProjects)
        {
            this.developers = developers;
            this.pagedListProjects = pagedListProjects;
        }
        

        public class BasicProjectData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string CreatedBy { get; set; }
            public ICollection<string> AssignedTo { get; set; }

            public ICollection<BasicTicketData> Tickets { get; set; }

        }

        public class BasicTicketData
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string OwnerName { get; set; }
            public int RequiredHours { get; set; }
            public string Priority { get; set; }
            public bool Watching { get; set; }
            public bool? Completed { get; set; }

        }
    }


}
