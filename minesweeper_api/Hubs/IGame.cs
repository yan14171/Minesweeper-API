using minesweeper_api.Data.Models.DTOs;
using static minesweeper_api.GameLogic.Board;

namespace minesweeper_api.Hubs;

public interface IGame
{
    Task StateChange(BoardStateDTO state);
    Task EndGame(BoardStateDTO state, int winnerId);
}
