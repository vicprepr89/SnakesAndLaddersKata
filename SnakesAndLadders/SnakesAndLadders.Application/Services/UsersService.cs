using AutoMapper;
using SnakesAndLadders.Application.Dto;
using SnakesAndLadders.Application.Interfaces;
using SnakesAndLadders.Data.DataContext;
using SnakesAndLadders.Data.Entities;

namespace SnakesAndLadders.Application.Services
{
    public class UsersService : IUsersService
    {
        private readonly SnakesAndLaddersDataContext _context;

        private readonly IMapper _mapper;

        public UsersService(SnakesAndLaddersDataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<UserDto> GetUsers() => _mapper.Map<IEnumerable<UserDto>>(_context.Users);

        public int? GetUserPosition(int id)
        {
            User? user = _context.Users.FirstOrDefault(u => u.Id == id);

            return user is null ? null : user.Position + 1;
        }

        /// <summary>
        /// Moves the user position accord to generated dice roll and returns the new user position.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>The new user position.</returns>
        /// <exception cref="ArgumentOutOfRangeException" />
        public int? MoveUserPosition(int id, int diceRollValue)
        {
            if (diceRollValue is < 1 or > 6)
            {
                throw new ArgumentOutOfRangeException(nameof(diceRollValue), diceRollValue, "The dice roll value must be between 1 and 6 (both inclusive).");
            }

            User? user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user is null)
            {
                return null;
            }

            int initialMovement = user.Position + diceRollValue;

            // If the roll is too high, user "bounces" off the last position and moves back.
            if (initialMovement >= _context.Board.Length)
            {
                int pendingSpaces = initialMovement - (_context.Board.Length - 1);
                initialMovement = _context.Board.Length - 1 - pendingSpaces;
            }

            user.Position = initialMovement + _context.Board[initialMovement];

            return user.Position + 1;
        }

        public bool? UserHasWon(int id)
        {
            User? user = _context.Users.FirstOrDefault(u => u.Id == id);

            return user is null ? null : user.Position == _context.Board.Length - 1;
        }
    }
}