using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.Shared.SignalR.Models;

namespace VotingSystem.Shared.SignalR.HubInterfaces
{
    public interface ITopicsHub
    {
        Task JoinTopicsGroup(int topicId);
        Task LeaveTopicGroup(int topicId);
        Task VoteStatusChanged(int topicId, TopicNotificationDto topicNotificationDto);
        Task ReceiveVoteUpdate(TopicNotificationDto topicNotificationDto);
    }
}
