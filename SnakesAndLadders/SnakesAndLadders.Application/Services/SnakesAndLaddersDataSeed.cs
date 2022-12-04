using SnakesAndLadders.Data.Entities;
using SnakesAndLadders.Data.Interfaces;

namespace SnakesAndLadders.Application.Services
{
    public class SnakesAndLaddersDataSeed : ISnakesAndLaddersDataSeed
    {
        public User[] GetInitialUsers() => new User[]
        {
            new User { Id = 0, Position = 0 },
            new User { Id = 1, Position = 0 }
        };

        public int[] GetBoard()
        {
            int[] board = new int[100];

            // Set snakes.
            board[15] = -10;
            board[45] = -21;
            board[48] = -38;
            board[61] = -43;
            board[63] = -4;
            board[73] = -21;
            board[88] = -21;
            board[91] = -4;
            board[94] = -20;
            board[98] = -19;

            // Set ladders.
            board[1] = 36;
            board[6] = 7;
            board[7] = 23;
            board[14] = 11;
            board[20] = 21;
            board[27] = 56;
            board[35] = 8;
            board[50] = 16;
            board[70] = 20;
            board[77] = 20;
            board[86] = 7;

            return board;
        }
    }
}