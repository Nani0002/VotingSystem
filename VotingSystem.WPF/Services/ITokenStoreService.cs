using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.WPF.Services
{
    public interface ITokenStoreService
    {
        string? AuthToken { get; set; }
        string? RefreshToken { get; set; }
        string? UserId { get; set; }
    }
}
