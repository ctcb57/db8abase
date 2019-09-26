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

        // GET: Details
        public IActionResult Details(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            List<IndividualTeam> teams = GetTeamEntries(id);

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Tournament = tournament,
                Teams = teams,
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
    }
}
