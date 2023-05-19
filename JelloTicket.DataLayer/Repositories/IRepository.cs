using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JelloTicket.DataLayer.Repositories
{
    public interface IRepository<T> where T: class
    {
        void Create(T entity);

        T? Get(int? id);
        ICollection<T> GetAll();

        void Update(T entity);

        void Delete(int? id);

        void Save();

        bool Exists(int id);
    }
}
