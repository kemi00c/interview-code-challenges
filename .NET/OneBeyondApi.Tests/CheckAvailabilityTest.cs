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

            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository(), new BookRepository(), new BorrowerRepository());

            // Act
            var result = catalogueController.CheckAvailability("Dave Smith", "Rust Development Cookbook");
            var resultValue = (DateOnly)((OkObjectResult)result).Value;

            // Assert
            Assert.AreEqual(DateTime.Now.Year, resultValue.Year);
            Assert.AreEqual(DateTime.Now.Month, resultValue.Month);
            Assert.AreEqual(DateTime.Now.Day, resultValue.Day);
        }

        [TestMethod]
        public void FreeStockOfBookAvailableNow()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();

            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository(), new BookRepository(), new BorrowerRepository());

            // Act
            var result = catalogueController.CheckAvailability("Liana James", "The Importance of Clay");
            var resultValue = (DateOnly)((OkObjectResult)result).Value;

            // Assert
            Assert.AreEqual(DateTime.Now.Year, resultValue.Year);
            Assert.AreEqual(DateTime.Now.Month, resultValue.Month);
            Assert.AreEqual(DateTime.Now.Day, resultValue.Day);
        }

        [TestMethod]
        public void BookIsOnLoanToOtherBorrowerLastEndDateIsReturned()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();

            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository(), new BookRepository(), new BorrowerRepository());

            // Act
            var result = catalogueController.CheckAvailability("Dave Smith", "Agile Project Management - A Primer");
            var resultValue = (DateOnly)((OkObjectResult)result).Value;

            // Assert
            var availableDate = DateTime.Now.AddDays(7);
            Assert.AreEqual(availableDate.Year, resultValue.Year);
            Assert.AreEqual(availableDate.Month, resultValue.Month);
            Assert.AreEqual(availableDate.Day, resultValue.Day);
        }

        [TestMethod]
        public void BookIsOnLoanToBorrowerAvailableNow()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();

            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository(), new BookRepository(), new BorrowerRepository());

            // Act
            var result = catalogueController.CheckAvailability("Liana James", "Agile Project Management - A Primer");
            var resultValue = (DateOnly)((OkObjectResult)result).Value;

            // Assert
            Assert.AreEqual(DateTime.Now.Year, resultValue.Year);
            Assert.AreEqual(DateTime.Now.Month, resultValue.Month);
            Assert.AreEqual(DateTime.Now.Day, resultValue.Day);
        }

        [TestMethod]
        public void TestBorrowerNotExistsBadRequestReturned()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();


            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository(), new BookRepository(), new BorrowerRepository());

            // Act
            var result = catalogueController.CheckAvailability("asdf", "Agile Project Management - A Primer");

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual("No registered borrower with name asdf exists.", ((BadRequestObjectResult)result).Value as string);
        }

        [TestMethod]
        public void TestBookNotExistsBadRequestReturned()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();


            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository(), new BookRepository(), new BorrowerRepository());

            // Act
            var result = catalogueController.CheckAvailability("Liana James", "asdf");

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual("No book with title asdf exists.", ((BadRequestObjectResult)result).Value as string);
        }

    }
}
