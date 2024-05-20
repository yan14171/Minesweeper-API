using minesweeper_api.Data.Models.DTOs;

namespace minesweeper_api.Hubs
{
    public interface IGameHub
    {
        Task EndGame();
        Task Flag(CellDTO cell);
        Task PrepareGame();
        Task Reveal(CellDTO cell);
        Task RevealAround(CellDTO cell);
    }
}