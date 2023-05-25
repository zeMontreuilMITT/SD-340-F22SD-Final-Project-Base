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
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Project = JelloTicket.DataLayer.Models.Project;

namespace UnitTests
{
    [TestClass]
    public class ProjectBusinessLogicTests
    {
        public ProjectBusinessLogic projectBL { get; set; }
        public List<Project> data { get; set; }
        public IQueryable<ApplicationUser> users { get; set; }
        private Mock<IRepository<Project>> _projectRepositoryMock;

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
            data = stub;

            List<ApplicationUser> users = new List<ApplicationUser>
            {
                new ApplicationUser{UserName = "Jim"}
            };

            Mock<DbSet<Project>> mockProjects = new Mock<DbSet<Project>>();
            mockProjects.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(data.AsQueryable().Provider);
            mockProjects.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(data.AsQueryable().ElementType);
            mockProjects.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(data.AsQueryable().Expression);
            mockProjects.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(() => data.AsQueryable().GetEnumerator());

            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Projects).Returns(mockProjects.Object);
            // setup the id to pass int and find the project to remove through int
            mockContext.Setup(c => c.DeleteStuff(It.IsAny<Project>()))
                .Callback<Project>(a =>
                {
                    Project projectToRemove = data.FirstOrDefault(p => p.Id == a.Id);
                    if (projectToRemove != null)
                    {
                        data.Remove(projectToRemove);
                    }
                });

            // i am literally too lazy to put types here please understand
            var projectRepositoryMock = new Mock<IRepository<Project>>();
            _projectRepositoryMock = projectRepositoryMock;

            var userManagerBusinessLogic = new Mock<UserManagerBusinessLogic>();
            var userProjectRepository = new Mock<UserProjectRepo>();
            var ticketRepository = new Mock<IRepository<Ticket>>();
            var userProjectRepo = new Mock<IUserProjectRepo>();

            projectBL = new ProjectBusinessLogic(
                projectRepositoryMock.Object,
                _userManager,
                userManagerBusinessLogic.Object,
                userProjectRepository.Object,
                ticketRepository.Object,
                userProjectRepo.Object
            );

            projectRepositoryMock.Setup(pr => pr.Get(It.IsAny<int>()))
                .Returns((int projectId) => data.FirstOrDefault(p => p.Id == projectId));

            projectRepositoryMock.Setup(pr => pr.Delete(It.IsAny<int?>()))
                .Callback<int?>(projectId =>
                {
                    if (projectId.HasValue)
                    {
                        Project projectToRemove = data.FirstOrDefault(p => p.Id == projectId.Value);
                        if (projectToRemove != null)
                        {
                            data.Remove(projectToRemove);
                        }
                    }
                });

            userProjectRepository.Setup(pr => pr.GetAll())
                .Returns(new List<UserProject>());

        }

        

        [TestMethod]
        public void GetProject_WithValidId_ReturnsProject()
        {
            // Arrange 
            Project realProject = data.First();

            // Act
            Project returnedProject = projectBL.GetProject(realProject.Id);

            // Assert
            Assert.AreEqual(realProject, returnedProject);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetProject_ProjectNotFound_ThrowsException()
        {
            // Act
            // 9999 not found in database => throws argumentnullexception from business logic
            Project nullProject = projectBL.GetProject(9999);
        }

        [TestMethod]
        public void CreateProject_ReturnsTrue_OnSuccessfulCreation()
        {
            // Arrange
            Project project = new Project{
                Id = 1,
                ProjectName= "Test",
            };
            _projectRepositoryMock.Setup(pr => pr.Create(It.IsAny<Project>()))
                .Callback<Project>(p => project.Id = 1);

            _projectRepositoryMock.Setup(pr => pr.Get(It.IsAny<int>()))
                .Returns(project);

            // Act
            bool result = projectBL.CreateProject(project);
            Project returnedProject = projectBL.GetProject(project.Id);

            // Assert
            Assert.IsTrue(result);

            // assert if the project creation was successful
            Assert.IsNotNull(returnedProject);
            Assert.AreEqual(project, returnedProject);
            _projectRepositoryMock.Verify(pr => pr.Create(project), Times.Once);
        }

        [TestMethod]
        public void CreateProject_ReturnsFalse_OnException()
        {
            // Arrange
            Project project = new Project();
            _projectRepositoryMock.Setup(pr => pr.Create(project)).Throws(new Exception());

            // Act
            bool result = projectBL.CreateProject(project);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetProjectsWithAssociations_OnSuccess_ReturnsAllProjects()
        {
            // Arrange
            ApplicationUser newUser = new ApplicationUser { UserName = "CreatorUser" };
            Project project = new Project
            {
                Id = 555,
                CreatedBy = newUser,
            };
            Ticket ticket = new Ticket{
                Body = "body",
                Owner = new ApplicationUser { UserName = "TestAppUser" },
                ProjectId = project.Id,
                RequiredHours = 15
            };

            List<Ticket> tickets = new List<Ticket> { ticket };

            project.Tickets = tickets;

            List<Project> projectList = new List<Project> { project };
            _projectRepositoryMock.Setup(pr => pr.GetAll())
                .Returns(projectList);

            // Act
            List<Project> returnedProjects = projectBL.GetProjectsWithAssociations().ToList();

            // Assert

            Assert.IsTrue(returnedProjects.Any());
            Assert.IsNotNull(returnedProjects[0]);
            Assert.AreEqual(returnedProjects[0], project);
            Assert.AreEqual(returnedProjects[0].CreatedBy, project.CreatedBy);
        }

        [TestMethod]
        public void DeleteProjectAndAssociations_WithProjectId_SuccessfullyDeleteProjectReturnsTrue()
        {
            // Arrange
            Project project = projectBL.GetProject(102);
            // ACt
            bool isDeleted = projectBL.DeleteProjectAndAssociations(project.Id).Result;
            // Assert
            
            Assert.IsTrue(isDeleted);
            // only returns false when project is not found in context! 
            Assert.IsFalse(projectBL.DeleteProjectAndAssociations(project.Id).Result);
        }

        [TestMethod]
        public void DeleteProjectAndAssociations_WithInvalidId_ReturnsFalse()
        {
            //Arrange
            int invalidID = 7777;

            // Act/Assert
            Assert.IsFalse(projectBL.DeleteProjectAndAssociations(invalidID).Result);
        }

        [TestMethod]
        // I'm unsure why I'm unable to set up the repo to handle this correctly
        // The internal method is returning null so it must be using the wrong repo
        // but I don't understand how it could be doing that
        public void RemoveAssignedUser_WithValidArgs_RemoveUserFromProject()
        {
            // Arrange
            Project project = new Project { Id = 555 };
            ApplicationUser user = new ApplicationUser { Id = 666.ToString() };
            UserProject userProject = new UserProject
            {
                ProjectId = project.Id,
                UserId = user.Id
            };

            List<UserProject> userProjects = new List<UserProject>
            {
                userProject,
            };

            var userProjectRepo = new Mock<UserProjectRepo>();
            userProjectRepo.Setup(pr => pr.GetAll())
                .Returns(userProjects);

            // Act/Assert

            Assert.IsTrue(projectBL.RemoveAssignedUser(user.Id, project.Id).Result);
            
        }

    }
}
