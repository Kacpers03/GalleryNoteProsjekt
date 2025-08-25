using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GalleryNote.Models;

namespace GalleryNote.DAL;

/*The database is called ImageDatabase because we originally only planned to store
images in the application but we added notes later*/

public class ImageDbContext : IdentityDbContext
{
	public ImageDbContext(DbContextOptions<ImageDbContext> options) : base(options)
	{
        Database.EnsureCreated();
	}

	public DbSet<Image> Images { get; set; } = null!;

    public DbSet<Note> Notes { get; set; } = null!;

    public ImageDbContext() { }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }
}