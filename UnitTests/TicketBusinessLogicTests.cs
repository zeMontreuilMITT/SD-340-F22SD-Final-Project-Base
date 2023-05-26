using Castle.Core.Resource;
using JelloTicket.BusinessLayer.Services;
using JelloTicket.BusinessLayer.ViewModels;
using JelloTicket.DataLayer.Data;
using JelloTicket.DataLayer.Models;
using JelloTicket.DataLayer.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Ticket = JelloTicket.DataLayer.Models.Ticket;

namespace UnitTests
{
    [TestClass]
    public class TicketBusinessLogicTests
    {
        public TicketBusinessLogic ticketBL { get; set; }        
        public IQueryable<Ticket> ticketData { get; set; }
        public IQueryable<Project> projectData { get; set; }
        public IQueryable<ApplicationUser> users { get; set; }

        private Mock<IRepository<Ticket>> _ticketRepositoryMock;

        private Mock<IRepository<Project>> _projectRepositoryMock;

        private Mock<IRepository<Comment>> _commentRepositoryMock;

        private readonly UserManager<ApplicationUser> _userManager = MockUserManager<ApplicationUser>(_users).Object;

        private static List<ApplicationUser> _users = new List<ApplicationUser>
        {
            new ApplicationUser{UserName = "Jim", Id = Guid.NewGuid().ToString()},
            new ApplicationUser{UserName = "Tom", Id = Guid.NewGuid().ToString()},
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
            List<Ticket> tickets = new List<Ticket>
            {
                new Ticket {Id = 1, Title = "Update User Profile", Body = "Implement functionality to update user profiles", RequiredHours = 8, Completed = false },
                new Ticket {Id = 2, Title = "Fix Login Bug", Body = "Investigate and fix the bug causing login issues for some users", RequiredHours = 6, Completed = true },
                new Ticket {Id = 3, Title = "Implement Payment Gateway", Body = "Integrate a payment gateway to enable online transactions", RequiredHours = 10, Completed = false },
                new Ticket {Id = 4, Title = "Improve Search Algorithm", Body = "Optimize the search algorithm to provide faster and more accurate results", RequiredHours = 12, Completed = false }
            };
            ticketData = tickets.AsQueryable();

            Mock<DbSet<Ticket>> mockTickets = new Mock<DbSet<Ticket>>();
            mockTickets.As<IQueryable<Ticket>>().Setup(m => m.Provider).Returns(ticketData.Provider);
            mockTickets.As<IQueryable<Ticket>>().Setup(m => m.ElementType).Returns(ticketData.ElementType);
            mockTickets.As<IQueryable<Ticket>>().Setup(m => m.Expression).Returns(ticketData.Expression);
            mockTickets.As<IQueryable<Ticket>>().Setup(m => m.GetEnumerator()).Returns(() => ticketData.GetEnumerator());

            List<Project> projects = new List<Project>
            {
                new Project{Id = 101, ProjectName = "TestName" },
                new Project{Id = 102, ProjectName = "TestName2" },
                new Project{Id = 103, ProjectName = "TestName3" }
            };
            projectData = projects.AsQueryable();

            Mock<DbSet<Project>> mockProjects = new Mock<DbSet<Project>>();
            mockProjects.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(projectData.Provider);
            mockProjects.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(projectData.ElementType);
            mockProjects.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(projectData.Expression);
            mockProjects.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(() => projectData.GetEnumerator());          
      
            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(t => t.Tickets).Returns(mockTickets.Object);
            mockContext.Setup(c => c.Projects).Returns(mockProjects.Object);
            
            // i am literally too lazy to put types here please understand
            Mock<IRepository<Ticket>> ticketRepositoryMock = new Mock<IRepository<Ticket>>();
            ticketRepositoryMock.Setup(tr => tr.GetAll()).Returns(ticketData.ToList());
            ticketRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>()))
                .Returns((int ticketId) => ticketData.FirstOrDefault(p => p.Id == ticketId));
            ticketRepositoryMock.Setup(tr => tr.Create(It.IsAny<Ticket>()))
               .Callback((Ticket ticket) => ticket.Id = 201 );
            _ticketRepositoryMock = ticketRepositoryMock;

