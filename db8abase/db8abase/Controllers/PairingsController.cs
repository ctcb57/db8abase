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
using System.Security.Claims;

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

        public IActionResult ViewRoundDetails(int id)
        {
            List<Ballot> ballots = GetBallots(id);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
            var currentDirector = _context.TournamentDirector.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == currentDirector.TournamentId);
            List<Room> rooms = GetRooms(currentDirector.TournamentId);
            List<IndividualTeam> affirmativeTeams = GetRoundAffTeams(id);
            List<IndividualTeam> negativeTeams = GetRoundNegTeams(id);
            List<Judge> judges = GetRoundJudges(id);

            PairingsTabulationViewModel viewModelData = new PairingsTabulationViewModel()
            {
                Tournament = tournament,
                Rooms = rooms,
                AffirmativeTeams = affirmativeTeams,
                NegativeTeams = negativeTeams,
                Judges = judges,
                Ballots = ballots,
            };
            return View(viewModelData);
        }
        public List<IndividualTeam> GetRoundAffTeams(int id)
        {
            List<Pairing> roundPairing = _context.Pairing.Where(p => p.RoundId == id).ToList();
            List<IndividualTeam> roundAffTeams = new List<IndividualTeam>();
            for(int i = 0; i < roundPairing.Count(); i++)
            {
                IndividualTeam team = _context.IndividualTeam.FirstOrDefault(t => t.IndividualTeamId == roundPairing[i].AffirmativeTeamId);
                roundAffTeams.Add(team);
            }
            return roundAffTeams;
        }
        public List<IndividualTeam> GetRoundNegTeams(int id)
        {
            List<Pairing> roundPairing = _context.Pairing.Where(p => p.RoundId == id).ToList();
            List<IndividualTeam> roundNegTeams = new List<IndividualTeam>();
            for (int i = 0; i < roundPairing.Count(); i++)
            {
                IndividualTeam team = _context.IndividualTeam.FirstOrDefault(t => t.IndividualTeamId == roundPairing[i].NegativeTeamId);
                roundNegTeams.Add(team);
            }
            return roundNegTeams;
        }
        public List<Judge> GetRoundJudges(int id)
        {
            List<Pairing> roundPairing = _context.Pairing.Where(p => p.RoundId == id).ToList();
            List<Judge> judges = new List<Judge>();
            for (int i = 0; i < roundPairing.Count(); i++)
            {
                Judge judge = _context.Judge.FirstOrDefault(t => t.JudgeId == roundPairing[i].JudgeId);
                judges.Add(judge);
            }
            return judges;
        }

        public List<Ballot> GetBallots(int id)
        {
            List<Ballot> ballots = _context.Ballot.Where(b => b.RoundId == id).ToList();
            return ballots;
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
            Round round = CreateRoundOne(id);
            CreateRoundOneDebate(id);
            List<IndividualTeam> affirmativeTeams = GetRoundOneAffirmativeTeams(id);
            List<IndividualTeam> negativeTeams = GetRoundOneNegativeTeams(id);
            List<Judge> judges = AssignRoundOneJudges(id);
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
            List<Judge> judges = AssignRoundOneJudges(id);
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
                ballot.BallotTurnedIn = false;
                _context.Add(ballot);
                _context.SaveChanges();
            }
        }

        public Round CreateRoundOne(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            Round round = new Round();
            round.RoundNumber = 1;
            round.RoundType = "prelim";
            round.TournamentId = tournament.TournamentId;
            _context.Add(round);
            _context.SaveChanges();
            return round;
        }
        public Round CreateRoundTwo(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            Round round = new Round();
            round.RoundNumber = 2;
            round.RoundType = "prelim";
            round.TournamentId = tournament.TournamentId;
            _context.Add(round);
            _context.SaveChanges();
            return round;
        }
        public Round CreateRoundThree(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            Round round = new Round();
            round.RoundNumber = 3;
            round.RoundType = "prelim";
            round.TournamentId = tournament.TournamentId;
            _context.Add(round);
            _context.SaveChanges();
            return round;
        }
        public Round CreateRoundFour(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            Round round = new Round();
            round.RoundNumber = 4;
            round.RoundType = "prelim";
            round.TournamentId = tournament.TournamentId;
            _context.Add(round);
            _context.SaveChanges();
            return round;
        }
        public Round CreateQuarterFinal(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            Round round = new Round();
            round.RoundNumber = 5;
            round.RoundType = "elim";
            round.TournamentId = tournament.TournamentId;
            _context.Add(round);
            _context.SaveChanges();
            return round;
        }
        public Round CreateSemiFinal(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            Round round = new Round();
            round.RoundNumber = 6;
            round.RoundType = "elim";
            round.TournamentId = tournament.TournamentId;
            _context.Add(round);
            _context.SaveChanges();
            return round;
        }
        public Round CreateFinal(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            Round round = new Round();
            round.RoundNumber = 7;
            round.RoundType = "elim";
            round.TournamentId = tournament.TournamentId;
            _context.Add(round);
            _context.SaveChanges();
            return round;
        }

        public void CreateRoundOneDebate(int id)
        {
            List<Room> rooms = GetRooms(id);
            List<Judge> judges = AssignRoundOneJudges(id);
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
            for(int i = 0; i < entryCount / 2; i++)
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
            List<IndividualTeam> affirmativeTeams = GetRoundOneAffirmativeTeams(id);
            List<IndividualTeam> dueNegTeams = new List<IndividualTeam>();
            List<IndividualTeam> enteredTeams = GetTeams(id);
            for (int i = enteredTeams.Count() - 1; i >= enteredTeams.Count() / 2; i--)
            {
                dueNegTeams.Add(enteredTeams[i]);
            }
            for(int j = 0; j < affirmativeTeams.Count(); j++)
            {
                for(int k = 0; k < dueNegTeams.Count(); k++)
                {
                    if(affirmativeTeams[j].SchoolId != dueNegTeams[k].SchoolId)
                    {
                        negativeTeams.Add(dueNegTeams[k]);
                        dueNegTeams.Remove(dueNegTeams[k]);
                        break;
                    }
                }
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
        public IActionResult PairRoundThree(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            List<Room> rooms = GetRooms(id);
            List<IndividualTeam> sortedTeams = SortTeamsByRecord(id);
            List<IndividualTeam> finalOrder = SortRoundThreeTeams(sortedTeams, id);
            List<IndividualTeam> affirmativeTeams = new List<IndividualTeam>();
            List<IndividualTeam> negativeTeams = new List<IndividualTeam>();
            if (sortedTeams.Count() != 0)
            {
                List<IndividualTeam> sortedAgain = SortTeamsByRecord(id);
                List<IndividualTeam> teamOrder = SortRoundThreeTeams(sortedAgain, id);
                for (int i = 0; i < finalOrder.Count() / 2; i++)
                {
                    affirmativeTeams.Add(finalOrder[i]);
                }
                for (int i = finalOrder.Count()/2; i < finalOrder.Count(); i++)
                {
                    negativeTeams.Add(finalOrder[i]);
                }
            }
            else
            {
                for (int i = 0; i < finalOrder.Count() / 2; i++)
                {
                    affirmativeTeams.Add(finalOrder[i]);
                }
                for (int i = finalOrder.Count()/2; i < finalOrder.Count(); i++)
                {
                    negativeTeams.Add(finalOrder[i]);
                }
            }
            List<Judge> judges = AssignRoundFourJudges(affirmativeTeams, negativeTeams, id);
            PushRoundThreePairing(id, affirmativeTeams, negativeTeams, judges, rooms);

            PairingsTabulationViewModel pairingVM = new PairingsTabulationViewModel()
            {
                Tournament = tournament,
                Rooms = rooms,
                AffirmativeTeams = affirmativeTeams,
                NegativeTeams = negativeTeams,
                Judges = judges,
            };
            return View(pairingVM);
        }

        public void PushRoundThreePairing(int id, List<IndividualTeam> affTeams, List<IndividualTeam> negTeams, List<Judge> judges, List<Room> rooms)
        {
            Round round = CreateRoundThree(id);
            CreateRoundTwoDebate(id, affTeams, negTeams, judges, rooms);
            List<Pairing> pairings = CreateRoundTwoPairing(id, affTeams, negTeams, judges, round);
            CreateRoundTwoBallots(id, pairings);
        }
        public List<Judge> AssignRoundThreeJudges(List<IndividualTeam> affTeams, List<IndividualTeam> negTeams, int id)
        {
            List<Judge> assignedJudges = new List<Judge>();
            List<Judge> judges = GetJudges(id);
            for (int i = 0; i < affTeams.Count(); i++)
            {
                foreach (var judge in judges)
                {
                    if (judge.SchoolId != affTeams[i].SchoolId && judge.SchoolId != negTeams[i].SchoolId)
                    {
                        List<Pairing> affTeamPairings = _context.Pairing.Where(p => p.NegativeTeamId == affTeams[i].IndividualTeamId).ToList();
                        List<Pairing> negTeamPairings = _context.Pairing.Where(p => p.AffirmativeTeamId == negTeams[i].IndividualTeamId).ToList();
                        if (judge.JudgeId != affTeamPairings[0].JudgeId && judge.JudgeId != negTeamPairings[0].JudgeId)
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

        

        public List<IndividualTeam> SortTeamsByRecord(int id)
        {
            List<IndividualTeam> tournamentTeams = GetTeams(id);
            List<IndividualTeam> sort = tournamentTeams.OrderByDescending(t => t.SingleTournamentWins).ThenByDescending(t => t.SingleTournamentSpeakerPoints).ToList();
            return sort;
        }

        public List<IndividualTeam> SortRoundThreeTeams(List<IndividualTeam> sortedTeams, int id)
        {
            List<IndividualTeam> teams = sortedTeams;
            List<IndividualTeam> affTeams = new List<IndividualTeam>();
            List<IndividualTeam> negTeams = new List<IndividualTeam>();
            List<IndividualTeam> finalOrder = new List<IndividualTeam>();
            List<Pairing> tournamentPairings = _context.Pairing.Where(t => t.TournamentId == id).ToList();
            for(int i = 0; i < teams.Count(); i++)
            {
                var pairings = tournamentPairings.Where(t => t.AffirmativeTeamId == teams[i].IndividualTeamId || t.NegativeTeamId == teams[i].IndividualTeamId).ToList();
                for(int j = i + 1; j < teams.Count(); j++)
                {
                    for(int k = 0; k < pairings.Count(); k++)
                    {
                        if(pairings[k].AffirmativeTeamId == teams[j].IndividualTeamId || pairings[k].NegativeTeamId == teams[j].IndividualTeamId || teams[i].SchoolId == teams[j].SchoolId)
                        {
                            break;
                        }
                        else if((pairings.Count() - 1) == k)
                        {
                            affTeams.Add(teams[i]);
                            negTeams.Add(teams[j]);
                            teams.Remove(teams[i]);
                            teams.Remove(teams[j-1]);
                            j = 0;
                            break;
                        }
                    }
                    if(j == 0)
                    {
                        i = -1;
                        break;
                    }
                }
            }           
            foreach(var team in affTeams)
            {
                finalOrder.Add(team);
            }
            foreach(var team in negTeams)
            {
                finalOrder.Add(team);
            }
            return finalOrder;
        }


        public List<IndividualTeam> SortBySpeaks(List<IndividualTeam> teams)
        {
            List<IndividualTeam> sortedList = teams.OrderByDescending(t => t.SingleTournamentSpeakerPoints).ToList();
            return sortedList;
        }

        public bool CheckBallotStatus(int id)
        {
            List<Round> rounds = _context.Round.Where(r => r.TournamentId == id).ToList();
            List<Round> sortedRounds = rounds.OrderByDescending(r => r.RoundNumber).ToList();
            Round round = sortedRounds[0];
            List<Ballot> ballots = _context.Ballot.Where(b => b.RoundId == round.RoundId).ToList();
            List<Ballot> turnedInBallots = new List<Ballot>();
            foreach(var ballot in ballots)
            {
                if(ballot.BallotTurnedIn == true)
                {
                    turnedInBallots.Add(ballot);
                }
            }
            if(ballots.Count() == turnedInBallots.Count())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public IActionResult PairRoundTwo(int id)
        {

            if (CheckBallotStatus(id) == true)
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
            else
            {
                return RedirectToAction("Pairings", "BallotsIncomplete");
            }
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
                ballot.BallotTurnedIn = false;
                _context.Add(ballot);
                _context.SaveChanges();
            }
        }
        

        public void PushRoundTwoPairing(int id, List<IndividualTeam> affTeams, List<IndividualTeam> negTeams, List<Judge> judges, List<Room> rooms)
        {
            Round round = CreateRoundTwo(id);
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
                if (pairing.AffirmativeTeamId != team.IndividualTeamId && roundTwoAffTeams[i].SchoolId != team.SchoolId)
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
                        if (pairing2.AffirmativeTeamId != team.IndividualTeamId && roundTwoAffTeams[i].SchoolId != team2.SchoolId)
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

        public IActionResult PairRoundFour(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            List<IndividualTeam> allTeams = GetTeams(id);
            List<Room> rooms = GetRooms(id);
            List<IndividualTeam> affTeams = new List<IndividualTeam>();
            List<IndividualTeam> negTeams = new List<IndividualTeam>();
            List<IndividualTeam> orderedTeams = MatchRoundFourTeams(id);
            for (int i = 0; i < orderedTeams.Count() / 2; i++)
            {
                affTeams.Add(orderedTeams[i]);
            }
            for(int j = orderedTeams.Count()/2; j < orderedTeams.Count(); j++)
            {
                negTeams.Add(orderedTeams[j]);
            }
            List<Judge> judges = AssignRoundFourJudges(affTeams, negTeams, id);
            PushRoundFourPairing(id, affTeams, negTeams, judges, rooms);

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
        public void PushRoundFourPairing(int id, List<IndividualTeam> affTeams, List<IndividualTeam> negTeams, List<Judge> judges, List<Room> rooms)
        {
            Round round = CreateRoundFour(id);
            CreateRoundTwoDebate(id, affTeams, negTeams, judges, rooms);
            List<Pairing> pairings = CreateRoundTwoPairing(id, affTeams, negTeams, judges, round);
            CreateRoundTwoBallots(id, pairings);
        }
        public List<Judge> AssignRoundFourJudges(List<IndividualTeam> affTeams, List<IndividualTeam> negTeams, int id)
        {
            List<Judge> assignedJudges = new List<Judge>();
            List<Judge> judges = GetJudges(id);
            for(int i = 0; i < affTeams.Count(); i++)
            {
                List<Pairing> affPairings = _context.Pairing.Where(p => p.AffirmativeTeamId == affTeams[i].IndividualTeamId || p.NegativeTeamId == affTeams[i].IndividualTeamId).ToList();
                List<Pairing> negPairings = _context.Pairing.Where(p => p.AffirmativeTeamId == negTeams[i].IndividualTeamId || p.NegativeTeamId == negTeams[i].IndividualTeamId).ToList();
                var count = 0; 
                for (int j = 0; j < judges.Count(); j++)
                {
                    count++;
                    if (judges[j].SchoolId != affTeams[i].SchoolId && judges[j].SchoolId != negTeams[i].SchoolId)
                    {
                        for(int k = 0; k < affPairings.Count(); k++)
                        {
                            if(judges[j].JudgeId == affPairings[k].JudgeId || judges[j].JudgeId == negPairings[k].JudgeId)
                            {
                                break;
                            }
                            else if(k == (affPairings.Count() - 1))
                            {
                                assignedJudges.Add(judges[j]);
                                judges.Remove(judges[j]);
                                j = 0;
                                count = 0;
                                break;
                            }
                        }
                    }
                    if (j == 0 && count == 0)
                    {
                        break;
                    }
                }
                
            }
            return assignedJudges;
        }
        
        public void PushPrelimResults(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            List<IndividualTeam> teams = GetTeams(id);
            foreach(var team in teams)
            {
                TournamentResults result = new TournamentResults();
                result.IndividualTeamId = team.IndividualTeamId;
                result.TeamWins = team.SingleTournamentWins;
                result.TeamLosses = team.SingleTournamentLosses;
                int speakerPoints = Convert.ToInt32(team.SingleTournamentSpeakerPoints);
                result.SpeakerPoints = speakerPoints;
                _context.Add(result);
                _context.SaveChanges();
            }
        }

        public List<Debater> SpeakerAwardsList(int id)
        {
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            List<IndividualTeam> teams = GetTeams(id);
            List<Debater> debaters = new List<Debater>();
            List<Debater> speakerAwards = new List<Debater>();
            int speakerAwardThreshold = 10;
            foreach(var team in teams)
            {
                Debater debater1 = _context.Debater.FirstOrDefault(d => d.DebaterId == team.FirstSpeakerId);
                Debater debater2 = _context.Debater.FirstOrDefault(d => d.DebaterId == team.SecondSpeakerId);
                debaters.Add(debater1);
                debaters.Add(debater2);
            }
            List<Debater> speakerRankings = debaters.OrderByDescending(d => d.IndividualTournamentSpeakerPoints).ToList();
            for(int i = 0; i < speakerAwardThreshold; i++)
            {
                speakerAwards.Add(speakerRankings[i]);
            }
            return speakerAwards;
        }
        public List<IndividualTeam> SortRoundFourNegTeams(int id)
        {
            List<IndividualTeam> dueNegTeams = FindDueNegTeams(id);
            List<IndividualTeam> sortedNegTeams = dueNegTeams.OrderByDescending(s => s.SingleTournamentWins).ThenByDescending(s => s.SingleTournamentSpeakerPoints).ToList();
            return sortedNegTeams;
        }

        public List<IndividualTeam> SortRoundFourAffTeams(int id)
        {
            List<IndividualTeam> dueAffTeams = FindDueAffTeams(id);
            List<IndividualTeam> sortedAffTeams = dueAffTeams.OrderByDescending(s => s.SingleTournamentWins).ThenByDescending(s => s.SingleTournamentSpeakerPoints).ToList();
            return sortedAffTeams;
        }
        public List<IndividualTeam> MatchRoundFourTeams(int id)
        {
            List<IndividualTeam> allTeams = GetTeams(id);
            List<IndividualTeam> dueAffTeams = SortRoundFourAffTeams(id);
            List<IndividualTeam> dueNegTeams = SortRoundFourNegTeams(id);
            List<IndividualTeam> affTeams = new List<IndividualTeam>();
            List<IndividualTeam> negTeams = new List<IndividualTeam>();
            List<IndividualTeam> finalOrder = new List<IndividualTeam>();
            List<Pairing> pairings = _context.Pairing.Where(p => p.TournamentId == id).ToList();
            for(int i = 0; i < dueAffTeams.Count(); i++)
            {
                var teamPairings = pairings.Where(p => p.AffirmativeTeamId == dueAffTeams[i].IndividualTeamId || p.NegativeTeamId == dueAffTeams[i].IndividualTeamId).ToList();
                var count = 0;
                for (int j = 0; j < dueNegTeams.Count(); j++)
                {
                    for(int k = 0; k < teamPairings.Count(); k++)
                    {
                        if (teamPairings[k].AffirmativeTeamId == dueNegTeams[j].IndividualTeamId || teamPairings[k].NegativeTeamId == dueNegTeams[j].IndividualTeamId || dueAffTeams[i].SchoolId == dueNegTeams[j].SchoolId)
                        {
                            count++;
                            break;
                        }
                        else if(k == (teamPairings.Count() - 1))
                        {
                            affTeams.Add(dueAffTeams[i]);
                            negTeams.Add(dueNegTeams[j]);
                            dueNegTeams.Remove(dueNegTeams[j]);
                            j = 0;
                            count = 0;
                            break;
                        }
                    }
                    if (j == 0 && count == 0)
                    {
                        break;
                    }
                }
            }
            foreach(var team in affTeams)
            {
                finalOrder.Add(team);
            }
            foreach(var team in negTeams)
            {
                finalOrder.Add(team);
            }
            if (allTeams.Count() != finalOrder.Count())
            {
                List<IndividualTeam> newNegList = MatchRoundFourPlease(id);
                List<IndividualTeam> newAffList = SortRoundFourAffTeams(id);
                List<IndividualTeam> anotherOrder = new List<IndividualTeam>();
                foreach (var team in newAffList)
                {
                    anotherOrder.Add(team);
                }
                foreach (var team in newNegList)
                {
                    anotherOrder.Add(team);
                }
                if (anotherOrder.Count() != allTeams.Count())
                {
                    List<IndividualTeam> newerNegList = MatchRoundFourPlease(id);
                    List<IndividualTeam> newerAffList = SortRoundFourAffTeams(id);
                    List<IndividualTeam> differentOrder = new List<IndividualTeam>();
                    foreach (var team in newerAffList)
                    {
                        differentOrder.Add(team);
                    }
                    foreach (var team in newerNegList)
                    {
                        differentOrder.Add(team);
                    }
                    return differentOrder;
                }
                else
                {
                    return anotherOrder;
                }
            }
            else
            {
                return finalOrder;
            }
        }
        public List<IndividualTeam> MatchRoundFourPlease(int id)
        {
            List<IndividualTeam> dueAffTeams = SortRoundFourAffTeams(id);
            List<IndividualTeam> dueNegTeams = SortRoundFourNegTeams(id);
            List<IndividualTeam> negTeams = new List<IndividualTeam>();
            int count = 0;
            for (int i = 0; i < dueAffTeams.Count(); i++)
            {
                var random = new Random();
                int index = random.Next(dueNegTeams.Count);
                var team = dueNegTeams[index];
                List<Pairing> affPairings = _context.Pairing.Where(p => p.TournamentId == id && (p.AffirmativeTeamId == dueAffTeams[i].IndividualTeamId || p.NegativeTeamId == dueAffTeams[i].IndividualTeamId)).ToList();
                for(int j = 0; j < affPairings.Count(); j++)
                {
                    if (affPairings[j].AffirmativeTeamId == team.IndividualTeamId || affPairings[j].NegativeTeamId == team.IndividualTeamId || dueAffTeams[i].SchoolId == team.SchoolId)
                    {
                        i -= 1;
                        break;
                    }
                    else if(j == (affPairings.Count() - 1))
                    {
                        negTeams.Add(team);
                        dueNegTeams.Remove(team);
                    }
                }
                count++;
                if(count == (dueAffTeams.Count() * 5))
                {
                    return negTeams;
                }
            }
            return negTeams;
        }


        public List<IndividualTeam> MatchRoundFourDifferentMethod(int id)
        {
            List<IndividualTeam> dueAffTeams = SortRoundFourAffTeams(id);
            List<IndividualTeam> dueNegTeams = SortRoundFourNegTeams(id);
            List<IndividualTeam> negTeams = new List<IndividualTeam>();
            List<IndividualTeam> finalOrder = new List<IndividualTeam>();
            List<Pairing> pairings = _context.Pairing.Where(p => p.TournamentId == id).ToList();
            int i = 0;
            int j = 0;
            while (negTeams.Count() < dueAffTeams.Count())
            {
                List<Pairing> affPairings = _context.Pairing.Where(p => p.TournamentId == id && (p.AffirmativeTeamId == dueAffTeams[i].IndividualTeamId || p.NegativeTeamId == dueAffTeams[i].IndividualTeamId)).ToList();
                var random = new Random();
                int index = random.Next(dueNegTeams.Count);
                var team = dueNegTeams[index];
                for(int k = 0; k < affPairings.Count(); k++)
                {
                    if(j == dueNegTeams.Count)
                    {
                        j -= 1;
                    }
                    if (affPairings[k].AffirmativeTeamId == dueNegTeams[j].IndividualTeamId || affPairings[j].NegativeTeamId == dueNegTeams[j].IndividualTeamId || dueAffTeams[i].SchoolId == dueNegTeams[j].SchoolId)
                    {
                        j++;
                        break;
                    }
                    else if(k == (affPairings.Count() - 1))
                    {
                        negTeams.Add(dueNegTeams[j]);
                        dueNegTeams.Remove(dueNegTeams[j]);
                        i++;
                        j = 0;
                    }
                }
            }
            return negTeams;
        }
       
        public IActionResult DetermineNumberOfElimRounds(int id)
        {
            int quarterFinalsThreshold = 16;
            string quarters = "quarters";
            int semiFinalsThreshold = 8;
            string semis = "semis";
            int numberOfTeams = GetTeams(id).Count();
            if (numberOfTeams > quarterFinalsThreshold)
            {
                PairingsTabulationViewModel pairingVM = PairFirstOutRound(id, quarterFinalsThreshold, quarters);
                return View(pairingVM);
            }
            else
            {
                PairingsTabulationViewModel pairingVM = PairFirstOutRound(id, semiFinalsThreshold, semis);
                return View(pairingVM);
            }
            return View();
        }

        public IActionResult PairSecondElimRound(int id)
        {
            var semiFinalsThreshold = 8;
            int teamEntries = GetTeams(id).Count();
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            List<Room> rooms = GetRooms(id);
            List<IndividualTeam> affTeams = new List<IndividualTeam>();
            List<IndividualTeam> negTeams = new List<IndividualTeam>();
            List<IndividualTeam> judges = new List<IndividualTeam>();
            if(teamEntries / 2 > semiFinalsThreshold)
            {
                Round previousRound = _context.Round.Where(r => r.TournamentId == id && r.RoundNumber == 5).Single();
                List<Pairing> previousPairings = _context.Pairing.Where(p => p.RoundId == previousRound.RoundId).ToList();
                List<IndividualTeam> quartersWinners = new List<IndividualTeam>();
                foreach (var pairing in previousPairings)
                {
                    IndividualTeam team = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == pairing.WinnerId);
                    quartersWinners.Add(team);
                }
                List<IndividualTeam> sortedTeams = PairSemifinals(id);
                for(int i = 0; i < sortedTeams.Count(); i++)
                {
                    affTeams.Add(sortedTeams[i]);
                    negTeams.Add(sortedTeams[i + 1]);
                    sortedTeams.Remove(sortedTeams[i]);
                    sortedTeams.Remove(sortedTeams[i]);
                    i = -1;
                }
                List<Judge> semisJudges = AssignOutRoundJudges(affTeams, negTeams, id, semiFinalsThreshold);
                PushOutRoundPairing(id, quartersWinners, affTeams, negTeams, semisJudges, rooms);

                PairingsTabulationViewModel pairingVM = new PairingsTabulationViewModel()
                {
                    Tournament = tournament,
                    Rooms = rooms,
                    AffirmativeTeams = affTeams,
                    NegativeTeams = negTeams,
                    Judges = semisJudges,
                };
                return View(pairingVM);
            }
            else
            {
                Round previousRound = _context.Round.Where(r => r.TournamentId == id && r.RoundNumber == 6).Single();
                List<Pairing> previousPairings = _context.Pairing.Where(p => p.RoundId == previousRound.RoundId).ToList();
                List<IndividualTeam> semisWinners = new List<IndividualTeam>();
                foreach (var pairing in previousPairings)
                {
                    IndividualTeam team = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == pairing.WinnerId);
                    semisWinners.Add(team);
                }
                List<IndividualTeam> sortedTeams = PairFinals(id);
                for (int i = 0; i < sortedTeams.Count(); i++)
                {
                    affTeams.Add(sortedTeams[i]);
                    negTeams.Add(sortedTeams[i + 1]);
                    sortedTeams.Remove(sortedTeams[i]);
                    sortedTeams.Remove(sortedTeams[i]);
                    i = -1;
                }
                List<Judge> finalsJudges = AssignOutRoundJudges(affTeams, negTeams, id, semiFinalsThreshold);
                PushOutRoundPairing(id, semisWinners, affTeams, negTeams, finalsJudges, rooms);

                PairingsTabulationViewModel pairingVM = new PairingsTabulationViewModel()
                {
                    Tournament = tournament,
                    Rooms = rooms,
                    AffirmativeTeams = affTeams,
                    NegativeTeams = negTeams,
                    Judges = finalsJudges,
                };
                return View(pairingVM);
            }
        }

        public List<IndividualTeam> PairSemifinals(int id)
        {
            Round previousRound = _context.Round.Where(r => r.TournamentId == id && r.RoundNumber == 5).Single();
            List<Pairing> previousPairings = _context.Pairing.Where(p => p.RoundId == previousRound.RoundId).ToList();
            List<IndividualTeam> quartersWinners = new List<IndividualTeam>();
            foreach(var pairing in previousPairings)
            {
                IndividualTeam team = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == pairing.WinnerId);
                quartersWinners.Add(team);
            }
            List<IndividualTeam> sortedWinners = quartersWinners.OrderByDescending(q => q.SingleTournamentWins).ThenByDescending(q => q.SingleTournamentSpeakerPoints).ToList();
            List<IndividualTeam> highSeeds = new List<IndividualTeam>();
            List<IndividualTeam> lowSeeds = new List<IndividualTeam>();
            for(int i = 0; i < sortedWinners.Count() / 2; i++)
            {
                highSeeds.Add(sortedWinners[i]);
            }
            for(int j = sortedWinners.Count() -1; j >= sortedWinners.Count() / 2; j--)
            {
                lowSeeds.Add(sortedWinners[j]);
            }
            List<IndividualTeam> sortedTeams = new List<IndividualTeam>();
            for(int k = 0; k < highSeeds.Count(); k++)
            {
                List<Pairing> highSeedPairings = _context.Pairing.Where(p => p.TournamentId == id && (p.AffirmativeTeamId == highSeeds[k].IndividualTeamId || p.NegativeTeamId == highSeeds[k].IndividualTeamId)).ToList();
                for(int l = 0; l < highSeedPairings.Count(); l++)
                {
                    if(highSeedPairings[l].AffirmativeTeamId == lowSeeds[k].IndividualTeamId)
                    {
                        sortedTeams.Add(highSeeds[k]);
                        sortedTeams.Add(lowSeeds[k]);
                        break;
                    }
                    else if(highSeedPairings[l].NegativeTeamId == lowSeeds[k].IndividualTeamId)
                    {
                        sortedTeams.Add(lowSeeds[k]);
                        sortedTeams.Add(highSeeds[k]);
                        break;
                    }
                    else if(k == (highSeedPairings.Count() - 1))
                    {
                        sortedTeams.Add(highSeeds[k]);
                        sortedTeams.Add(lowSeeds[k]);
                        break;
                    }
                }
            }
            return sortedTeams;
        }
        public List<IndividualTeam> PairFinals(int id)
        {
            Round previousRound = _context.Round.Where(r => r.TournamentId == id && r.RoundNumber == 6).Single();
            List<Pairing> previousPairings = _context.Pairing.Where(r => r.RoundId == previousRound.RoundId).ToList();
            List<IndividualTeam> semisWinners = new List<IndividualTeam>();
            foreach(var pairing in previousPairings)
            {
                IndividualTeam team = _context.IndividualTeam.FirstOrDefault(i => i.IndividualTeamId == pairing.WinnerId);
                semisWinners.Add(team);
            }
            List<IndividualTeam> sortedTeams = new List<IndividualTeam>();
            for(int i = 0; i < semisWinners.Count(); i++)
            {
                List<Pairing> pairings = _context.Pairing.Where(p => p.TournamentId == id && (p.AffirmativeTeamId == semisWinners[i].IndividualTeamId || p.NegativeTeamId == semisWinners[i].IndividualTeamId)).ToList();
                for(int j = 0; j < pairings.Count(); j++)
                {
                    if(pairings[j].AffirmativeTeamId == semisWinners[i + 1].IndividualTeamId)
                    {
                        sortedTeams.Add(semisWinners[i]);
                        sortedTeams.Add(semisWinners[i + 1]);
                        break;
                    }
                    else if(pairings[j].NegativeTeamId == semisWinners[i + 1].IndividualTeamId)
                    {
                        sortedTeams.Add(semisWinners[i + 1]);
                        sortedTeams.Add(semisWinners[i]);
                        break; 
                    }
                    else if(j == (pairings.Count() - 1))
                    {
                        sortedTeams.Add(semisWinners[i]);
                        sortedTeams.Add(semisWinners[i + 1]);
                        break;
                    }
                }
                break;
            }
            return sortedTeams;
        }

        public List<IndividualTeam> GetPrelimFinalStandings(int id)
        {
            List<IndividualTeam> teams = GetTeams(id);
            List<IndividualTeam> finalStandings = teams.OrderByDescending(t => t.SingleTournamentWins).ThenByDescending(t => t.SingleTournamentSpeakerPoints).ToList();
            return finalStandings;
        }

        public PairingsTabulationViewModel PairFirstOutRound(int id, int outRoundThreshold, string outRoundTitle)
        {
            PushPrelimResults(id);
            Tournament tournament = _context.Tournament.FirstOrDefault(t => t.TournamentId == id);
            List<Room> rooms = GetRooms(id);
            List<IndividualTeam> finalStandings = GetPrelimFinalStandings(id);
            List<IndividualTeam> allTeams = GetTeams(id);
            List<IndividualTeam> outRoundsTeams = new List<IndividualTeam>();
            List<IndividualTeam> nonOutRoundTeams = new List<IndividualTeam>();
            for(int i = 0; i < outRoundThreshold / 2; i++)
            {
                outRoundsTeams.Add(finalStandings[i]);
            }
            for(int k = 0; k < allTeams.Count(); k++)
            {
                for(int l = 0; l < outRoundsTeams.Count(); l++)
                {
                    if(allTeams[k].IndividualTeamId == outRoundsTeams[l].IndividualTeamId)
                    {
                        break;
                    }
                    else if(l == (outRoundsTeams.Count() - 1))
                    {
                        nonOutRoundTeams.Add(allTeams[k]);
                    }
                }
            }
            foreach(var team in nonOutRoundTeams)
            {
                TournamentResults result = _context.TournamentResults.Where(t => t.TournamentId == id && t.IndividualTeamId == team.IndividualTeamId).Single();
                team.SingleTournamentSpeakerPoints = 0;
                team.SingleTournamentWins = 0;
                team.SingleTournamentLosses = 0;
                result.EliminationRoundResult = "none";
                _context.Update(team);
                _context.Update(result);
                _context.SaveChanges();
            }
            foreach(var team in outRoundsTeams)
            {
                TournamentResults result = _context.TournamentResults.Where(t => t.TournamentId == id && t.IndividualTeamId == team.IndividualTeamId).Single();
                result.EliminationRoundResult = outRoundTitle;
                _context.Update(result);
                _context.SaveChanges();
            }
            List<IndividualTeam> seedingOrder = SeedOutrounds(outRoundsTeams);
            List<IndividualTeam> affTeams = new List<IndividualTeam>();
            List<IndividualTeam> negTeams = new List<IndividualTeam>();
            for(int j = 0; j < seedingOrder.Count(); j++)
            {
                affTeams.Add(seedingOrder[j]);
                negTeams.Add(seedingOrder[j + 1]);
                seedingOrder.Remove(seedingOrder[j]);
                seedingOrder.Remove(seedingOrder[j]);
                j = -1;
            }
            List<Judge> judges = AssignOutRoundJudges(affTeams, negTeams, id, outRoundThreshold);
            PushOutRoundPairing(id, outRoundsTeams, affTeams, negTeams, judges, rooms);

            PairingsTabulationViewModel pairingVM = new PairingsTabulationViewModel()
            {
                Tournament = tournament,
                Rooms = rooms,
                AffirmativeTeams = affTeams,
                NegativeTeams = negTeams,
                Judges = judges,
            };
            return pairingVM;
        }


        public void PushOutRoundPairing(int id, List<IndividualTeam> outRoundTeamsLeft, List<IndividualTeam> affTeams, List<IndividualTeam> negTeams, List<Judge> judges, List<Room> rooms)
        {
            if(outRoundTeamsLeft.Count() == 8)
            {
                Round round = CreateQuarterFinal(id);
                CreateRoundTwoDebate(id, affTeams, negTeams, judges, rooms);
                List<Pairing> pairing = CreateRoundTwoPairing(id, affTeams, negTeams, judges, round);
                CreateRoundTwoBallots(id, pairing);
                UpdateElimAppearances(affTeams, negTeams);
            }
            else if(outRoundTeamsLeft.Count() == 4)
            {
                Round round = CreateSemiFinal(id);
                CreateRoundTwoDebate(id, affTeams, negTeams, judges, rooms);
                List<Pairing> pairing = CreateRoundTwoPairing(id, affTeams, negTeams, judges, round);
                CreateRoundTwoBallots(id, pairing);
                UpdateElimAppearances(affTeams, negTeams);
            }
            else if(outRoundTeamsLeft.Count() == 2)
            {
                Round round = CreateFinal(id);
                CreateRoundTwoDebate(id, affTeams, negTeams, judges, rooms);
                List<Pairing> pairing = CreateRoundTwoPairing(id, affTeams, negTeams, judges, round);
                CreateRoundTwoBallots(id, pairing);
                UpdateElimAppearances(affTeams, negTeams);
            }
        }

        public void UpdateElimAppearances(List<IndividualTeam> affTeams, List<IndividualTeam> negTeams)
        {
            foreach(var team in affTeams)
            {
                team.AnnualEliminationRoundAppearances++;
                _context.Update(team);
                _context.SaveChanges();
            }
            foreach (var team in negTeams)
            {
                team.AnnualEliminationRoundAppearances++;
                _context.Update(team);
                _context.SaveChanges();
            }

        }

        public List<Judge> AssignOutRoundJudges(List<IndividualTeam> affTeams, List<IndividualTeam> negTeams, int id, int outroundTeams)
        {
            List<Judge> allJudges = GetJudges(id);
            List<Judge> judges = new List<Judge>();
            for(int i = 0; i < affTeams.Count(); i++)
            {
                if(judges.Count() == outroundTeams / 2)
                {
                    break;
                }
                for (int j = 0; j < allJudges.Count(); j++)
                {
                    if(allJudges[j].SchoolId != affTeams[i].SchoolId && allJudges[j].SchoolId != negTeams[i].SchoolId)
                    {
                        judges.Add(allJudges[j]);
                        allJudges.Remove(allJudges[j]);
                        break;
                    }
                }
            }
            return judges;
        }

        public List<IndividualTeam> SeedOutrounds(List<IndividualTeam> outRoundsTeams)
        {
            List<IndividualTeam> highSeeds = new List<IndividualTeam>();
            List<IndividualTeam> lowSeeds = new List<IndividualTeam>();
            List<IndividualTeam> affTeams = new List<IndividualTeam>();
            List<IndividualTeam> negTeams = new List<IndividualTeam>();
            List<IndividualTeam> seedingOrder = new List<IndividualTeam>();
            for(int i = 0; i < outRoundsTeams.Count()/2; i++)
            {
                highSeeds.Add(outRoundsTeams[i]);
            }
            for(int j = outRoundsTeams.Count() - 1; j >= outRoundsTeams.Count()/2; j--)
            {
                lowSeeds.Add(outRoundsTeams[j]);
            }
            for(int k = 0; k < highSeeds.Count(); k++)
            {
                var affPairings = _context.Pairing.Where(p => p.AffirmativeTeamId == highSeeds[k].IndividualTeamId || p.NegativeTeamId == highSeeds[k].IndividualTeamId).ToList();
                for(int m = 0; m < affPairings.Count(); m++)
                {
                    if (affPairings[m].AffirmativeTeamId == lowSeeds[k].IndividualTeamId)
                    {
                        negTeams.Add(lowSeeds[k]);
                        affTeams.Add(highSeeds[k]);
                        break;
                    }
                    else if(affPairings[m].NegativeTeamId == lowSeeds[k].IndividualTeamId)
                    {
                        affTeams.Add(lowSeeds[k]);
                        negTeams.Add(highSeeds[k]);
                        break;
                    }
                    else if(m == (affPairings.Count() - 1))
                    {
                        affTeams.Add(highSeeds[k]);
                        negTeams.Add(lowSeeds[k]);
                        break;
                    }
                }
            }
            for(int l = 0; l < affTeams.Count(); l++)
            {
                seedingOrder.Add(affTeams[l]);
                seedingOrder.Add(negTeams[l]);
            }
            return seedingOrder;
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
