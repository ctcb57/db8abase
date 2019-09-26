using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using db8abase.Data;
using System.Security.Claims;

namespace db8abase.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;
        ApplicationDbContext _context;
        public FileUploadController(IHostingEnvironment environment, ApplicationDbContext context)
        {
            _context = context;
            hostingEnvironment = environment;
        }
        [HttpPost("FileUpload")]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            var filePaths = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    //full path to file in temp location
                    var filePath = Path.GetTempFileName();
                    filePaths.Add(filePath);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
                foreach(var file in files)
                {
                    var uploads = Path.Combine(hostingEnvironment.WebRootPath, "Docs");
                    var uniqueFileName = GetUniqueFileName(file.FileName);
                    var fullPath = Path.Combine(uploads, uniqueFileName);
                    file.CopyTo(new FileStream(fullPath, FileMode.Create));
                    var currentUser = this.User.FindFirstValue(ClaimTypes.NameIdentifier).ToString();
                    var currentDirector = _context.TournamentDirector.FirstOrDefault(t => t.ApplicationUserId == currentUser);
                    currentDirector.FilePath = uniqueFileName;
                    _context.Update(currentDirector);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("CreateTournament", "TournamentDirectors");
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                + "_"
                + Guid.NewGuid().ToString().Substring(0, 4)
                + Path.GetExtension(fileName);
        }
    }
}