using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using SD_340_W22SD_Final_Project_Group6.BLL;
using SD_340_W22SD_Final_Project_Group6.Data;
using SD_340_W22SD_Final_Project_Group6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class TicketBLTests
    {
		public TicketBL TicketBusinessLogic { get; set; }
		public IQueryable<Ticket> ticketData { get; set; }

		private List<ApplicationUser> _usersData = new List<ApplicationUser>
		{
			new ApplicationUser() {UserName = "User1", Id = Guid.NewGuid().ToString(), Email = "user1@test.com"},
			new ApplicationUser() {UserName = "User2", Id = Guid.NewGuid().ToString(), Email = "user2@test.com" }
		};

		[TestInitialize]
		public void Initialize()
		{
			Mock<DbSet<Ticket>> mockTicketDbSet = new Mock<DbSet<Ticket>>();

			ticketData = new List<Ticket>
			{
				new Ticket {Id = 1, Title = "Ticket1", Body = "abcd", RequiredHours = 10, OwnerId = _usersData.First().Id},
				new Ticket {Id = 2, Title = "Ticket2", Body = "efgh", RequiredHours = 20, OwnerId = _usersData.First().Id}
			}.AsQueryable();

			mockTicketDbSet.As<IQueryable<Ticket>>().Setup(m => m.Provider).Returns(ticketData.Provider);
			mockTicketDbSet.As<IQueryable<Ticket>>().Setup(m => m.Expression).Returns(ticketData.Expression);
			mockTicketDbSet.As<IQueryable<Ticket>>().Setup(m => m.ElementType).Returns(ticketData.ElementType);
			mockTicketDbSet.As<IQueryable<Ticket>>().Setup(m => m.GetEnumerator()).Returns(ticketData.GetEnumerator());

			Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();

			mockContext.Setup(x => x.Tickets).Returns(mockTicketDbSet.Object);

			TicketBusinessLogic = new TicketBL(
				new ProjectRepo(mockContext.Object),
				new TicketRepo(mockContext.Object),
				new CommentRepo(mockContext.Object),
				new TicketWatcherRepo(mockContext.Object),
				new UserProjectRepo(mockContext.Object),
				new UserRepo(mockContext.Object),
				MockUserManager(_usersData).Object);
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
