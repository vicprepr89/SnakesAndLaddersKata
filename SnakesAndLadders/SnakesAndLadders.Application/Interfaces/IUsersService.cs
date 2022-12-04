using SnakesAndLadders.Application.Dto;

namespace SnakesAndLadders.Application.Interfaces
{
    public interface IUsersService
    {
        IEnumerable<UserDto> GetUsers();

        int? GetUserPosition(int id);

        /// <summary>
        /// Moves the user position accord to generated dice roll and returns the new user position.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>The new user position.</returns>
        /// <exception cref="ArgumentOutOfRangeException" />
        int? MoveUserPosition(int id, int diceRollValue);

        bool? UserHasWon(int id);
    }
}