namespace minesweeper_api
{
    public class Stat
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public int MinesAtStart { get; set; }

        public int MinesLeft { get; set; }

        public int SecondsTaken { get; set; }
    }
}