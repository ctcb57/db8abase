﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using db8abase.Data;

namespace db8abase.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190930231444_TestMigration")]
    partial class TestMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("db8abase.Models.Address", b =>
                {
                    b.Property<int>("AddressId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<string>("StateAbbreviation");

                    b.Property<string>("StreetAddress");

                    b.Property<int>("ZipCode");

                    b.HasKey("AddressId");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("db8abase.Models.Ballot", b =>
                {
                    b.Property<int>("BallotId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("BallotTurnedIn");

                    b.Property<int>("DebateId");

                    b.Property<double>("FirstAffSpeakerPoints");

                    b.Property<double>("FirstNegSpeakerPoints");

                    b.Property<int>("JudgeId");

                    b.Property<string>("ReasonForDecision");

                    b.Property<int>("RoundId");

                    b.Property<double>("SecondAffSpeakerPoints");

                    b.Property<double>("SecondNegSpeakerPoints");

                    b.Property<int>("TournamentId");

                    b.Property<int>("WinnerId");

                    b.HasKey("BallotId");

                    b.ToTable("Ballot");
                });

            modelBuilder.Entity("db8abase.Models.Coach", b =>
                {
                    b.Property<int>("CoachId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ApplicationUserId");

                    b.Property<double>("Balance");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<int>("PhoneNumber");

                    b.Property<int>("SchoolId");

                    b.HasKey("CoachId");

                    b.ToTable("Coach");
                });

            modelBuilder.Entity("db8abase.Models.Debate", b =>
                {
                    b.Property<int>("DebateId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AffirmativeTeamId");

                    b.Property<int>("JudgeId");

                    b.Property<int>("NegativeTeamId");

                    b.Property<int>("RoomId");

                    b.HasKey("DebateId");

                    b.ToTable("Debate");
                });

            modelBuilder.Entity("db8abase.Models.Debater", b =>
                {
                    b.Property<int>("DebaterId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("AnnualAverageSpeakerPoints");

                    b.Property<string>("ApplicationUserId");

                    b.Property<int>("CoachId");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<int>("IndividualTeamId");

                    b.Property<double>("IndividualTournamentSpeakerPoints");

                    b.Property<string>("LastName");

                    b.Property<int>("PartnerId");

                    b.Property<int>("PhoneNumber");

                    b.Property<int>("SchoolId");

                    b.Property<int>("SpeakerPosition");

                    b.HasKey("DebaterId");

                    b.ToTable("Debater");
                });

            modelBuilder.Entity("db8abase.Models.IndividualTeam", b =>
                {
                    b.Property<int>("IndividualTeamId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AnnualEliminationRoundAppearances");

                    b.Property<int>("CoachId");

                    b.Property<int>("CumulativeAnnualElminationRoundWins");

                    b.Property<int>("CumulativeAnnualLosses");

                    b.Property<int>("CumulativeAnnualWins");

                    b.Property<int?>("FirstSpeakerDebaterId");

                    b.Property<int>("FirstSpeakerId");

                    b.Property<string>("IndividualTeamName");

                    b.Property<int>("SchoolId");

                    b.Property<int?>("SecondSpeakerDebaterId");

                    b.Property<int>("SecondSpeakerId");

                    b.Property<int>("SingleTournamentLosses");

                    b.Property<double>("SingleTournamentSpeakerPoints");

                    b.Property<int>("SingleTournamentWins");

                    b.Property<int>("TocBids");

                    b.Property<int>("TournamentAffirmativeRounds");

                    b.Property<int>("TournamentNegativeRounds");

                    b.HasKey("IndividualTeamId");

                    b.HasIndex("FirstSpeakerDebaterId");

                    b.HasIndex("SecondSpeakerDebaterId");

                    b.ToTable("IndividualTeam");
                });

            modelBuilder.Entity("db8abase.Models.Judge", b =>
                {
                    b.Property<int>("JudgeId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ApplicationUserId");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("JudgingPhilosophy");

                    b.Property<string>("LastName");

                    b.Property<int>("PhoneNumber");

                    b.Property<int>("SchoolId");

                    b.HasKey("JudgeId");

                    b.ToTable("Judge");
                });

            modelBuilder.Entity("db8abase.Models.JudgeEntry", b =>
                {
                    b.Property<int>("JudgeEntryId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("JudgeId");

                    b.Property<int>("TournamentId");

                    b.HasKey("JudgeEntryId");

                    b.ToTable("JudgeEntry");
                });

            modelBuilder.Entity("db8abase.Models.Pairing", b =>
                {
                    b.Property<int>("PairingId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AffirmativeTeamId");

                    b.Property<int>("DebateId");

                    b.Property<int>("JudgeId");

                    b.Property<int>("NegativeTeamId");

                    b.Property<int>("RoomId");

                    b.Property<int>("RoundId");

                    b.Property<int>("TournamentId");

                    b.Property<int>("WinnerId");

                    b.HasKey("PairingId");

                    b.ToTable("Pairing");
                });

            modelBuilder.Entity("db8abase.Models.Room", b =>
                {
                    b.Property<int>("RoomId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("RoomNumber");

                    b.Property<int>("SchoolId");

                    b.HasKey("RoomId");

                    b.ToTable("Room");
                });

            modelBuilder.Entity("db8abase.Models.Round", b =>
                {
                    b.Property<int>("RoundId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("RoundNumber");

                    b.Property<string>("RoundType");

                    b.Property<DateTime>("StartTime");

                    b.Property<int>("TournamentId");

                    b.HasKey("RoundId");

                    b.ToTable("Round");
                });

            modelBuilder.Entity("db8abase.Models.School", b =>
                {
                    b.Property<int>("SchoolId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AddressId");

                    b.Property<int>("CoachId");

                    b.Property<string>("Name");

                    b.Property<int>("TournamentDirectorId");

                    b.HasKey("SchoolId");

                    b.HasIndex("AddressId");

                    b.ToTable("School");
                });

            modelBuilder.Entity("db8abase.Models.TeamEntry", b =>
                {
                    b.Property<int>("EntryId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("IndividualTeamId");

                    b.Property<int>("TournamentId");

                    b.HasKey("EntryId");

                    b.ToTable("TeamEntry");
                });

            modelBuilder.Entity("db8abase.Models.Tournament", b =>
                {
                    b.Property<int>("TournamentId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("EntryFee");

                    b.Property<string>("FilePath");

                    b.Property<string>("Name");

                    b.Property<int>("NumberOfEliminationRounds");

                    b.Property<int>("NumberOfRounds");

                    b.Property<int>("SchoolId");

                    b.Property<int>("TeamLimit");

                    b.Property<DateTime>("TournamentDate");

                    b.HasKey("TournamentId");

                    b.HasIndex("SchoolId");

                    b.ToTable("Tournament");
                });

            modelBuilder.Entity("db8abase.Models.TournamentDirector", b =>
                {
                    b.Property<int>("TournamentDirectorId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ApplicationUserId");

                    b.Property<string>("Email");

                    b.Property<string>("FilePath");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("Message");

                    b.Property<int>("SchoolId");

                    b.Property<int>("TournamentId");

                    b.HasKey("TournamentDirectorId");

                    b.ToTable("TournamentDirector");
                });

            modelBuilder.Entity("db8abase.Models.TournamentResults", b =>
                {
                    b.Property<int>("ResultsId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("EliminationRoundResult");

                    b.Property<int>("IndividualTeamId");

                    b.Property<int>("SpeakerPoints");

                    b.Property<int>("TeamLosses");

                    b.Property<int>("TeamWins");

                    b.Property<int>("TournamentId");

                    b.HasKey("ResultsId");

                    b.ToTable("TournamentResults");
                });

            modelBuilder.Entity("db8abase.Models.ApplicationUser", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUser");

                    b.Property<string>("Name");

                    b.Property<string>("Role");

                    b.HasDiscriminator().HasValue("ApplicationUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("db8abase.Models.IndividualTeam", b =>
                {
                    b.HasOne("db8abase.Models.Debater", "FirstSpeaker")
                        .WithMany()
                        .HasForeignKey("FirstSpeakerDebaterId");

                    b.HasOne("db8abase.Models.Debater", "SecondSpeaker")
                        .WithMany()
                        .HasForeignKey("SecondSpeakerDebaterId");
                });

            modelBuilder.Entity("db8abase.Models.School", b =>
                {
                    b.HasOne("db8abase.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("db8abase.Models.Tournament", b =>
                {
                    b.HasOne("db8abase.Models.School", "School")
                        .WithMany()
                        .HasForeignKey("SchoolId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
