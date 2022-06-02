using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using minesweeper_api.GameLogic;

namespace minesweeper_api.Data.Models.DTOs;
public class CellDTO 
{
    [Required]
    [Range(0, Board.COLUMN_COUNT)]
    public int X { get; set; }

    [Required]
    [Range(0, Board.ROW_COUNT)]
    public int Y { get; set; }
    public bool isFlaged { get; private set; }
    public bool isRevealed { get; set; }
    public bool isBomb { get; set; }
    public int bombCount { get; set; }
}
