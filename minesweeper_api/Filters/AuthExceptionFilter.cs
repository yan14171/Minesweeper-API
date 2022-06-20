using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace minesweeper_api.Filters;

public class AuthExceptionFilter : Attribute, IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exceptionMessages = new List<string>();
        var resultBody = new ObjectResult("")
        {
            ContentTypes = { "application/json" },
        };

        try { context?.ExceptionDispatchInfo?.Throw(); }
        catch (BadHttpRequestException)
        {
            resultBody.StatusCode = StatusCodes.Status400BadRequest;
            exceptionMessages.Add("Bad Request");
        }
        catch (UnauthorizedAccessException)
        {
            resultBody.StatusCode = StatusCodes.Status401Unauthorized;
            exceptionMessages.Add("Unauthorized Access. Invalid or abscent credentials");
        }
        catch (Exception)
        {
            resultBody.StatusCode = StatusCodes.Status403Forbidden;
            exceptionMessages.Add("Unknown error");
        }

        exceptionMessages.AddRange(context.Exception.GetExceptionMessages());
        resultBody.Value = new { IsSuccess = false, Messages = exceptionMessages, Token = "" };
        context.Result = resultBody;
    }
}

static class ExceptionExtensions
{
    public static List<string> GetExceptionMessages(this Exception e)
    {
        if (e == null) return new List<string> { string.Empty };
        
        List<string> msgs = new List<string> { e.Message };
        if (e.InnerException != null)
            msgs.AddRange(GetExceptionMessages(e.InnerException));
        return msgs;
    }
}

