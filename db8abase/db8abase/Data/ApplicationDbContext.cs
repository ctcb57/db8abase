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
        public DbSet<Coach> Coach { get; set; }
        public DbSet<Debater> Debater { get; set; }
        public DbSet<Tournament> Tournament { get; set; }
        public DbSet<Team> Team { get; set; }
        public DbSet<Judge> Judge { get; set; }
        public DbSet<IndividualTeam> IndividualTeam { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<TeamEntry> TeamEntry { get; set; }
        public DbSet<JudgeEntry> JudgeEntry { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<db8abase.Models.Pairing> Pairing { get; set; }
    }
}
