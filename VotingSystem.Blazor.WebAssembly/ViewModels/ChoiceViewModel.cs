using System.ComponentModel.DataAnnotations;

namespace VotingSystem.Blazor.WebAssembly.ViewModels
{
    public class ChoiceViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Szavazat érték megadása kötelező")]
        public string? Value { get; set; }
        public int VoteCount { get; set; }
    }
}
