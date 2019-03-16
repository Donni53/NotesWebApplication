using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesWebApplication.Helpers;
using NotesWebApplication.Models;
using NotesWebApplication.Services;
using NotesWebApplication.ViewModels;

namespace NotesWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly NotesService _notesSevice;
        public NotesController(NotesService notesService)
        {
            _notesSevice = notesService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return StatusCode(404);
        }

        [HttpGet]
        [Route("Read")]
        public async Task<JsonResult> ReadNote(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new Exception("Id is empty or null");
                id = id.Replace(" ","+"); //js bug?
                var note = await _notesSevice.GetNoteByIdAsync(id);
                return new JsonResult(new ReadResponse(0, note.Data, note.SyntaxHighlighting, "", note.Destroying));
            }
            catch (Exception e)
            {
                return new JsonResult(new ReadResponse(e.HResult, "", false, e.Message, false));
            }
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<JsonResult> DeleteNote(string id, string deleteToken)
        {
            try
            {
                if (string.IsNullOrEmpty(deleteToken))
                    throw new Exception("Token is empty or null");
                await _notesSevice.DeleteNoteAsync(id, deleteToken);
                return new JsonResult(new DeleteResponse(0, ""));
            }
            catch (Exception e)
            {
                return new JsonResult(new DeleteResponse(e.HResult, e.Message));
            }
        }

        [HttpPost]
        [Route("Add")]
        public async Task<JsonResult> Add([FromForm] NoteViewModel noteViewModel)
        {
            try
            {
                if (string.IsNullOrEmpty(noteViewModel.Data))
                    throw new Exception("Data is empty or null");
                var note = await _notesSevice.AddNoteAsync(noteViewModel);
                return new JsonResult(new AddResponse(0, note.StringId, note.DeleteToken, ""));
            }
            catch (Exception e)
            {
                return new JsonResult(new AddResponse(e.HResult, "", "", e.Message));
            }
        }

    }
}
