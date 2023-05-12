namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public interface IRepository<T> where T : class
    {
        public T Create(T entity);

        public T? Get(int id);
        public ICollection<T> GetAll();

        public T Update(T entity);

        public T Delete (T entity);
    }
}
