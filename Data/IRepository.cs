namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public interface IRepository<T> where T : class
    {
        // CREATE
        void Create(T entity);

        // READ
        T? Get(int? id);

        // UPDATE
        void Update(T entity);  

        // DELETE
        void Delete(int? id);
    }
}
