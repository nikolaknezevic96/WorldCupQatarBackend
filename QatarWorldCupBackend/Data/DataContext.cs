using Domain.Model;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace QatarWorldCupBackend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Domain.Model.Group> Groups { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Stadium> Stadiums { get; set; }
        public DbSet<Domain.Model.Match> Matches { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Domain.Model.Group>().HasKey(g => g.Id);
            modelBuilder.Entity<Team>().HasKey(t => t.Id);
            modelBuilder.Entity<Stadium>().HasKey(s => s.Id);
            modelBuilder.Entity<Domain.Model.Match>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().HasKey(u => u.Id);

            modelBuilder.Entity<Domain.Model.Group>()
                .HasMany(g => g.Teams)
                .WithOne(t => t.Group)
                .HasForeignKey(t => t.GroupId)
                .IsRequired(false);  // Make group association optional

            modelBuilder.Entity<Domain.Model.Match>()
                .HasOne(u => u.Team1)
                .WithMany()
                .HasForeignKey(u => u.Team1Id)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent team deletion if involved in matches

            modelBuilder.Entity<Domain.Model.Match>()
                .HasOne(u => u.Team2)
                .WithMany()
                .HasForeignKey(u => u.Team2Id)
                .OnDelete(DeleteBehavior.Restrict);
            //aaaa
            modelBuilder.Entity<Domain.Model.Match>(entity =>
            {
                entity.Property(e => e.Team1GoalsScored).IsRequired(false);
                entity.Property(e => e.Team2GoalsScored).IsRequired(false);
                entity.Property(e => e.StadiumId).IsRequired(false);
            });

            modelBuilder.Entity<Stadium>()
                .HasMany(s => s.Matches)
                .WithOne(u => u.Stadium)
                .HasForeignKey(u => u.StadiumId)
                .IsRequired(false);  // Stadium association is optional


            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.FirstName).IsRequired();
                entity.Property(u => u.LastName).IsRequired();
            });
        }
    }
}
