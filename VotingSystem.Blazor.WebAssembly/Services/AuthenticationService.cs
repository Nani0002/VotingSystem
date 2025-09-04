
using System.Net.Http.Json;
using AutoMapper;
using Blazored.LocalStorage;
using VotingSystem.Blazor.WebAssembly.Exception;
using VotingSystem.Blazor.WebAssembly.Infrastructure;
using VotingSystem.Blazor.WebAssembly.ViewModels;
using VotingSystem.Shared.Models;

namespace VotingSystem.Blazor.WebAssembly.Services
{
    public class AuthenticationService : BaseService, IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        private readonly IMapper _mapper;
        private readonly IHttpRequestUtility _httpRequestUtility;

        public AuthenticationService(HttpClient httpClient, ILocalStorageService localStorageService,
            IMapper mapper, IHttpRequestUtility httpRequestUtility)
        {
            _httpClient = httpClient;
            _mapper = mapper;
            _localStorageService = localStorageService;
            _httpRequestUtility = httpRequestUtility;
        }

        public async Task<string?> GetCurrentlyLoggedInUserAsync()
        {
            return await _localStorageService.GetItemAsStringAsync("UserName");
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
                //ShowErrorMessage("Unknown error occured");
                return false;
            }

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadFromJsonAsync<LoginResponseDto>()
                    ?? throw new ArgumentNullException("Error with auth response.");

                await _localStorageService.SetItemAsStringAsync("AuthToken", responseBody.AuthToken);
                await _localStorageService.SetItemAsStringAsync("RefreshToken", responseBody.RefreshToken);
                await SetCurrentUserNameAsync(responseBody.UserId);

                return true;
            }
            else
            {
                await HandleError(response);
            }

            return false;
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _httpRequestUtility.ExecutePostHttpRequestAsync("users/logout");
            }
            catch (HttpRequestException) { }

            var keys = new List<string>() { "AuthToken", "RefreshToken", "UserName" };
            await _localStorageService.RemoveItemsAsync(keys);
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

                    return await LoginAsync(new LoginViewModel() { Email = registerViewModel.Email, Password = registerViewModel.Password});
                }
                else
                {
                    await HandleError(response);
                }
            }
            else
            {
                //ShowErrorMessage("Passwords do not match");
            }

            return false;
        }

        public async Task<bool> TryAutoLoginAsync()
        {
            if (!(await _localStorageService.ContainKeyAsync("RefreshToken")))
                return false;

            try
            {
                await _httpRequestUtility.RedeemTokenAsync();
            }
            catch (HttpRequestErrorException)
            {
                var keys = new List<string>() { "AuthToken", "RefreshToken", "UserName" };
                await _localStorageService.RemoveItemsAsync(keys);
                return false;
            }
            return true;
        }

        private async Task SetCurrentUserNameAsync(string currentUserId)
        {
            var response = await _httpRequestUtility.ExecuteGetHttpRequestAsync<UserResponseDto>($"users/{currentUserId}");
            await _localStorageService.SetItemAsStringAsync("UserName", response.Response.Name);
        }
    }
}
