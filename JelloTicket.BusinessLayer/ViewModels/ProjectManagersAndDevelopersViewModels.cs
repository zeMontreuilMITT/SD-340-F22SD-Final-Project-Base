using JelloTicket.DataLayer.Models;

namespace JelloTicket.BusinessLayer.ViewModels
{
    public class ProjectManagersAndDevelopersViewModels
    {
        public ICollection<ApplicationUser> pms { get; set; } = new HashSet<ApplicationUser>();
        public ICollection<ApplicationUser> devs { get; set; } = new HashSet<ApplicationUser>();
        public ICollection<ApplicationUser> allUsers { get; set; } = new HashSet<ApplicationUser>();
        public ICollection<string> roles { get; set; } = new HashSet<string>();
    }
}
