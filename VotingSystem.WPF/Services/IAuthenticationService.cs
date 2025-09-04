using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.WPF.ViewModel;

namespace VotingSystem.WPF.Services
{
    public interface IAuthenticationService
    {
        public Task<bool> LoginAsync(LoginViewModel loginBindingViewModel);
        public Task LogoutAsync();
        public string? GetCurrentlyLoggedInUser();
        public Task<bool> RegisterAsync(RegisterViewModel registerViewModel);
    }
}