            Mock<IRepository<Project>> projectRepositoryMock = new Mock<IRepository<Project>>();
            projectRepositoryMock.Setup(tr => tr.GetAll()).Returns(projectData.ToList());
            projectRepositoryMock.Setup(tr => tr.Get(It.IsAny<int>()))
                .Returns((int ticketId) => projectData.FirstOrDefault(p => p.Id == ticketId));
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
            
           
        }


        [TestMethod]
        public void GetTicketFromId_Ideal()
        {
            // Arrange 
            Ticket realTicket = ticketData.First();

            // Act
            Ticket returnedTicket = ticketBL.GetTicketById(realTicket.Id);

            // Assert
            Assert.AreEqual(realTicket, returnedTicket);
        }


        [TestMethod]
        public async Task GetTicketDetails_WithId_ReturnsTicket()
        {
            // Arrange
            Ticket realTicket = ticketData.First();

            // Act
            Ticket ticketTested = await ticketBL.GetTicketDetails(realTicket.Id);

            // Assert
            Assert.AreEqual(realTicket, ticketTested);
        }


        [TestMethod]
        public async Task GetTicketDetails_WithNullId_ThrowsException()
        {
            // Arrange
            int? nullId = null;

            //Act & Assert

            await Assert.ThrowsExceptionAsync<NullReferenceException>(async () =>
            {
                await ticketBL.GetTicketDetails(nullId);
            });
        }


        [TestMethod]
        public void RemoveTicket_WithTicket_DeletesTicketFromDb()
        {
            // Arrange
            int initialCountOfTickets = ticketData.Count();

            Ticket ticketTested = ticketBL.GetTicketById(1);
            
            int ticketId = ticketTested.Id;

            _ticketRepositoryMock.Setup(tr => tr.Delete(ticketId)).Callback(() =>
            {
                ticketData = ticketData.Where(t => t.Id != ticketId);
            });

            // Act
            ticketBL.RemoveTicket(ticketTested);

            int finalCountOfTickets = ticketData.Count();

            // Assert
            Assert.AreEqual(initialCountOfTickets - 1, finalCountOfTickets);
        }


        [TestMethod]
        public void RemoveTicket_NullTicket_ThrowsInvalidOperationException()
        {
            // Arrange
            Ticket nullTicket = null;

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => { ticketBL.RemoveTicket(nullTicket); });
        }


        [TestMethod]
        public void GetTickets_ReturnsAllTickets()
        {
            // Arrange
            List<Ticket> expectedTickets = ticketData.ToList();

            // Act
            ICollection<Ticket> actualTickets = ticketBL.GetTickets();

            // Assert
            CollectionAssert.AreEqual(expectedTickets, actualTickets.ToList());
        }


        [TestMethod]
        public void CreatePost_WithValidArgs_AddsTicketToDb()
        {
            // Arrange
            int projectId = projectData.First().Id;

            Ticket ticket = new Ticket()
            {
                Title = "Test Ticket Title",
                Body = "Test Ticket Body",
                ProjectId = projectId,
                RequiredHours = 10,
                TicketPriority = Ticket.Priority.High
            };

            // Act
            Ticket ticketResult = ticketBL.CreatePost(projectId, _users.First().Id, ticket);

            // Assert
            Assert.AreEqual(ticketResult.Id, 201);
        }

        [TestMethod]
        public void CreatePost_WithNullArgs_ThrowsException()
        {
            // Arrange

            int? nullId = null;

            int projectId = projectData.First().Id;

            Ticket ticket = new Ticket()
            {
                Title = "Test Ticket Title",
                Body = "Test Ticket Body",
                ProjectId = projectId,
                RequiredHours = 10,
                TicketPriority = Ticket.Priority.High
            };

            // Act and Assert
            Assert.ThrowsException<ArgumentNullException>(() => ticketBL.CreatePost(projectId, null, ticket));
        }
    }
}
