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
    private readonly ILogger<StatsController> _logger;

    public StatsController(
        IAsyncManipulator<User> userManipulator,
        IAsyncManipulator<Stat> statManipulator,
        ILogger<StatsController> logger)
    {
        this._userManipulator = userManipulator;
        this._statManipulator = statManipulator;
        this._logger = logger;
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
