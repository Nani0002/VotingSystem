using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VotingSystem.DataAccess.Models;

namespace VotingSystem.DataAccess
{
    public class VotingSystemDbContext : IdentityDbContext<User, UserRole, string>
    {
        public DbSet<Topic> Topics { get; set; } = null!;
        public DbSet<Vote> Votes { get; set; } = null!;
        public DbSet<Choice> Choices { get; set; } = null!;
        public DbSet<VoteRecord> VoteRecords { get; set; } = null!;

        public VotingSystemDbContext(DbContextOptions<VotingSystemDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vote>()
                .HasMany(v => v.SelectedChoices)
                .WithMany(c => c.Votes)
                .UsingEntity(j => j.ToTable("ChoiceVote"));

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Topic)
                .WithMany(t => t.Votes)
                .HasForeignKey(v => v.TopicId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Choice>()
                .HasOne(c => c.Topic)
                .WithMany(t => t.Choices)
                .HasForeignKey(c => c.TopicId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
