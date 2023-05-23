using JelloTicket.BusinessLayer.Services;
using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using JelloTicket.DataLayer.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket = JelloTicket.DataLayer.Models.Ticket;

namespace UnitTests
{
    [TestClass]
    public class TicketBusinessLogicTests
    {
        public TicketBusinessLogic ticketBL { get; set; }
        public IQueryable<Ticket> data { get; set; }
        public IQueryable<ApplicationUser> users { get; set; }

        private Mock<IRepository<Ticket>> _ticketRepositoryMock;

        private Mock<IRepository<Project>> _projectRepositoryMock;

        private Mock<IRepository<Comment>> _commentRepositoryMock;

        private readonly UserManager<ApplicationUser> _userManager = MockUserManager<ApplicationUser>(_users).Object;

        private static List<ApplicationUser> _users = new List<ApplicationUser>
        {
            new ApplicationUser{UserName = "Jim"},
            new ApplicationUser{UserName = "Tom"},
        };

        // spooky magic I did not write!
        // https://stackoverflow.com/questions/49165810/how-to-mock-usermanager-in-net-core-testing
        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            Mock<IUserStore<TUser>> store = new Mock<IUserStore<TUser>>();
            Mock<UserManager<TUser>> mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }

        [TestInitialize]
        public void Initialize()
        {
            List<Ticket> tasks = new List<Ticket>
            {
                new Ticket {Id = 1, Title = "Update User Profile", Body = "Implement functionality to update user profiles", RequiredHours = 8, Completed = false },
                new Ticket {Id = 2, Title = "Fix Login Bug", Body = "Investigate and fix the bug causing login issues for some users", RequiredHours = 6, Completed = true },
                new Ticket {Id = 3, Title = "Implement Payment Gateway", Body = "Integrate a payment gateway to enable online transactions", RequiredHours = 10, Completed = false },
                new Ticket {Id = 4, Title = "Improve Search Algorithm", Body = "Optimize the search algorithm to provide faster and more accurate results", RequiredHours = 12, Completed = false }
            };
            data = tasks.AsQueryable();

            List<ApplicationUser> users = new List<ApplicationUser>
            {
                new ApplicationUser{UserName = "Jim"}
            };

            Mock<DbSet<Ticket>> mockTickets = new Mock<DbSet<Ticket>>();
            mockTickets.As<IQueryable<Ticket>>().Setup(m => m.Provider).Returns(data.Provider);
            mockTickets.As<IQueryable<Ticket>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockTickets.As<IQueryable<Ticket>>().Setup(m => m.Expression).Returns(data.Expression);
            mockTickets.As<IQueryable<Ticket>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(t => t.Tickets).Returns(mockTickets.Object);

            // i am literally too lazy to put types here please understand
            Mock<IRepository<Ticket>> ticketRepositoryMock = new Mock<IRepository<Ticket>>();
            _ticketRepositoryMock = ticketRepositoryMock;

            Mock<IRepository<Project>> projectRepositoryMock = new Mock<IRepository<Project>>();
            _projectRepositoryMock = projectRepositoryMock;

            Mock<IRepository<Comment>> commentRepositoryMock = new Mock<IRepository<Comment>>();
            _commentRepositoryMock = commentRepositoryMock;

            Mock<UserManagerBusinessLogic> userManagerBusinessLogic = new Mock<UserManagerBusinessLogic>();
            Mock<UserProjectRepo> userProjectRepository = new Mock<UserProjectRepo>();
            Mock<IRepository<Ticket>> ticketRepository = new Mock<IRepository<Ticket>>();
            Mock<IUserProjectRepo> userProjectRepo = new Mock<IUserProjectRepo>();
            Mock<TicketWatcherRepo> ticketWatcherRepo = new Mock<TicketWatcherRepo>();

            ticketBL = new TicketBusinessLogic(
                ticketRepositoryMock.Object,
                projectRepositoryMock.Object,
                commentRepositoryMock.Object,
                _userManager,
                userManagerBusinessLogic.Object,
                userProjectRepository.Object,
                ticketWatcherRepo.Object
            );

            ticketRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>()))
                .Returns((int ticketId) => data.FirstOrDefault(p => p.Id == ticketId));
        }
        [TestMethod]


        public void GetTicketFromId_Ideal()
        {
            // Arrange 
            Ticket realTicket = data.First();

            // Act
            Ticket returnedTicket = ticketBL.GetTicketById(realTicket.Id);

            // Assert
            Assert.AreEqual(realTicket, returnedTicket);
        }

    }
}
