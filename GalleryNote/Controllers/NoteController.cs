using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GalleryNote.DAL;
using GalleryNote.Models;
using GalleryNote.ViewModels;
using System.Threading.Tasks;

namespace GalleryNote.Controllers
{
    public class NoteController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly INoteRepository _noteRepository;

        public NoteController(UserManager<IdentityUser> userManager, INoteRepository noteRepository)
        {
            _userManager = userManager;
            _noteRepository = noteRepository;
        }

        //GET: Grid view (open to everyone)
        public async Task<IActionResult> Grid()
        {
            var notes = await _noteRepository.GetAll() ?? new List<Note>();
            var viewModel = new NotesViewModel(notes, "Grid");
            return View(viewModel);
        }

        //GET: Details (open to everyone). You get to details by clicking on the note
        public async Task<IActionResult> Details(int id)
        {
            var note = await _noteRepository.GetNoteById(id);
            if (note == null) return NotFound();
            return View(note);
        }

        //GET: Create note (only for logged-in users)
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        //POST: Create note (only for logged-in users)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Note note)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError(string.Empty, "User ID could not be found.");
                    return View(note);
                }
                note.UploadedById = userId;

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User could not be found.");
                    return View(note);
                }
                note.UploadedBy = user.UserName;

                var result = await _noteRepository.Create(note);
                if (result)
                {
                    return RedirectToAction(nameof(Grid));
                }

                ModelState.AddModelError("", "An error occurred while creating the note.");
            }

            return View(note);
        }

        //GET: Update note (only for owner of the note and logged-in users)
        [Authorize]
        public async Task<IActionResult> Update(int id)
        {
            var note = await _noteRepository.GetNoteById(id);
            if (note == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (note.UploadedById != userId)
            {
                return Forbid();
            }

            return View(note);
        }

        //POST: Update note (only for owner and logged-in users)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Note note)
        {
            var userId = _userManager.GetUserId(User);
            var existingNote = await _noteRepository.GetNoteById(note.Id);
            if (existingNote == null || existingNote.UploadedById != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                var result = await _noteRepository.Update(note);
                if (result)
                {
                    return RedirectToAction(nameof(Grid));
                }
                ModelState.AddModelError("", "An error occurred while updating the note.");
            }
            return View(note);
        }

        //GET: Delete confirmation (only for owner and logged-in users)
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var note = await _noteRepository.GetNoteById(id);
            if (note == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (note.UploadedById != userId)
            {
                return Forbid();
            }

            return View(note);
        }

        //POST: Delete note (only for owner and logged-in users)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var note = await _noteRepository.GetNoteById(id);
            if (note == null || note.UploadedById != userId)
            {
                return Forbid();
            }

            var result = await _noteRepository.Delete(id);
            if (!result)
            {
                ModelState.AddModelError("", "An error occurred while deleting the note.");
                return View();
            }
            return RedirectToAction(nameof(Grid));
        }
    }
}