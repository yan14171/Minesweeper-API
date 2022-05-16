namespace minesweeper_api.Data;

public class LocalRepository<T> : IRepository<T> where T : class
{
    private readonly List<T> _items;

    public LocalRepository()
    {
        _items = new List<T>();

    }
    public LocalRepository(IEnumerable<T> startingItems)
    {
        _items = new List<T>(startingItems);        
    }
    
    public IEnumerable<T> GetAll()
    {
        return _items.ToList();
    }

    public T GetById(int id)
    {
        var item = _items.ElementAtOrDefault(id);

        return item;
    }

    public bool Add(T obj)
    {
        if(!_items.Contains(obj))
        {
            _items.Add(obj);
            return true;
        }
        return false;
    }
}
