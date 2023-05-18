using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JelloTicket.DataLayer.Repositories;
using JelloTicket.DataLayer.Models;
using JelloTicket.BusinessLayer.HelperLibrary;
using Microsoft.AspNetCore.Identity;

namespace JelloTicket.BusinessLayer.Services
{
    public class ProjectBusinessLogic
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HelperMethods _helperMethods;
        private readonly UserManagerBusinessLogic _userManagerBusinessLogic;
        private readonly IRepository<UserProject> _userProjectRepository;

        public ProjectBusinessLogic(IRepository<Project> projectRepository
            , UserManager<ApplicationUser> userManager
            , UserManagerBusinessLogic userManagerBusinessLogic
            , IRepository<UserProject> userProjectRepository)
        {
            _projectRepository = projectRepository;
            _userManager = userManager;
            _helperMethods = new HelperMethods();
            _userManagerBusinessLogic = userManagerBusinessLogic;
            _userProjectRepository = userProjectRepository;
        }

        public Project GetProject(int? id)
        {
            return _projectRepository.Get(id);
        }

    }
}
