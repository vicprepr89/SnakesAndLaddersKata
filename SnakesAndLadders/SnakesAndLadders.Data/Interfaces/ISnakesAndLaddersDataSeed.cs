using SnakesAndLadders.Data.Entities;

namespace SnakesAndLadders.Data.Interfaces
{
    public interface ISnakesAndLaddersDataSeed
    {
        User[] GetInitialUsers();

        int[] GetBoard();
    }
}