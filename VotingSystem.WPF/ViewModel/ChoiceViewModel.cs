using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.WPF.ViewModel
{
    public class ChoiceViewModel : ViewModelBase
    {
        public int Id { get; set; }
        public string? Value { get; set; }

        private int _voteCount;
        public int VoteCount
        {
            get => _voteCount;
            set
            {
                if (_voteCount != value)
                {
                    _voteCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
    }
}
