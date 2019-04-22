using System.ComponentModel.DataAnnotations;

namespace NotesWebApplication.Models.Requests
{
    public class AddNoteRequest
    {
        [Required] public string Data { get; set; }
        [Required] public bool Destroying { get; set; }
        [Required] public bool SyntaxHighlighting { get; set; }
    }
}