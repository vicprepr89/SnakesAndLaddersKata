namespace SnakesAndLadders.Application.Services
{
    using SnakesAndLadders.Application.Interfaces;

    public class DiceRollService : IDiceRollService
    {
        public int RollDice()
        {
            return Random.Shared.Next(1, 7);
        }
    }
}