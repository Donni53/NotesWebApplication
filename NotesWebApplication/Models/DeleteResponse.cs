using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotesWebApplication.Models
{
    public class DeleteResponse
    {
        public int Status { get; set; }
        public string Error { get; set; }

        public DeleteResponse(int status, string error)
        {
            Status = status;
            Error = error;
        }
    }
}
