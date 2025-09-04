using System.Windows.Input;
using VotingSystem.WPF.Infrastructure;
using VotingSystem.WPF.Services;
using VotingSystem.WPF.View;

namespace VotingSystem.WPF.ViewModel
{
    public class TopicShowViewModel : ViewModelBase
    {
        public int TopicId { get; set; }

        private TopicViewModel? _topic;
        public TopicViewModel? Topic
        {
            get => _topic;
            set
            {
                _topic = value;
                OnPropertyChanged();
            }
        }

        private readonly ITopicsService _topicService;
        private readonly NavigationService _navigationService;

        public ICommand BackCommand { get; set; }
        public ICommand VoteCommand { get; set; }

        /*public TopicShowViewModel()
        {
            Topic = new TopicViewModel()
            {
                Choices =
                [
                    new ChoiceViewModel() { Value = "Option 1" },
                    new ChoiceViewModel() { Value = "Option 2" },
                    new ChoiceViewModel() { Value = "Option 3" },
                    new ChoiceViewModel() { Value = "Option 4" }
                ],
                Name = "Sample Topic",
                Description = "Sample text",
                CloseDate = DateTime.UtcNow.AddHours(10),
                StartDate = DateTime.UtcNow,
                MinimalVotes = 1,
                MaximalVotes = 2
            };
        }*/

        public TopicShowViewModel(ITopicsService topicService, NavigationService navigationService)
        {
            _topicService = topicService;
            _navigationService = navigationService;

            BackCommand = new DelegateCommand(_ => OnBack());
            VoteCommand = new DelegateCommand(_ => OnVote());
        }

        public async Task LoadTopicAsync()
        {
            TopicId = _navigationService.GetParameter<TopicShowPage, int>();
            _navigationService.ClearParameter<TopicShowPage>();
            Topic = await _topicService.GetTopicByIdAsync(TopicId);
        }

        public void OnBack()
        {
            _navigationService.Navigate<TopicsPage>();
        }

        public async void OnVote()
        {
            List<ChoiceViewModel> selectedChoices = Topic!.Choices.Where(c => c.IsSelected).ToList();

            if (selectedChoices.Count < Topic.MinimalVotes || selectedChoices.Count > Topic.MaximalVotes)
            {
                return;
            }

            bool success = await _topicService.VoteForTopicAsync(TopicId, selectedChoices);

            if (success)
            {
                OnBack();
            }
        }
    }
}
