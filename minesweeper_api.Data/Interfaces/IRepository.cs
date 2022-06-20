namespace minesweeper_api.Data.Interfaces;

public interface IRepository<T>
{
    T GetById(int id);

    IEnumerable<T> GetAll();
}
public interface IAsyncRepository<T>
{
    public string ConnectionString { get; init; }
    public string SQL_SELECT { get; init; }
    public string SQL_SELECT_BYID { get; init; }
    Task<T> GetById(int id);

    Task<IEnumerable<T>> GetAll();
}
