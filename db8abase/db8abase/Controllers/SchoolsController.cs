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

namespace db8abase.Controllers
{
    public class SchoolsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SchoolsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Schools
        public async Task<IActionResult> Index()
        {
            return View(await _context.School.ToListAsync());
        }

        // GET: Schools/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var school = await _context.School
                .FirstOrDefaultAsync(m => m.SchoolId == id);
            if (school == null)
            {
                return NotFound();
            }

            return View(school);
        }
        // GET: ListOfSchools
        public IActionResult GetListOfSchools()
        {
            List<School> listOfSchools = _context.School.ToList();
            return View(listOfSchools);
        }

        // GET: Schools/Create
        public IActionResult Create()
        {
            School school = new School();
            return View(school);
        }


        // POST: Schools/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SchoolId,Name,TournamentDirectorId,CoachId,Address")] School school)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
                var directorRoleId = _context.Roles.FirstOrDefault(r => r.Name == "TournamentDirector");
                var coachRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Coach");
                var currentUserRole = _context.UserRoles.FirstOrDefault(u => u.UserId == currentUserId);
                if(currentUserRole.RoleId == directorRoleId.Id)
                {
                    var currentDirector = _context.TournamentDirector.FirstOrDefault(t => t.ApplicationUserId == currentUserId);
                    school.TournamentDirectorId = currentDirector.TournamentDirectorId;
                    Address address = new Address();
                    address = school.Address;
                    address.Country = "USA";
                    _context.Add(school);
                    _context.Add(address);
                    await _context.SaveChangesAsync();
                    school.AddressId = address.AddressId;
                    _context.Update(school);
                    currentDirector.SchoolId = school.SchoolId;
                    _context.Update(currentDirector);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "TournamentDirectors");
                }
                else if(currentUserRole.RoleId == coachRoleId.Id)
                {
                    var currentCoach = _context.Coach.FirstOrDefault(c => c.ApplicationUserId == currentUserId);
                    school.CoachId = currentCoach.CoachId;
                    Address address = new Address();
                    address = school.Address;
                    address.Country = "USA";
                    _context.Add(school);
                    _context.Add(address);
                    await _context.SaveChangesAsync();
                    school.AddressId = address.AddressId;
                    _context.Update(school);
                    currentCoach.SchoolId = school.SchoolId;
                    _context.Update(currentCoach);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Coaches");
                }
                else
                {
                    return RedirectToAction("GetTournamentListing", "Home");
                }
            }
            return View(school);
        }


        // GET: Schools/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var school = await _context.School.FindAsync(id);
            if (school == null)
            {
                return NotFound();
            }
            return View(school);
        }

        // POST: Schools/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SchoolId,Name,TournamentDirectorId,CoachId")] School school)
        {
            if (id != school.SchoolId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(school);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SchoolExists(school.SchoolId))
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
            return View(school);
        }

        // GET: Schools/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var school = await _context.School
                .FirstOrDefaultAsync(m => m.SchoolId == id);
            if (school == null)
            {
                return NotFound();
            }

            return View(school);
        }

        // POST: Schools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var school = await _context.School.FindAsync(id);
            _context.School.Remove(school);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SchoolExists(int id)
        {
            return _context.School.Any(e => e.SchoolId == id);
        }
    }
}
