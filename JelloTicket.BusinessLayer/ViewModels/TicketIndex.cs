using JelloTicket.DataLayer.Models;
using Microsoft.Extensions.FileSystemGlobbing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JelloTicket.BusinessLayer.ViewModels
{
    public class TicketIndex
    {
      
            public Ticket Ticket { get; set; }
            public Project Project { get; set; }
            public IEnumerable<ApplicationUser> TicketWatchers { get; set; }
            public ApplicationUser Owner { get; set; }
            public IEnumerable<Comment> Comments { get; set; }
            public ApplicationUser CommentCreator { get; set; }
        

    }
}