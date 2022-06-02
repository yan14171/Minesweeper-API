using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using minesweeper_api.Data.Models.DTOs;
using minesweeper_api.Services;

namespace minesweeper_api.Hubs;

public class GameHub : Hub<IGame>
{
    private readonly IMapper _mapper;
    private readonly GameService _gameService;

    public GameHub(IMapper mapper, GameService gameService)
    {
        _mapper = mapper;
        _gameService = gameService;
    }
    public async Task PrepareGame() 
    {
        await _gameService.PrepareGame();
        var gameStateDTO = _mapper.Map<BoardStateDTO>(_gameService.GameState);
        await Clients.All.StateChange(gameStateDTO);
    }
    public async Task EndGame()
    {
        await _gameService.EndGame();
        var gameStateDTO = _mapper.Map<BoardStateDTO>(_gameService.GameState);
        await Clients.All.EndGame(gameStateDTO, 1);
    }
    public async Task Reveal(CellDTO cell)
    {
        await _gameService.RevealCell(cell.X, cell.Y);
        var gameStateDTO = _mapper.Map<BoardStateDTO>(_gameService.GameState);
        await Clients.All.StateChange(gameStateDTO);
    }
    public async Task Flag(CellDTO cell)
    {
        await _gameService.FlagCell(cell.X, cell.Y);
        var gameStateDTO = _mapper.Map<BoardStateDTO>(_gameService.GameState);
        await Clients.All.StateChange(gameStateDTO);
    }
    public async Task RevealAround(CellDTO cell)
    {
        await _gameService.RevealAroundCell(cell.X, cell.Y);
        var gameStateDTO = _mapper.Map<BoardStateDTO>(_gameService.GameState);
        await Clients.All.StateChange(gameStateDTO);
    }
}

