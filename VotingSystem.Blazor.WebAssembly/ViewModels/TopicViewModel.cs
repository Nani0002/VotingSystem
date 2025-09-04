using System.ComponentModel.DataAnnotations;

namespace VotingSystem.Blazor.WebAssembly.ViewModels
{
    public class TopicViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Szavazatnév megadása kötelező")]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Kezdő dátum megadása kötelező")]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "Zárási dátum megadása kötelező")]
        public DateTime? CloseDate { get; set; }

        public List<ChoiceViewModel> Choices { get; set; } = [];

        [Range(1, int.MaxValue, ErrorMessage = "Legalább egy szavazat kötelező")]
        public int MinimalVotes { get; set; } = 1;
        public int MaximalVotes { get; set; } = 1;
        public bool Live { get; set; } = false;

        public List<VoteViewModel> Votes { get; set; } = [];

        public List<UserViewModel> Voters { get; set; } = [];
    }
}
