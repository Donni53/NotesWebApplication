namespace NotesWebApplication.Models.Responses
{
    public class ReadResponse
    {
        public int Status { get; set; }
        public string Data { get; set; }
        public bool SyntaxHighlighting { get; set; }
        public bool BurnAfterReading { get; set; }
    }
}