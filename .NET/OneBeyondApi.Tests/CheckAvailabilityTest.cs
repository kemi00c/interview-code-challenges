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
    public class CheckAvailabilityTest
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
        public void NoReservationBookAvailableNow()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();

            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository());

            // Act
            var result = catalogueController.CheckAvailability("Dave Smith", "Rust Development Cookbook");

            // Assert
            Assert.AreEqual(DateTime.Now.Year, result.Year);
            Assert.AreEqual(DateTime.Now.Month, result.Month);
            Assert.AreEqual(DateTime.Now.Day, result.Day);
        }

        [TestMethod]
        public void FreeStockOfBookAvailableNow()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();

            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository());

            // Act
            var result = catalogueController.CheckAvailability("Liana James", "The Importance of Clay");

            // Assert
            Assert.AreEqual(DateTime.Now.Year, result.Year);
            Assert.AreEqual(DateTime.Now.Month, result.Month);
            Assert.AreEqual(DateTime.Now.Day, result.Day);
        }

        [TestMethod]
        public void BookIsOnLoanToOtherBorrowerLastEndDateIsReturned()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();

            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository());

            // Act
            var result = catalogueController.CheckAvailability("Dave Smith", "Agile Project Management - A Primer");

            // Assert
            var availableDate = DateTime.Now.AddDays(7);
            Assert.AreEqual(availableDate.Year, result.Year);
            Assert.AreEqual(availableDate.Month, result.Month);
            Assert.AreEqual(availableDate.Day, result.Day);
        }

        [TestMethod]
        public void BookIsOnLoanToBorrowerAvailableNow()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();

            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository());

            // Act
            var result = catalogueController.CheckAvailability("Liana James", "Agile Project Management - A Primer");

            // Assert
            Assert.AreEqual(DateTime.Now.Year, result.Year);
            Assert.AreEqual(DateTime.Now.Month, result.Month);
            Assert.AreEqual(DateTime.Now.Day, result.Day);
        }

    }
}
