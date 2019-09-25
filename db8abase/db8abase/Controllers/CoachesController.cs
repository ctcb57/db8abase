using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using db8abase.Data;
using db8abase.Models;
using System.Security.Claims;

namespace db8abase.Controllers
{
    public class CoachesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoachesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Coaches
        public async Task<IActionResult> Index()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentCoach = _context.Coach.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            return View(currentCoach);
        }

        // GET: Coaches/ManageTeam
        public IActionResult ManageTeam()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentCoach = _context.Coach.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            var currentTeam = _context.Debater.Where(d => d.SchoolId == currentCoach.SchoolId).ToList();
            return View(currentTeam);
        }

        // GET: Coaches/SelectSchool
        public IActionResult SelectSchool([Bind("SchoolId")] Coach coach, int id)
        {
            var selectedSchool = _context.School.FirstOrDefault(s => s.SchoolId == id);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentCoach = _context.Coach.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            coach = currentCoach;
            coach.SchoolId = selectedSchool.SchoolId;
            _context.Attach(coach);
            _context.SaveChanges();
            return RedirectToAction("Index", "Coaches");
        }

        // GET: Coaches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coach = await _context.Coach
                .FirstOrDefaultAsync(m => m.CoachId == id);
            if (coach == null)
            {
                return NotFound();
            }

            return View(coach);
        }

        // GET: Coaches/Create
        public IActionResult Create()
        {
            Coach coach = new Coach();
            return View(coach);
        }

        // POST: Coaches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CoachId,FirstName,LastName,Email,PhoneNumber,Balance,ApplicationUserId,SchoolId")] Coach coach, string Id)
        {
            if (ModelState.IsValid)
            {
                coach.ApplicationUserId = Id;
                _context.Add(coach);
                await _context.SaveChangesAsync();
                return RedirectToAction("RegistrationConfirmation", "Home");
            }
            return View(coach);
        }

        // GET: Coaches/Edit/5
        public async Task<IActionResult> Edit()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentCoach = _context.Coach.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            return View(currentCoach);
        }

        // POST: Coaches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("CoachId,FirstName,LastName,Email,PhoneNumber,Balance,ApplicationUserId,SchoolId")] Coach coach)
        {
            _context.Update(coach);
            _context.SaveChanges();
            return RedirectToAction("GetListOfSchools", "Schools");
        }

        // GET: Coaches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coach = await _context.Coach
                .FirstOrDefaultAsync(m => m.CoachId == id);
            if (coach == null)
            {
                return NotFound();
            }

            return View(coach);
        }

        // POST: Coaches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coach = await _context.Coach.FindAsync(id);
            _context.Coach.Remove(coach);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoachExists(int id)
        {
            return _context.Coach.Any(e => e.CoachId == id);
        }
    }
}
