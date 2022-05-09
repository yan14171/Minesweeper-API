using Microsoft.AspNetCore.Mvc;
using minesweeper_api;

var stats = new List<Stat>
{
    new Stat(){ Name = "Player", Date = DateTime.Now, MinesAtStart = 91, MinesLeft = 0, SecondsTaken = 100 },
    new Stat(){ Name = "Player1", Date = DateTime.Now, MinesAtStart = 91, MinesLeft = 0, SecondsTaken = 100 },
    new Stat(){ Name = "Player2", Date = DateTime.Now, MinesAtStart = 91, MinesLeft = 0, SecondsTaken = 100 },
    new Stat(){ Name = "Player3", Date = DateTime.Now, MinesAtStart = 91, MinesLeft = 0, SecondsTaken = 100 },
    new Stat(){ Name = "Player4", Date = DateTime.Now, MinesAtStart = 91, MinesLeft = 0, SecondsTaken = 100 },
    new Stat(){ Name = "Player5", Date = DateTime.Now, MinesAtStart = 91, MinesLeft = 0, SecondsTaken = 100 },
    new Stat(){ Name = "Player6", Date = DateTime.Now, MinesAtStart = 91, MinesLeft = 0, SecondsTaken = 100 },
    new Stat(){ Name = "Player7", Date = DateTime.Now, MinesAtStart = 91, MinesLeft = 0, SecondsTaken = 100 },
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("stats", () =>
{
    return stats;
});

app.MapPost("stats", ([FromBody] Stat stat) =>
{
    stats.Add(stat);
    return Results.Ok(stat);
});
 
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.Run();
