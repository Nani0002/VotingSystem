using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.WPF.Services
{
    public class TokenStoreService : ITokenStoreService
    {
        public string? AuthToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? UserId { get; set; }
    }
}
