using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.DataAccess.Models;
using VotingSystem.DataAccess.Services;
using VotingSystem.Shared.Models;

namespace VotingSystem.WebAPI.Controllers
{
    [Route("/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;

        public UsersController(IMapper mapper, IUsersService usersService)
        {
            _mapper = mapper;
            _usersService = usersService;
        }

        [HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created, type: typeof(UserResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateUser([FromBody] UserRequestDto userRequestDto)
        {
            var user = _mapper.Map<User>(userRequestDto);

            await _usersService.AddUserAsync(user, userRequestDto.Password);

            var userResponseDto = _mapper.Map<UserResponseDto>(user);

            return StatusCode(StatusCodes.Status201Created, userResponseDto);
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(UserResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser([FromRoute][Required] string id)
        {
            var user = await _usersService.GetUserByIdAsync(id);
            var userResponseDto = _mapper.Map<UserResponseDto>(user);

            return Ok(userResponseDto);
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(UserResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var (authToken, refreshToken, userId) = await _usersService.LoginAsync(loginRequestDto.Email, loginRequestDto.Password);

            var loginResponseDto = new LoginResponseDto
            {
                UserId = userId,
                AuthToken = authToken,
                RefreshToken = refreshToken,
            };

            return Ok(loginResponseDto);
        }

        [HttpPost]
        [Route("logout")]
        [Authorize]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent, type: typeof(void))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            await _usersService.LogoutAsync();

            return NoContent();
        }

        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(UserResponseDto))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RedeemRefreshToken([FromBody] string refreshToken)
        {
            var (authToken, newRefreshToken, userId) = await _usersService.RedeemRefreshTokenAsync(refreshToken);

            var loginResponseDto = new LoginResponseDto
            {
                UserId = userId,
                AuthToken = authToken,
                RefreshToken = newRefreshToken,
            };

            return Ok(loginResponseDto);
        }
    }
}
