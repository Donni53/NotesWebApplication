using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NotesWebApplication.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [HttpGet]
        [Route("/")]
        public ActionResult Index()
        {
            return View("~/Views/Index.cshtml");
        }
   
    }
}