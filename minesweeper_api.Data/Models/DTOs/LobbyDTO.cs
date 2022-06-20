using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minesweeper_api.Data.Models.DTOs;

public class LobbyDTO
{
    public int id { get; set; }
    public bool isStarted { get; set; }
    public IEnumerable<string> userIdentities { get; set; }
}
