using System.Linq;
using minesweeper_api.GameLogic;
using Xunit;

namespace minesweeper_api.Tests.GameLogic;

public class BasicStartUpTests
{
    [Fact]
    public void GameShouldPrepareTheFieldCorrectly()
    {
        //Arrange
        var board = new Board();

        //Act
        board.PrepareGame();

        //Assert
        for (int i = 0; i < board.GetRows().GetLength(0); i++) {
            for (int j = 0; j < board.GetRows().GetLength(1); j++) {
                var currentCell = board.GetRows()[i, j];
                Assert.Equal(j, currentCell.X);
                Assert.Equal(i, currentCell.Y);
                Assert.False(currentCell.IsRevealed);
                Assert.False(currentCell.IsFlagged);
                Assert.False(currentCell.IsBomb);
                Assert.Equal(0, currentCell.BombCount);
            }
        }
    }
    [Fact]
    public void GameShouldPrepareEmptyFieldWithMineCount()
    {
        //Arrange
        var board = new Board();

        //Act
        board.PrepareGame();
        foreach (var item in board.State.grid) {

        }
        //Assert
        Assert.InRange(board.State.BombsGenerated, 0, 255);
        Assert.All(board.State.grid.Cast<Cell>(), n => Assert.False(n.IsBomb));
    }

    [Fact]
    public void GameShouldPrepareNotStartedField()
    {
        //Arrange
        var board = new Board();

        //Act
        board.PrepareGame();

        //Assert
        Assert.False(board.State.isStarted);
    }

    [Fact]
    public void GameShouldRevealTheFieldCorrectly()
    {
        //Arrange
        var board = new Board();
        board.PrepareGame();

        //Act
        board.State.grid[0, 0].Reveal();

        //Assert
        Assert.True(board.GetRows()[0, 0].IsRevealed);
        Assert.Equal(0, board.GetRows()[0, 0].BombCount);
    }
}
