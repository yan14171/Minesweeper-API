using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minesweeper_api.Data.Models
{
    public class Lobby
    {
        public int? Id { get; set; }
        public List<string>? UserIdentifiers { get; set; } = new();
        public bool? IsStarted { get; set; } = false;
    }
}
