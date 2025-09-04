using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.Shared.SignalR.Models;

namespace VotingSystem.SignalR.Services
{
    public interface ITopicsNotificationService
    {
        Task AddToGroupAsync(string connectionId, string groupName);
        Task RemoveFromGroupAsync(string connectionId, string groupName);
        Task SendVoteUpdateToGroupAsync(int topicId, TopicNotificationDto dto);
    }
}
