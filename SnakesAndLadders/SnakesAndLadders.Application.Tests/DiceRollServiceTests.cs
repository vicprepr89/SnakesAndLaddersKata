using SnakesAndLadders.Application.Interfaces;
using SnakesAndLadders.Application.Services;

namespace SnakesAndLadders.Application.Tests
{
    public class DiceRollServiceTests
    {
        [Fact]
        public void RollDice_NoConditions_ReturnsValueBetweenOneAndSix()
        {
            // US 3 - Moves Are Determined By Dice Rolls: UAT1

            // Arrange
            IDiceRollService diceRollService = new DiceRollService();

            // Act
            int diceRollValue = diceRollService.RollDice();

            // Assert
            Assert.True(diceRollValue is >= 1 and <= 6);
        }
    }
}