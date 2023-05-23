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
			;
            projectData = new List<Project> {
                new Project(1, "Project1", _usersData.First(), _usersData.First().Id),
				new Project(2, "Project2", _usersData.First(), _usersData.First().Id),
				
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

			List<Ticket> tickets = new List<Ticket>
			{
				new Ticket { Id = 1, ProjectId = 1 },
				new Ticket { Id = 2, ProjectId = 2 }
			};

			Mock<IRepository<Ticket>> mockTicketRepo = new Mock<IRepository<Ticket>>();

			mockProjectDbSet.Setup(m => m.Find(It.IsAny<object[]>()))
	                        .Returns<object[]>(ids => projectData.FirstOrDefault(p => p.Id == (int)ids[0]));



			mockTicketRepo.Setup(repo => repo.GetAll()).Returns(tickets.ToList());
			//mockUserProjectDbSet.Setup(repo => repo.GetAll()).Returns(userProjectData.ToList());
			mockProjectDbSet.Setup(m => m.Remove(It.IsAny<Project>())).Returns<Project>(entity =>
			{
				var entityEntry = new Mock<EntityEntry<Project>>();
				entityEntry.Setup(e => e.Entity).Returns(entity);
				return entityEntry.Object;
			});

			Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();

            mockContext.Setup(c => c.Projects).Returns(mockProjectDbSet.Object);
			mockContext.Setup(c => c.UserProjects).Returns(mockUserProjectDbSet.Object);
			mockContext.Setup(c => c.Remove(It.IsAny<UserProject>())).Callback(() => hasRemovedUserProject = true);

			ProjectBusinessLogic = new ProjectBL(
                new ProjectRepo(mockContext.Object),
				mockTicketRepo.Object,
				new CommentRepo(mockContext.Object),
                new TicketWatcherRepo(mockContext.Object),
				new UserProjectRepo(mockContext.Object),
				new UserRepo(mockContext.Object),
                MockUserManager(_usersData).Object
                );

		}

        public static Mock<UserManager<ApplicationUser>> MockUserManager(List<ApplicationUser> ls)
        {
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
        public void GetProject__ReturnsIdIsNull()
        {
			int? projectId = null;

			Project project = ProjectBusinessLogic.GetProject(projectId);

            Assert.IsNull(project);
		}

        [TestMethod]

        public void DeleteProject_ReturnsProjectCountAndProjectIsDeleted()
        {
            int projectIdToDelete = 1;

			int initialProjectCount = projectData.Count();

			Project deletedProject = ProjectBusinessLogic.DeleteProject(projectIdToDelete);

			projectData = projectData.Where(p => p.Id != projectIdToDelete);

			int updatedProjectCount = projectData.Count();

			Assert.AreEqual(projectIdToDelete, deletedProject.Id);

			Assert.AreEqual(initialProjectCount - 1, updatedProjectCount);

			bool isProjectDeleted = !projectData.Any(p => p.Id == projectIdToDelete);

			Assert.IsTrue(isProjectDeleted);

		}

		[TestMethod]
		public void DeleteProject_ThrowsArgumentNullExceptionOnNoArgument()
		{
			int projectIdToDelete = 3;


			Assert.ThrowsException<ArgumentNullException>(() => ProjectBusinessLogic.DeleteProject(projectIdToDelete));
		}

	}
}