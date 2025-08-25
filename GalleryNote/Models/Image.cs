using System.ComponentModel.DataAnnotations;

namespace GalleryNote.Models
{
    public class Image
    {
        public int ImageId { get; set; }

        [StringLength(200, ErrorMessage = "The bio must be between 2 and 200 characters.")]
        [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \-]{2,200}", ErrorMessage = "The bio must consist of letters, numbers, spaces, or allowed special characters.")]
        public string Bio { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "The URL must be a maximum of 500 characters.")]
        public string? ImageUrl { get; set; }

        public string UploadedById { get; set; } = string.Empty;

        [Display(Name = "Uploaded By")]
        public string? UploadedBy { get; set; }
    }
}