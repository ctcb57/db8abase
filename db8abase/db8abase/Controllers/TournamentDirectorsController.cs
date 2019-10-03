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
using Microsoft.AspNetCore.Http;
using System.IO;
using db8abase.Models.ViewModels;

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
        
        public IActionResult SelectSchool([Bind("SchoolId")] TournamentDirector tournamentDirector, int id)
        {
            var selectedSchool = _context.School.FirstOrDefault(s => s.SchoolId == id);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentDirector = _context.TournamentDirector.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            tournamentDirector = currentDirector;
            tournamentDirector.SchoolId = selectedSchool.SchoolId;
            _context.Attach(tournamentDirector);
            _context.SaveChanges();
            return RedirectToAction("Index", "TournamentDirectors");
        }

        public IActionResult GetRoomsList()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentDirector = _context.TournamentDirector.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            var roomList = _context.Room.Where(r => r.SchoolId == currentDirector.SchoolId).ToList();
            return View(roomList);
        }

        public IActionResult TournamentPortal()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentDirector = _context.TournamentDirector.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == currentDirector.TournamentId);
            Pairing pairing = _context.Pairing.FirstOrDefault(p => p.TournamentId == tournament.TournamentId);
            List<Round> rounds = _context.Round.Where(t => t.TournamentId == tournament.TournamentId).ToList();
            List<Round> roundsInOrder = rounds.OrderByDescending(r => r.RoundNumber).ToList();
            List<Debater> speakerAwards = SpeakerAwardsList(tournament.TournamentId, roundsInOrder);
            Round round = new Round();
            if(rounds.Count() == 0)
            {
                round = null;
            }
            else
            {
                round = roundsInOrder[0];
            }


            DirectorsTournamentPortalViewModel portalVM = new DirectorsTournamentPortalViewModel()
            {
                Tournament = tournament,
                Rounds = rounds,
                Round = round,
                SpeakerAwards = speakerAwards,
            };
            return View(portalVM);
        }
        public IActionResult DisplaySpeakerAwards(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            List<Round> rounds = _context.Round.Where(t => t.TournamentId == tournament.TournamentId).ToList();
            List<Round> roundsInOrder = rounds.OrderByDescending(r => r.RoundNumber).ToList();
            List<Debater> speakerAwards = SpeakerAwardsList(id, roundsInOrder);
            List<School> schools = GetSpeakerAwardSchools(speakerAwards);
            List<int> rankings = CreateRankings();

            PairingsSpeakerPointsViewModel data = new PairingsSpeakerPointsViewModel()
            {
                Tournament = tournament,
                Ranking = rankings,
                Schools = schools,
                SpeakerAwards = speakerAwards,
            };
            return View(data);
        }
        public IActionResult CompleteSpeakerAwards(int id)
        {
            List<IndividualTeam> teams = GetTeams(id);
            List<Debater> debaters = new List<Debater>();
            foreach(var team in teams)
            {
                Debater debater1 = _context.Debater.FirstOrDefault(d => d.DebaterId == team.FirstSpeakerId);
                Debater debater2 = _context.Debater.FirstOrDefault(d => d.DebaterId == team.SecondSpeakerId);
                debaters.Add(debater1);
                debaters.Add(debater2);
            }
            foreach(var debater in debaters)
            {
                int numberOfTournaments = _context.TeamEntry.Where(t => t.IndividualTeamId == debater.IndividualTeamId).ToList().Count();
                debater.AnnualAverageSpeakerPoints = (((debater.AnnualAverageSpeakerPoints * (numberOfTournaments - 1)) + debater.IndividualTournamentSpeakerPoints) / numberOfTournaments);
                debater.IndividualTournamentSpeakerPoints = 0;
                _context.Update(debater);
                _context.SaveChanges();
            }
            return View();
        }
        
        public List<int> CreateRankings()
        {
            List<int> rankings = new List<int>();
            int speakerThreshold = 10;
            int count = 0;
            for(int i = 0; i < speakerThreshold; i++)
            {
                count++;
                rankings.Add(count);
            }
            return rankings;
        }

        public List<School> GetSpeakerAwardSchools(List<Debater> speakers)
        {
            List<School> schools = new List<School>();
            foreach(var speaker in speakers)
            {
                School school = _context.School.FirstOrDefault(s => s.SchoolId == speaker.SchoolId);
                schools.Add(school);
            }
            return schools;
        }

        public List<Debater> SpeakerAwardsList(int id, List<Round> roundsInOrder)
        {
            if (roundsInOrder[0].RoundNumber == 4)
            {
                Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
                List<IndividualTeam> teams = GetTeams(id);
                List<Debater> debaters = new List<Debater>();
                List<Debater> speakerAwards = new List<Debater>();
                List<Debater> emptyList = new List<Debater>();
                int speakerAwardThreshold = 10;
                foreach (var team in teams)
                {
                    Debater debater1 = _context.Debater.FirstOrDefault(d => d.DebaterId == team.FirstSpeakerId);
                    Debater debater2 = _context.Debater.FirstOrDefault(d => d.DebaterId == team.SecondSpeakerId);
                    debaters.Add(debater1);
                    debaters.Add(debater2);
                }
                List<Debater> speakerRankings = debaters.OrderByDescending(d => d.IndividualTournamentSpeakerPoints).ToList();
                for (int i = 0; i < speakerAwardThreshold; i++)
                {
                    speakerAwards.Add(speakerRankings[i]);
                }
                if(speakerAwards[0].IndividualTournamentSpeakerPoints == 0)
                {
                    return emptyList;
                }
                else
                {
                    return speakerAwards;
                }
            }
            else
            {
                List<Debater> fakeList = new List<Debater>();
                return fakeList;
            }
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
        public IActionResult ViewIndividualRound(int id)
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentDirector = _context.TournamentDirector.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            Round round = _context.Round.FirstOrDefault(r => r.RoundId == id);
            List<Room> rooms = GetRooms(currentDirector.TournamentId);
            List<IndividualTeam> affTeams = GetAffirmativeTeams(id);
            List<IndividualTeam> negTeams = GetNegativeTeams(id);
            List<Judge> judges = GetJudges(id);
            List<Ballot> ballots = GetBallots(id);
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == currentDirector.TournamentId);

            PairingsTabulationViewModel pairingVM = new PairingsTabulationViewModel()
            {
                Tournament = tournament,
                Rooms = rooms,
                AffirmativeTeams = affTeams,
                NegativeTeams = negTeams,
                Judges = judges,
                Ballots = ballots,
                Round = round,
            };
            return View(pairingVM);
        }
        public List<Room> GetRooms(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            List<Room> rooms = _context.Room.Where(r => r.SchoolId == tournament.SchoolId).ToList();
            return rooms;
        }
        public List<IndividualTeam> GetAffirmativeTeams(int id)
        {
            List<IndividualTeam> affTeams = new List<IndividualTeam>();
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentDirector = _context.TournamentDirector.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == currentDirector.TournamentId);
            List<Pairing> pairings = _context.Pairing.Where(p => p.RoundId == id).ToList();
            foreach(var pairing in pairings)
            {
                IndividualTeam affTeam = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == pairing.AffirmativeTeamId);
                affTeams.Add(affTeam);
            }
            return affTeams;
        }
        public List<IndividualTeam> GetNegativeTeams(int id)
        {
            List<IndividualTeam> negTeams = new List<IndividualTeam>();
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentDirector = _context.TournamentDirector.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == currentDirector.TournamentId);
            List<Pairing> pairings = _context.Pairing.Where(p => p.RoundId == id).ToList();
            foreach (var pairing in pairings)
            {
                IndividualTeam negTeam = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == pairing.NegativeTeamId);
                negTeams.Add(negTeam);
            }
            return negTeams;
        }
        public List<Judge> GetJudges(int id)
        {
            List<Judge> judges = new List<Judge>();
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentDirector = _context.TournamentDirector.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == currentDirector.TournamentId);
            List<Pairing> pairings = _context.Pairing.Where(p => p.RoundId == id).ToList();
            foreach (var pairing in pairings)
            {
                Judge judge = _context.Judge.FirstOrDefault(i => i.JudgeId == pairing.JudgeId);
                judges.Add(judge);
            }
            return judges;
        }
        public List<Ballot> GetBallots (int id)
        {
            List<Ballot> ballots = _context.Ballot.Where(b => b.RoundId == id).ToList();
            return ballots;
        }

        public List<Pairing> GetRoundPairings (int id)
        {
            List<Pairing> roundPairings = _context.Pairing.Where(p => p.RoundId == id).ToList();
            return roundPairings;
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
                return RedirectToAction("RegistrationConfirmation", "Home");
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
        public IActionResult CreateTournament([Bind("TournamentId,Name,School,NumberOfRounds,NumberOfEliminationRounds,EntryFee,TournamentDate,TeamLimit,FilePath")] Tournament tournament)
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentDirector = _context.TournamentDirector.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            var directorSchool = _context.School.FirstOrDefault(s => s.SchoolId == currentDirector.SchoolId);
            tournament.School = directorSchool;
            tournament.FilePath = currentDirector.FilePath;
            _context.Add(tournament);
            _context.SaveChanges();
            currentDirector.TournamentId = tournament.TournamentId;
            _context.Attach(currentDirector);
            _context.SaveChanges();
            return RedirectToAction("GetRoomsList", "TournamentDirectors");
        }

        // GET: Tabulation
        public IActionResult TournamentListing()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentDirector = _context.TournamentDirector.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            var currentTournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == currentDirector.TournamentId);
            return View(currentTournament);
        }


        // GET: TournamentDirectors/Edit/5
        public async Task<IActionResult> Edit()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentDirector = _context.TournamentDirector.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            return View(currentDirector);
        }

        // POST: TournamentDirectors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit([Bind("TournamentDirectorId,FirstName,LastName,Email,Message,TournamentId,ApplicationUserId,SchoolId")] TournamentDirector tournamentDirector)
        {
            _context.Update(tournamentDirector);
            _context.SaveChanges();
            return RedirectToAction("GetListOfSchools", "Schools");
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
