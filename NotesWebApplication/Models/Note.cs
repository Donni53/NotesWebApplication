namespace NotesWebApplication.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string StringId { get; set; }
        public string Data { get; set; }
        public bool Destroying { get; set; }
        public bool SyntaxHighlighting { get; set; }
        public string DeleteToken { get; set; }
    }
}