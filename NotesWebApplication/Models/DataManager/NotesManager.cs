using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NotesWebApplication.Models.Repository;

namespace NotesWebApplication.Models.DataManager
{
    public class NotesManager : INotesRepository<Note>
    {
        private readonly NotesContext _db;

        public NotesManager(NotesContext db)
        {
            _db = db;
        }


        public async Task<Note> GetNoteByIdAsync(string id)
        {
            var note = await _db.Notes.FirstOrDefaultAsync(p => p.StringId == id);
            if (note == null) throw new NullReferenceException("Wrong id");
            return note;
        }

        public async Task DeleteNoteAsync(string id, string deleteToken)
        {
            var note = await _db.Notes.FirstOrDefaultAsync(p => p.DeleteToken == deleteToken && p.StringId == id);
            if (note == null) throw new NullReferenceException("Wrong id");
            _db.Notes.Remove(note);
        }

        public async Task AddNoteAsync(Note note)
        {
            await _db.Notes.AddAsync(note);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}