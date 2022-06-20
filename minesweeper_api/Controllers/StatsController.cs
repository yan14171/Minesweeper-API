using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minesweeper_api.Data.Interfaces;
using minesweeper_api.Data.Models;
using minesweeper_api.Filters;

namespace minesweeper_api.Controllers;

[ApiController]
[Route("[controller]")]
public class StatsController : ControllerBase
{
    private readonly IAsyncManipulator<User> _userManipulator;
    private readonly IAsyncManipulator<Stat> _statManipulator;

    public StatsController(
        IAsyncManipulator<User> userManipulator,
        IAsyncManipulator<Stat> statManipulator)
    {
        _userManipulator = userManipulator;
        _statManipulator = statManipulator;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [AuthExceptionFilter]
    [EmailCheckActionFilter]
    [NameGenerationActionFilter]
    [HttpPost]
    public async Task<IActionResult> PostStat([FromBody] Stat stat, string? Name)
    {
        var user = (await _userManipulator.GetAll()).FirstOrDefault(n => n.Email == stat.UserEmail);
        if (user is null)
            throw new UnauthorizedAccessException("Current user is not valid. Try and login again");

        stat.UserName = Name ??= user.Name;
        await _statManipulator.AddAsync(stat);
        return Ok(stat);
        // use sql join here, instead of manual referencing
    }

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        return Ok(await _statManipulator.GetAll());
    }
}
