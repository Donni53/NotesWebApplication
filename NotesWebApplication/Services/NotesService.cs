using System.Threading.Tasks;
using NotesWebApplication.Helpers;
using NotesWebApplication.Models;
using NotesWebApplication.Models.Repository;
using NotesWebApplication.Models.Requests;

namespace NotesWebApplication.Services
{
    public class NotesService : INotesService
    {
        private readonly INotesRepository<Note> _db;

        public NotesService(INotesRepository<Note> db)
        {
            _db = db;
        }


        public async Task<Note> GetNoteByIdAsync(string id)
        {
            var note = await _db.GetNoteByIdAsync(id);
            if (!note.Destroying) return note;
            await _db.DeleteNoteAsync(note.StringId, note.DeleteToken);
            await _db.SaveChangesAsync();

            return note;
        }

        public async Task DeleteNoteAsync(string id, string deleteToken)
        {
            await _db.DeleteNoteAsync(id, deleteToken);
            await _db.SaveChangesAsync();
        }

        public async Task<Note> AddNoteAsync(AddNoteRequest addNoteRequest)
        {
            var deleteToken = Cryptography.GetHash(addNoteRequest.Data, 16);
            var id = Cryptography.GetHash(addNoteRequest.Data, 16);
            var note = new Note
            {
                StringId = id,
                Data = addNoteRequest.Data,
                DeleteToken = deleteToken,
                Destroying = addNoteRequest.Destroying,
                SyntaxHighlighting = addNoteRequest.SyntaxHighlighting
            };
            await _db.AddNoteAsync(note);
            await _db.SaveChangesAsync();
            return note;
        }
    }
}