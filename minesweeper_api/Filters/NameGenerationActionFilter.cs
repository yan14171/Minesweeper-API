using Microsoft.AspNetCore.Mvc.Filters;
using minesweeper_api.Data.Models;

namespace minesweeper_api.Filters;

public class NameGenerationActionFilter : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var model = context.ActionArguments.Select(n => n.Value).FirstOrDefault(n => n is Stat) as Stat;
        if (model.UserName is not null)
            await next();

        var generatedName = GenerateName(model.UserEmail);
        context.ActionArguments["Name"] = generatedName;
        await next();
    }

    private string GenerateName(string email)
    {
        if (email == null)
            throw new UnauthorizedAccessException("Email must be provided");
        var @index = email.IndexOf('@');
        if (@index == -1)
            return email;

        var cutEmail = email.Substring(0, index);

        return cutEmail;
    }
}
