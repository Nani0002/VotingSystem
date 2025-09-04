namespace VotingSystem.Blazor.WebAssembly.ViewModels
{
    public class VoteViewModel
    {
        public List<string> SelectedChoices { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
