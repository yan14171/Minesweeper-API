﻿using minesweeper_api.GameLogic;
using static minesweeper_api.GameLogic.Board;

namespace minesweeper_api.Services;

public class GameService : IDisposable
{
    //private List<BoardInfo>
    private Board _game;

    public BoardState GameState { get => _game.State; }

    public GameService()
    {
        PrepareGame().GetAwaiter().GetResult();
    }

    public Task PrepareGame()
    {
        if (_game?.State.isStarted ?? false)
            return Task.CompletedTask;
        
        _game = new Board();
        _game.PrepareGame();

        return Task.CompletedTask;
    }

    public Task EndGame()
    {
        this._game.EndGame();
        return Task.CompletedTask;
    }

    public Task RevealCell(int x, int y)
    {
        var cells = _game.GetRows();

        cells[y, x].Reveal();

        return Task.CompletedTask;
    }
    
    public Task FlagCell(int x, int y)
    {
        var cells = _game.GetRows();

        cells[y, x].Flag();

        return Task.CompletedTask;
    }

    public Task RevealAroundCell(int x, int y)
    {
        var cells = _game.GetRows();

        cells[y, x].RevealAround();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

//public record BoardInfo(int Id, Board board);