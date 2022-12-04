using SnakesAndLadders.Application.Interfaces;

namespace SnakesAndLadders.Application.Services
{
    public class DiceRollService : IDiceRollService
    {
        public int RollDice() => Random.Shared.Next(1, 6);
    }
}