namespace minesweeper_api.GameLogic;

public class Cell
{
    public bool IsFlagged { get; private set; }
    public bool IsRevealed { get; set; }
    public bool IsBomb { get; set; }
    public int BombCount { get; set; }
    public int X { get; }
    public int Y { get; }

    private Board _board;

    public Cell(int X, int Y, bool isRevealed, bool isBomb, Board board, int bombCount, bool isFlagged)
    {
        this.X = X;
        this.Y = Y;
        this.IsRevealed = isRevealed;
        this.IsBomb = isBomb;
        this._board = board;
        this.BombCount = bombCount;
        this.IsFlagged = isFlagged;
    }
    public Cell()
    { }
    public int CountBombs()
    {
        var count = 0;
        for (var i = -1; i <= 1; i++)
            for (var j = -1; j <= 1; j++) {
                var neighbour = getNeighbour(i, j);
                if (neighbour is not null && neighbour.IsBomb)
                    count++;
            }

        BombCount = count;
        return count;
    }
    public int CountFlags()
    {
        var count = 0;
        for (var i = -1; i <= 1; i++)
            for (var j = -1; j <= 1; j++) {
                var neighbour = getNeighbour(i, j);
                if (neighbour is not null && neighbour.IsFlagged)
                    count++;
            }
        return count;
    }
    public void Reveal()
    {
        if (!_board.State.isStarted)
            _board.StartGame(X, Y);

        if (IsFlagged || _board.State.isGameOver)
            return;

        IsRevealed = true;

        if (IsBomb)
            _board.EndGame();

        if (BombCount == 0)
            for (var i = -1; i <= 1; i++)
                for (var j = -1; j <= 1; j++) {
                    var neighbour = getNeighbour(i, j);

                    if (neighbour is null)
                        continue;

                    if (!neighbour.IsRevealed &&
                      !neighbour.IsBomb)
                        neighbour.Reveal();
                }
    }
    public void RevealAround()
    {
        if (!IsRevealed)
            return;
        if (!(CountFlags() == BombCount))
            return;

        for (var i = -1; i <= 1; i++)
            for (var j = -1; j <= 1; j++) {
                var neighbour = getNeighbour(i, j);
                neighbour?.Reveal();
            }

    }
    public void Flag()
    {
        if (IsFlagged) {
            IsFlagged = false;
            _board.AddBomb();
            return;
        }
        if (!IsRevealed) {
            IsFlagged = true;
            _board.RemoveBomb();
            if (IsBomb && _board.State.BombsLeft == 0)
                _board.EndGame();
        }
    }
    private Cell getNeighbour(int dx, int dy)
    {
        var neighbourX = X + dx;
        var neighbourY = Y + dy;
        if (dx == 0 && dy == 0)
            return null;

        if (!inbound(neighbourX, neighbourY))
            return null;

        var neighbour = _board.GetRows()[neighbourY, neighbourX];

        return neighbour;
    }
    private bool inbound(int x, int y)
    {
        return x >= 0 && x < Board.COLUMN_COUNT &&
          y >= 0 && y < Board.ROW_COUNT;
    }
}