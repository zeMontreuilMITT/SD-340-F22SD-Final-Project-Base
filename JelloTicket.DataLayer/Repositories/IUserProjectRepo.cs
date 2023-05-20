using JelloTicket.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JelloTicket.DataLayer.Repositories
{
    public interface IUserProjectRepo
    {
        Task<UserProject> GetUserProjectByProjectIdAndUSerId(int projectId, string userId);
    }
}
