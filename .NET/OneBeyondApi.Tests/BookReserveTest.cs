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
    public class BookReserveTest
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
        public void TestReserveBookDueDateInPastFails()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();


            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository(), new BookRepository(), new BorrowerRepository());

            // Act
            var result = catalogueController.ReserveBook("Liana James", "Rust Development Cookbook", new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1));

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual("Due date is in the past.", ((BadRequestObjectResult)result).Value as string);
        }

        [TestMethod]
        public void TestFreeStockReserveOk()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();


            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository(), new BookRepository(), new BorrowerRepository());

            // Act
            var result = catalogueController.ReserveBook("Liana James", "Rust Development Cookbook", new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1));

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));

        }

        [TestMethod]
        public void TestBookIsNotAvailableAtDateFails()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();


            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository(), new BookRepository(), new BorrowerRepository());
            var bookTitle = "Agile Project Management - A Primer";
            var dueDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1);

            // Act
            var result = catalogueController.ReserveBook("Dave Smith", bookTitle, dueDate);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.IsTrue((((BadRequestObjectResult)result).Value as string).StartsWith($"{bookTitle} will not be available until"));
        }

        [TestMethod]
        public void TestBookIsAvailableAtDateOk()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();


            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository(), new BookRepository(), new BorrowerRepository());
            var bookTitle = "Agile Project Management - A Primer";
            var dueDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 8);

            // Act
            var result = catalogueController.ReserveBook("Dave Smith", bookTitle, dueDate);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void TestBorrowerNotExistsBadRequestReturned()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();


            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository(), new BookRepository(), new BorrowerRepository());

            // Act
            var result = catalogueController.ReserveBook("asdf", "Rust Development Cookbook", new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1));

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
            var result = catalogueController.ReserveBook("Liana James", "asdf", new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1));

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual("No book with title asdf exists.", ((BadRequestObjectResult)result).Value as string);
        }


    }
}
