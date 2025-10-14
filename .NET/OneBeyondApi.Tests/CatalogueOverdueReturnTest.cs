using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OneBeyondApi.Controllers;
using OneBeyondApi.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBeyondApi.Tests
{
    [TestClass]
    public class CatalogueOverdueReturnTest
    {

        [TestInitialize]
        public void Init()
        {
            using (var context = new LibraryContext())
            {
                context.Database.EnsureDeleted();
            }
            SeedDataWithOverdueBook.SetInitialData();
        }

        [TestMethod]
        public void TestOverdueReturns()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();

            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository());

            // Act
            var result1 = catalogueController.OnLoanReturn("Liana James", "Agile Project Management - A Primer");
            var result2 = catalogueController.OnLoanReturn("Liana James", "The Importance of Clay");

            // Assert
            Assert.IsInstanceOfType(result1, typeof(OkResult));
            Assert.IsInstanceOfType(result2, typeof(OkResult));

            using (var context = new LibraryContext())
            {
                var overdueReturns = context.OverdueReturns.Where(o => o.Borrower != null && o.Borrower.Name == "Liana James").FirstOrDefault();

                Assert.IsNotNull(overdueReturns);
                Assert.AreEqual(2, overdueReturns.NumberOfOverdueReturns);
            }
        }
    }
}
