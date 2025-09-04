using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.SignalR.Models
{
    public class ChoiceNotificationDto
    {
        public string Value { get; set; } = string.Empty;
        public int VoteCount { get; set; }
    }
}
