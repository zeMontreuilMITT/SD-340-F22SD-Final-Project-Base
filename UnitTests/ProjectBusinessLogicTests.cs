using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using JelloTicket.DataLayer.Models;
using JelloTicket.DataLayer.Repositories;
using JelloTicket.BusinessLayer.HelperLibrary;
using JelloTicket.BusinessLayer.Services;
using Microsoft.EntityFrameworkCore;
using JelloTicket.DataLayer.Data;
using Microsoft.AspNetCore.Identity;

namespace UnitTests
{
    [TestClass]
    public class ProjectBusinessLogicTests
    {
        public ProjectBusinessLogic projectBL { get; set; }
        public IQueryable<Project> data { get; set;}
        public IQueryable<ApplicationUser> users { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            List<Project> stub = new List<Project>
            {
                new Project{Id = 101, ProjectName = "TestName" },
                new Project{Id = 102, ProjectName = "TestName2" },
                new Project{Id = 103, ProjectName = "TestName3" }
            };
            data = stub.AsQueryable();

            List<ApplicationUser> users = new List<ApplicationUser>
            {
                new ApplicationUser{UserName = "Jim"}
            };

            Mock<DbSet<Project>> mockProjects = new Mock<DbSet<Project>>();
            mockProjects.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(data.Provider);
            mockProjects.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockProjects.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(data.Expression);
            mockProjects.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Projects).Returns(mockProjects.Object);

            // i am literally too lazy to put types here please understand
            var projectRepositoryMock = new Mock<IRepository<Project>>();

            var userManagerBusinessLogic = new Mock<UserManagerBusinessLogic>();
            var userProjectRepository = new Mock<UserProjectRepo>();
            var ticketRepository = new Mock<TicketRepo>();

            projectBL = new ProjectBusinessLogic(
                projectRepositoryMock.Object,
                userManager,
                userManagerBusinessLogic.Object,
                userProjectRepository.Object,
                ticketRepository.Object
            );

            
        }

        [TestMethod]
        public void GetProject_ReturnsProjectFromId()
        {
            // 
            Project realProject = data.First();
            Project returnedProject = projectBL.GetProject(realProject.Id);

            // Assert
            Assert.AreEqual(realProject, returnedProject);

        }
    }
}
