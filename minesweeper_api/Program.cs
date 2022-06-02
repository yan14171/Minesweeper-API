using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using minesweeper_api.Data.Interfaces;
using minesweeper_api.Data.Models;
using minesweeper_api.Data.Models.DTOs;
using minesweeper_api.Data.Repositories.Concrete;
using minesweeper_api.Data.Repositories.Generic;
using minesweeper_api.Hubs;
using minesweeper_api.Services;
using Newtonsoft.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

ConfigureServices(builder);

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = global::Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = global::Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = global::Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(conf => ConfigureJWT(conf, builder));

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(builder =>
{
    builder.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .WithMethods("GET", "POST")
        .AllowCredentials();
});

app.MapHub<GameHub>("/game");

app.MapGet("/state", ( GameService service) =>
{
    service.PrepareGame();
    return JsonConvert.SerializeObject(service.GameState);
});

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
    builder.Services.AddResponseCompression(ops =>
    {
        ops.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            new[] { "application/octet-stream" });
    });
    builder.Services.AddSignalR().AddNewtonsoftJsonProtocol();
    builder.Services.AddSingleton<IAsyncRepository<User>, DapperRepository<User>>();
    builder.Services.AddSingleton<IAsyncRepository<Stat>, DapperRepository<Stat>>();
    builder.Services.AddSingleton<IAsyncCommand<Stat>, StatManipulator>();
    builder.Services.AddSingleton<IAsyncManipulator<Stat>, StatManipulator>();
    builder.Services.AddSingleton<IAsyncManipulator<User>, UserManipulator>();
    builder.Services.AddSingleton<IRepository<User>, LocalRepository<User>>();
    builder.Services.AddSingleton<IRepository<Stat>, LocalRepository<Stat>>();
    builder.Services.AddSingleton<GameService>();
}

static JwtBearerOptions ConfigureJWT(JwtBearerOptions conf, WebApplicationBuilder builder)
{
    var jwt_key = GetToken();
    var jwtConfig = new JwtConfig { Secret = Encoding.ASCII.GetString(jwt_key) };
    builder.Services.AddSingleton<JwtConfig>(jwtConfig);

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