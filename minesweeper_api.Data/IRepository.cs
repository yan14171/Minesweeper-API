namespace minesweeper_api.Data;

public interface IRepository<T> where T : class
{
    T GetById(int id);

    IEnumerable<T> GetAll();

    bool Add(T obj);
}
