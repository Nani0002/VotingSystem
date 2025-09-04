using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VotingSystem.WPF.Infrastructure;
using VotingSystem.WPF.Services;
using VotingSystem.WPF.View;

namespace VotingSystem.WPF.ViewModel
{
    public class RegisterViewModel
    {
        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? PasswordConfirmation { get; set; }

        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }

        private IAuthenticationService _authenticationService;
        private NavigationService _navigationService;

        public RegisterViewModel(IAuthenticationService authenticationService, NavigationService navigationService)
        {
            _authenticationService = authenticationService;
            _navigationService = navigationService;

            LoginCommand = new DelegateCommand(_ => OnLogin());
            RegisterCommand = new DelegateCommand(_ => OnRegister());
        }

        public void OnLogin()
        {
            _navigationService.Navigate<LoginPage>();
        }

        public async void OnRegister()
        {
            var success = await _authenticationService.RegisterAsync(this);
            if (success)
            {
                _navigationService.Navigate<LoginPage>();
            }
        }
    }
}
