using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using VotingSystem.Shared.Models;
using VotingSystem.WPF.Infrastructure;
using VotingSystem.WPF.ViewModel;

namespace VotingSystem.WPF.Services
{
    public class AuthenticationService : BaseService, IAuthenticationService
    {
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IHttpRequestUtility _httpRequestUtility;
        private readonly ITokenStoreService _tokenStoreService;

        public AuthenticationService(IMapper mapper, HttpClient httpClient, IHttpRequestUtility httpRequestUtility, ITokenStoreService tokenStoreService)
        {
            _mapper = mapper;
            _httpClient = httpClient;
            _httpRequestUtility = httpRequestUtility;
            _tokenStoreService = tokenStoreService;
        }

        public string? GetCurrentlyLoggedInUser()
        {
            return _tokenStoreService.UserId;
        }

        public async Task<bool> LoginAsync(LoginViewModel loginBindingViewModel)
        {
            var loginDto = _mapper.Map<LoginRequestDto>(loginBindingViewModel);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsJsonAsync("users/login", loginDto);
            }
            catch (System.Exception)
            {
                ShowErrorMessage("Ismeretlen hiba történt.");
                return false;
            }

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadFromJsonAsync<LoginResponseDto>()
                    ?? throw new ArgumentNullException("Error with auth response.");

                _tokenStoreService.AuthToken = responseBody.AuthToken;
                _tokenStoreService.RefreshToken = responseBody.RefreshToken;
                _tokenStoreService.UserId = responseBody.UserId;

                return true;
            }
            else
            {
                HandleError(await response.Content.ReadAsStringAsync());
            }

            return false;
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _httpRequestUtility.ExecutePostHttpRequestAsync("users/logout");
            }
            catch (HttpRequestException) 
            {
                ShowErrorMessage("Ismeretlen hiba történt.");
            }

            _tokenStoreService.AuthToken = null;
            _tokenStoreService.RefreshToken = null;
        }

        public async Task<bool> RegisterAsync(RegisterViewModel registerViewModel)
        {
            if (registerViewModel.Password == registerViewModel.PasswordConfirmation)
            {
                var userDto = _mapper.Map<UserRequestDto>(registerViewModel);
                HttpResponseMessage response;
                try
                {
                    response = await _httpClient.PostAsJsonAsync("/users", userDto);
                }
                catch (System.Exception) { return false; }

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadFromJsonAsync<UserResponseDto>()
                        ?? throw new ArgumentNullException("Error with auth response.");

                    return true;
                }
                else
                {
                    HandleError(await response.Content.ReadAsStringAsync());
                }
            }
            else
            {
                ShowErrorMessage("A jelszavak nem egyeznek meg!");
            }

            return false;
        }
    }
}
