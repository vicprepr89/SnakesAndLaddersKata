using AutoMapper;
using Moq;
using SnakesAndLadders.Application.Dto;
using SnakesAndLadders.Application.Interfaces;
using SnakesAndLadders.Application.Services;
using SnakesAndLadders.Data.DataContext;
using SnakesAndLadders.Data.Entities;
using SnakesAndLadders.Data.Interfaces;

namespace SnakesAndLadders.Application.Tests
{
    public class UsersServiceTests
    {
        private readonly Mock<ISnakesAndLaddersDataSeed> _dataSeedMock;

        private readonly Mock<IMapper> _mapperMock;

        public UsersServiceTests()
        {
            _dataSeedMock = new Mock<ISnakesAndLaddersDataSeed>();

            _ = _dataSeedMock
                .Setup(m => m.GetBoard())
                .Returns(() =>
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
                });

            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public void GetUsers_NoConditions_ReturnsTheProperUsers()
        {
            // Arrange
            User[] initialUsers = { new User { Id = 0, Position = 0 } };

            _ = _dataSeedMock
                .Setup(m => m.GetInitialUsers())
                .Returns(initialUsers);

            _ = _mapperMock
                .Setup(m => m.Map<IEnumerable<UserDto>>(It.IsAny<List<User>>()))
                .Returns(initialUsers.Select(u => new UserDto { Id = u.Id, Position = u.Position }));

            IUsersService usersService = new UsersService(new SnakesAndLaddersDataContext(_dataSeedMock.Object), _mapperMock.Object);

            // Act
            IEnumerable<UserDto> users = usersService.GetUsers();

            // Assert
            Assert.Equivalent(initialUsers, users);
        }

        [Fact]
        public void GetUserPosition_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            User[] initialUsers = { new User { Id = 0, Position = 0 } };

            _ = _dataSeedMock
                .Setup(m => m.GetInitialUsers())
                .Returns(initialUsers);

            IUsersService usersService = new UsersService(new SnakesAndLaddersDataContext(_dataSeedMock.Object), _mapperMock.Object);

            // Act
            int? userPosition = usersService.GetUserPosition(1);

            // Assert
            Assert.Null(userPosition);
        }

        [Fact]
        public void GetUserPosition_UserExists_ReturnsUserPosition()
        {
            // US 1 - Token Can Move Across the Board: UAT1

            // Arrange
            User[] initialUsers = { new User { Id = 0, Position = 0 } };

            _ = _dataSeedMock
                .Setup(m => m.GetInitialUsers())
                .Returns(initialUsers);

            IUsersService usersService = new UsersService(new SnakesAndLaddersDataContext(_dataSeedMock.Object), _mapperMock.Object);

            // Act
            int? userPosition = usersService.GetUserPosition(0);

            // Assert
            Assert.Equal(1, userPosition);
        }

        [Fact]
        public void MoveUserPosition_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            User[] initialUsers = { new User { Id = 0, Position = 0 } };

            _ = _dataSeedMock
                .Setup(m => m.GetInitialUsers())
                .Returns(initialUsers);

            IUsersService usersService = new UsersService(new SnakesAndLaddersDataContext(_dataSeedMock.Object), _mapperMock.Object);

            // Act
            int? newUserPosition = usersService.MoveUserPosition(id: 1, diceRollValue: 1);

