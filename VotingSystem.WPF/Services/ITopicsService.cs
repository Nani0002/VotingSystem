using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.WPF.ViewModel;

namespace VotingSystem.WPF.Services
{
    public interface ITopicsService
    {
        public Task<List<TopicViewModel>> GetTopicsAsync(string? nameFilter = null, DateTime? startDateFilter = null, DateTime? endDateFilter = null, bool onlyOpen = false, bool onlyClosed = false);
        public Task<TopicViewModel> GetTopicByIdAsync(int topicId);
        public Task<bool> VoteForTopicAsync(int topicId, List<ChoiceViewModel> selectedChoices);
    }
}