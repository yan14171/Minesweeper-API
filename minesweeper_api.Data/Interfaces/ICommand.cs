using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minesweeper_api.Data.Interfaces;
public interface ICommand<T>
{
    public T Add(T obj);
}
public interface IAsyncCommand<T>
{
    Task<T> AddAsync(T obj);
}