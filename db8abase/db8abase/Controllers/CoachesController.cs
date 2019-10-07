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
using Stripe;

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

        public IEnumerable<SelectListItem> BuildTeamList(int id)
        {
            List<SelectListItem> teamList = new List<SelectListItem>();
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentCoach = _context.Coach.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            List<IndividualTeam> teams = new List<IndividualTeam>();
            var currentEntries = _context.TeamEntry.Where(t => t.TournamentId == id).ToList();
            var teamsFromSchool = _context.IndividualTeam.Where(t => t.SchoolId == currentCoach.SchoolId).ToList();
            for(int i = 0; i < teamsFromSchool.Count(); i++)
            {
                if (currentEntries.Count() == 0)
                {
                    teams.Add(teamsFromSchool[i]);
                }
                else
                {
                    for (int j = 0; j < currentEntries.Count(); j++)
                    {
                        if (teamsFromSchool[i].IndividualTeamId == currentEntries[j].IndividualTeamId)
                        {
                            break;
                        }
                        else if(currentEntries.Count() == 1)
                        {
                            teams.Add(teamsFromSchool[i]);
                            break;
                        }
                        else if (j == (currentEntries.Count() - 1))
                        {
                            teams.Add(teamsFromSchool[i]);
                            break;
                        }
                    }
                }
            }

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

        public IEnumerable<SelectListItem> BuildJudgesList(int id)
        {
            List<SelectListItem> judgeList = new List<SelectListItem>();
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentCoach = _context.Coach.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            List<Judge> judges = new List<Judge>();
            var currentEntries = _context.JudgeEntry.Where(t => t.TournamentId == id).ToList();
            var teamJudges = _context.Judge.Where(j => j.SchoolId == currentCoach.SchoolId).ToList();
            for (int i = 0; i < teamJudges.Count(); i++)
            {
                if (currentEntries.Count() == 0)
                {
                    judges.Add(teamJudges[i]);
                }
                else
                {
                    for (int j = 0; j < currentEntries.Count(); j++)
                    {
                        if (teamJudges[i].JudgeId == currentEntries[j].JudgeId)
                        {
                            break;
                        }
                        else if (currentEntries.Count() == 1)
                        {
                            judges.Add(teamJudges[i]);
                            break;
                        }
                        else if (j == (currentEntries.Count() - 1))
                        {
                            judges.Add(teamJudges[i]);
                            break;
                        }
                    }
                }
            }
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
            var judges = BuildJudgesList(id);
            if (judges.Count() > 0)
            {
                judgeEntry.TournamentId = tournament.TournamentId;
                _context.Add(judgeEntry);
                _context.SaveChanges();
            }

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
            _context.Update(judgeEntry);
            _context.SaveChanges();
            return RedirectToAction("TournamentManagement", "Coaches");
        }

        // GET: Coaches/EnterTeam
        public ViewResult EnterTeams(int id)
        {
            TeamEntry teamEntry = new TeamEntry();
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            var teams = BuildTeamList(id);
            if(teams.Count() > 0)
            {
                teamEntry.TournamentId = tournament.TournamentId;
                _context.Add(teamEntry);
                _context.SaveChanges();
            }

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
            _context.Update(teamEntry);
            _context.SaveChanges();
            var tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == teamEntry.TournamentId);
            var enteredTeam = _context.IndividualTeam.FirstOrDefault(t => t.IndividualTeamId == teamId);
            var teamCoach = _context.Coach.FirstOrDefault(c => c.CoachId == enteredTeam.CoachId);
            teamCoach.Balance = tournament.EntryFee;
            _context.Update(teamCoach);
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
        public IActionResult FormTeam([Bind("IndividualTeamId,FirstSpeaker,SecondSpeaker,FirstSpeakerId,SecondSpeakerId,SingleTournamentWins,SingleTournamentLosses,SingleTournamentSpeakerPoints,CumulativeAnnualWins,CumulativeAnnualLosses,CumulativeAnuualEliminationRoundWins,AnnualEliminationRoundAppearances,TournamentAffirmativeRounds,TournamentNegativeRounds,TocBids,CoachId,SchoolId")]IndividualTeam team)
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
            team.FirstSpeakerId = debater1.DebaterId;
            team.SecondSpeakerId = debater2.DebaterId;
            debater1.IndividualTeamId = team.IndividualTeamId;
            debater2.IndividualTeamId = team.IndividualTeamId;
            debater1.SpeakerPosition = 1;
            debater2.SpeakerPosition = 2;
            _context.Update(team);
            _context.Update(debater1);
            _context.Update(debater2);
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
                var firstSpeaker = _context.Debater.FirstOrDefault(d => d.IndividualTeamId == team.IndividualTeamId && d.SpeakerPosition == 1);
                var secondSpeaker = _context.Debater.FirstOrDefault(d => d.IndividualTeamId == team.IndividualTeamId && d.SpeakerPosition ==2);
                items.Add(new SelectListItem
                {
                    Text = $"{firstSpeaker.LastName} & {secondSpeaker.LastName}",
                    Value = team.IndividualTeamId.ToString()
                });
            }
            return items;
        }

        public List<SelectListItem> PopulateGraphs()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            int count = 0;
            List<int> numbers = new List<int>();
            List<string> graphs = new List<string>() { "Tournament Records", "Wins Line Graph", "Speaker Points" };
            for(int i = 0; i < graphs.Count(); i++)
            {
                count++;
                numbers.Add(count);
            }
            for(int j = 0; j < graphs.Count(); j++)
            {
                items.Add(new SelectListItem
                {
                    Text = graphs[j],
                    Value = numbers[j].ToString()
                });
            }
            return items;
        }

        public IActionResult PickTeams()
        {
            List<SelectListItem> firstTeam = PopulateAllTeams();
            List<SelectListItem> secondTeam = PopulateAllTeams();
            List<SelectListItem> graphs = PopulateGraphs();

            CoachesViewReportsViewModel data = new CoachesViewReportsViewModel()
            {
                FirstTeamList = firstTeam,
                SecondTeamList = secondTeam,
                TeamA = "team",
                TeamB = "team",
                Graphs = graphs,
                GraphNumber = "one",
            };
            return View(data);
        }
        [HttpPost]
        public IActionResult ViewRecords(CoachesViewReportsViewModel data)
        {
            int graph = Convert.ToInt32(data.GraphNumber);
            int firstTeamId = Convert.ToInt32(data.TeamA);
            int secondTeamId = Convert.ToInt32(data.TeamB);
            IndividualTeam team1 = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == firstTeamId);
            IndividualTeam team2 = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == secondTeamId);
            var firstModel = new List<SimpleReportViewModel>();
            List<Tournament> firstTeamTournament = PopulateTournaments(team1.IndividualTeamId);
            List<Tournament> secondTeamTournament = PopulateTournaments(team2.IndividualTeamId);
            List<TournamentResults> firstTeamResults = PopulateResults(team1.IndividualTeamId);
            List<TournamentResults> secondTeamResults = PopulateResults(team2.IndividualTeamId);
            List<CoachesViewReportsViewModel> firstTeamData = new List<CoachesViewReportsViewModel>();
            List<CoachesViewReportsViewModel> secondTeamData = new List<CoachesViewReportsViewModel>();
            List<SimpleReportViewModel> firstTeamReports = new List<SimpleReportViewModel>();
            List<SimpleReportViewModel> secondTeamReports = new List<SimpleReportViewModel>();
            if(graph == 3)
            {
                for (int i = 0; i < firstTeamResults.Count(); i++)
                {
                    CoachesViewReportsViewModel teamData = new CoachesViewReportsViewModel()
                    {
                        TournamentName = firstTeamTournament[i].Name,
                        SpeakerPoints = firstTeamResults[i].SpeakerPoints,
                    };
                    firstTeamData.Add(teamData);
                }
                for (int j = 0; j < secondTeamResults.Count(); j++)
                {
                    CoachesViewReportsViewModel teamData = new CoachesViewReportsViewModel()
                    {
                        TournamentName = secondTeamTournament[j].Name,
                        SpeakerPoints = secondTeamResults[j].SpeakerPoints,
                    };
                    secondTeamData.Add(teamData);
                }
                foreach (var item in firstTeamData)
                {
                    firstTeamReports.Add(new SimpleReportViewModel
                    {
                        DimensionOne = item.TournamentName,
                        Quantity = item.SpeakerPoints,
                    });
                }
                foreach (var item in secondTeamData)
                {
                    secondTeamReports.Add(new SimpleReportViewModel
                    {
                        DimensionOne = item.TournamentName,
                        Quantity = item.SpeakerPoints,
                    });
                }
            }
            else
            {
                for (int i = 0; i < firstTeamResults.Count(); i++)
                {
                    CoachesViewReportsViewModel teamData = new CoachesViewReportsViewModel()
                    {
                        TournamentName = firstTeamTournament[i].Name,
                        Wins = firstTeamResults[i].TeamWins,
                    };
                    firstTeamData.Add(teamData);
                }
                foreach (var item in firstTeamData)
                {
                    firstTeamReports.Add(new SimpleReportViewModel
                    {
                        DimensionOne = item.TournamentName,
                        Quantity = item.Wins,
                    });
                }
                for (int j = 0; j < secondTeamResults.Count(); j++)
                {
                    CoachesViewReportsViewModel teamData = new CoachesViewReportsViewModel()
                    {
                        TournamentName = secondTeamTournament[j].Name,
                        Wins = secondTeamResults[j].TeamWins,
                    };
                    secondTeamData.Add(teamData);
                }
                foreach (var item in secondTeamData)
                {
                    secondTeamReports.Add(new SimpleReportViewModel
                    {
                        DimensionOne = item.TournamentName,
                        Quantity = item.Wins,
                    });
                }
            }

            CoachesViewReportsViewModel viewData = new CoachesViewReportsViewModel()
            {
                FirstTeamReports = firstTeamReports,
                SecondTeamReports = secondTeamReports,
                TeamOne = team1,
                TeamTwo = team2,
            };
            return View(viewData);
        }

        public List<TournamentResults> PopulateResults(int id)
        {
            List<TournamentResults> results = _context.TournamentResults.Where(t => t.IndividualTeamId == id).ToList();
            return results;
        }

        public List<Tournament> PopulateTournaments(int id)
        {
            List<Tournament> tournaments = new List<Tournament>();
            List<TeamEntry> entries = _context.TeamEntry.Where(t => t.IndividualTeamId == id).ToList();
            foreach(var entry in entries)
            {
                Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == entry.TournamentId);
                tournaments.Add(tournament);
            }
            return tournaments;
        }

        public List<SelectListItem> PopulateAllTeams()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            List<IndividualTeam> teams = _context.IndividualTeam.ToList();
            foreach(var team in teams)
            {
                items.Add(new SelectListItem
                {
                    Text = $"{ team.IndividualTeamName}",
                    Value = team.IndividualTeamId.ToString()
                });
            }
            return items;
        }

        // GET: Coaches/ViewTournamentDetails
        public IActionResult ViewTournamentDetails(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            List<IndividualTeam> teams = GetMyEntries(id);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            Coach currentCoach = _context.Coach.FirstOrDefault(t => t.ApplicationUserId == currentUserId);

            CoachesViewTournamentDetailsViewModel coachesViewTournamentDetailsViewModel = new CoachesViewTournamentDetailsViewModel()
            {
                Tournament = tournament,
                Teams = teams,
                Coach = currentCoach,
            };
            return View(coachesViewTournamentDetailsViewModel);
        }
        [HttpPost]
        public IActionResult Charge(string stripeEmail, string stripeToken)
        {
            var customers = new CustomerService();
            var charges = new ChargeService();
            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var coach = _context.Coach.FirstOrDefault(m => m.ApplicationUserId == currentUserId);

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = Convert.ToInt64(coach.Balance * 100),
                Description = "Sample Charge",
                Currency = "usd",
                CustomerId = customer.Id
            });
            coach.Balance = 0;
            _context.SaveChanges();
            return RedirectToAction("ViewTournamentDetails", "Coaches");
        }
        public List<IndividualTeam> GetMyEntries(int id)
        {
            List<IndividualTeam> teams = new List<IndividualTeam>();
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentCoach = _context.Coach.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            var entries = _context.TeamEntry.Where(t => t.TournamentId == id).ToList();
            var individualTeams = _context.IndividualTeam.ToList();
            foreach (var entry in entries)
            {
                for (int i = 0; i < individualTeams.Count; i++)
                {
                    if (entry.IndividualTeamId == individualTeams[i].IndividualTeamId)
                    {
                        var locatedTeam = _context.IndividualTeam.FirstOrDefault(t => t.IndividualTeamId == individualTeams[i].IndividualTeamId && t.CoachId == currentCoach.CoachId);
                        teams.Add(locatedTeam);
                    }
                }
            }
            return teams;
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
