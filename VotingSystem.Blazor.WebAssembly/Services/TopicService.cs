using AutoMapper;
using VotingSystem.Blazor.WebAssembly.Exception;
using VotingSystem.Blazor.WebAssembly.Infrastructure;
using VotingSystem.Blazor.WebAssembly.ViewModels;
using VotingSystem.Shared.Models;

namespace VotingSystem.Blazor.WebAssembly.Services
{
    public class TopicService : BaseService, ITopicService
    {
        private readonly IMapper _mapper;
        private readonly IHttpRequestUtility _httpRequestUtility;

        public TopicService(IMapper mapper, IHttpRequestUtility httpRequestUtility)
        {
            _mapper = mapper;
            _httpRequestUtility = httpRequestUtility;
        }

        public async Task CreateTopicAsync(TopicViewModel topic)
        {
            var topicRequestDto = _mapper.Map<TopicRequestDto>(topic);
            try
            {
                await _httpRequestUtility.ExecutePostHttpRequestAsync<TopicRequestDto, TopicResponseDto>("topics", topicRequestDto);
            }
            catch (HttpRequestErrorException exp)
            {
                await HandleError(exp.Response);
            }
        }

        public async Task DeleteTopicAsync(int topicId)
        {
            try
            {
                await _httpRequestUtility.ExecuteDeleteHttpRequestAsync($"topics/{topicId}");
            }
            catch (HttpRequestErrorException exp)
            {
                await HandleError(exp.Response);
            }
        }

        public async Task<TopicViewModel> GetTopicByIdAsync(int topicId)
        {
            try
            {
                var response = await _httpRequestUtility.ExecuteGetHttpRequestAsync<TopicResponseDto>($"/topics/{topicId}");
                return _mapper.Map<TopicViewModel>(response.Response);
            }
            catch (HttpRequestErrorException exp)
            {
                await HandleError(exp.Response);
            }
            return new TopicViewModel();
        }

        public async Task<List<TopicViewModel>> GetTopicsAsync()
        {
            try
            {
                var response = await _httpRequestUtility.ExecuteGetHttpRequestAsync<List<TopicResponseDto>>("topics");
                var topicViewModels = _mapper.Map<List<TopicViewModel>>(response.Response);
                return topicViewModels;
            }
            catch (HttpRequestErrorException exp)
            {
                await HandleError(exp.Response);
            }
            return new();
        }

        public async Task UpdateTopicAsync(TopicViewModel topic)
        {
            try
            {
                var topicRequestDto = _mapper.Map<TopicRequestDto>(topic);
                var resp = await _httpRequestUtility.ExecutePutHttpRequestAsync<TopicRequestDto, TopicResponseDto>($"topics/{topic.Id}", topicRequestDto);
            }
            catch (HttpRequestErrorException exp)
            {
                await HandleError(exp.Response);
            }
        }
    }
}
