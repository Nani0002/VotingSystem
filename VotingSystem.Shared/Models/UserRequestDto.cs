using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.Shared.Models
{
    public record UserRequestDto
    {
        [StringLength(255, ErrorMessage = "Túl hosszú név")]
        public required string Name { get; init; }
        [EmailAddress(ErrorMessage = "Nem megfelelő email cím")]
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}
