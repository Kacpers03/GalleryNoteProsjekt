using GalleryNote.Controllers;
using GalleryNote.DAL;
using GalleryNote.Models;
using GalleryNote.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GalleryNote.Test.Controllers
{
    public class NoteControllerTests
    {
        //Test Grid View
        [Fact]
        public async Task TestGrid()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ImageDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_Grid")
                .Options;

            using var context = new ImageDbContext(options);
            var mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            var mockNoteRepository = new Mock<INoteRepository>();
            mockNoteRepository.Setup(repo => repo.GetAll()).ReturnsAsync(new List<Note>
            {
                new Note
                {
                    Id = 1,
                    Content = "Meeting with the team at 10 AM.",
                    UploadedById = "default-user-id",
                    UploadedBy = "Default User"
                },
                new Note
                {
                    Id = 2,
                    Content = "Complete the project documentation.",
                    UploadedById = "default-user-id",
                    UploadedBy = "Default User"
                }
            });

            var noteController = new NoteController(mockUserManager.Object, mockNoteRepository.Object);

            //Act
            var result = await noteController.Grid();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var notesViewModel = Assert.IsAssignableFrom<NotesViewModel>(viewResult.ViewData.Model);
            Assert.Equal(2, notesViewModel.Notes.Count());
        }

        //Test Create GET
        [Fact]
        public void TestCreateView()
        {
            //Arrange
            var mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            var mockNoteRepository = new Mock<INoteRepository>();
            var noteController = new NoteController(mockUserManager.Object, mockNoteRepository.Object);

            //Act
            var result = noteController.Create();

            //Assert
            Assert.IsType<ViewResult>(result);
        }

        //Test Create POST - Invalid Model
        [Fact]
        public async Task TestCreateInvalidNote()
        {
            //Arrange
            var mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            var mockNoteRepository = new Mock<INoteRepository>();
            var noteController = new NoteController(mockUserManager.Object, mockNoteRepository.Object);
            noteController.ModelState.AddModelError("Content", "Content is required");

            var note = new Note
            {
                Content = ""
            };

            //Act
            var result = await noteController.Create(note);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(noteController.ModelState.IsValid);
        }

        //Test Delete GET - NotFound
        [Fact]
        public async Task TestDeleteNotFound()
        {
            //Arrange
            var mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            var mockNoteRepository = new Mock<INoteRepository>();
            mockNoteRepository.Setup(repo => repo.GetNoteById(It.IsAny<int>())).ReturnsAsync((Note)null);

            var noteController = new NoteController(mockUserManager.Object, mockNoteRepository.Object);

            //Act
            var result = await noteController.Delete(999);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}