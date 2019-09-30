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
            };
            return View(viewModelData);
        }

        public IActionResult PairingsView(int id)
        {
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
                IndividualTeam affTeam = affirmativeTeams[i];
                IndividualTeam negTeam = negativeTeams[i];
                Pairing pairing = new Pairing();
                pairing.TournamentId = id;
                pairing.RoundId = round.RoundId;
                pairing.AffirmativeTeamId = affirmativeTeams[i].IndividualTeamId;
                pairing.NegativeTeamId = negativeTeams[i].IndividualTeamId;
                pairing.JudgeId = judges[i].JudgeId;
                pairing.DebateId = _context.Debate.Where(d => d.JudgeId == pairing.JudgeId && d.AffirmativeTeamId == pairing.AffirmativeTeamId && d.NegativeTeamId == pairing.NegativeTeamId).Single().DebateId;
                pairing.RoomId = _context.Debate.Where(d => d.DebateId == pairing.DebateId).Single().RoomId;
                affTeam.TournamentAffirmativeRounds++;
                negTeam.TournamentNegativeRounds++;
                _context.Add(pairing);
                _context.Update(affTeam);
                _context.Update(negTeam);
                _context.SaveChanges();
            }
        }

        public List<IndividualTeam> FindDueNegTeams(int id)
        {
            var teams = GetTeams(id);
            List<IndividualTeam> dueNegTeams = new List<IndividualTeam>();
            foreach(var team in teams)
            {
                if(team.TournamentAffirmativeRounds > team.TournamentNegativeRounds)
                {
                    dueNegTeams.Add(team);
                }
            }
            return dueNegTeams;
        }

        public List<IndividualTeam> FindDueAffTeams(int id)
        {
            var teams = GetTeams(id);
            List<IndividualTeam> dueAffTeams = new List<IndividualTeam>();
            foreach (var team in teams)
            {
                if (team.TournamentAffirmativeRounds < team.TournamentNegativeRounds)
                {
                    dueAffTeams.Add(team);
                }
            }
            return dueAffTeams;
        }

        public IActionResult SendRoundOnePairing(int id)
        {
            PushRoundOnePairing(id);
            CreateRoundOneBallots(id);
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            return View(tournament);
        }

        public void CreateRoundOneBallots(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            List<IndividualTeam> affirmativeTeams = GetRoundOneAffirmativeTeams(id);
            List<IndividualTeam> negativeTeams = GetRoundOneNegativeTeams(id);
            List<Judge> judges = GetJudges(id);
            List<Pairing> pairings = new List<Pairing>();
            for(int i = 0; i < affirmativeTeams.Count(); i++)
            {
                Pairing pairing = _context.Pairing.Where(p => p.TournamentId == id && p.AffirmativeTeamId == affirmativeTeams[i].IndividualTeamId && p.NegativeTeamId == negativeTeams[i].IndividualTeamId).Single();
                pairings.Add(pairing);
            }
            for(int j = 0; j < pairings.Count(); j++)
            {
                Ballot ballot = new Ballot();
                ballot.JudgeId = pairings[j].JudgeId;
                ballot.RoundId = pairings[j].RoundId;
                ballot.TournamentId = id;
                ballot.DebateId = pairings[j].DebateId;
                _context.Add(ballot);
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
        public Round CreateRoundTwo()
        {
            Round round = new Round();
            round.RoundNumber = 2;
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
        public List<Judge> AssignRoundTwoJudges(List<IndividualTeam> affTeams, List<IndividualTeam> negTeams, int id)
        {
            List<Judge> assignedJudges = new List<Judge>();
            List<Judge> judges = GetJudges(id);
            for(int i = 0; i < affTeams.Count(); i++)
            {
                foreach(var judge in judges)
                {
                    if (judge.SchoolId != affTeams[i].SchoolId && judge.SchoolId != negTeams[i].SchoolId)
                    {
                        List<Pairing> affTeamPairings = _context.Pairing.Where(p => p.NegativeTeamId == affTeams[i].IndividualTeamId).ToList();
                        List<Pairing> negTeamPairings = _context.Pairing.Where(p => p.AffirmativeTeamId == negTeams[i].IndividualTeamId).ToList();
                        if(judge.JudgeId != affTeamPairings[0].JudgeId && judge.JudgeId != negTeamPairings[0].JudgeId)
                        {
                            assignedJudges.Add(judge);
                            judges.Remove(judge);
                            break;
                        }
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


        public IActionResult PairRoundTwo(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            List<Room> rooms = GetRooms(id);
            List<IndividualTeam> affTeams = GetRoundTwoAffirmativeTeams(id);
            List<IndividualTeam> negTeams = GetRoundTwoNegativeTeams(id);
            List<Judge> judges = AssignRoundTwoJudges(affTeams, negTeams, id);
            PushRoundTwoPairing(id, affTeams, negTeams, judges, rooms);

            PairingsTabulationViewModel pairingVM = new PairingsTabulationViewModel()
            {
                Tournament = tournament,
                Rooms = rooms,
                AffirmativeTeams = affTeams,
                NegativeTeams = negTeams,
                Judges = judges,
            };
            return View(pairingVM);
        }

        public IActionResult SendRoundTwoPairing(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            return View(tournament);
        }
        
        public void CreateRoundTwoBallots(int id, List<Pairing> pairings)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            for(int i = 0; i < pairings.Count(); i++)
            {
                Ballot ballot = new Ballot();
                ballot.JudgeId = pairings[i].JudgeId;
                ballot.RoundId = pairings[i].RoundId;
                ballot.TournamentId = id;
                ballot.DebateId = pairings[i].DebateId;
                _context.Add(ballot);
                _context.SaveChanges();
            }
        }
        

        public void PushRoundTwoPairing(int id, List<IndividualTeam> affTeams, List<IndividualTeam> negTeams, List<Judge> judges, List<Room> rooms)
        {
            Round round = CreateRoundTwo();
            CreateRoundTwoDebate(id, affTeams, negTeams, judges, rooms);
            List<Pairing> pairings = CreateRoundTwoPairing(id, affTeams, negTeams, judges, round);
            CreateRoundTwoBallots(id, pairings);
        }
        public void CreateRoundTwoDebate(int id, List<IndividualTeam> affTeams, List<IndividualTeam> negTeams, List<Judge> judges, List<Room> rooms)
        {
            for(int i = 0; i < affTeams.Count(); i++)
            {
                Debate debate = new Debate();
                debate.RoomId = rooms[i].RoomId;
                debate.JudgeId = judges[i].JudgeId;
                debate.AffirmativeTeamId = affTeams[i].IndividualTeamId;
                debate.NegativeTeamId = negTeams[i].IndividualTeamId;
                _context.Add(debate);
                _context.SaveChanges();
            }
        }
        public List<Pairing> CreateRoundTwoPairing(int id, List<IndividualTeam> affTeams, List<IndividualTeam> negTeams, List<Judge> judges, Round round)
        {
            List<Pairing> pairings = new List<Pairing>();
            for (int i = 0; i < affTeams.Count(); i++)
            {
                IndividualTeam affTeam = affTeams[i];
                IndividualTeam negTeam = negTeams[i];
                Pairing pairing = new Pairing();
                pairing.TournamentId = id;
                pairing.RoundId = round.RoundId;
                pairing.AffirmativeTeamId = affTeams[i].IndividualTeamId;
                pairing.NegativeTeamId = negTeams[i].IndividualTeamId;
                pairing.JudgeId = judges[i].JudgeId;
                pairing.DebateId = _context.Debate.Where(d => d.JudgeId == pairing.JudgeId && d.AffirmativeTeamId == pairing.AffirmativeTeamId && d.NegativeTeamId == pairing.NegativeTeamId).Single().DebateId;
                pairing.RoomId = _context.Debate.Where(d => d.DebateId == pairing.DebateId).Single().RoomId;
                affTeam.TournamentAffirmativeRounds++;
                negTeam.TournamentNegativeRounds++;
                _context.Add(pairing);
                _context.Update(affTeam);
                _context.Update(negTeam);
                _context.SaveChanges();
                pairings.Add(pairing);
            }
            return pairings;
        }

        public List<IndividualTeam> GetRoundTwoAffirmativeTeams(int id)
        {
            List<IndividualTeam> roundTwoAffTeams = new List<IndividualTeam>();
            List<Pairing> pairings = _context.Pairing.Where(p => p.TournamentId == id).ToList();
            List<Pairing> roundOnePairings = new List<Pairing>();
            foreach(var pairing in pairings)
            {
                Round roundOne = _context.Round.Where(r => r.RoundNumber == 1).Single();
                if(pairing.RoundId == roundOne.RoundId)
                {
                    roundOnePairings.Add(pairing);
                }
            }
            foreach(var pair in roundOnePairings)
            {
                IndividualTeam negTeam = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == pair.NegativeTeamId);
                roundTwoAffTeams.Add(negTeam);
            }
            return roundTwoAffTeams;
        }

        public List<IndividualTeam> GetRoundTwoNegativeTeams(int id)
        {
            List<IndividualTeam> negTeams = new List<IndividualTeam>();
            List<Pairing> pairings = _context.Pairing.Where(p => p.TournamentId == id).ToList();
            List<Pairing> roundOnePairings = new List<Pairing>();
            List<IndividualTeam> roundTwoAffTeams = GetRoundTwoAffirmativeTeams(id);
            foreach (var pairing in pairings)
            {
                Round roundOne = _context.Round.Where(r => r.RoundNumber == 1).Single();
                if (pairing.RoundId == roundOne.RoundId)
                {
                    roundOnePairings.Add(pairing);
                }
            }
            foreach (var pair in roundOnePairings)
            {
                IndividualTeam affTeam = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == pair.AffirmativeTeamId);
                negTeams.Add(affTeam);
            }
            List<IndividualTeam> roundTwoNegTeams = PairEvenRounds(roundOnePairings, roundTwoAffTeams, negTeams);
            return roundTwoNegTeams;
        }

        public List<IndividualTeam> PairEvenRounds(List<Pairing> pairings, List<IndividualTeam> roundTwoAffTeams, List<IndividualTeam> negTeams)
        {
            List<IndividualTeam> roundTwoNegTeams = new List<IndividualTeam>();
            int i = 0;
            int j = 0;
            while (roundTwoNegTeams.Count() < roundTwoAffTeams.Count())
            {
                var pairing = pairings.Where(r => r.NegativeTeamId == roundTwoAffTeams[i].IndividualTeamId).Single();
                var random = new Random();
                int index = random.Next(negTeams.Count);
                var team = negTeams[index];
                if (pairing.AffirmativeTeamId != team.IndividualTeamId)
                {
                    roundTwoNegTeams.Add(team);
                    negTeams.Remove(team);
                    i++;
                }
                j++;
                if(j == (roundTwoAffTeams.Count() * 4))
                {
                    int k = 0;
                    while (roundTwoNegTeams.Count() > 0)
                    {
                        roundTwoNegTeams.Remove(roundTwoNegTeams[k]);
                        i--;
                    }
                    while (roundTwoNegTeams.Count() < roundTwoAffTeams.Count())
                    {
                        var pairing2 = pairings.Where(r => r.NegativeTeamId == roundTwoAffTeams[i].IndividualTeamId).Single();
                        var random2 = new Random();
                        int index2 = random.Next(negTeams.Count);
                        var team2 = negTeams[index2];
                        if (pairing2.AffirmativeTeamId != team.IndividualTeamId)
                        {
                            roundTwoNegTeams.Add(team2);
                            negTeams.Remove(team2);
                            i++;
                        }
                    }
                }
            }
            return roundTwoNegTeams;
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
