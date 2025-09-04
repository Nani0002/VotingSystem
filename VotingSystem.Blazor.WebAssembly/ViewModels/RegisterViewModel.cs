using System.ComponentModel.DataAnnotations;

namespace VotingSystem.Blazor.WebAssembly.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Név megadása kötelező")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email cím megadása kötelező")]
        [EmailAddress] 
        public string? Email { get; set; }

        [Required(ErrorMessage = "Jelszó megadása kötelező")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Jelszó megadása kötelező")]
        public string? PasswordConfirmation { get; set; }
    }
}
