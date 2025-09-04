using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public class TopicRequestDto
    {
        public required string Name { get; set; }
        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime CloseDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Legalább egy szavazat kötelző")]
        public uint MinimalVotes { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "Legalább egy szavazat kötelző")]
        public uint MaximalVotes { get; set; } = 1;
        public bool Live { get; set; } = false;

        public List<string> Choices { get; set; } = new();
    }
}
