using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public Note(string stringId, string data, bool destroying, bool syntaxHighlighting, string deleteToken)
        {
            StringId = stringId;
            Data = data;
            Destroying = destroying;
            SyntaxHighlighting = syntaxHighlighting;
            DeleteToken = deleteToken;
        }
    }
}
