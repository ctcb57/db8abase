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
        // GET: Coaches/TournamentManagement
        public IActionResult TournamentManagement()
        {
            var tournamentListing = _context.Tournament.ToList();
            return View(tournamentListing);
        }

        public IEnumerable<SelectListItem> BuildTeamList()
        {
            List<SelectListItem> teamList = new List<SelectListItem>();
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentCoach = _context.Coach.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            var teams = _context.IndividualTeam.Where(t => t.SchoolId == currentCoach.SchoolId).ToList();
            foreach(var team in teams)
            {
                teamList.Add(
                                new SelectListItem
                                {
                                    Value = team.IndividualTeamId.ToString(),
                                    Text = team.IndividualTeamName,
                                }); ;
            }
            return teamList;
        }

        public IEnumerable<SelectListItem> BuildJudgesList()
        {
            List<SelectListItem> judgeList = new List<SelectListItem>();
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentCoach = _context.Coach.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            var judges = _context.Judge.Where(j => j.SchoolId == currentCoach.SchoolId).ToList();
            foreach (var judge in judges)
            {
                judgeList.Add(
                                new SelectListItem
                                {
                                    Value = judge.JudgeId.ToString(),
                                    Text = $"{judge.FirstName} {judge.LastName}",
                                }); ;
            }
            return judgeList;
        }

        // GET: EnterJudges
        public ViewResult EnterJudges(int id)
        {
            JudgeEntry judgeEntry = new JudgeEntry();
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            var judges = BuildJudgesList();
            judgeEntry.TournamentId = tournament.TournamentId;
            _context.Add(judgeEntry);
            _context.SaveChanges();

            CoachesEnterJudgesViewModel coachesEnterJudgesViewModel = new CoachesEnterJudgesViewModel()
            {
                JudgeEntry = judgeEntry,
                Judges = judges,
                Judge = "judge",
            };
            return View(coachesEnterJudgesViewModel);
        }
        // POST: EnterJudges
        [HttpPost]
        public IActionResult EnterJudges(CoachesEnterJudgesViewModel data)
        {
            var judgeId = int.Parse(data.Judge);
            JudgeEntry judgeEntry = _context.JudgeEntry.Where(t => t.JudgeEntryId == data.JudgeEntry.JudgeEntryId).Single();
            judgeEntry.JudgeId = judgeId;
            _context.Attach(judgeEntry);
            _context.SaveChanges();
            return RedirectToAction("TournamentManagement", "Coaches");
        }

        // GET: Coaches/EnterTeam
        public ViewResult EnterTeams(int id)
        {
            TeamEntry teamEntry = new TeamEntry();
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            var teams = BuildTeamList();
            teamEntry.TournamentId = tournament.TournamentId;
            _context.Add(teamEntry);
            _context.SaveChanges();

            CoachesEnterTeamsViewModel coachesEnterTeamsViewModel = new CoachesEnterTeamsViewModel()
            {
                TeamEntry = teamEntry,
                Teams = teams,
                Team = "team",
            };
            return View(coachesEnterTeamsViewModel);
        }

        [HttpPost]
        public IActionResult EnterTeams(CoachesEnterTeamsViewModel data)
        {
            var teamId = int.Parse(data.Team);
            TeamEntry teamEntry = _context.TeamEntry.Where(t => t.EntryId == data.TeamEntry.EntryId).Single();
            teamEntry.IndividualTeamId = teamId;
            _context.Attach(teamEntry);
            _context.SaveChanges();
            var tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == teamEntry.TournamentId);
            var enteredTeam = _context.IndividualTeam.FirstOrDefault(t => t.IndividualTeamId == teamId);
            var teamCoach = _context.Coach.FirstOrDefault(c => c.CoachId == enteredTeam.CoachId);
            teamCoach.Balance = tournament.EntryFee;
            _context.Attach(teamCoach);
            _context.SaveChanges();
            return RedirectToAction("TournamentManagement", "Coaches");
        }


        // GET: Form Individual Team
        public IActionResult FormTeam()
        {
            IndividualTeam team = new IndividualTeam();
            return View(team);
        }
        [HttpPost]
        public IActionResult FormTeam([Bind("IndividualTeamId,FirstSpeaker,SecondSpeaker,SingleTournamentWins,SingleTournamentLosses,CumulativeAnnualWins,CumulativeAnnualLosses,CumulativeAnuualEliminationRoundWins,AnnualEliminationRoundAppearances,TournamentAffirmativeRounds,TournamentNegativeRounds,TocBids,CoachId,SchoolId")]IndividualTeam team)
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentCoach = _context.Coach.FirstOrDefault(c => c.ApplicationUserId == currentUserId);
            var school = _context.School.Where(s => s.SchoolId == currentCoach.SchoolId).Single();
            team.CoachId = currentCoach.CoachId;
            team.SchoolId = currentCoach.SchoolId;
            Debater debater1 = new Debater();
            debater1 = team.FirstSpeaker;
            debater1.CoachId = currentCoach.CoachId;
            debater1.SchoolId = currentCoach.SchoolId;
            Debater debater2 = new Debater();
            debater2 = team.SecondSpeaker;
            debater2.CoachId = currentCoach.CoachId;
            debater2.SchoolId = currentCoach.SchoolId;
            _context.Add(debater1);
            _context.Add(debater2);
            _context.SaveChanges();
            debater1.PartnerId = debater2.DebaterId;
            debater2.PartnerId = debater1.DebaterId;
            team.IndividualTeamName = $"{school.Name} {debater1.LastName} & {debater2.LastName}";
            _context.Update(debater1);
            _context.Update(debater2);
            _context.SaveChanges();
            _context.Add(team);
            _context.SaveChanges();
            return RedirectToAction("ManageTeam", "Coaches");
        }

        public List<SelectListItem> PopulateTeams()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentCoach = _context.Coach.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            var teams = _context.IndividualTeam.Where(d => d.SchoolId == currentCoach.SchoolId);
            foreach(var team in teams)
            {
                items.Add(new SelectListItem
                {
                    Text = $"{team.FirstSpeaker.LastName} & {team.SecondSpeaker.LastName}",
                    Value = team.IndividualTeamId.ToString()
                });
            }
            return items;
        }

        // GET: Coaches/SelectSchool
        public IActionResult SelectSchool([Bind("SchoolId")] Coach coach, int id)
        {
            var selectedSchool = _context.School.FirstOrDefault(s => s.SchoolId == id);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentCoach = _context.Coach.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            coach = currentCoach;
            coach.SchoolId = selectedSchool.SchoolId;
            _context.Update(coach);
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
