using System.Text.Json.Serialization;

namespace minesweeper_api.GameLogic;
public class Board
{
    public const byte ROW_COUNT = 16;
    public const byte COLUMN_COUNT = 30;
    [Serializable]
    public struct BoardState
    {
        public Cell[,] grid { get; set; }
        public byte BombsGenerated { get; set; }
        public byte BombsLeft { get; set; }
        public bool isGameOver { get; set; }
        public bool isStarted { get; set; }
    }
    public BoardState State { get; private set; }
    public Cell[,] GetRows()
    {
        return State.grid;
    }
    public void EndGame()
    {
        revealBombs();
        State = State with { isGameOver = true, isStarted = false };
    }
    public void StartGame(int startX, int startY)
    {
        do {
            generateClean(ROW_COUNT, COLUMN_COUNT);
            generateMines(State.BombsGenerated);
            generateNumbers(ROW_COUNT, COLUMN_COUNT);
        }
        while (State.grid[startY, startX].IsBomb || State.grid[startY, startX].BombCount != 0);

        State = State with { isStarted = true, BombsLeft = State.BombsGenerated };
        State.grid[startY, startX].Reveal();
    }
    public void AddBomb()
    {
        State = State with { BombsLeft = (byte)(State.BombsLeft + 1) };
    }
    public void RemoveBomb()
    {
        State = State with { BombsLeft = (byte)(State.BombsLeft - 1) };
    }
    public void PrepareGame()
    {
        generateClean(ROW_COUNT, COLUMN_COUNT);
        try {
            var mineCount = (byte)getMineCount(ROW_COUNT, COLUMN_COUNT);
            State = State with { BombsGenerated = mineCount, BombsLeft = mineCount };
        }
        catch (InvalidCastException e) {
            throw new InvalidOperationException("Mine count generated should not be more than 255", e);
        }

    }
    private void generateNumbers(byte rowCount, byte columnCount)
    {
        for (var i = 0; i < rowCount; i++)
            for (var j = 0; j < columnCount; j++)
                State.grid[i, j].CountBombs();
    }
    private void generateClean(byte rowCount, byte columnCount)
    {
        var newGrid = new Cell[rowCount, columnCount];
        for (var i = 0; i < rowCount; i++) {
            for (var j = 0; j < columnCount; j++) {
                var curCell = new Cell(
                  j,
                  i,
                  false,//isRevealed
                  false,//isBomb
                  this,//board field
                  0, //neighbour bomb count
                  false);

                newGrid[i, j] = curCell;
            }
        }
        State = State with { grid = newGrid };
    }
    private void generateMines(int mineCount)
    {
        var mineLocations = new List<(int X, int Y)>();
        var rand = new Random();

        while (mineLocations.Count < mineCount) {
            var mineRow = (int)(rand.NextDouble() * ROW_COUNT);
            var mineColumn = (int)(rand.NextDouble() * COLUMN_COUNT);

            if (mineLocations.FindIndex(n => n.Y == mineRow && n.X == mineColumn) == -1) {
                mineLocations.Add((mineColumn, mineRow));
                State.grid[mineRow, mineColumn].IsBomb = true;
            }
        }
    }
    private int getMineCount(byte rowCount, byte columnCount)
    {
        var cellCount = rowCount * columnCount;
        var mineCount = 0.0002 * Math.Pow(cellCount, 2) + 0.0938 * cellCount + 0.8937;
        //let mineCount = cellCount; //full mine field for testing 
        //let mineCount = 1; //1 mine for testing 

        return (int)mineCount;
    }
    private void revealBombs()
    {
        foreach (var cell in State.grid) {
            if (cell.IsBomb)
                cell.IsRevealed = true;
        }
    }
}
