﻿using Newtonsoft.Json;

namespace NotesWebApplication.Models.Responses
{
    public class ErrorResponse
    {
        public int Status { get; set; }
        public int ErrorCode { get; set; }
        public string DeveloperMessage { get; set; }
        public string UserMessage { get; set; }
        public string MoreInfo { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}