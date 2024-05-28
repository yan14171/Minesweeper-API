using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Text.Json.Serialization;

namespace minesweeper_api.Data.Models;

public class Stat
{
    [JsonPropertyName("name")]
    public string? UserName { get; set; }

    [JsonPropertyName("email")]
    public string? UserEmail { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public int MinesAtStart { get; set; }

    [Required]
    public int MinesLeft { get; set; }

    [Required]
    public int SecondsTaken { get; set; }

    public int RevealMovesMade { get; set; }

    public int FlagMovesMade { get; set; }
}
