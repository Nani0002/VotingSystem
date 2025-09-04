using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using VotingSystem.Shared.SignalR.Models;
using VotingSystem.WPF.Infrastructure;
using VotingSystem.WPF.Services;
using VotingSystem.WPF.Services.HubServices;
using VotingSystem.WPF.View;

namespace VotingSystem.WPF.ViewModel
{
    public class TopicsListViewModel : ViewModelBase
    {
        public ObservableCollection<TopicViewModel> Topics { get; set; } = [];

        private readonly ITopicsService _topicsService;
        private readonly IAuthenticationService _authenticationService;
        private readonly NavigationService _navigationService;
        private readonly ITopicHubService _topicHubService;

        private string search;
        private DateTime? startDate;
        private DateTime? endDate;

        public string Search
        {
            get => search;
            set
            {
                search = value;
                OnPropertyChanged(nameof(Search));
            }
        }

        public DateTime? StartDate
        {
            get => startDate;
            set
            {
                startDate = value < DateTime.Now ? value : DateTime.Now;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                endDate = value < DateTime.Now ? value : DateTime.Now;
                OnPropertyChanged(nameof(EndDate));
            }
        }

        public ICommand LogOutCommand { get; set; }
        public ICommand ShowCommand { get; set; }
        public ICommand NowCommand { get; set; }
        public ICommand ListAllCommand { get; set; }
        public ICommand ListOpenCommand { get; set; }
        public ICommand ListClosedCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        /*public TopicsListViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                UserViewModel previewUser = new() { Id = "123", Name = "DesignUser", Email = "email@example.com" };
                TopicViewModel vm = new()
                {
                    Name = "Sample Topic",
                    Description = "This is a design-time topic.",
                    Choices =
                    [
                        new() { Value = "Option 1", VoteCount = 2 },
                        new() { Value = "Option 2", VoteCount = 3 },
                        new() { Value = "Option 3", VoteCount = 0 },
                        new() { Value = "Option 4", VoteCount = 1 },
                    ],
                    StartDate = DateTime.Now,
                    CloseDate = DateTime.Now,
                    Voters = [previewUser],
                    Live = true
                };
                vm.SetCurrentUser("123");
                Topics.Add(vm); 
                Topics.Add(new TopicViewModel
                {
                    Name = "Sample Topic",
                    Description = "This is a design-time topic.",
                    Choices = [],
                    StartDate = DateTime.Now,
                    CloseDate = DateTime.Now,
                    Live = true
                });
                Topics.Add(new TopicViewModel
                {
                    Name = "Sample Topic",
                    Description = "This is a design-time topic.",
                    Choices =
                    [
                        new ChoiceViewModel { Value = "Option 1", VoteCount = 2 },
                        new ChoiceViewModel { Value = "Option 2", VoteCount = 3 },
                        new ChoiceViewModel { Value = "Option 3", VoteCount = 0 },
                        new ChoiceViewModel { Value = "Option 4", VoteCount = 1 },
                    ],
                    StartDate = DateTime.Now,
                    CloseDate = DateTime.Now
                });

                Topics.Add(new TopicViewModel
                {
                    Name = "Sample Topic",
                    Description = "This is a design-time topic.",
                    Choices = [],
                    StartDate = DateTime.Now,
                    CloseDate = DateTime.Now
                });
                Search = "Search";

                StartDate = DateTime.Now;
                EndDate = DateTime.Now;
            }
        }*/

        public TopicsListViewModel(ITopicsService topicsService, IAuthenticationService authenticationService, NavigationService navigationService, ITopicHubService topicHubService)
        {
            _topicsService = topicsService;
            _authenticationService = authenticationService;
            _navigationService = navigationService;
            _topicHubService = topicHubService;

            _topicHubService.OnVoteUpdateReceived += OnVoteUpdateReceived;

            search = "";
            LogOutCommand = new DelegateCommand(_ => OnLogOut());
            ShowCommand = new DelegateCommand(OnShow);
            NowCommand = new DelegateCommand(OnNow);

            ListAllCommand = new DelegateCommand(_ => OnListAll());
            ListOpenCommand = new DelegateCommand(_ => OnListOpen());
            ListClosedCommand = new DelegateCommand(_ => OnListClosed());
            SearchCommand = new DelegateCommand(_ => OnSearch());
        }

