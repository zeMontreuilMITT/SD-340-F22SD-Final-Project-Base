namespace SD_340_W22SD_Final_Project_Group6.Data
{
        public interface IRepository<T> where T : class
        {
            // CREATE
            public T Create(T entity);

            // READ
            public T? Get(int? id);
            ICollection<T> GetAll();

            // UPDATE
            public T Update(T entity);

            // DELETE
            public T Delete(T entity);
        }

}
