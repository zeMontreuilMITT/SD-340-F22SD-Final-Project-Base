using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;

namespace UnitTests
{
    [TestClass]
    public class ProjectBLTests
    {
		public ProjectBL ProjectBusinessLogic { get; set; }
		public IQueryable<Project> projectData { get; set; }

		public IQueryable<UserProject> userProjectData { get; set; }

		public IQueryable<Ticket> ticketData { get; set; }

		private bool hasRemovedUserProject = false;


		private List<ApplicationUser> _usersData = new List<ApplicationUser>

        {
            new ApplicationUser() {UserName = "User1", Id = Guid.NewGuid().ToString(), Email = "user1@test.com"},
            new ApplicationUser() {UserName = "User2", Id = Guid.NewGuid().ToString(), Email = "user2@test.com" }
        };

        [TestInitialize]
        public void Initialize()
        {

			Mock<DbSet<Project>> mockProjectDbSet = new Mock<DbSet<Project>>();
			Mock<DbSet<UserProject>> mockUserProjectDbSet = new Mock<DbSet<UserProject>>();
			Mock<DbSet<Ticket>> mockTicketDbSet = new Mock<DbSet<Ticket>>();

			
			projectData = new List<Project> {
				new Project{ Id = 1, ProjectName = "Project1", CreatedById = _usersData.First().Id, CreatedBy = _usersData.First()},
				new Project{ Id = 2, ProjectName = "Project2", CreatedById = _usersData.First().Id, CreatedBy = _usersData.First()}

			}.AsQueryable();

			mockProjectDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(projectData.Provider);
			mockProjectDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(projectData.Expression);
			mockProjectDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(projectData.ElementType);
			mockProjectDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(projectData.GetEnumerator());


			userProjectData = new List<UserProject> {
				new UserProject{ Id = 1, ProjectId = projectData.First().Id, UserId = _usersData.First().Id},
				new UserProject{ Id = 2, ProjectId = projectData.Last().Id, UserId = _usersData.Last().Id},
			}.AsQueryable();

			mockUserProjectDbSet.As<IQueryable<UserProject>>().Setup(m => m.Provider).Returns(userProjectData.Provider);
			mockUserProjectDbSet.As<IQueryable<UserProject>>().Setup(m => m.Expression).Returns(userProjectData.Expression);
			mockUserProjectDbSet.As<IQueryable<UserProject>>().Setup(m => m.ElementType).Returns(userProjectData.ElementType);
			mockUserProjectDbSet.As<IQueryable<UserProject>>().Setup(m => m.GetEnumerator()).Returns(userProjectData.GetEnumerator());


			ticketData = new List<Ticket>
			{
				new Ticket {Id = 1, Title = "Ticket1", Body = "abcd", RequiredHours = 10, OwnerId = _usersData.First().Id, ProjectId = projectData.FirstOrDefault().Id},
				new Ticket {Id = 2, Title = "Ticket2", Body = "efgh", RequiredHours = 20, OwnerId = _usersData.First().Id, ProjectId = projectData.FirstOrDefault().Id}
			}.AsQueryable();

			mockTicketDbSet.As<IQueryable<Ticket>>().Setup(m => m.Provider).Returns(ticketData.Provider);
			mockTicketDbSet.As<IQueryable<Ticket>>().Setup(m => m.Expression).Returns(ticketData.Expression);
			mockTicketDbSet.As<IQueryable<Ticket>>().Setup(m => m.ElementType).Returns(ticketData.ElementType);
			mockTicketDbSet.As<IQueryable<Ticket>>().Setup(m => m.GetEnumerator()).Returns(ticketData.GetEnumerator());


			mockProjectDbSet.Setup(m => m.Find(It.IsAny<object[]>()))
	                        .Returns<object[]>(ids => projectData.FirstOrDefault(p => p.Id == (int)ids[0]));

			mockProjectDbSet.Setup(m => m.Remove(It.IsAny<Project>())).Returns<Project>(entity =>
			{
				var entityEntry = new Mock<EntityEntry<Project>>();
				entityEntry.Setup(e => e.Entity).Returns(entity);
				return entityEntry.Object;
			});

			Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();

            mockContext.Setup(c => c.Projects).Returns(mockProjectDbSet.Object);
			mockContext.Setup(c => c.UserProjects).Returns(mockUserProjectDbSet.Object);
			mockContext.Setup(c => c.Tickets).Returns(mockTicketDbSet.Object);
			mockContext.Setup(c => c.Remove(It.IsAny<UserProject>())).Callback(() => hasRemovedUserProject = true);


			ProjectBusinessLogic = new ProjectBL(
                new ProjectRepo(mockContext.Object),
				new TicketRepo(mockContext.Object),
				new CommentRepo(mockContext.Object),
                new TicketWatcherRepo(mockContext.Object),
				new UserProjectRepo(mockContext.Object),
				new UserRepo(mockContext.Object),
                MockUserManager(_usersData).Object
                );


		}


        public static Mock<UserManager<ApplicationUser>> MockUserManager(List<ApplicationUser> ls)
        {
			/// This method copied from Stack Overflow User
            /// https://stackoverflow.com/questions/49165810/how-to-mock-usermanager-in-net-core-testing
			/// 
            Mock<IUserStore<ApplicationUser>> store = new Mock<IUserStore<ApplicationUser>>();

            Mock<UserManager<ApplicationUser>> mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            mgr.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<ApplicationUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }

        [TestMethod]
		public void GetProject_ReturnsProjectWithIdOfArgument()
		{
			Project project = ProjectBusinessLogic.GetProject(projectData.FirstOrDefault()?.Id);
			Assert.AreEqual(projectData.First().Id, project.Id);
		}

        [TestMethod]
        public void GetProject__ReturnsNullWhenArgumentIsNull()
        {
			int? projectId = null;
			Project? project = ProjectBusinessLogic.GetProject(projectId);

            Assert.IsNull(project);
		}


		[TestMethod]
		public void DeleteProject_ReturnsDeletedProject()
		{
			int projectIdToDelete = projectData.FirstOrDefault().Id;

			Project deletedProject = ProjectBusinessLogic.DeleteProject(projectIdToDelete);

			Assert.AreEqual(ProjectBusinessLogic.GetProject(projectIdToDelete), deletedProject);
		}

		[TestMethod]
		public void DeleteProject_ThrowsArgumentNullExceptionOnNoArgument()
		{
			int projectIdToDelete = 3;


			Assert.ThrowsException<ArgumentNullException>(() => ProjectBusinessLogic.DeleteProject(projectIdToDelete));
		}

	

        public void RemoveUserFromProject_Should_Delete_UserProject()
        {
            // act
            ProjectBusinessLogic.RemoveUserFromProject(_usersData.FirstOrDefault()?.Id, projectData.First().Id);
            // assert
            Assert.IsTrue(hasRemovedUserProject);
        }

        [TestMethod]
        public void RemoveUserFromProject_NonExistingUser_Should_Not_Delete_UserProject()
        {
            // act
            ProjectBusinessLogic.RemoveUserFromProject("WrongUserId", projectData.First().Id);
            // assert
            Assert.IsFalse(hasRemovedUserProject);
        }

        [TestMethod]
        public void RemoveUserFromProject_WithNullUserId_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ProjectBusinessLogic.RemoveUserFromProject(null, projectData.First().Id));
        }
    }

}