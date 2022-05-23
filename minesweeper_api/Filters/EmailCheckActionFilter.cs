using Microsoft.AspNetCore.Mvc.Filters;
using minesweeper_api.Data.Models;

namespace minesweeper_api.Filters;

public class EmailCheckActionFilter : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var model = context.ActionArguments.Select(n => n.Value).FirstOrDefault(n => n is Stat) as Stat;
        if (model?.UserEmail is not null)
            await next();

        var emailClaim = context.HttpContext.User.Claims.FirstOrDefault(n => n.Type.Contains("email"));
        if (emailClaim is null)
            throw new UnauthorizedAccessException("Current User's email is not confirmed. Try and login again");

        model.UserEmail = emailClaim.Value;
        await next();
    }
}

