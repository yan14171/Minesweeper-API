using minesweeper_api.Data.Interfaces;
using minesweeper_api.Data.Models;
using minesweeper_api.GameLogic;
using System.Collections.Concurrent;

namespace minesweeper_api.Data.Repositories.Concrete;

public class LocalBoardManipulator : IBoardManipulator
{
    private ConcurrentDictionary<int, Board> boards = new();
    public Board Add(Board obj)
    {
        if (obj.Id is null || boards.ContainsKey(obj.Id.Value))
            obj.Id = GenerateId();
        if (boards.TryAdd(obj?.Id ?? 0, obj))
            return obj;
        else
            throw new InvalidOperationException("Board already exists");
    }
    public LocalBoardManipulator()
    {
        var board1 = new Board() { LobbyId = 1 };
        var board2 = new Board() { LobbyId = 2 };
        Add(board2);
        Add(board1);
    }
    public IEnumerable<Board> GetAll()
    {
        return boards.Values;
    }

    public Board GetById(int id)
    {
        if (boards.TryGetValue(id, out Board board))
            return board;
        else
            throw new InvalidOperationException("Board does not exist");
    }

    public Board Remove(Board obj)
    {
        if (boards.TryRemove(obj?.Id ?? 0, out Board board))
            return board;
        else
            throw new InvalidOperationException("Board does not exist");
    }

    private int GenerateId()
    {
        var lastValue = boards.LastOrDefault().Value;
        if (lastValue is null)
            return 1;
        return lastValue.Id.Value + 1;
    }
}
