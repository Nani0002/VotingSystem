using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.WPF.ViewModel
{
    public class TopicViewModel : ViewModelBase
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? CloseDate { get; set; }

        public ObservableCollection<ChoiceViewModel> Choices { get; set; } = new();

        public int MinimalVotes { get; set; } = 1;
        public int MaximalVotes { get; set; } = 1;
        public bool Live { get; set; } = false;


        public List<VoteViewModel> Votes { get; set; } = new();
        public List<UserViewModel> Voters { get; set; } = new();

        private string? _currentUserId;

        public void SetCurrentUser(string? userId)
        {
            _currentUserId = userId;
            OnPropertyChanged(nameof(UserVoted));
        }

        public bool UserVoted => _currentUserId != null && Voters.Any(x => x.Id == _currentUserId);

        public bool ShowResults => Live && UserVoted || CloseDate < DateTime.Now;
        public int VoteCount => Choices.Select(x => x.VoteCount).Sum();
    }
}
