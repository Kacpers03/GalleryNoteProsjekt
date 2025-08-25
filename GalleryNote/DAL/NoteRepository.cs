using Microsoft.EntityFrameworkCore;
using GalleryNote.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GalleryNote.DAL
{
    public class NoteRepository : INoteRepository
    {
        private readonly ImageDbContext _db;
        private readonly ILogger<NoteRepository> _logger;

        public NoteRepository(ImageDbContext db, ILogger<NoteRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        //Retrieve all notes from the database
        public async Task<IEnumerable<Note>?> GetAll()
        {
            try
            {
                return await _db.Notes.ToListAsync(); //Get all notes as a list
            }
            catch (Exception e)
            {
                _logger.LogError("[NoteRepository] notes ToListAsync() failed in GetAll(), error message: {e}", e.Message);
                return null;  //Return null if an exception occurs
            }
        }

        //Retrieve a specific note by its ID
        public async Task<Note?> GetNoteById(int id)
        {
            try
            {
                return await _db.Notes.FindAsync(id); //Find note by ID
            }
            catch (Exception e)
            {
                _logger.LogError("[NoteRepository] note FindAsync(id) failed in GetNoteById for NoteId {NoteId:0000}, error message: {e}", id, e.Message);
                return null; //Return null if an exception occurs
            }
        }

        //Create a new note entry in the database
        public async Task<bool> Create(Note note)
        {
            try
            {
                _db.Notes.Add(note); //Add new note to the database context
                await _db.SaveChangesAsync(); //Save changes to the database
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[NoteRepository] note creation failed for note {@note}, error message: {e}", note, e.Message);
                return false; //Return false if an exception occurs
            }
        }

        //Update an existing note in the database
        public async Task<bool> Update(Note note)
        {
            try
            {
                //Retrieve the existing note from the database
                var existingNote = await _db.Notes.FindAsync(note.Id);
                if (existingNote == null)
                {
                    _logger.LogError("[NoteRepository] Note with Id {NoteId:0000} not found", note.Id);
                    return false; //Return false if the note is not found
                }

                //Update fields (only Content is updated here)
                existingNote.Content = note.Content;

                //Save the changes to the database
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[NoteRepository] note update failed for NoteId {NoteId:0000}, error message: {e}", note.Id, e.Message);
                return false; //Return false if an exception occurs
            }
        }

        //Delete a note from the database by its ID
        public async Task<bool> Delete(int id)
        {
            try
            {
                //Retrieve the note by its ID
                var note = await _db.Notes.FindAsync(id);
                if (note == null)
                {
                    _logger.LogError("[NoteRepository] note not found for NoteId {NoteId:0000}", id);
                    return false;
                }

                //Remove the note from the database context
                _db.Notes.Remove(note);
                await _db.SaveChangesAsync(); //Save the changes to the database
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[NoteRepository] note deletion failed for NoteId {NoteId:0000}, error message: {e}", id, e.Message);
                return false;
            }
        }
    }
}