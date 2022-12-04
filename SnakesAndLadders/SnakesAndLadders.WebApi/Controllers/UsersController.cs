using Microsoft.AspNetCore.Mvc;
using SnakesAndLadders.Application.Dto;
using SnakesAndLadders.Application.Interfaces;

namespace SnakesAndLadders.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _userService;

        private readonly IDiceRollService _diceRollService;

        private readonly ILogger<UsersController> _logger;

        public UsersController(IUsersService userService, IDiceRollService diceRollService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _diceRollService = diceRollService;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> GetUsers()
        {
            _logger.LogInformation("Getting users...");

            return Ok(_userService.GetUsers());
        }

        [HttpGet("{id:int}/position")]
        public ActionResult<int> GetUserPosition(int id)
        {
            _logger.LogInformation($"Getting the position of the user '{id}'...");

            int? userPosition = _userService.GetUserPosition(id);

            return userPosition is null
                ? NotFound($"The user with the identifier '{id}' does not exist.")
                : Ok(userPosition);
        }

        /// <summary>
        /// Moves the user position accord to generated dice roll and returns the new user position.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>The new user position.</returns>
        [HttpPost("{id:int}/move")]
        public ActionResult<int> MoveUserPosition(int id)
        {
            try
            {
                _logger.LogInformation($"Rolling the dice for user '{id}'...");

                int diceRollValue = _diceRollService.RollDice();

                _logger.LogInformation($"Moving {diceRollValue} spaces to the user '{id}'...");

                int? newUserPosition = _userService.MoveUserPosition(id, diceRollValue);

                return newUserPosition is null
                    ? NotFound($"The user with the identifier '{id}' does not exist.")
                    : Ok(newUserPosition);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError(ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id:int}/hasWon")]
        public ActionResult<bool> UserHasWon(int id)
        {
            _logger.LogInformation($"Getting whether the user '{id}' has won...");

            bool? userHasWon = _userService.UserHasWon(id);

            return userHasWon is null
                ? NotFound($"The user with the identifier '{id}' does not exist.")
                : Ok(userHasWon);
        }
    }
}