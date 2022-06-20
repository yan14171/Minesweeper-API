using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using minesweeper_api.Data.Interfaces;
using minesweeper_api.Data.Models;
using minesweeper_api.Data.Repositories.Concrete;
using minesweeper_api.Data.Repositories.Generic;
using minesweeper_api.Filters;
using minesweeper_api.Hubs;
using minesweeper_api.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

var jwt_key = GetToken();
ConfigureServices(jwt_key);
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(conf => ConfigureJWTOptions(conf, jwt_key));

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors(builder =>
{
    builder.WithOrigins("http://localhost:4200")
    .SetIsOriginAllowed(_ => true)
        .AllowAnyHeader()
        .WithMethods("GET", "POST", "PATCH", "DELETE")
        .AllowCredentials();
});

app.UseAuthorization();

app.MapControllers();

app.MapHub<GameHub>("/game");

app.MapGet("/state", (IHubContext<GameHub, IGame> hub) =>
{
    hub.Clients.All.StateChange(
        new minesweeper_api.Data.Models.DTOs.BoardStateDTO
        (
            new minesweeper_api.Data.Models.DTOs.CellDTO[30, 60],
            1,
            1,
            false,
            false
        ));
    return Results.Ok("StateChanged sent!");
});
app.MapGet("/heathcheck", () => "Checked!");

app.Run();

static byte[] GetToken()
{
    var data = Environment.GetEnvironmentVariables();

    var key = data["minesweeper_jwt_key"] as string;

    if (key is null)
        throw new InvalidOperationException("Couldn't find the environmental jwt key");

    return Encoding.ASCII.GetBytes(key);
}
void ConfigureServices(byte[] jwt_key = default)
{
    if (jwt_key != default)
        ConfigureJWT(jwt_key);
    
    builder.Services.AddResponseCompression(ops =>
    {
        ops.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            new[] { "application/octet-stream" });
    });
    builder.Services.AddSignalR().AddNewtonsoftJsonProtocol().AddHubOptions<GameHub>((options) =>
    {
        options.EnableDetailedErrors = true;
        options.AddFilter<GameHubLobbyFilter>();
    });
    builder.Services.AddSingleton<IAsyncRepository<User>, DapperRepository<User>>();
    builder.Services.AddSingleton<IAsyncRepository<Stat>, DapperRepository<Stat>>();
    builder.Services.AddSingleton<IAsyncCommand<Stat>, StatManipulator>();
    builder.Services.AddSingleton<IAsyncManipulator<Stat>, StatManipulator>();
    builder.Services.AddSingleton<IAsyncManipulator<User>, UserManipulator>();
    builder.Services.AddSingleton<IRepository<User>, LocalRepository<User>>();
    builder.Services.AddSingleton<IRepository<Stat>, LocalRepository<Stat>>();
    builder.Services.AddSingleton<ILobbyManipulator, LocalLobbyManipulator>();
    builder.Services.AddSingleton<IBoardManipulator, LocalBoardManipulator>();
    builder.Services.AddSingleton<GameService>();
}
void ConfigureJWT(byte[] jwt_key)
{
    var jwtConfig = new JwtConfig { Secret = Encoding.ASCII.GetString(jwt_key) };
    builder.Services.AddSingleton<JwtConfig>(jwtConfig);
}
JwtBearerOptions ConfigureJWTOptions(JwtBearerOptions conf, byte[] jwt_key)
{
    conf.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        { 
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/game"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
    conf.SaveToken = true;
    conf.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(jwt_key),
        ValidateIssuer = false, //
        ValidateAudience = false, //
        RequireExpirationTime = false,
        ValidateLifetime = true,
    };

    return conf;
}