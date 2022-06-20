using minesweeper_api.Data.Interfaces;
using minesweeper_api.Data.Models;
using System.Collections.Concurrent;

namespace minesweeper_api.Data.Repositories.Concrete;

public class LocalLobbyManipulator : ILobbyManipulator
{
    private ConcurrentDictionary<int, Lobby> lobbys = new();

    public LocalLobbyManipulator()
    {
        lobbys.TryAdd(1, new Lobby { Id = 1, IsStarted = false, UserIdentifiers = new List<string>() { "sd", "Dsd" }});
        lobbys.TryAdd(2, new Lobby { Id = 2, IsStarted = false, UserIdentifiers = new List<string>() { "sdfsdd" } });
    }
    
    public Lobby Add(Lobby obj)
    {
        if (obj.Id is null || lobbys.ContainsKey(obj.Id.Value))
            obj.Id = GenerateId();
        if (lobbys.TryAdd(obj?.Id ?? 0, obj))
            return obj;
        else
            throw new InvalidOperationException("Lobby already exists");
    }

    public Lobby Close(int lobbyId, string userIdentifier)
    {
        var lobby = GetById(lobbyId);
        if (lobby is null)
            throw new InvalidOperationException("Invalid lobby credentials");
        if (!lobby.UserIdentifiers.Contains(userIdentifier))
            throw new InvalidOperationException("User is not in lobby. Can't close the lobby!");
        return Remove(lobby);
    }

    public Lobby Disconnect(int lobbyId, string userIdentifier)
    {
        var lobby = GetById(lobbyId);
        if (lobby is null)
            throw new InvalidOperationException("Invalid lobby credentials");
        if (lobby.UserIdentifiers.Remove(userIdentifier))
            return lobby;
        else
            throw new UnauthorizedAccessException("User is not in lobby");
    }

    public IEnumerable<Lobby> GetAll()
    {
        return lobbys.Values;
    }

    public Lobby GetById(int id)
    {
        if (lobbys.TryGetValue(id, out Lobby lobby))
            return lobby;
        else
            throw new InvalidOperationException("Lobby does not exist");
    }

    public Lobby Remove(Lobby obj)
    {
        if (lobbys.TryRemove(obj?.Id ?? 0, out Lobby lobby))
            return lobby;
        else
            throw new InvalidOperationException("Lobby does not exist");
    }

    private int GenerateId()
    {
        var lastValue = lobbys.LastOrDefault().Value;
        if (lastValue is null)
            return 1;
        return lastValue.Id.Value + 1;
    }
}
