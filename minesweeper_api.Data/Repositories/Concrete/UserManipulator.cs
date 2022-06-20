using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using minesweeper_api.Data.Interfaces;
using minesweeper_api.Data.Models;

namespace minesweeper_api.Data.Repositories.Concrete;
public class UserManipulator : IAsyncManipulator<User>
{
    private readonly IAsyncRepository<User> _userRepo;
    private readonly string SQL_INSERT_USER = @"INSERT INTO [dbo].[User]
           ([Name]
           ,[Email]
           ,[Password])
     VALUES
           (@Name
           ,@Email
           ,@Password)";
    public UserManipulator(IAsyncRepository<User> userRepo)
    {
        _userRepo = userRepo;
    }
    public string ConnectionString { get => this._userRepo.ConnectionString; init => ConnectionString = value; }
    public string SQL_SELECT { get => this._userRepo.SQL_SELECT; init => SQL_SELECT = value; }
    public string SQL_SELECT_BYID { get => this._userRepo.SQL_SELECT; init => SQL_SELECT = value; }
    private readonly string SQL_DELETE_USER =
    @"DELETE FROM [dbo].[User]
      WHERE 
	 [Email] = @Email and
     [Name] = @Name and 
     [Password] = @Password";
    public async Task<User> AddAsync(User obj)
    {
        var connection = new SqlConnection(ConnectionString);
        await connection.ExecuteAsync(SQL_INSERT_USER, param: obj);
        return obj;
    }

    public Task<IEnumerable<User>> GetAll() => this._userRepo.GetAll();

    public Task<User> GetById(int id) => this._userRepo.GetById(id);

    public async Task<User> RemoveAsync(User obj)
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.ExecuteAsync(SQL_DELETE_USER, obj);
        return obj;
    }
}
