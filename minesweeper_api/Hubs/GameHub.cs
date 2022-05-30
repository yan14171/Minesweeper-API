using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;
using minesweeper_api.Data.Models.DTOs;
using minesweeper_api.Services;

namespace minesweeper_api.Hubs;

public class GameHub : Hub<IGame>
{
    private readonly GameService _gameService;

    public GameHub(GameService gameService)
    {
        _gameService = gameService;
    }
    public async Task Reveal(CellDTO cell)
    {
        await _gameService.RevealCell(cell.X, cell.Y);
        await Clients.All.StateChange(_gameService.GameState);
    }
    public async Task Flag(CellDTO cell)
    {
        await _gameService.FlagCell(cell.X, cell.Y);
        await Clients.All.StateChange(_gameService.GameState);
    }
    public async Task RevealAround(CellDTO cell)
    {
        await _gameService.RevealAroundCell(cell.X, cell.Y);
        await Clients.All.StateChange(_gameService.GameState);
    }
}
