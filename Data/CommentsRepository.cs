using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Models;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SD_340_W22SD_Final_Project_Group6.Data
{
    public class CommentsRepository : IRepository<Comment> 
    {
        private ApplicationDbContext _context;

        public CommentsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

       

        public Task Delete(Comment entity)
        {
            throw new NotImplementedException();
        }

        public Task<Comment?> Get(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Comment>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task Update(Comment entity)
        {
            throw new NotImplementedException();
        }

       async Task IRepository<Comment>.Create(Comment entity)
        {
            _context.Comments.Add(entity);

            await _context.SaveChangesAsync();
        }
    }
}
