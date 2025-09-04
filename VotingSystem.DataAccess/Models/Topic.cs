using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.DataAccess.Models
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime CloseDate { get; set; }
        public uint MinimalVotes { get; set; } = 1;
        public uint MaximalVotes { get; set; } = 1;
        public bool Live { get; set; } = false;

        [ForeignKey("User")]
        public required string OwnerId { get; set; }
        public virtual ICollection<Vote> Votes { get; set; } = [];
        public virtual ICollection<Choice> Choices { get; set; } = [];
        public virtual ICollection<VoteRecord> VoterRecords { get; set; } = [];

        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
