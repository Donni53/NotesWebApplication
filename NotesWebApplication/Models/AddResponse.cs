using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotesWebApplication.Models
{
    public class AddResponse
    {
        public int Status { get; set; }
        public string Id { get; set; }
        public string DeleteToken { get; set; }
        public string Error { get; set; }

        public AddResponse(int status, string id, string deleteToken, string error)
        {
            Status = status;
            Id = id;
            DeleteToken = deleteToken;
            Error = error;
        }
    }
}
