using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public class VoteRequestDto
    {
        public int TopicId { get; set; }
        public List<string> SelectedChoices { get; set; } = new();
    }
}
