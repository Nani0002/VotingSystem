using VotingSystem.Blazor.WebAssembly.ViewModels;

namespace VotingSystem.Blazor.WebAssembly.Services
{
    public interface ITopicService
    {
        public Task<List<TopicViewModel>> GetTopicsAsync();
        public Task<TopicViewModel> GetTopicByIdAsync(int topicId);
        public Task UpdateTopicAsync(TopicViewModel topic);
        public Task CreateTopicAsync(TopicViewModel topic);
        public Task DeleteTopicAsync(int topicId);
    }
}
