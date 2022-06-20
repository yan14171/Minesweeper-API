using minesweeper_api.Data.Models;
using minesweeper_api.GameLogic;

namespace minesweeper_api.Data.Interfaces;
public interface IAsyncManipulator<T> : IAsyncCommand<T>, IAsyncRepository<T>
{
}
public interface IManipulator<T> : ICommand<T>, IRepository<T>
{   
}
public interface ILobbyManipulator : IManipulator<Lobby>
{
    Lobby Disconnect(int lobbyId, string userIdentifier);
    Lobby Close(int id, string value);
}
public interface IBoardManipulator : IManipulator<Board>
{
    
}

