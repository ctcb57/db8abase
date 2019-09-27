using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using db8abase.Models;
using db8abase.Data;
using db8abase.Models.ViewModels;

namespace db8abase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // GET: TournamentList
        public IActionResult GetTournamentListing()
        {
            var tournamentList = _context.Tournament.ToList();
            return View(tournamentList);
        }

        public IActionResult RegistrationConfirmation()
        {
            return View();
        }

        // GET: JudgesList 
        public IActionResult GetJudgesList()
        {
            var judgesList = _context.Judge.ToList();
            return View(judgesList);
        }

        // GET: ViewPhilosophy
        public IActionResult ViewPhilosophy(int id)
        {
            var judge = _context.Judge.FirstOrDefault(j => j.JudgeId == id);
            return View(judge);
        }

        // GET: Details
        public IActionResult Details(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            List<IndividualTeam> teams = GetTeamEntries(id);
            List<Judge> judges = GetJudgeEntries(id);
            School school = _context.School.FirstOrDefault(s => s.SchoolId == tournament.SchoolId);
            Address address = _context.Address.FirstOrDefault(a => a.AddressId == school.AddressId);

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Tournament = tournament,
                Teams = teams,
                Judges = judges,
                Address = address,
                School = school,
            };
            return View(homeDetailsViewModel);
        }

        public List<IndividualTeam> GetTeamEntries(int id)
        {
            List<IndividualTeam> teams = new List<IndividualTeam>();
            var entries = _context.TeamEntry.Where(t => t.TournamentId == id).ToList();
            var individualTeams = _context.IndividualTeam.ToList();
            foreach(var entry in entries)
            {
                for(int i = 0; i < individualTeams.Count; i++)
                {
                    if(entry.IndividualTeamId == individualTeams[i].IndividualTeamId)
                    {
                        var locatedTeam = _context.IndividualTeam.FirstOrDefault(t => t.IndividualTeamId == individualTeams[i].IndividualTeamId);
                        teams.Add(locatedTeam);
                    }
                }
            }
            return teams;
        }

        public List<Judge> GetJudgeEntries(int id)
        {
            List<Judge> judges = new List<Judge>();
            var entries = _context.JudgeEntry.Where(t => t.TournamentId == id).ToList();
            var individualJudges = _context.Judge.ToList();
            foreach (var entry in entries)
            {
                for (int i = 0; i < individualJudges.Count; i++)
                {
                    if (entry.JudgeEntryId == individualJudges[i].JudgeId)
                    {
                        var locatedJudge = _context.Judge.FirstOrDefault(t => t.JudgeId == individualJudges[i].JudgeId);
                        judges.Add(locatedJudge);
                    }
                }
            }
            return judges;
        }
    }
}
