using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models.ViewModel;

namespace SD_340_W22SD_Final_Project_Group6.BLL
{
    public class AdminBusinessLogic
    {
        private readonly AdminRepo _adminRepo;

        public AdminBusinessLogic(AdminRepo adminRepo)
        {
            _adminRepo = adminRepo;
        }

        public async Task<ProjectManagersAndDevelopersViewModels> Index()
        {
             var users = await _adminRepo.GetUsersInRoles();

            return users;
        }
    }
}
