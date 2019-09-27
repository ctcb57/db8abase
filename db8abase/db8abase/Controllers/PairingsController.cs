using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using db8abase.Data;
using db8abase.Models;

namespace db8abase.Controllers
{
    public class PairingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PairingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pairings
        public async Task<IActionResult> Index()
        {
            return View(await _context.Pairing.ToListAsync());
        }

        // GET: Tabulate
        public IActionResult Tabulate(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            return View(tournament);
        }

        public IActionResult PairRoundOne(int id)
        {
            return View()
        }

        public List<Room> GetRooms(int id)
        {
            List<Room> neededRooms = new List<Room>();
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            int entryCount = _context.TeamEntry.Where(t => t.TournamentId == tournament.TournamentId).ToList().Count();
            List<Room> tournamentRooms = _context.Room.Where(r => r.SchoolId == tournament.SchoolId).ToList();
            for(int i = 0; i < entryCount; i++)
            {
                neededRooms.Add(tournamentRooms[i]);
            }
            return neededRooms;
        }

        public List<TeamEntry> GetEntries(int id)
        {

        }



        // GET: Pairings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pairing = await _context.Pairing
                .FirstOrDefaultAsync(m => m.PairingId == id);
            if (pairing == null)
            {
                return NotFound();
            }

            return View(pairing);
        }

        // GET: Pairings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pairings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PairingId,TournamentId,JudgeId,RoomId,AffirmativeTeamId,NegativeTeamId,RoundId,DebateId")] Pairing pairing)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pairing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pairing);
        }

        // GET: Pairings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pairing = await _context.Pairing.FindAsync(id);
            if (pairing == null)
            {
                return NotFound();
            }
            return View(pairing);
        }

        // POST: Pairings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PairingId,TournamentId,JudgeId,RoomId,AffirmativeTeamId,NegativeTeamId,RoundId,DebateId")] Pairing pairing)
        {
            if (id != pairing.PairingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pairing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PairingExists(pairing.PairingId))
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
            return View(pairing);
        }

        // GET: Pairings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pairing = await _context.Pairing
                .FirstOrDefaultAsync(m => m.PairingId == id);
            if (pairing == null)
            {
                return NotFound();
            }

            return View(pairing);
        }

        // POST: Pairings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pairing = await _context.Pairing.FindAsync(id);
            _context.Pairing.Remove(pairing);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PairingExists(int id)
        {
            return _context.Pairing.Any(e => e.PairingId == id);
        }
    }
}
