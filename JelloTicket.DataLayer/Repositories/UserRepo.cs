using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JelloTicket.DataLayer.Repositories
{
    public class UserRepo : IUserRepo<ApplicationUser>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserRepo(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public void Create(ApplicationUser entity)
        {
            _context.Users.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(int? id)
        {
            ApplicationUser user = Get(id);
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        public bool Exists(int? id)
        {
            throw new NotImplementedException();
        }

        public ApplicationUser? Get(int? id)
        {
            return _context.Users.First(u => u.Id == id.ToString());
        }

        public ICollection<ApplicationUser> GetAll()
        {
            return _context.Users.ToList();
        }

        public ApplicationUser? GetByStringId(string? id)
        {
            return _context.Users.First(u => u.Id == id);
        }

        public IEnumerable<ApplicationUser> GetUsersInRole(string roleName)
        {
            return _userManager.GetUsersInRoleAsync(roleName).Result;
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Update(ApplicationUser entity)
        {
            _context.Users.Update(entity);
            _context.SaveChanges();
        }
    }
}
