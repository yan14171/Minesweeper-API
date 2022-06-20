using Microsoft.AspNetCore.SignalR;
using minesweeper_api.Services;

namespace minesweeper_api.Filters;

public class GameHubLobbyFilter : IHubFilter
{
    public async ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object>> next)
    {
        var emailClaim = invocationContext.Context.User?.Claims.FirstOrDefault(n => n.Type.Contains("email"));
        if (emailClaim is null)
        {
            throw new HubException("User authorization failed. User doesn't specify the email");
        }
        try
        {
            var lobbyIdString = invocationContext.Context.GetHttpContext()?.Request.Query["lobbyId"];
            if (!int.TryParse(lobbyIdString, out int lobbyId))
            {
                invocationContext.Context.Abort();
                return new { error = "User authorization failed. User doesn't specify the lobbyId or it is in incorrect format" };
            }
            var gameService = invocationContext.ServiceProvider.GetRequiredService<GameService>();
            var isValid = await gameService.TryValidateUserGameInLobby(lobbyId, emailClaim.Value, out int gameId);
            if (!isValid)
            {
                invocationContext.Context.Abort();
                return new { error = "User authorization failed. User doesn't have access to the game" };
            }
            invocationContext.Context.Items.TryAdd("gameId", gameId);
            return await next(invocationContext);
        }
        catch 
        {
            throw;
        }
    }
    public async Task OnConnectedAsync(HubLifetimeContext invocationContext, Func<HubLifetimeContext, Task> next)
    {
        var emailClaim = invocationContext.Context.User?.Claims.FirstOrDefault(n => n.Type.Contains("email"));
        if (emailClaim is null)
        {
            throw new HubException("User authorization failed. User doesn't specify the email");
        }
        try
        {
            var lobbyIdString = invocationContext.Context.GetHttpContext()?.Request.Query["lobbyId"];
            if (!int.TryParse(lobbyIdString, out int lobbyId))
            {
                invocationContext.Context.Abort();
                return;
            }
            var gameService = invocationContext.ServiceProvider.GetRequiredService<GameService>();
            var isValid = await gameService.TryValidateUserGameInLobby(lobbyId, emailClaim.Value, out int gameId);
            if (!isValid)
            {
                invocationContext.Context.Abort();
                return;
            }
            invocationContext.Context.Items.TryAdd("gameId", gameId);
            await next(invocationContext);
        }
        catch
        {
            throw;
        }
    }  
    public async Task OnDisconnectedAsync(HubLifetimeContext invocationContext, Exception? exception, Func<HubLifetimeContext, Exception?, Task> next)
         {
        var emailClaim = invocationContext.Context.User?.Claims.FirstOrDefault(n => n.Type.Contains("email"));
        if (emailClaim is null)
        {
            throw new HubException("User authorization failed. User doesn't specify the email");
        }
        try
        {
            var lobbyIdString = invocationContext.Context.GetHttpContext()?.Request.Query["lobbyId"];
            if (!int.TryParse(lobbyIdString, out int lobbyId))
            {
                invocationContext.Context.Abort();
                return;
            }
            var gameService = invocationContext.ServiceProvider.GetRequiredService<GameService>();
            var isValid = await gameService.TryValidateUserGameInLobby(lobbyId, emailClaim.Value, out int gameId);
            if (!isValid)
            {
                invocationContext.Context.Abort();
                return;
            }
            invocationContext.Context.Items.TryAdd("gameId", gameId);
            await next(invocationContext, exception);
        }
        catch
        {
            throw;
        }
    }
}