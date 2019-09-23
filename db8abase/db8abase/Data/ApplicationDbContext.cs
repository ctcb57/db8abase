using System;
using System.Collections.Generic;
using System.Text;
using db8abase.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace db8abase.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<TournamentDirector> TournamentDirector { get; set; }

        public DbSet<School> School { get; set; }
    }
}
