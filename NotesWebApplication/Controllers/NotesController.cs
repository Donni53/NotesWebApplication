using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotesWebApplication.Models.Requests;
using NotesWebApplication.Models.Responses;
using NotesWebApplication.Services;

namespace NotesWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INotesService _notesSevice;

        public NotesController(INotesService notesService)
        {
            _notesSevice = notesService;
        }

        [HttpGet]
        [Route("Read")]
        public async Task<IActionResult> ReadNote(string id)
        {
            id = id.Replace(" ", "+"); //js bug?
            var note = await _notesSevice.GetNoteByIdAsync(id);
            return StatusCode(200,
                new ReadResponse
                {
                    Status = 0, Data = note.Data, SyntaxHighlighting = note.SyntaxHighlighting,
                    BurnAfterReading = note.Destroying
                });
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> DeleteNote(string id, string deleteToken)
        {
            await _notesSevice.DeleteNoteAsync(id, deleteToken);
            return StatusCode(204);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddAsync([FromForm] AddNoteRequest addNoteRequest)
        {
            var note = await _notesSevice.AddNoteAsync(addNoteRequest);
            return StatusCode(200,
                new AddNoteResponse {Status = 0, Id = note.StringId, DeleteToken = note.DeleteToken});
        }
    }
}