using GalleryNote.Models; 

namespace GalleryNote.ViewModels
{
    public class ImagesViewModel
    {
        public IEnumerable<Image> Images { get; set; } = new List<Image>(); //Images to be displayed
        public string? CurrentViewName { get; set; } //Name of the current view

        //Parameterless constructor for flexibility
        public ImagesViewModel()
        {
        }

        //Constructor for explicit initialization
        public ImagesViewModel(IEnumerable<Image> images, string? currentViewName)
        {
            Images = images;
            CurrentViewName = currentViewName;
        }
    }
}