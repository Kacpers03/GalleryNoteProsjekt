using Microsoft.EntityFrameworkCore;
using GalleryNote.Models;

namespace GalleryNote.DAL;

public static class DBInit
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        ImageDbContext context = serviceScope.ServiceProvider.GetRequiredService<ImageDbContext>();
        //context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        //Seed Images
        if (!context.Images.Any())
        {
            var images = new List<Image>
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
                },
                new Image
                {
                    Bio = "Skog",
                    ImageUrl = "/images/skog.jpg",
                    UploadedById = "default-user-id",
                    UploadedBy = "Default User"
                }
                
            };

            context.AddRange(images);
            context.SaveChanges();
        }



        //Seed Notes (examples from chatGPT)
        if (!context.Notes.Any())
        {
            var notes = new List<Note>
            {
                new Note
                {
                    Content = "Meeting with the team at 10 AM.",
                    UploadedById = "default-user-id",
                    UploadedBy = "Default User"
                },
                new Note
                {
                    Content = "Complete the project documentation.",
                    UploadedById = "default-user-id",
                    UploadedBy = "Default User"
                },
                new Note
                {
                    Content = "Prepare presentation slides for client meeting.",
                    UploadedById = "default-user-id",
                    UploadedBy = "Default User"
                },
                new Note
                {
                    Content = "Buy groceries: Milk, Bread, and Eggs.",
                    UploadedById = "default-user-id",
                    UploadedBy = "Default User"
                },
                new Note
                {
                    Content = "Schedule annual health check-up.",
                    UploadedById = "default-user-id",
                    UploadedBy = "Default User"
                },
                new Note
                {
                    Content = "Finish reading 'Clean Code' by Robert C. Martin.",
                    UploadedById = "default-user-id",
                    UploadedBy = "Default User"
                },
            };

            context.AddRange(notes);
            context.SaveChanges();
        }
    }
}
