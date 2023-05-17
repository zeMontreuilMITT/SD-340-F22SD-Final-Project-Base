using SD_340_W22SD_Final_Project_Group6.Models;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public interface IRepository<T> where T : class
    {
        // CREATE
        Task Create(T entity);

        // READ
        Task<T?> Get(int? id);
        Task<ICollection<T>> GetAll();

        // UPDATE
        Task Update(T entity);

        // DELETE
        Task Delete(T entity);
    }
}
