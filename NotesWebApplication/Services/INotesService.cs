using System.Threading.Tasks;
using NotesWebApplication.Models;
using NotesWebApplication.Models.Requests;

namespace NotesWebApplication.Services
{
    public interface INotesService
    {
        Task<Note> GetNoteByIdAsync(string id);
        Task DeleteNoteAsync(string id, string deleteToken);
        Task<Note> AddNoteAsync(AddNoteRequest addNoteRequest);
    }
}