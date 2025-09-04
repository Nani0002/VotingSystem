using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public record LoginRequestDto
    {
        [EmailAddress(ErrorMessage = "Nem megfelelő email cím")]
        [Required(ErrorMessage = "Email cím megadása kötelező")]
        public required string Email { get; init; }
        [Required(ErrorMessage = "Jelszó megadása kötelező")]
        public required string Password { get; init; }
    }
}
