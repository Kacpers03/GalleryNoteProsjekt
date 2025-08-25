using GalleryNote.Models;
using System.Collections.Generic;

namespace GalleryNote.ViewModels
{
    public class NotesViewModel
    {
        public IEnumerable<Note> Notes { get; }
        public string ViewType { get; }

        public NotesViewModel(IEnumerable<Note> notes, string viewType)
        {
            Notes = notes;
            ViewType = viewType;
        }
    }
}