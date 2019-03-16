using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NotesWebApplication.Helpers;
using NotesWebApplication.Models;
using NotesWebApplication.ViewModels;

namespace NotesWebApplication.Services
{
    public class NotesService
    {
        private NotesContext db;
        public NotesService(NotesContext context)
        {
            db = context;
        }


        public async Task<Note> GetNoteByIdAsync(string id)
        {
            var note = await db.Notes.FirstOrDefaultAsync(p => p.StringId == id);
            if (note == null) throw new Exception("Wrong id");
            if (note.Destroying)
            {
                db.Notes.Remove(note);
                await db.SaveChangesAsync();
            }

            return note;
        }

        public async Task DeleteNoteAsync(string id, string deleteToken)
        {
            var note = await db.Notes.FirstOrDefaultAsync(p => p.DeleteToken == deleteToken && p.StringId == id);
            if (note == null) throw new Exception("Wrong token");
            db.Notes.Remove(note);
            await db.SaveChangesAsync();
        }

        public async Task<Note> AddNoteAsync(NoteViewModel noteViewModel)
        {
            var deleteToken = Cryptography.GetHash(noteViewModel.Data, 16);
            var id = Cryptography.GetHash(noteViewModel.Data, 16);
            var note = await db.Notes.AddAsync(new Note(id, noteViewModel.Data, noteViewModel.Destroying, noteViewModel.SyntaxHighlighting, deleteToken));
            await db.SaveChangesAsync();
            return note.Entity;
        }
    }
}
