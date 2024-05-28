using minesweeper_api.Data.Interfaces;
using minesweeper_api.Data.Models;
using minesweeper_api.GameLogic;
using static minesweeper_api.GameLogic.Board;

namespace minesweeper_api.Services;

public class GameService : IDisposable
{
    private readonly ILobbyManipulator _lobbyManipulator;
    private readonly IBoardManipulator _boardManipulator;

    private Dictionary<int, AiStat> _moveStats = new Dictionary<int, AiStat>();

    public BoardState GameState(int gameId) => _boardManipulator.GetById(gameId).State;

    public GameService(ILobbyManipulator lobbyManipulator, IBoardManipulator boardManipulator)
    {
        _lobbyManipulator = lobbyManipulator;
        _boardManipulator = boardManipulator;
    }

    public async Task<Board> PrepareGame(int gameId)
    {
        var game = _boardManipulator.GetById(gameId);
        if (game is null)
            throw new NullReferenceException("Cannot prepare a game, which was not created before");
        
        if (game.State.isStarted)
            return game;
        
        game.PrepareGame();

        _moveStats[gameId] = new AiStat();
        
        return game;
    }

    public async Task<Board> EndGame(int gameId)
    {
        var game = _boardManipulator.GetById(gameId);
        game.EndGame();
        return game;
    }
    
    public async Task<Board> RevealCell(int x, int y, int gameId)
    {
        var game = _boardManipulator.GetById(gameId);

        _moveStats[gameId].RevealMovesMade++;

        var cells = game.GetRows();

        cells[y, x].Reveal();

        return game;
    }
    
    public async Task<Board> FlagCell(int x, int y, int gameId)
    {
        var game = _boardManipulator.GetById(gameId);

        _moveStats[gameId].FlagMovesMade++;

        var cells = game.GetRows();

        cells[y, x].Flag();

        return game;
    }

    public async Task<Board> RevealAroundCell(int x, int y, int gameId)
    {
        var game = _boardManipulator.GetById(gameId);

        var cells = game.GetRows();

        cells[y, x].RevealAround();

        return game;
    }

    public Task<bool> TryValidateUserGameInLobby(int lobbyId, string userEmail, out int gameId)
    {
        var board = _boardManipulator.GetAll().FirstOrDefault(b => b.LobbyId == lobbyId);
        if (board is null)
        {
            gameId = -1;
            return Task.FromResult(false);
        }
        try
        {
            var lobby = _lobbyManipulator.GetById(board.LobbyId.Value);
            if (!(lobby.UserIdentifiers?.Contains(userEmail) ?? false))
                throw new InvalidOperationException();
            gameId = GetGameIdByLobbyId(lobbyId);
            return Task.FromResult(true);
        }
        catch(InvalidOperationException ex)
        {
            gameId = -1;
            return Task.FromResult(false);
        }
    }

    private int GetGameIdByLobbyId(int lobbyId) => _boardManipulator.GetAll().FirstOrDefault(b => b.LobbyId == lobbyId)?.Id ?? -1;


    public Task<AiStat> GetAiStat(int lobbyId) 
    {
        var gameId = GetGameIdByLobbyId(lobbyId);
        return Task.FromResult(_moveStats[gameId]);
    }
    
    public void Dispose()
    {
        foreach (var item in _boardManipulator.GetAll().Where(n => n.State.isStarted))
                item.EndGame();
    }
}