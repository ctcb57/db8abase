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
using db8abase.Models.ViewModels;

namespace db8abase.Controllers
{
    public class DebatersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DebatersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Debaters
        public async Task<IActionResult> Index()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            Debater debater = _context.Debater.FirstOrDefault(d => d.ApplicationUserId == currentUserId);
            return View(debater);
        }

        // GET: Debaters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var debater = await _context.Debater
                .FirstOrDefaultAsync(m => m.DebaterId == id);
            if (debater == null)
            {
                return NotFound();
            }

            return View(debater);
        }

        public IActionResult TournamentPairings()
        {
            List<Tournament> tournaments = _context.Tournament.ToList();
            return View(tournaments);
        }

        public IActionResult ViewPairings(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            Debater debater = _context.Debater.FirstOrDefault(d => d.ApplicationUserId == currentUserId);
            IndividualTeam team = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == debater.IndividualTeamId);
            List<Pairing> pairings = _context.Pairing.Where(p => p.TournamentId == id && (p.AffirmativeTeamId == team.IndividualTeamId || p.NegativeTeamId == team.IndividualTeamId)).ToList();
            List<Judge> judges = new List<Judge>();
            List<IndividualTeam> affTeams = new List<IndividualTeam>();
            List<IndividualTeam> negTeams = new List<IndividualTeam>();
            List<Room> rooms = new List<Room>();
            List<Round> rounds = new List<Round>();
            foreach(var pair in pairings)
            {
                Judge judge = _context.Judge.FirstOrDefault(j => j.JudgeId == pair.JudgeId);
                judges.Add(judge);
                IndividualTeam affTeam = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == pair.AffirmativeTeamId);
                affTeams.Add(affTeam);
                IndividualTeam negTeam = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == pair.NegativeTeamId);
                negTeams.Add(negTeam);
                Room room = _context.Room.FirstOrDefault(r => r.RoomId == pair.RoomId);
                rooms.Add(room);
                Round round = _context.Round.FirstOrDefault(r => r.RoundId == pair.RoundId);
                rounds.Add(round);
            }

            PairingsTabulationViewModel data = new PairingsTabulationViewModel()
            {
                Tournament = tournament,
                Rounds = rounds,
                Rooms = rooms,
                AffirmativeTeams = affTeams,
                NegativeTeams = negTeams,
                Judges = judges,
                Debater = debater,
            };
            return View(data);
        }

        // GET: Debaters/Create
        public IActionResult Create()
        {
            Debater debater = new Debater();
            return View(debater);
        }

        // POST: Debaters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DebaterId,FirstName,LastName,Email,PhoneNumber,CoachId,PartnerId,SchoolId,IndividualRoundSpeakerPoints,IndividualTournamentSpeakerPoints,AnnualAverageSpeakerPoints,ApplicationUserId")] Debater debater/*, string Id*/)
        {
            if (ModelState.IsValid)
            {
                //debater.ApplicationUserId = Id;
                var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
                var currentCoach = _context.Coach.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
                debater.CoachId = currentCoach.CoachId;
                debater.SchoolId = currentCoach.SchoolId;
                _context.Add(debater);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageTeam", "Coaches");
            }
            return View(debater);
        }

        // GET: Debaters/Edit/5
        public IActionResult Edit()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentUser = _context.Users.FirstOrDefault(u => u.Id == currentUserId);
            Debater debater = _context.Debater.Where(d => d.Email == currentUser.Email).Single();
            debater.ApplicationUserId = currentUserId;
            _context.Update(debater);
            _context.SaveChanges();
            return RedirectToAction("RegistrationConfirmation", "Home");
        }



        // POST: Debaters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DebaterId,FirstName,LastName,Email,PhoneNumber,CoachId,PartnerId,SchoolId,IndividualRoundSpeakerPoints,IndividualTournamentSpeakerPoints,AnnualAverageSpeakerPoints,ApplicationUserId")] Debater debater)
        {
            if (id != debater.DebaterId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(debater);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DebaterExists(debater.DebaterId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ManageTeam", "Coaches");
            }
            return View(debater);
        }

        // GET: Debaters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var debater = await _context.Debater
                .FirstOrDefaultAsync(m => m.DebaterId == id);
            if (debater == null)
            {
                return NotFound();
            }

            return View(debater);
        }

        // POST: Debaters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var debater = await _context.Debater.FindAsync(id);
            _context.Debater.Remove(debater);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DebaterExists(int id)
        {
            return _context.Debater.Any(e => e.DebaterId == id);
        }
    }
}
