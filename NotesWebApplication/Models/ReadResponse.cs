using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotesWebApplication.Models
{
    public class ReadResponse
    {
        public int Status { get; set; }
        public string Data { get; set; }
        public bool SyntaxHighlighting { get; set; }
        public bool BurnAfterReading { get; set; }
        public string Error { get; set; }

        public ReadResponse(int status, string data, bool syntaxHighlighting, string error, bool burnAfterReading)
        {
            Status = status;
            Data = data;
            SyntaxHighlighting = syntaxHighlighting;
            Error = error;
            BurnAfterReading = burnAfterReading;
        }
    }
}
