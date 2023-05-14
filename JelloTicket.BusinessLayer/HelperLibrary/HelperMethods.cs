using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JelloTicket.DataLayer.Models;
using JelloTicket.DataLayer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JelloTicket.BusinessLayer.HelperLibrary
{
    public class HelperMethods
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Ticket> _ticketRepository;

        public ICollection<Comment> GetCommentsByTicketId(int? ticketId)
        {
            ICollection<Comment> comments = _commentRepository.GetAll()
                .Where(c => c.TicketId == ticketId).ToHashSet();

            return comments;
        }

        public ICollection<Ticket> GetTicketsByProjectId(int? projectId)
        {
            ICollection<Ticket> tickets = _ticketRepository.GetAll()
                .Where(t => t.ProjectId == projectId).ToHashSet();

            return tickets;
        }

        public void MarkTicketAsCompleted(int? id)
        {
            Ticket repoTicket = _ticketRepository.Get(id);
            repoTicket.Completed = true;

            _ticketRepository.Update(repoTicket);
        }

        public void UnMarkTicketAsCompleted(int? id)
        {
            Ticket repoTicket = _ticketRepository.Get(id);
            repoTicket.Completed = false;

            _ticketRepository.Update(repoTicket);
        }
    }
}
