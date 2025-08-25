using GalleryNote.Models;

namespace GalleryNote.DAL;

public interface IImageRepository
{
	Task<IEnumerable<Image>?> GetAll();
    Task<Image?> GetImageById(int id);
	Task<bool> Create(Image image);
    Task<bool> Update(Image image);
    Task<bool> Delete(int id);
}