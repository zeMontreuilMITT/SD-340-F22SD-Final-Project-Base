using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public interface ITicketRepository
    {

        Task<Project?> GetProectById(int? proId);
      
      
         
    }
}
