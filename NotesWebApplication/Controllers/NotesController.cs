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
using NotesWebApplication.ViewModels;

namespace NotesWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private NotesContext db;
        public NotesController(NotesContext context)
        {
            db = context;
        }

        [HttpGet]
        public string Get()
        {
            return "Hi!";
        }

        [HttpGet]
        [Route("Read")]
        public async Task<JsonResult> ReadNote(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new Exception("Id is empty or null");
                var note = await db.Notes.FirstOrDefaultAsync(p => p.StringId == id);
                if (note == null) throw new Exception("Wrong id");
                if (note.Destroying)
                {
                    db.Notes.Remove(note);
                    await db.SaveChangesAsync();
                }
                return new JsonResult(new ReadResponse(0, note.Data, note.SyntaxHighlighting, ""));
            }
            catch (Exception e)
            {
                return new JsonResult(new ReadResponse(e.HResult, "", false, e.Message));
            }
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<JsonResult> DeleteNote(string deleteToken)
        {
            try
            {
                if (string.IsNullOrEmpty(deleteToken))
                    throw new Exception("Token is empty or null");
                var note = await db.Notes.FirstOrDefaultAsync(p => p.DeleteToken == deleteToken);
                if (note == null) throw new Exception("Wrong token");
                db.Notes.Remove(note);
                await db.SaveChangesAsync();
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
                var deleteToken = Cryptography.GetHash(noteViewModel.Data, 32);
                var id = Cryptography.GetHash(noteViewModel.Data, 16);
                await db.Notes.AddAsync(new Note(id, noteViewModel.Data, noteViewModel.Destroying, noteViewModel.SyntaxHighlighting, deleteToken));
                await db.SaveChangesAsync();
                return new JsonResult(new AddResponse(0, WebUtility.UrlEncode(id), WebUtility.UrlEncode(deleteToken), ""));
            }
            catch (Exception e)
            {
                return new JsonResult(new AddResponse(e.HResult, "", "", e.Message));
            }
        }

    }
}
