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
    public class JudgesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JudgesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Judges
        public async Task<IActionResult> Index()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentJudge = _context.Judge.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            return View(currentJudge);
        }

        // GET: Judges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var judge = await _context.Judge
                .FirstOrDefaultAsync(m => m.JudgeId == id);
            if (judge == null)
            {
                return NotFound();
            }

            return View(judge);
        }

        public IActionResult SelectSchool([Bind("SchoolId")] Judge judge, int id)
        {
            var selectedSchool = _context.School.FirstOrDefault(s => s.SchoolId == id);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentJudge = _context.Judge.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            judge = currentJudge;
            judge.SchoolId = selectedSchool.SchoolId;
            _context.Attach(judge);
            _context.SaveChanges();
            return RedirectToAction("Index", "Judges");
        }

        // GET: EditPhilosophy
        public IActionResult EditPhilosophy()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentJudge = _context.Judge.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            return View(currentJudge);
        }
        // POST: EditPhilosophy
        [HttpPost]
        public IActionResult EditPhilosophy([Bind("JudgeId,FirstName,LastName,Email,PhoneNumber,JudgingPhilosophy,SchoolId,ApplicationUserId")] Judge judge)
        {
            _context.Update(judge);
            _context.SaveChanges();
            return RedirectToAction("Index", "Judges");
        }
            // GET: Judges/Create
            public IActionResult Create()
        {
            Judge judge = new Judge();
            return View(judge);
        }

        public IActionResult ViewBallots()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentJudge = _context.Judge.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            List<Ballot> ballots = _context.Ballot.Where(b => b.JudgeId == currentJudge.JudgeId).ToList();
            return View(ballots);
        }

        // POST: Judges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JudgeId,FirstName,LastName,Email,PhoneNumber,JudgingPhilosophy,SchoolId,ApplicationUserId")] Judge judge, string Id)
        {
            if (ModelState.IsValid)
            {
                judge.ApplicationUserId = Id;
                _context.Add(judge);
                await _context.SaveChangesAsync();
                return RedirectToAction("RegistrationConfirmation", "Home");
            }
            return View(judge);
        }

        // GET: Judges/Edit/5
        public IActionResult Edit()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentJudge = _context.Judge.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            return View(currentJudge);
        }

        // POST: Judges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("JudgeId,FirstName,LastName,Email,PhoneNumber,JudgingPhilosophy,SchoolId,ApplicationUserId")] Judge judge)
        {
            _context.Update(judge);
            _context.SaveChanges();
            return RedirectToAction("GetListOfSchools", "Schools");
        }

        // GET: Judges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var judge = await _context.Judge
                .FirstOrDefaultAsync(m => m.JudgeId == id);
            if (judge == null)
            {
                return NotFound();
            }

            return View(judge);
        }

        // POST: Judges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var judge = await _context.Judge.FindAsync(id);
            _context.Judge.Remove(judge);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JudgeExists(int id)
        {
            return _context.Judge.Any(e => e.JudgeId == id);
        }
    }
}
