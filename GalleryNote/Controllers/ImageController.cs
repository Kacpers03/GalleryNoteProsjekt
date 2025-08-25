using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GalleryNote.DAL;
using GalleryNote.Models;
using GalleryNote.ViewModels;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GalleryNote.Controllers
{
    public class ImageController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ImageDbContext _context;
        private readonly ILogger<ImageController> _logger;

        public ImageController(UserManager<IdentityUser> userManager, ImageDbContext context, ILogger<ImageController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        //GET: Grid view (open to everyone). No longer a grid view, but we kept the name to avoid having issues later
        public IActionResult Grid()
        {
            var images = _context.Images.ToList();
            var viewModel = new ImagesViewModel(images, "Grid");
            return View(viewModel);
        }

        //GET: Create image (must be logged in)
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        //POST: Create image (must be logged in)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Image image, IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                //Validate file type to check if it is a image-file
                var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                if (!validExtensions.Contains(Path.GetExtension(ImageFile.FileName).ToLower()))
                {
                    ModelState.AddModelError("ImageFile", "Only image files are allowed.");
                    return View(image);
                }

                //Saves the file in the images-folder
                var filePath = Path.Combine("wwwroot/images", ImageFile.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                //Set the file path in ImageUrl
                image.ImageUrl = $"/images/{ImageFile.FileName}";
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Please select a valid image file.");
                return View(image);
            }

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError(string.Empty, "User ID could not be found.");
                    return View(image);
                }

                image.UploadedById = userId;

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User could not be found.");
                    return View(image);
                }

                image.UploadedBy = user.UserName;

                _context.Images.Add(image);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Grid));
            }

            return View(image);
        }

        //GET: Update image (only for owner of image and logged-in users)
        [Authorize]
        public IActionResult Update(int id)
        {
            var image = _context.Images.FirstOrDefault(i => i.ImageId == id);
            if (image == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (image.UploadedById != userId)
            {
                return Forbid();
            }

            return View(image);
        }

        //POST: Update image (only for owner of image and logged-in users)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Image image)
        {
            var userId = _userManager.GetUserId(User);
            var existingImage = await _context.Images.FirstOrDefaultAsync(i => i.ImageId == image.ImageId);

            if (existingImage == null || existingImage.UploadedById != userId)
            {
                return Forbid(); //User does not have permission to update the image uploaded earlier
            }

            //Update only the Bio
            existingImage.Bio = image.Bio;

            //Check if the model is valid
            if (!ModelState.IsValid)
            {
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError("ModelState error: {ErrorMessage}", modelError.ErrorMessage);
                }
                return View(existingImage);
            }

            //Save changes
            _context.Attach(existingImage); //Using Attach to avoid double tracking
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Grid));
        }

        //GET: Delete confirmation (only for owner and logged-in users)
        [Authorize]
        public IActionResult Delete(int id)
        {
            var image = _context.Images.FirstOrDefault(i => i.ImageId == id);
            if (image == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (image.UploadedById != userId)
            {
                return Forbid();
            }

            return View(image);
        }

        //POST: Delete image (only for owner and logged-in users)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var image = _context.Images.FirstOrDefault(i => i.ImageId == id);
            if (image == null || image.UploadedById != userId)
            {
                return Forbid();
            }

            //Delete the file from the server
            if (!string.IsNullOrEmpty(image.ImageUrl))
            {
                var filePath = Path.Combine("wwwroot", image.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Images.Remove(image);
            _context.SaveChanges();
            return RedirectToAction(nameof(Grid));
        }

        //GET: Details (open to everyone). To access details you click on the image
        public IActionResult Details(int id)
        {
            var image = _context.Images.FirstOrDefault(i => i.ImageId == id);
            if (image == null) return NotFound();
            return View(image);
        }
    }
}