using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace minesweeper_api.Data.Models
{
    public class Stat
    {
        [JsonPropertyName("name")]
        public string? UserName { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int MinesAtStart { get; set; }

        [Required]
        public int MinesLeft { get; set; }

        [Required]
        public int SecondsTaken { get; set; }
    }
}