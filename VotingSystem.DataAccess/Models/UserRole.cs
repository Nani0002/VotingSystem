using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace VotingSystem.DataAccess.Models
{
    public class UserRole : IdentityRole
    {
        public UserRole() { }
        public UserRole(string role) : base(role) { }
    }
}
