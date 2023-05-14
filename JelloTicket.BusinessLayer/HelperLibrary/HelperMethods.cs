using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JelloTicket.DataLayer.Models;
using JelloTicket.DataLayer.Repositories;

namespace JelloTicket.BusinessLayer.HelperLibrary
{
    public class HelperLibrary
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Ticket> _ticketRepository;

        public ICollection<Comment> GetCommentsByTicketId(int? ticketId)
        {
            ICollection<Comment> comments = _commentRepository.GetAll().Where(c => c.Ticket.Id == ticketId).ToHashSet();

            return comments;
        }
    }
}
