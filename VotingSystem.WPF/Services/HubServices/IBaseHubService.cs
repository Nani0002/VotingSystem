using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.WPF.Services.HubServices
{
    public interface IBaseHubService
    {
        Task DisconnectHubAsync();
    }
}
