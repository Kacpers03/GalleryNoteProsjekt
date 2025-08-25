using GalleryNote.DAL;
using GalleryNote.Models;
using GalleryNote.Controllers;
using GalleryNote.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;



namespace GalleryNote.Test.Controllers
{
    public class ImageControllerTests
    {
        [Fact]
        public void TestGrid()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ImageDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_Grid")
                .Options;

            using var context = new ImageDbContext(options);
            
            //Seed data
            var imageList = new List<Image>
            {
                new Image
                {
                    Bio = "Lilla blomst",
                    ImageUrl = "/images/blomst.jpg",
                    UploadedById = "default-user-id",
                    UploadedBy = "Default User"
                },
                new Image
                {
                    Bio = "Grønt blad",
                    ImageUrl = "/images/blad.jpg",
                    UploadedById = "default-user-id",
                    UploadedBy = "Default User"
                }
            };
            context.Images.AddRange(imageList);
            context.SaveChanges();

            var mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            var mockLogger = new Mock<ILogger<ImageController>>();

            var imageController = new ImageController(mockUserManager.Object, context, mockLogger.Object);

            //Act
            var result = imageController.Grid();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var imagesViewModel = Assert.IsAssignableFrom<ImagesViewModel>(viewResult.ViewData.Model);
            Assert.Equal(2, imagesViewModel.Images.Count());
        }
        [Fact]
        public async Task TestCreateInvalidImage()
        {
            //arrange
            var options = new DbContextOptionsBuilder<ImageDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new ImageDbContext(options);
            var mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            var mockLogger = new Mock<ILogger<ImageController>>();

            var imageController = new ImageController(mockUserManager.Object, context, mockLogger.Object);

            var image = new Image
            {
                Bio = "Missing ImageFile"
            };

            //act
            var result = await imageController.Create(image, null);

            //assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(imageController.ModelState.IsValid);
        }

        [Fact]
        public async Task TestUpdateForbidden()
        {
            //arrange
            var options = new DbContextOptionsBuilder<ImageDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_UpdateForbidden")
                .Options;

            using var context = new ImageDbContext(options);

            //Adds an existing image in database with a different user
            var existingImage = new Image
            {
                ImageId = 1,
                Bio = "Lilla blomst",
                UploadedById = "another-user-id"
            };
            context.Images.Add(existingImage);
            await context.SaveChangesAsync();

            var mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            var mockLogger = new Mock<ILogger<ImageController>>();

            //Mock UserManager to return a specific user ID
            mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("user-123");

            var imageController = new ImageController(mockUserManager.Object, context, mockLogger.Object);

            //act
            var result = await imageController.Update(existingImage);

            //assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public void TestDeleteNotFound()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ImageDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_DeleteNotFound")
                .Options;

            using var context = new ImageDbContext(options);
            var mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            var mockLogger = new Mock<ILogger<ImageController>>();

            var imageController = new ImageController(mockUserManager.Object, context, mockLogger.Object);

            //Act
            var result = imageController.Delete(999); //Id that do not exist

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}