using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebChat.Models;

namespace WebChat.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        
        [HttpGet("index")]
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        [Authorize]
        public IActionResult RoomLogin()
        {
            return View();
        }
        
        [HttpGet("chat")]
        [Authorize]
        public IActionResult Chat()
        {
            return View();
        }

        [HttpGet("/Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}