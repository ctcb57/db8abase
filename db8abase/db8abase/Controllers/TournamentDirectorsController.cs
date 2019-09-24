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
    public class TournamentDirectorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TournamentDirectorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TournamentDirectors
        public async Task<IActionResult> Index()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var tournamentDirector = _context.TournamentDirector.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            return View(tournamentDirector);
        }

        // GET: TournamentDirectors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentDirector = await _context.TournamentDirector
                .FirstOrDefaultAsync(m => m.TournamentDirectorId == id);
            if (tournamentDirector == null)
            {
                return NotFound();
            }

            return View(tournamentDirector);
        }

        // GET: TournamentDirectors/Create
        public IActionResult Create()
        {
            TournamentDirector TournamentDirector = new TournamentDirector();
            return View(TournamentDirector);
        }

        // GET: TournamentDirectors/SelectSchool
        public IActionResult SelectSchool(int Id)
        {
            School school = _context.School.FirstOrDefault(s => s.SchoolId == Id);
            return View(school);
        }
        // POST: TournamentDirectors/SelectSchool
        [HttpPost]
        public IActionResult SelectSchool([Bind("SchoolId")] TournamentDirector tournamentDirector, int id)
        {
            var selectedSchool = _context.School.FirstOrDefault(s => s.SchoolId == id);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentDirector = _context.TournamentDirector.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            var tournamentToAdd = _context.Tournament.FirstOrDefault(t => t.TournamentId == currentDirector.TournamentId);
            tournamentDirector = currentDirector;
            tournamentDirector.SchoolId = selectedSchool.SchoolId;
            tournamentToAdd.School = selectedSchool;
            _context.Attach(tournamentDirector);
            _context.Attach(tournamentToAdd);
            _context.SaveChanges();
            return RedirectToAction("GetTournamentListing", "Home");
        }

        // POST: TournamentDirectors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TournamentDirectorId,FirstName,LastName,Email,Message,ApplicationUserId")] TournamentDirector tournamentDirector, string Id)
        {
            if (ModelState.IsValid)
            {
                tournamentDirector.ApplicationUserId = Id;
                _context.Add(tournamentDirector);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tournamentDirector);
        }

        // GET: CreateTournament
        public IActionResult CreateTournament()
        {
            Tournament tournament = new Tournament();
            return View(tournament);
        }

        [HttpPost]
        public IActionResult CreateTournament([Bind("TournamentId,Name,School,NumberOfRounds,NumberOfEliminationRounds,EntryFee,TournamentDate,TeamLimit")] Tournament tournament)
        {
            _context.Add(tournament);
            _context.SaveChanges();
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentDirector = _context.TournamentDirector.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            currentDirector.TournamentId = tournament.TournamentId;
            _context.Attach(currentDirector);
            _context.SaveChanges();
            return RedirectToAction("GetListOfSchools", "Schools");
        }

        // GET: TournamentDirectors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentDirector = await _context.TournamentDirector.FindAsync(id);
            if (tournamentDirector == null)
            {
                return NotFound();
            }
            return View(tournamentDirector);
        }

        // POST: TournamentDirectors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TournamentDirectorId,FirstName,LastName,Email,Message")] TournamentDirector tournamentDirector)
        {
            if (id != tournamentDirector.TournamentDirectorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tournamentDirector);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TournamentDirectorExists(tournamentDirector.TournamentDirectorId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tournamentDirector);
        }

        // GET: TournamentDirectors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentDirector = await _context.TournamentDirector
                .FirstOrDefaultAsync(m => m.TournamentDirectorId == id);
            if (tournamentDirector == null)
            {
                return NotFound();
            }

            return View(tournamentDirector);
        }

        // POST: TournamentDirectors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tournamentDirector = await _context.TournamentDirector.FindAsync(id);
            _context.TournamentDirector.Remove(tournamentDirector);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TournamentDirectorExists(int id)
        {
            return _context.TournamentDirector.Any(e => e.TournamentDirectorId == id);
        }
    }
}
