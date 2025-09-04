using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.DataAccess;

namespace VotingSystem.WebAPI.Controllers
{
    [ApiController]
    [Route("e2e-test")]
    public class E2ETestController : ControllerBase
    {
        private readonly DbResetter _dbResetter;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public E2ETestController(DbResetter dbResetter, IWebHostEnvironment webHostEnvironment)
        {
            _dbResetter = dbResetter;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("reset-database")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status501NotImplemented)]
        public async Task<IActionResult> ResetDatabase()
        {
            if (!_webHostEnvironment.IsEnvironment("E2E"))
                return StatusCode(501);

            await _dbResetter.ResetAsync();
            return NoContent();
        }
    }
}
