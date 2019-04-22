namespace NotesWebApplication.Models.Responses
{
    public class AddNoteResponse
    {
        public int Status { get; set; }
        public string Id { get; set; }
        public string DeleteToken { get; set; }
    }
}