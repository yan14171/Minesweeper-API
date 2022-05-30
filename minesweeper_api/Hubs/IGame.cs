using static minesweeper_api.GameLogic.Board;

namespace minesweeper_api.Hubs;

public interface IGame
{
    Task StateChange(BoardState state);
    Task EndGame(BoardState state, int winnerId);
}
