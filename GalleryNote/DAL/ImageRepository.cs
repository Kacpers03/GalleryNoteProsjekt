using Microsoft.EntityFrameworkCore;
using GalleryNote.Models;

namespace GalleryNote.DAL;

public class ImageRepository : IImageRepository
{
    private readonly ImageDbContext _db;

    private readonly ILogger<ImageRepository> _logger;

    public ImageRepository(ImageDbContext db, ILogger<ImageRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    //Retrieve all images from the database
    public async Task<IEnumerable<Image>?> GetAll()
    {
        try
        {
            return await _db.Images.ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[ImageRepository] images ToListAsync() failed when calling GetAll(), error message: {e}", e.Message);
            return null;
        }
    }

    //Retrieve a specific image by its ID
    public async Task<Image?> GetImageById(int id)
    {
        try
        {
            return await _db.Images.FindAsync(id);
        }
        catch (Exception e)
        {
            _logger.LogError("[ImageRepository] image FindAsync(id) failed when calling GetImageById for ImageId {ImageId:0000}, error message: {e}", id, e.Message);
            return null;
        }
    }

    //Create a new image entry in the database
    public async Task<bool> Create(Image image)
    {
        try
        {
            _db.Images.Add(image);
            await _db.SaveChangesAsync(); //Save changes to the database
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[ImageRepository] image creation failed for image {@image}, error message: {e}", image, e.Message);
            return false;
        }
    }

    //Update an existing image entry in the database
    public async Task<bool> Update(Image image)
    {
        try
        {
            //Retrieve the existing entity from the database
            var existingImage = await _db.Images.FindAsync(image.ImageId);
            if (existingImage == null)
            {
                _logger.LogError("[ImageRepository] Image with Id {ImageId:0000} not found", image.ImageId);
                return false;  //Return false if the image is not found
            }

            //Log before updating
            _logger.LogInformation("[ImageRepository] Updating ImageId {ImageId:0000}", image.ImageId);

            //Update only the Bio property, do not modify ImageUrl
            existingImage.Bio = image.Bio;

            //Save the changes to the database
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[ImageRepository] image update failed for ImageId {ImageId:0000}, error message: {e}", image.ImageId, e.Message);
            return false;
        }
    }

    //Delete an image entry from the database by its ID
    public async Task<bool> Delete(int id)
    {
        try
        {
            //Retrieve the image by its ID
            var image = await _db.Images.FindAsync(id);
            if (image == null)
            {
                _logger.LogError("[ImageRepository] image not found for the ImageId {ImageId:0000}", id);
                return false;  //Return false if the image is not found
            }

            //Remove the image from the database
            _db.Images.Remove(image);
            await _db.SaveChangesAsync();  //Save the changes to the database
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[ImageRepository] image deletion failed for the ImageId {ImageId:0000}, error message: {e}", id, e.Message);
            return false;
        }
    }
}