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
        private readonly IRepository<Comment> _commentRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HelperMethods _helperMethods;
        private readonly UserManagerBusinessLogic _userManagerBusinessLogic;

        public TicketBusinessLogic(IRepository<Ticket> ticketRepository
            , UserManager<ApplicationUser> userManager
            , UserManagerBusinessLogic userManagerBusinessLogic)
        {
            _ticketRepository = ticketRepository;
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

        //public async Task<Comment> MakeCommentToTask(int TaskId, string? TaskText)
        //{
        //    if (TaskId != null || TaskText != null)
        //    {
        //        try
        //        {
        //            Comment newComment = new Comment();
        //            string userName = _userManager;
        //            ApplicationUser applicationUser = _userManager.Users.FirstOrDefault(u => u.UserName == userName);
        //            Ticket ticket = _ticketRepository.Get(TaskId);

        //            newComment.CreatedBy = applicationUser;
        //            newComment.Description = TaskText;
        //            newComment.Ticket = ticket;
        //            applicationUser.Comments.Add(newComment);
        //            _commentRepository.Create(newComment);
        //            ticket.Comments.Add(newComment);

        //            int Id = TaskId;

        //            return RedirectToAction("Details", new{ Id });
        //        }
        //        catch (Exception ex)
        //        {
        //            return RedirectToAction("Error", "Home");
        //        }
        //    }
        //}

    }
}
