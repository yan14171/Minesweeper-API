using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using minesweeper_api.Data.Interfaces;
using minesweeper_api.Data.Models;
using minesweeper_api.Data.Repositories.Concrete;
using minesweeper_api.Data.Repositories.Generic;
using minesweeper_api.Filters;
using System.Text;

var stats = new List<Stat>
{
    new Stat{ UserName = "Player", Date = DateTime.Now, MinesAtStart = 91, MinesLeft = 0, SecondsTaken = 350 },
    new Stat{ UserName = "Player1", Date = DateTime.Now, MinesAtStart = 91, MinesLeft = 0, SecondsTaken = 782 },
};
var players = new List<User>
{
    new User{ Email = "player@test.com", Name = "Player", Password = "password"},
    new User{ Email = "player1@test.com", Name = "Player1", Password = "password"},
};

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

ConfigureServices(builder);

var jwt_key = GetToken();

var jwtConfig = new JwtConfig { Secret = Encoding.ASCII.GetString(jwt_key) };

builder.Services.AddSingleton<JwtConfig>(jwtConfig);

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(conf =>
    {
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
    });

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.Run();

static byte[] GetToken()
{
    var data = Environment.GetEnvironmentVariables();

    var key = data["minesweeper_jwt_key"] as string;

    if (key is null)
        throw new InvalidOperationException("Couldn't find the environmental jwt key");

    return Encoding.ASCII.GetBytes(key);
}

static void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddSingleton<IAsyncRepository<User>, DapperRepository<User>>();
    builder.Services.AddSingleton<IAsyncRepository<Stat>, DapperRepository<Stat>>();
    builder.Services.AddSingleton<IAsyncCommand<Stat>, StatManipulator>();
    builder.Services.AddSingleton<IAsyncManipulator<Stat>, StatManipulator>();
    builder.Services.AddSingleton<IAsyncManipulator<User>, UserManipulator>();
    builder.Services.AddSingleton<IRepository<User>, LocalRepository<User>>();
    builder.Services.AddSingleton<IRepository<Stat>, LocalRepository<Stat>>();
}