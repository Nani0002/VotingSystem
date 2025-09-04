using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace VotingSystem.DataAccess.Models
{
    public class User : IdentityUser
    {
        public required string Name { get; set; }
        public Guid? RefreshToken { get; set; }
        public virtual ICollection<VoteRecord> VoteRecords { get; set; } = [];
    }
}
