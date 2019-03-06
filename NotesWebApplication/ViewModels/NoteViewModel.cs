using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotesWebApplication.ViewModels
{
    public class NoteViewModel
    {
        public string Data { get; set; }
        public bool Destroying { get; set; }
        public bool SyntaxHighlighting { get; set; }
    }
}