        public async Task LoadTopicsAsync()
        {
            List<TopicViewModel> result = await _topicsService.GetTopicsAsync(onlyOpen: true);
            Topics.Clear();
            string currentUserId = _authenticationService.GetCurrentlyLoggedInUser()!;
            foreach (TopicViewModel topic in result)
            {
                topic.SetCurrentUser(currentUserId);
                Topics.Add(topic);

                if (topic.Live && topic.UserVoted)
                {
                    await _topicHubService.StartHubConnectionAsync(topic.Id);
                }
            }
        }

        public async void OnLogOut()
        {
            await _authenticationService.LogoutAsync();
            _navigationService.Navigate<LoginPage>();
        }

        public void OnShow(object? id)
        {
            if (id is int topicId)
            {
                if (Topics.FirstOrDefault(x => x.Id == topicId) is TopicViewModel topic)
                {
                    string? currentUserId = _authenticationService.GetCurrentlyLoggedInUser();
                    if (!topic.Voters.Any(v => v.Id == currentUserId))
                    {
                        _navigationService.Navigate<TopicShowPage, int>(topicId);
                    }
                }
            }
        }

        public void OnNow(object? param)
        {
            string name = param!.ToString()!;
            switch (name)
            {
                case "end":
                    EndDate = DateTime.Now;
                    break;
                case "start":
                    StartDate = DateTime.Now;
                    break;
                default:
                    break;
            }
        }

        public async void OnSearch()
        {
            if (StartDate is null && EndDate is not null || EndDate is null && StartDate is not null)
            {
                MessageWindow w = new MessageWindow("Minden időpontot meg kell adni idő alapú kereséshez!");
                w.Show();
            }
            else
            {
                List<TopicViewModel> result = await _topicsService.GetTopicsAsync(Search, StartDate, EndDate);
                UpdateTopics(result);
            }
        }

        public async void OnListAll()
        {
            Search = "";
            StartDate = null;
            EndDate = null;

            List<TopicViewModel> result = await _topicsService.GetTopicsAsync();
            UpdateTopics(result);
        }

        public async void OnListOpen()
        {
            Search = "";
            StartDate = null;
            EndDate = null;

            List<TopicViewModel> result = await _topicsService.GetTopicsAsync(onlyOpen: true);
            UpdateTopics(result);
        }

        public async void OnListClosed()
        {
            Search = "";
            StartDate = null;
            EndDate = null;

            List<TopicViewModel> result = await _topicsService.GetTopicsAsync(onlyClosed: true);
            UpdateTopics(result);
        }

        private void UpdateTopics(List<TopicViewModel> topics)
        {
            Topics.Clear();
            string currentUserId = _authenticationService.GetCurrentlyLoggedInUser()!;
            foreach (TopicViewModel topic in topics)
            {
                topic.SetCurrentUser(currentUserId);
                Topics.Add(topic);
            }
        }

        public void Dispose()
        {
            _topicHubService.OnVoteUpdateReceived -= OnVoteUpdateReceived;
            foreach (TopicViewModel topic in Topics)
            {
                _ = _topicHubService.DisconnectHubAsync(topic.Id);
            }
        }

        private void OnVoteUpdateReceived(TopicNotificationDto dto)
        {
            TopicViewModel? topic = Topics.FirstOrDefault(t => t.Id == dto.Id);
            if (topic == null)
            {
                return;
            }

            foreach (ChoiceNotificationDto updatedChoice in dto.Choices)
            {
                ChoiceViewModel? localChoice = topic.Choices.FirstOrDefault(c => c.Value == updatedChoice.Value);
                if (localChoice != null)
                {
                    localChoice.VoteCount = updatedChoice.VoteCount;
                }
            }
        }
    }
}
