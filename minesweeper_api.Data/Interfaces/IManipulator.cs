using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minesweeper_api.Data.Interfaces;
public interface IAsyncManipulator<T> : IAsyncCommand<T>, IAsyncRepository<T>
{
}
