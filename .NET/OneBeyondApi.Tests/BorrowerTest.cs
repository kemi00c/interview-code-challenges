using Microsoft.Extensions.Logging;
using Moq;
using OneBeyondApi.Controllers;
using OneBeyondApi.DataAccess;

namespace OneBeyondApi.Tests
{
    [TestClass]
    public class BorrowerTest
    {
        [TestMethod]
        public void TestBorrowersOnLoan()
        {
            // Arrange
            SeedData.SetInitialData();
            var mockLogger = new Mock<ILogger<BorrowerController>>();

            var borrowerController = new BorrowerController(mockLogger.Object, new BorrowerRepository(), new CatalogueRepository());

            // Act
            var borrowersOnLoan = borrowerController.OnLoan();

            // Assert
            Assert.AreEqual(2, borrowersOnLoan.Count);
            Assert.AreEqual(1, borrowersOnLoan.Where(b => b.Borrower?.Name == "Liana James").Count());
            Assert.AreEqual(1, borrowersOnLoan.Where(b => b.Borrower?.Name == "Dave Smith").Count());
            Assert.AreEqual("Agile Project Management - A Primer", borrowersOnLoan?.Where(b => b.Borrower?.Name == "Liana James")?.FirstOrDefault()?.BookTitles.FirstOrDefault());
            Assert.AreEqual("The Importance of Clay", borrowersOnLoan?.Where(b => b.Borrower?.Name == "Dave Smith")?.FirstOrDefault()?.BookTitles.FirstOrDefault());


        }
    }
}