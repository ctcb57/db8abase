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
    public class JudgesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JudgesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Judges
        public async Task<IActionResult> Index()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentJudge = _context.Judge.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            return View(currentJudge);
        }

        // GET: Judges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var judge = await _context.Judge
                .FirstOrDefaultAsync(m => m.JudgeId == id);
            if (judge == null)
            {
                return NotFound();
            }

            return View(judge);
        }

        public IActionResult SelectSchool([Bind("SchoolId")] Judge judge, int id)
        {
            var selectedSchool = _context.School.FirstOrDefault(s => s.SchoolId == id);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentJudge = _context.Judge.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            judge = currentJudge;
            judge.SchoolId = selectedSchool.SchoolId;
            _context.Attach(judge);
            _context.SaveChanges();
            return RedirectToAction("Index", "Judges");
        }

        // GET: EditPhilosophy
        public IActionResult EditPhilosophy()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentJudge = _context.Judge.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            return View(currentJudge);
        }
        // POST: EditPhilosophy
        [HttpPost]
        public IActionResult EditPhilosophy([Bind("JudgeId,FirstName,LastName,Email,PhoneNumber,JudgingPhilosophy,SchoolId,ApplicationUserId")] Judge judge)
        {
            _context.Update(judge);
            _context.SaveChanges();
            return RedirectToAction("Index", "Judges");
        }
            // GET: Judges/Create
            public IActionResult Create()
        {
            Judge judge = new Judge();
            return View(judge);
        }

        public IActionResult ViewBallots()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentJudge = _context.Judge.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            List<Ballot> ballots = _context.Ballot.Where(b => b.JudgeId == currentJudge.JudgeId && b.BallotTurnedIn == false).ToList();
            return View(ballots);
        }

        public IActionResult ViewBallot(int id)
        {
            var teams = BuildTeamList(id);
            Ballot ballot = _context.Ballot.FirstOrDefault(b => b.BallotId == id);
            Debate debate = _context.Debate.FirstOrDefault(d => d.DebateId == ballot.DebateId);
            IndividualTeam affirmativeTeam = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == debate.AffirmativeTeamId);
            IndividualTeam negativeTeam = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == debate.NegativeTeamId);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            Judge judge = _context.Judge.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            Round round = _context.Round.FirstOrDefault(r => r.RoundId == ballot.RoundId);

            JudgesBallotViewModel ballotVM = new JudgesBallotViewModel()
            {
                Ballot = ballot,
                AffirmativeTeam = affirmativeTeam,
                NegativeTeam = negativeTeam,
                Round = round,
                Debate = debate,
                Judge = judge,
                Winner = "Team",
                Loser = "Team",
                TeamsInRound = teams,
            };
            return View(ballotVM);
        }
        [HttpPost]
        public IActionResult ViewBallot(JudgesBallotViewModel data)
        {
            Pairing pairing = _context.Pairing.Where(p => p.JudgeId == data.Ballot.JudgeId && p.RoundId == data.Ballot.RoundId).Single();
            IndividualTeam affTeam = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == pairing.AffirmativeTeamId);
            IndividualTeam negTeam = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == pairing.NegativeTeamId);
            Debater firstAffSpeaker = _context.Debater.FirstOrDefault(d => d.IndividualTeamId == affTeam.IndividualTeamId && d.SpeakerPosition == 1);
            Debater secondAffSpeaker = _context.Debater.FirstOrDefault(d => d.IndividualTeamId == affTeam.IndividualTeamId && d.SpeakerPosition == 2);
            Debater firstNegSpeaker = _context.Debater.FirstOrDefault(d => d.IndividualTeamId == negTeam.IndividualTeamId && d.SpeakerPosition == 1);
            Debater secondNegSpeaker = _context.Debater.FirstOrDefault(d => d.IndividualTeamId == negTeam.IndividualTeamId && d.SpeakerPosition == 2);
            Ballot ballot = data.Ballot;
            var winnerId = int.Parse(data.Winner);
            var loserId = int.Parse(data.Loser);
            IndividualTeam winningTeam = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == winnerId);
            IndividualTeam losingTeam = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == loserId);
            firstAffSpeaker.IndividualTournamentSpeakerPoints += data.Ballot.FirstAffSpeakerPoints;
            secondAffSpeaker.IndividualTournamentSpeakerPoints += data.Ballot.SecondAffSpeakerPoints;
            firstNegSpeaker.IndividualTournamentSpeakerPoints += data.Ballot.FirstNegSpeakerPoints;
            secondNegSpeaker.IndividualTournamentSpeakerPoints += data.Ballot.SecondNegSpeakerPoints;
            affTeam.SingleTournamentSpeakerPoints += (firstAffSpeaker.IndividualTournamentSpeakerPoints + secondAffSpeaker.IndividualTournamentSpeakerPoints);
            negTeam.SingleTournamentSpeakerPoints += (firstNegSpeaker.IndividualTournamentSpeakerPoints + secondNegSpeaker.IndividualTournamentSpeakerPoints);
            ballot.WinnerId = winnerId;
            pairing.WinnerId = winnerId;
            winningTeam.SingleTournamentWins++;
            winningTeam.CumulativeAnnualElminationRoundWins++;
            winningTeam.CumulativeAnnualWins++;
            losingTeam.SingleTournamentLosses++;
            losingTeam.CumulativeAnnualLosses++;
            ballot.BallotTurnedIn = true;
            _context.Update(ballot);
            _context.Update(pairing);
            _context.Update(winningTeam);
            _context.Update(losingTeam);
            _context.Update(firstAffSpeaker);
            _context.Update(secondAffSpeaker);
            _context.Update(firstNegSpeaker);
            _context.Update(secondNegSpeaker);
            _context.SaveChanges();
            return RedirectToAction("BallotSubmission", "Judges");
        }

        public IActionResult BallotSubmission()
        {
            return View();
        }

        public IEnumerable<SelectListItem> BuildTeamList(int id)
        {
            List<SelectListItem> teamsToChoose = new List<SelectListItem>();
            Ballot ballot = _context.Ballot.FirstOrDefault(b => b.BallotId == id);
            Debate debate = _context.Debate.FirstOrDefault(d => d.DebateId == ballot.DebateId);
            IndividualTeam affirmativeTeam = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == debate.AffirmativeTeamId);
            IndividualTeam negativeTeam = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == debate.NegativeTeamId);
            List<IndividualTeam> teams = new List<IndividualTeam>();
            teams.Add(affirmativeTeam);
            teams.Add(negativeTeam);
            foreach(var team in teams)
            {
                teamsToChoose.Add(
                                    new SelectListItem
                                    {
                                        Value = team.IndividualTeamId.ToString(),
                                        Text = team.IndividualTeamName,
                                    });
            }
            return teamsToChoose;
        }

        // POST: Judges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JudgeId,FirstName,LastName,Email,PhoneNumber,JudgingPhilosophy,SchoolId,ApplicationUserId")] Judge judge, string Id)
        {
            if (ModelState.IsValid)
            {
                judge.ApplicationUserId = Id;
                _context.Add(judge);
                await _context.SaveChangesAsync();
                return RedirectToAction("RegistrationConfirmation", "Home");
            }
            return View(judge);
        }

        // GET: Judges/Edit/5
        public IActionResult Edit()
        {
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentJudge = _context.Judge.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            return View(currentJudge);
        }

        // POST: Judges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("JudgeId,FirstName,LastName,Email,PhoneNumber,JudgingPhilosophy,SchoolId,ApplicationUserId")] Judge judge)
        {
            _context.Update(judge);
            _context.SaveChanges();
            return RedirectToAction("GetListOfSchools", "Schools");
        }

        // GET: Judges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var judge = await _context.Judge
                .FirstOrDefaultAsync(m => m.JudgeId == id);
            if (judge == null)
            {
                return NotFound();
            }

            return View(judge);
        }

        // POST: Judges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var judge = await _context.Judge.FindAsync(id);
            _context.Judge.Remove(judge);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JudgeExists(int id)
        {
            return _context.Judge.Any(e => e.JudgeId == id);
        }
    }
}
