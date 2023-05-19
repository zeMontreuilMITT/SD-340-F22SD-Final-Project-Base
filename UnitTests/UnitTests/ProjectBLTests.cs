using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private List<ApplicationUser> _usersData = new List<ApplicationUser>
        {
            new ApplicationUser() {UserName = "User1", Id = Guid.NewGuid().ToString(), Email = "user1@test.com"},
            new ApplicationUser() {UserName = "User2", Id = Guid.NewGuid().ToString(), Email = "user2@test.com" }
        };

        [TestInitialize]
        public void Initialize()
        {
            Mock<DbSet<Project>> mockProjectDbSet = new Mock<DbSet<Project>>();


            projectData = new List<Project> {
                new Project{ Id = 1, ProjectName = "Project1", CreatedById = _usersData.First().Id, CreatedBy = _usersData.First()},
                new Project{ Id = 2, ProjectName = "Project2", CreatedById = _usersData.First().Id, CreatedBy = _usersData.First()}

            }.AsQueryable();



            mockProjectDbSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(projectData.Provider);
            mockProjectDbSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(projectData.Expression);
            mockProjectDbSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(projectData.ElementType);
            mockProjectDbSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(projectData.GetEnumerator());

            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();

            mockContext.Setup(c => c.Projects).Returns(mockProjectDbSet.Object);

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
        public void TestMethod1()
        {
        }
    }
}