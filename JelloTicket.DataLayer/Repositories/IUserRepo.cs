using JelloTicket.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JelloTicket.DataLayer.Repositories
{
    public interface IUserRepo<T> : IRepository<ApplicationUser> where T: class 
    {
        void Create(T entity);

        T? Get(int? id);
        ICollection<T> GetAll();

        void Update(T entity);

        void Delete(int? id);

        IEnumerable<ApplicationUser> GetUsersInRole(string roleName);

        T? GetByStringId(string? id);
    }
}
