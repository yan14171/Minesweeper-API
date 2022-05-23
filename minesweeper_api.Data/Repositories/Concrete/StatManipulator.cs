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
              @"SELECT s.[UserEmail], s.[Date], s.[MinesAtStart], s.[MinesLeft], s.[SecondsTaken], 'split' as split, u.[Name]
                FROM Stat s 
                INNER JOIN [User] u ON s.[UserEmail] = u.[Email]";
    private readonly IAsyncRepository<Stat> _asyncRepository;

    private readonly string SQL_INSERT_STAT =
        @"INSERT INTO [dbo].[Stat]
           ([UserEmail]
           ,[Date]
           ,[MinesAtStart]
           ,[MinesLeft]
           ,[SecondsTaken])
     VALUES
           (@UserEmail
           ,@Date
           ,@MinesAtStart
           ,@MinesLeft
           ,@SecondsTaken)";

    public StatManipulator(IAsyncRepository<Stat> asyncRepository)
    {
        this._asyncRepository = asyncRepository;
    }

    public async Task<Stat> AddAsync(Stat obj)
    {
        var connection = new SqlConnection(ConnectionString);
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

    public Task<Stat> GetById(int id)
    {
        return _asyncRepository.GetById(id);
    }
}