            // Assert
            Assert.Null(newUserPosition);
        }

        [Fact]
        public void MoveUserPosition_UserExistsButDiceRollIsInvalid_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            User[] initialUsers = { new User { Id = 0, Position = 0 } };

            _ = _dataSeedMock
                .Setup(m => m.GetInitialUsers())
                .Returns(initialUsers);

            IUsersService usersService = new UsersService(new SnakesAndLaddersDataContext(_dataSeedMock.Object), _mapperMock.Object);

            // Act, Assert
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => usersService.MoveUserPosition(id: 0, diceRollValue: 0));
        }

        [Fact]
        public void MoveUserPosition_UserExistsDiceRollIsValidAndLandsOnNormalSquare_ReturnsNewUserPosition()
        {
            // US 1 - Token Can Move Across the Board: UAT2

            // Arrange
            User[] initialUsers = { new User { Id = 0, Position = 0 } };

            _ = _dataSeedMock
                .Setup(m => m.GetInitialUsers())
                .Returns(initialUsers);

            IUsersService usersService = new UsersService(new SnakesAndLaddersDataContext(_dataSeedMock.Object), _mapperMock.Object);

            // Act
            int? newUserPosition = usersService.MoveUserPosition(id: 0, diceRollValue: 3);

            // Assert
            Assert.Equal(4, newUserPosition);
        }

        [Fact]
        public void MoveUserPosition_UserExistsDiceRollIsValidAndLandsOnLadderSquare_ReturnsNewUserPosition()
        {
            // US 1 - Token Can Move Across the Board: UAT3
            // US 3 - Moves Are Determined By Dice Rolls: UAT2

            // Arrange
            User[] initialUsers = { new User { Id = 0, Position = 0 } };

            _ = _dataSeedMock
                .Setup(m => m.GetInitialUsers())
                .Returns(initialUsers);

            IUsersService usersService = new UsersService(new SnakesAndLaddersDataContext(_dataSeedMock.Object), _mapperMock.Object);

            // Act
            _ = usersService.MoveUserPosition(id: 0, diceRollValue: 3);
            int? newUserPosition = usersService.MoveUserPosition(id: 0, diceRollValue: 4);

            // Assert
            Assert.Equal(31, newUserPosition);
        }

        [Fact]
        public void MoveUserPosition_UserExistsDiceRollIsValidAndLandsOnFinalSquare_ReturnsNewUserPosition()
        {
            // US 2 - Player Can Win the Game - UAT1

            // Arrange
            User[] initialUsers = { new User { Id = 0, Position = 96 } };

            _ = _dataSeedMock
                .Setup(m => m.GetInitialUsers())
                .Returns(initialUsers);

            IUsersService usersService = new UsersService(new SnakesAndLaddersDataContext(_dataSeedMock.Object), _mapperMock.Object);

            // Act
            int? newUserPosition = usersService.MoveUserPosition(id: 0, diceRollValue: 3);

            // Assert
            Assert.Equal(100, newUserPosition);
        }

        [Fact]
        public void MoveUserPosition_UserExistsDiceRollIsValidAndLandsOnSnakeSquare_ReturnsNewUserPosition()
        {
            // US 2 - Player Can Win the Game - UAT2

            // Arrange
            User[] initialUsers = { new User { Id = 0, Position = 96 } };

            _ = _dataSeedMock
                .Setup(m => m.GetInitialUsers())
                .Returns(initialUsers);

            IUsersService usersService = new UsersService(new SnakesAndLaddersDataContext(_dataSeedMock.Object), _mapperMock.Object);

            // Act
            int? newUserPosition = usersService.MoveUserPosition(id: 0, diceRollValue: 4);

            // Assert
            Assert.Equal(80, newUserPosition);
        }

        [Fact]
        public void UserHasWon_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            User[] initialUsers = { new User { Id = 0, Position = 0 } };

            _ = _dataSeedMock
                .Setup(m => m.GetInitialUsers())
                .Returns(initialUsers);

            IUsersService usersService = new UsersService(new SnakesAndLaddersDataContext(_dataSeedMock.Object), _mapperMock.Object);

            // Act
            bool? userHasWon = usersService.UserHasWon(1);

            // Assert
            Assert.Null(userHasWon);
        }

        [Fact]
        public void UserHasWon_UserExistsAndHasNotWon_ReturnsFalse()
        {
            // US 2 - Player Can Win the Game - UAT2

            // Arrange
            User[] initialUsers = { new User { Id = 0, Position = 79 } };

            _ = _dataSeedMock
                .Setup(m => m.GetInitialUsers())
                .Returns(initialUsers);

            IUsersService usersService = new UsersService(new SnakesAndLaddersDataContext(_dataSeedMock.Object), _mapperMock.Object);

            // Act
            bool? userHasWon = usersService.UserHasWon(0);

            // Assert
            Assert.False(userHasWon);
        }

        [Fact]
        public void UserHasWon_UserExistsAndHasWon_ReturnsTrue()
        {
            // US 2 - Player Can Win the Game - UAT1

            // Arrange
            User[] initialUsers = { new User { Id = 0, Position = 99 } };

            _ = _dataSeedMock
                .Setup(m => m.GetInitialUsers())
                .Returns(initialUsers);

            IUsersService usersService = new UsersService(new SnakesAndLaddersDataContext(_dataSeedMock.Object), _mapperMock.Object);

            // Act
            bool? userHasWon = usersService.UserHasWon(0);

            // Assert
            Assert.True(userHasWon);
        }
    }
}