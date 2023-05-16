using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using JelloTicket.DataLayer.Models;
using JelloTicket.DataLayer.Repositories;
using JelloTicket.BusinessLayer.HelperLibrary;
using JelloTicket.BusinessLayer.Services;
using Microsoft.EntityFrameworkCore;
using JelloTicket.DataLayer.Data;

namespace UnitTests
{
    [TestClass]
    public class ProjectBusinessLogicTests
    {
        public ProjectBusinessLogic projectBL { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            List<Project> stub = new List<Project>
            {
                new Project{Id = 101, ProjectName = "TestName" },
                new Project{Id = 102, ProjectName = "TestName2" },
                new Project{Id = 103, ProjectName = "TestName3" }
            };
            var data = stub.AsQueryable();

            Mock<DbSet<Project>> mockProjects = new Mock<DbSet<Project>>();
            Mock<ApplicationDbContext> mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Projects).Returns(mockProjects.Object);
            mockContext.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(data.Provider);
            mockContext.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockContext.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(data.Expression);
            mockContext.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());


            //projectBL = new ProjectBusinessLogic(new ProjectRepo(mockContext.Object));

            
        }

        //[TestMethod]
        //public void TestMethod()
        //{
        //    projectBL.GetProjectsWithAssociations()

        //}
    }
}
