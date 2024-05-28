using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using minesweeper_api.Data.Interfaces;
using minesweeper_api.Data.Models;

namespace minesweeper_api.Data.Repositories.Concrete;
public class StatManipulator : IAsyncManipulator<Stat>
{
    public string ConnectionString { get => _asyncRepository.ConnectionString; init => ConnectionString = value; }
    public string SQL_SELECT { get => _asyncRepository.SQL_SELECT; init => SQL_SELECT = value; }
    public string SQL_SELECT_BYID { get => _asyncRepository.SQL_SELECT_BYID; init => SQL_SELECT_BYID = value; }
    public string SQL_SELECT_WITHUSER { get; set; } =
              @"SELECT s.[UserEmail], s.[Date], s.[MinesAtStart], s.[MinesLeft], s.[SecondsTaken], s.[RevealMovesMade], s.[FlagMovesMade], 'split' as split, u.[Name]
                FROM Stat s 
                INNER JOIN [User] u ON s.[UserEmail] = u.[Email]";
    private readonly string SQL_INSERT_STAT =
    @"INSERT INTO [dbo].[Stat]
           ([UserEmail]
           ,[Date]
           ,[MinesAtStart]
           ,[MinesLeft]
           ,[SecondsTaken]
           ,[RevealMovesMade]
           ,[FlagMovesMade])
     VALUES
           (@UserEmail
           ,@Date
           ,@MinesAtStart
           ,@MinesLeft
           ,@SecondsTaken
           ,@RevealMovesMade
           ,@FlagMovesMade)";
    private readonly string SQL_DELETE_STAT =
    @"DELETE FROM [dbo].[Stat]
      WHERE 
	 [UserEmail] = @UserEmail and
     [Date] = @Date and 
     [MinesAtStart] = @MinesAtStart and
     [MinesLeft] = @MinesLeft and 
     [SecondsTaken] = @SecondsTaken and 
     [RevealMovesMade] = @RevealMovesMade and 
     [FlagMovesMade] = @FlagMovesMade";

    private readonly IAsyncRepository<Stat> _asyncRepository;

    public StatManipulator(IAsyncRepository<Stat> asyncRepository)
    {
        this._asyncRepository = asyncRepository;
    }

    public async Task<Stat> AddAsync(Stat obj)
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.ExecuteAsync(SQL_INSERT_STAT, param: obj);
        return obj;
    }

    public async Task<IEnumerable<Stat>> GetAll()
    {
        using var connection = new SqlConnection(ConnectionString);

        var stats = await connection.QueryAsync<Stat, User, Stat>(SQL_SELECT_WITHUSER, (stat, user) => {
            stat.UserName = user.Name;
            return stat;
        },
        splitOn: "split");

        return stats;
    }

    public Task<Stat> GetById(int id) => _asyncRepository.GetById(id);

    public async Task<Stat> RemoveAsync(Stat obj)
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.ExecuteAsync(SQL_DELETE_STAT, param: obj);
        return obj;
    }
}
