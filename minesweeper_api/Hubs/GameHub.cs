using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using minesweeper_api.Data.Models.DTOs;
using minesweeper_api.Services;

namespace minesweeper_api.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class GameHub : Hub<IGame>, IGameHub
{
    private readonly IMapper _mapper;
    private readonly GameService _gameService;

    public GameHub(IMapper mapper, GameService gameService)
    {
        _mapper = mapper;
        _gameService = gameService;
    }

    public async override Task OnConnectedAsync()
    {
        var gameId = GetGameId();
        await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(gameId));
    }
    public async override Task OnDisconnectedAsync(Exception? ex)
    {
        var gameId = GetGameId();
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName(gameId));
    }
    public async Task PrepareGame()
    {
        var gameId = GetGameId();
        var game = await _gameService.PrepareGame(gameId);
        var gameStateDTO = _mapper.Map<BoardStateDTO>(game.State);
        await Clients.Group(GetGroupName(gameId)).StateChange(gameStateDTO);
    }
    public async Task EndGame()
    {
        var gameId = GetGameId();
        var game = await _gameService.EndGame(gameId);
        var gameStateDTO = _mapper.Map<BoardStateDTO>(game.State);
        await Clients.Group(GetGroupName(gameId)).StateChange(gameStateDTO);
    }
    public async Task Reveal(CellDTO cell)
    {
        var gameId = GetGameId();
        var game = await _gameService.RevealCell(cell.X, cell.Y, gameId);
        var gameStateDTO = _mapper.Map<BoardStateDTO>(game.State);
        await Clients.Group(GetGroupName(gameId)).StateChange(gameStateDTO);
    }
    public async Task Flag(CellDTO cell)
    {
        var gameId = GetGameId();
        var game = await _gameService.FlagCell(cell.X, cell.Y, gameId);
        var gameStateDTO = _mapper.Map<BoardStateDTO>(game.State);
        await Clients.Group(GetGroupName(gameId)).StateChange(gameStateDTO);
    }
    public async Task RevealAround(CellDTO cell)
    {
        var gameId = GetGameId();
        var game = await _gameService.RevealAroundCell(cell.X, cell.Y, gameId);
        var gameStateDTO = _mapper.Map<BoardStateDTO>(game.State);
        await Clients.Group(GetGroupName(gameId)).StateChange(gameStateDTO);
    }
    private int GetGameId()
    {
        return int.Parse(Context.Items["gameId"]?.ToString() ?? "-1");
    }
    private string GetGroupName(int gameId) => $"group_{gameId}";
}

