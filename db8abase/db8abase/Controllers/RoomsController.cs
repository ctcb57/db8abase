using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using db8abase.Data;
using db8abase.Models;
using Microsoft.AspNetCore.Mvc;

namespace db8abase.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            Room room = new Room();
            return View(room);
        }
        [HttpPost]
        public IActionResult Create([Bind("RoomId,RoomNumber,SchoolId")] Room room)
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentDirector = _context.TournamentDirector.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            room.SchoolId = currentDirector.SchoolId;
            _context.Add(room);
            _context.SaveChanges();
            return RedirectToAction("GetRoomsList", "TournamentDirectors");
        }
    }
}