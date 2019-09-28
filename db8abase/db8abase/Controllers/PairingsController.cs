using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using db8abase.Data;
using db8abase.Models;
using db8abase.Models.ViewModels;

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
        // GET: PairRoundOne
        public IActionResult PairRoundOne(int id)
        {
            PushRoundOnePairing(id);
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            List<Room> rooms = GetRooms(id);
            List<IndividualTeam> affirmativeTeams = GetRoundOneAffirmativeTeams(id);
            List<IndividualTeam> negativeTeams = GetRoundOneNegativeTeams(id);
            List<Judge> judges = AssignRoundOneJudges(id);

            PairingsTabulationViewModel viewModelData = new PairingsTabulationViewModel()
            {
                Tournament = tournament,
                Rooms = rooms,
                AffirmativeTeams = affirmativeTeams,
                NegativeTeams = negativeTeams,
                Judges = judges,
                //Round = round,
            };
            return View(viewModelData);
        }
        public void PushRoundOnePairing(int id)
        {
            Round round = CreateRoundOne();
            CreateRoundOneDebate(id);
            List<IndividualTeam> affirmativeTeams = GetRoundOneAffirmativeTeams(id);
            List<IndividualTeam> negativeTeams = GetRoundOneNegativeTeams(id);
            List<Judge> judges = GetJudges(id);
            for(int i = 0; i < affirmativeTeams.Count(); i++)
            {
                Pairing pairing = new Pairing();
                pairing.TournamentId = id;
                pairing.RoundId = round.RoundId;
                pairing.AffirmativeTeamId = affirmativeTeams[i].IndividualTeamId;
                pairing.NegativeTeamId = negativeTeams[i].IndividualTeamId;
                pairing.JudgeId = judges[i].JudgeId;
                pairing.DebateId = _context.Debate.Where(d => d.JudgeId == pairing.JudgeId && d.AffirmativeTeamId == pairing.AffirmativeTeamId && d.NegativeTeamId == pairing.NegativeTeamId).Single().DebateId;
                pairing.RoomId = _context.Debate.Where(d => d.DebateId == pairing.DebateId).Single().RoomId;
                _context.Add(pairing);
                _context.SaveChanges();
            }
        }

        public Round CreateRoundOne()
        {
            Round round = new Round();
            round.RoundNumber = 1;
            round.RoundType = "prelim";
            _context.Add(round);
            _context.SaveChanges();
            return round;
        }

        public void CreateRoundOneDebate(int id)
        {
            List<Room> rooms = GetRooms(id);
            List<Judge> judges = GetJudges(id);
            List<IndividualTeam> affirmativeTeams = GetRoundOneAffirmativeTeams(id);
            List<IndividualTeam> negativeTeams = GetRoundOneNegativeTeams(id);
            for(int i = 0; i < affirmativeTeams.Count(); i++)
            {
                Debate debate = new Debate();
                debate.RoomId = rooms[i].RoomId;
                debate.JudgeId = judges[i].JudgeId;
                debate.AffirmativeTeamId = affirmativeTeams[i].IndividualTeamId;
                debate.NegativeTeamId = negativeTeams[i].IndividualTeamId;
                _context.Add(debate);
                _context.SaveChanges();
            }
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

        public List<IndividualTeam> GetRoundOneAffirmativeTeams(int id)
        {
            List<IndividualTeam> affirmativeTeams = new List<IndividualTeam>();
            List<IndividualTeam> enteredTeams = GetTeams(id);
            int teamCount = enteredTeams.Count();
            for(int i = 0; i < teamCount / 2; i++)
            {
                affirmativeTeams.Add(enteredTeams[i]);
            }
            return affirmativeTeams;
        }

        public List<Judge> AssignRoundOneJudges(int id)
        {
            List<Judge> assignedJudges = new List<Judge>();
            List<IndividualTeam> affirmativeTeams = GetRoundOneAffirmativeTeams(id);
            List<IndividualTeam> negativeTeams = GetRoundOneNegativeTeams(id);
            List<Judge> judges = GetJudges(id);
            for(int i = 0; i < affirmativeTeams.Count; i++)
            {
                foreach(var judge in judges)
                {
                    if(judge.SchoolId != affirmativeTeams[i].SchoolId && judge.SchoolId != negativeTeams[i].SchoolId)
                    {
                        assignedJudges.Add(judge);
                        judges.Remove(judge);
                        break;
                    }
                }
            }
            return assignedJudges;
        }

        public List<IndividualTeam> GetRoundOneNegativeTeams(int id)
        {
            List<IndividualTeam> negativeTeams = new List<IndividualTeam>();
            List<IndividualTeam> enteredTeams = GetTeams(id);
            int teamCount = enteredTeams.Count();
            for(int i = teamCount; i > teamCount / 2; i--)
            {
                negativeTeams.Add(enteredTeams[i - 1]);
            }
            return negativeTeams;
        }

        public List<IndividualTeam> GetTeams(int id)
        {
            List<IndividualTeam> teams = new List<IndividualTeam>();
            var entries = _context.TeamEntry.Where(t => t.TournamentId == id).ToList();
            var individualTeams = _context.IndividualTeam.ToList();
            foreach (var entry in entries)
            {
                for (int i = 0; i < individualTeams.Count; i++)
                {
                    if (entry.IndividualTeamId == individualTeams[i].IndividualTeamId)
                    {
                        var locatedTeam = _context.IndividualTeam.FirstOrDefault(t => t.IndividualTeamId == individualTeams[i].IndividualTeamId);
                        teams.Add(locatedTeam);
                    }
                }
            }
            return teams;
        }
        public List<Judge> GetJudges(int id)
        {
            List<Judge> judges = new List<Judge>();
            var entries = _context.JudgeEntry.Where(t => t.TournamentId == id).ToList();
            var individualJudges = _context.Judge.ToList();
            foreach (var entry in entries)
            {
                for (int i = 0; i < individualJudges.Count; i++)
                {
                    if (entry.JudgeId == individualJudges[i].JudgeId)
                    {
                        var locatedJudge = _context.Judge.FirstOrDefault(t => t.JudgeId == individualJudges[i].JudgeId);
                        judges.Add(locatedJudge);
                    }
                }
            }
            return judges;
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
