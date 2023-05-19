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
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace JelloTicket.BusinessLayer.Services
{
    public class TicketBusinessLogic
    {
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IRepository<DataLayer.Models.Project> _projectRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HelperMethods _helperMethods;
        private readonly UserManagerBusinessLogic _userManagerBusinessLogic;

        public TicketBusinessLogic(IRepository<Ticket> ticketRepository
            , IRepository<DataLayer.Models.Project> projectRepository
            , IRepository<Comment> commentRepository
            , UserManager<ApplicationUser> userManager
            , UserManagerBusinessLogic userManagerBusinessLogic)
        {
            _ticketRepository = ticketRepository;
            _projectRepository = projectRepository;
            _commentRepository = commentRepository;
            _userManager = userManager;
            _userManagerBusinessLogic = userManagerBusinessLogic;
        }

        public Ticket GetTicketById(int? id)
        {
            if (id == null)
            {
                throw new NullReferenceException("Id is NUll");
            }
            else
            {
                Ticket ticket = _ticketRepository.Get(id);
                if (ticket == null)
                {
                    throw new Exception("ticket with given id is not found");
                }
                else
                {
                    // Only return the Ticket for the HTTP GET method
                    return ticket;
                }
            }
        }

        public async Task<Ticket> GetTicketDetails(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Ticket? ticket = _ticketRepository.Get(id);

            return (ticket);
        }
       
        public void RemoveUser(string userId, int ticketId)
        {
            if (userId == null)
            {
                throw new ArgumentException(nameof(userId));
            }

            Ticket currentTicket = _ticketRepository.Get(ticketId);

            ApplicationUser applicationUser = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            
            if (currentTicket != null)
            {
                    currentTicket.Owner = applicationUser;
            }
        }

        public void Save()
        {
            _ticketRepository.Save();
        }

        public void RemoveTicket(Ticket ticket)
        {
            int ticketId = ticket.Id;
            _ticketRepository.Delete(ticketId);
        }

        public bool DoesTicketExist(int id)
        {
            return _ticketRepository.Exists(id);
        }
    }
}
