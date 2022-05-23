using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using minesweeper_api.Data.Interfaces;

namespace minesweeper_api.Data.Repositories.Generic;

public class DapperRepository<T> : IAsyncRepository<T> where T : class
{
    private readonly IConfiguration _config;
    private readonly ILogger<DapperRepository<T>> _logger;

    public string ConnectionString { get; init; }
    public string SQL_SELECT { get; init; } = @$"SELECT * FROM [{typeof(T).Name}]";
    public string SQL_SELECT_BYID { get; init; } = @$"SELECT * FROM [{typeof(T).Name}] WHERE Id = @Id";

    public DapperRepository(IConfiguration config,
                            ILogger<DapperRepository<T>> logger,
                            string connectionStringName = "Default")
    {
        this._config = config;
        this._logger = logger;
        this.ConnectionString = config.GetConnectionString(connectionStringName);
        _logger.LogInformation(ConnectionString);
        _logger.LogInformation(SQL_SELECT);
    }

    public async Task<T> GetById(int id)
    {
        using var connection = new SqlConnection(ConnectionString);
        var obj = await connection.QueryFirstAsync<T>(SQL_SELECT_BYID, param: new { Id = id });
        return obj;
    }

    public async Task<IEnumerable<T>> GetAll()
    {
        using var connection = new SqlConnection(ConnectionString);
        var objs = await connection.QueryAsync<T>(SQL_SELECT);
        _logger.LogInformation($"Queried : {objs}");
        return objs;
    }
}
