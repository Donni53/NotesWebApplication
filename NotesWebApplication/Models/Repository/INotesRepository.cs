using System.Threading.Tasks;

namespace NotesWebApplication.Models.Repository
{
    public interface INotesRepository<TEntity>
    {
        Task<TEntity> GetNoteByIdAsync(string id);
        Task DeleteNoteAsync(string id, string deleteToken);
        Task AddNoteAsync(TEntity note);
        Task SaveChangesAsync();
    }
}