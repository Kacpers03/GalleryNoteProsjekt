using System.ComponentModel.DataAnnotations;

namespace GalleryNote.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "The note content cannot exceed 500 characters.")]
        public string Content { get; set; } = string.Empty;

        public string UploadedById { get; set; } = string.Empty;

        [Display(Name = "Uploaded By")]
        public string? UploadedBy { get; set; }
    }
}