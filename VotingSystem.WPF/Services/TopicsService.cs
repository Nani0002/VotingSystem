using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using VotingSystem.Shared.Models;
using VotingSystem.WPF.Exception;
using VotingSystem.WPF.Infrastructure;
using VotingSystem.WPF.ViewModel;

namespace VotingSystem.WPF.Services
{
    public class TopicsService : BaseService, ITopicsService
    {
        private readonly IMapper _mapper;
        private readonly IHttpRequestUtility _httpRequestUtility;
        private readonly IAuthenticationService _authenticationService;
        private readonly HttpClient _httpClient;

        public TopicsService(IMapper mapper, IHttpRequestUtility httpRequestUtility, IAuthenticationService authenticationService, HttpClient httpClient)
        {
            _mapper = mapper;
            _httpRequestUtility = httpRequestUtility;
            _authenticationService = authenticationService;
            _httpClient = httpClient;
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
                HandleError(await exp.Response.Content.ReadAsStringAsync());
            }
            return new();
        }

        public async Task<List<TopicViewModel>> GetTopicsAsync(string? nameFilter = null, DateTime? startDateFilter = null, DateTime? endDateFilter = null, bool onlyOpen = false, bool onlyClosed = false)
        {
            try
            {
                var queryParams = new List<string>();
                if (!string.IsNullOrWhiteSpace(nameFilter))
                    queryParams.Add($"name={Uri.EscapeDataString(nameFilter)}");
                if (startDateFilter.HasValue)
                    queryParams.Add($"startDate={startDateFilter.Value}");
                if (endDateFilter.HasValue)
                    queryParams.Add($"endDate={endDateFilter.Value}");
                if (onlyOpen)
                    queryParams.Add("onlyOpen=true");
                if (onlyClosed)
                    queryParams.Add("onlyClosed=true");

                var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

                var response = await _httpRequestUtility.ExecuteGetHttpRequestAsync<List<TopicResponseDto>>($"topics/all{query}");
                var topicViewModels = _mapper.Map<List<TopicViewModel>>(response.Response);
                return topicViewModels;
            }
            catch (HttpRequestErrorException exp)
            {
                HandleError(await exp.Response.Content.ReadAsStringAsync());
            }

            return new();
        }

        public async Task<bool> VoteForTopicAsync(int topicId, List<ChoiceViewModel> selectedChoices)
        {
            var userId = _authenticationService.GetCurrentlyLoggedInUser()!;

            VoteRequestDto dto = new VoteRequestDto
            {
                TopicId = topicId,
                SelectedChoices = selectedChoices.Select(x => x.Value).ToList()!
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/topics/vote", dto);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
            catch (HttpRequestException exp)
            {
                HandleError($"Ismeretlen hiba történt: {exp.Message}");
                return false;
            }
        }
    }
}
