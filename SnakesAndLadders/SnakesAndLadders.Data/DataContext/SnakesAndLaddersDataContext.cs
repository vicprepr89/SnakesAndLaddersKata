using SnakesAndLadders.Data.Entities;
using SnakesAndLadders.Data.Interfaces;

namespace SnakesAndLadders.Data.DataContext
{
    public class SnakesAndLaddersDataContext
    {
        private readonly ISnakesAndLaddersDataSeed _dataSeed;

        public List<User> Users { get; set; } = new List<User>();

        public int[] Board { get; set; } = Array.Empty<int>();

        public SnakesAndLaddersDataContext(ISnakesAndLaddersDataSeed dataSeed)
        {
            _dataSeed = dataSeed;

            OnModelCreating();
        }

        private void OnModelCreating()
        {
            Users.AddRange(_dataSeed.GetInitialUsers());
            Board = _dataSeed.GetBoard();
        }
    }
}