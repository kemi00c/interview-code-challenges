using Microsoft.Extensions.Logging;
using Moq;
using OneBeyondApi.Controllers;
using OneBeyondApi.DataAccess;

namespace OneBeyondApi.Tests
{
    [TestClass]
    public class CatalogueOnLoanTest
    {
        [TestInitialize]
        public void Init()
        {
            using (var context = new LibraryContext())
            {
                context.Database.EnsureDeleted();
            }
            SeedData.SetInitialData();
        }

        [TestMethod]
        public void TestBorrowersOnLoan()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();

           
            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository());

            // Act
            var borrowersOnLoan = catalogueController.OnLoan();

            // Assert
            Assert.AreEqual(2, borrowersOnLoan.Count);
            Assert.AreEqual(1, borrowersOnLoan.Where(b => b.Borrower?.Name == "Liana James").Count());
            Assert.AreEqual(1, borrowersOnLoan.Where(b => b.Borrower?.Name == "Dave Smith").Count());
            Assert.AreEqual("Agile Project Management - A Primer", borrowersOnLoan?.Where(b => b.Borrower?.Name == "Liana James")?.FirstOrDefault()?.BookTitles.FirstOrDefault());
            Assert.AreEqual("The Importance of Clay", borrowersOnLoan?.Where(b => b.Borrower?.Name == "Dave Smith")?.FirstOrDefault()?.BookTitles.FirstOrDefault());


        }
    }
}