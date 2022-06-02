using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minesweeper_api.Data.Models.DTOs;

public record BoardStateDTO(CellDTO[,] grid, byte bombsGenerated, byte bombsLeft, bool isGameOver, bool isStarted);
