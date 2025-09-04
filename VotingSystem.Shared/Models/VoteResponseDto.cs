using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public record VoteResponseDto
    {
        public List<string> SelectedChoices { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
