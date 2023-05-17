using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JelloTicket.DataLayer.Repositories;
using JelloTicket.DataLayer.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JelloTicket.BusinessLayer.HelperLibrary;
using Microsoft.AspNetCore.Identity;

namespace JelloTicket.BusinessLayer.Services
{
    public class TicketBusinessLogic
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HelperMethods _helperMethods;
        private readonly UserManagerBusinessLogic _userManagerBusinessLogic;

        public TicketBusinessLogic(IRepository<Project> projectRepository
            , UserManager<ApplicationUser> userManager
            , UserManagerBusinessLogic userManagerBusinessLogic)
        {
            _projectRepository = projectRepository;
            _userManager = userManager;
            _helperMethods = new HelperMethods();
            _userManagerBusinessLogic = userManagerBusinessLogic;
        }



        public async Task<Ticket> GetTicketDetails(int? id)
        {
            Ticket ticket = _ticketRepository.Get(id);

            return (ticket);
        }
    }
}
