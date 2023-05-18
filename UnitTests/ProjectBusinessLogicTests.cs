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
        public IQueryable<Project> data { get; set; }
        public IQueryable<ApplicationUser> users { get; set; }

        private readonly UserManager<ApplicationUser> _userManager = MockUserManager<ApplicationUser>(_users).Object;

        private static List<ApplicationUser> _users = new List<ApplicationUser>
        {
            new ApplicationUser{UserName = "Jim"},
            new ApplicationUser{UserName = "Tom"},
        };

        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
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
            var ticketRepository = new Mock<IRepository<Ticket>>();

            projectBL = new ProjectBusinessLogic(
                projectRepositoryMock.Object,
                _userManager,
                userManagerBusinessLogic.Object,
                userProjectRepository.Object,
                ticketRepository.Object
            );

            projectRepositoryMock.Setup(pr => pr.Get(It.IsAny<int>()))
                .Returns((int projectId) => data.FirstOrDefault(p => p.Id == projectId));

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
