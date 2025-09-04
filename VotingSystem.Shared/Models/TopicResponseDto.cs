using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public class TopicResponseDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime CloseDate { get; set; }
        public uint MinimalVotes { get; set; } = 1;
        public uint MaximalVotes { get; set; } = 1;
        public bool Live { get; set; } = false;
        public List<string> Choices { get; set; } = new();
        public List<UserResponseDto> Voters { get; set; } = new();
        public List<VoteResponseDto> Votes { get; set; } = new();
    }
}
