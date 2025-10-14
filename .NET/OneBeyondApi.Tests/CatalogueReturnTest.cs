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
    public class CatalogueReturnTest
    {
        [TestInitialize]
        public void Init()
        {
            SeedData.SetInitialData();
        }

        [TestMethod]
        public void TestReturnNotExists()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();

            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository());

            // Act
            var result = catalogueController.OnLoanReturn("asdf", "asdf");

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual("asdf is not on loan to asdf", ((NotFoundObjectResult)result).Value as string);
        }

        [TestMethod]
        public void TestBookOnLoanReturned()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CatalogueController>>();

            var catalogueController = new CatalogueController(mockLogger.Object, new CatalogueRepository());

            // Act
            var result = catalogueController.OnLoanReturn("Dave Smith", "The Importance of Clay");

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
    }
}
