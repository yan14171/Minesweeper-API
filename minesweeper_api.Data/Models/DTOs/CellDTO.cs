using System.ComponentModel.DataAnnotations;
using minesweeper_api.GameLogic;

namespace minesweeper_api.Data.Models.DTOs;

public class CellDTO
{
    public CellDTO()
    {
        
    }
    [Required]
    [Range(0, Board.COLUMN_COUNT)]
    public int X { get; set; }

    [property: Required]
    [property: Range(0, Board.ROW_COUNT)]
    public int Y { get; set; }
    public bool isFlaged { get; set; }
    public bool isRevealed{ get; set; }
    public bool isBomb{ get; set; }
    public int bombCount { get; set; }
}