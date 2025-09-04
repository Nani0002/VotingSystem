using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public record LoginResponseDto
    {
        public required string UserId { get; init; }
        public required string AuthToken { get; init; }
        public required string RefreshToken { get; init; }

    }
}
