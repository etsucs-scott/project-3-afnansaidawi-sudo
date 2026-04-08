using Minesweeper.Core;
using Xunit;

// اختبارات للعبة Minesweeper
public class MinesweeperTests
{
    // اختبار: اللوحة تُنشأ بالحجم الصحيح
    [Fact]
    public void Board_CreatedWithCorrectSize()
    {
        Board board = new Board(8, 10, 42);
        Assert.Equal(8, board.Size);
    }

    // اختبار: عدد الألغام صحيح
    [Fact]
    public void Board_CorrectMineCount()
    {
        Board board = new Board(8, 10, 42);
        int mineCount = 0;

        for (int row = 0; row < board.Size; row++)
        {
            for (int col = 0; col < board.Size; col++)
            {
                Cell? cell = board.GetCell(row, col);
                if (cell != null && cell.IsMine)
                    mineCount++;
            }
        }

        Assert.Equal(10, mineCount);
    }

    // اختبار: نفس البذرة = نفس الألغام
    [Fact]
    public void Board_SameSeedProducesSameMines()
    {
        Board board1 = new Board(8, 10, 12345);
        Board board2 = new Board(8, 10, 12345);

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Cell? cell1 = board1.GetCell(row, col);
                Cell? cell2 = board2.GetCell(row, col);

                Assert.NotNull(cell1);
                Assert.NotNull(cell2);
                Assert.Equal(cell1.IsMine, cell2.IsMine);
            }
        }
    }

    // اختبار: بذور مختلفة = ألغام مختلفة (غالباً)
    [Fact]
    public void Board_DifferentSeedsProduceDifferentMines()
    {
        Board board1 = new Board(8, 10, 111);
        Board board2 = new Board(8, 10, 222);

        bool minesDifferent = false;
        for (int row = 0; row < 8 && !minesDifferent; row++)
        {
            for (int col = 0; col < 8 && !minesDifferent; col++)
            {
                Cell? cell1 = board1.GetCell(row, col);
                Cell? cell2 = board2.GetCell(row, col);

                if (cell1 != null && cell2 != null && cell1.IsMine != cell2.IsMine)
                    minesDifferent = true;
            }
        }

        Assert.True(minesDifferent);
    }

    // اختبار: حساب الألغام المجاورة
    [Fact]
    public void Board_AdjacentMinesCountedCorrectly()
    {
        Board board = new Board(8, 10, 42);

        for (int row = 0; row < board.Size; row++)
        {
            for (int col = 0; col < board.Size; col++)
            {
                Cell? cell = board.GetCell(row, col);
                Assert.NotNull(cell);

                if (!cell.IsMine)
                {
                    Assert.InRange(cell.AdjacentMines, 0, 8);
                }
            }
        }
    }

    // اختبار: الكشف المتسلسل يعمل
    [Fact]
    public void Board_CascadeRevealWorksForZeroAdjacentMines()
    {
        Board board = new Board(12, 25, 99);
        int cascadeRow = -1;
        int cascadeCol = -1;

        for (int row = 0; row < board.Size && cascadeRow == -1; row++)
        {
            for (int col = 0; col < board.Size && cascadeCol == -1; col++)
            {
                Cell? cell = board.GetCell(row, col);
                if (cell != null && cell.AdjacentMines == 0 && !cell.IsMine)
                {
                    cascadeRow = row;
                    cascadeCol = col;
                }
            }
        }

        if (cascadeRow != -1)
        {
            board.Reveal(cascadeRow, cascadeCol);

            bool cascadeOccurred = false;
            for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
            {
                for (int colOffset = -1; colOffset <= 1; colOffset++)
                {
                    int adjacentRow = cascadeRow + rowOffset;
                    int adjacentCol = cascadeCol + colOffset;

                    if (board.IsValidPosition(adjacentRow, adjacentCol))
                    {
                        Cell? adjacentCell = board.GetCell(adjacentRow, adjacentCol);
                        if (adjacentCell != null && !adjacentCell.IsMine && adjacentCell.IsRevealed)
                            cascadeOccurred = true;
                    }
                }
            }

            Assert.True(cascadeOccurred);
        }
    }

    // اختبار: الخلايا المعلمة لا تُكشف
    [Fact]
    public void Board_FlaggedCellCannotBeRevealed()
    {
        Board board = new Board(8, 10, 42);

        board.ToggleFlag(0, 0);
        Cell? cell = board.GetCell(0, 0);
        bool initiallyFlagged = cell?.IsFlagged ?? false;

        board.Reveal(0, 0);
        cell = board.GetCell(0, 0);
        bool stillUnrevealed = !cell?.IsRevealed ?? false;

        Assert.True(initiallyFlagged);
        Assert.True(stillUnrevealed);
    }

    // اختبار: الخسارة تُكتشف عند اللغم
    [Fact]
    public void Game_LossDetectedWhenMineHit()
    {
        Board board = new Board(8, 10, 42);

        int mineRow = -1;
        int mineCol = -1;
        for (int row = 0; row < board.Size && mineRow == -1; row++)
        {
            for (int col = 0; col < board.Size && mineCol == -1; col++)
            {
                Cell? cell = board.GetCell(row, col);
                if (cell != null && cell.IsMine)
                {
                    mineRow = row;
                    mineCol = col;
                }
            }
        }

        Game game = new Game(8, 10, 42);
        bool revealResult = game.MakeMove(mineRow, mineCol, false);

        Assert.False(revealResult);
        Assert.True(game.IsGameOver);
        Assert.False(game.IsWon);
    }

    // اختبار: الأعلام يمكن تبديلها
    [Fact]
    public void Board_FlagCanBeToggled()
    {
        Board board = new Board(8, 10, 42);
        Cell? cell = board.GetCell(0, 0);

        Assert.NotNull(cell);
        Assert.False(cell.IsFlagged);

        board.ToggleFlag(0, 0);
        cell = board.GetCell(0, 0);
        Assert.True(cell?.IsFlagged);

        board.ToggleFlag(0, 0);
        cell = board.GetCell(0, 0);
        Assert.False(cell?.IsFlagged);
    }

    // اختبار: الفوز يُكتشف
    [Fact]
    public void Game_WinDetectionWorks()
    {
        Board board = new Board(8, 10, 42);

        for (int row = 0; row < board.Size; row++)
        {
            for (int col = 0; col < board.Size; col++)
            {
                Cell? cell = board.GetCell(row, col);
                if (cell != null && !cell.IsMine)
                {
                    cell.Reveal();
                }
            }
        }

        Assert.True(board.CheckWin());
    }

    // اختبار: لا فوز إذا كانت خلايا مختفية
    [Fact]
    public void Game_NotWonIfCellsStillHidden()
    {
        Board board = new Board(8, 10, 42);

        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                Cell? cell = board.GetCell(row, col);
                if (cell != null && !cell.IsMine)
                {
                    cell.Reveal();
                }
            }
        }

        Assert.False(board.CheckWin());
    }

    // اختبار: فحص المواضع الصحيحة
    [Fact]
    public void Board_InvalidPositionDetection()
    {
        Board board = new Board(8, 10, 42);

        Assert.True(board.IsValidPosition(0, 0));
        Assert.True(board.IsValidPosition(7, 7));
        Assert.False(board.IsValidPosition(-1, 0));
        Assert.False(board.IsValidPosition(0, -1));
        Assert.False(board.IsValidPosition(8, 0));
        Assert.False(board.IsValidPosition(0, 8));
        Assert.False(board.IsValidPosition(10, 10));
    }

    // اختبار: عد الحركات يزيد
    [Fact]
    public void Game_MovesCountIncrement()
    {
        Game game = new Game(8, 10, 42);

        game.MakeMove(0, 0, false);
        game.MakeMove(1, 1, true);
        game.MakeMove(2, 2, false);

        Assert.Equal(3, game.MoveCount);
    }

    // اختبار: الوقت يزيد
    [Fact]
    public void Game_ElapsedTimeIncreases()
    {
        Game game = new Game(8, 10, 42);
        int time1 = game.GetElapsedTime();

        System.Threading.Thread.Sleep(100);
        int time2 = game.GetElapsedTime();

        Assert.True(time2 >= time1);
    }

    // اختبار: جميع الأحجام تُنشأ
    [Fact]
    public void Board_AllSizesCanBeCreated()
    {
        Board board8 = new Board(8, 10, 1);
        Assert.Equal(8, board8.Size);

        Board board12 = new Board(12, 25, 2);
        Assert.Equal(12, board12.Size);

        Board board16 = new Board(16, 40, 3);
        Assert.Equal(16, board16.Size);
    }

    // اختبار: الكشف مرتين آمن
    [Fact]
    public void Board_RevealingCellTwiceIsIdempotent()
    {
        Board board = new Board(8, 10, 42);

        for (int row = 0; row < board.Size; row++)
        {
            for (int col = 0; col < board.Size; col++)
            {
                Cell? cell = board.GetCell(row, col);
                if (cell != null && !cell.IsMine)
                {
                    board.Reveal(row, col);
                    bool afterFirstReveal = cell.IsRevealed;

                    board.Reveal(row, col);
                    bool afterSecondReveal = cell.IsRevealed;

                    Assert.True(afterFirstReveal);
                    Assert.True(afterSecondReveal);
                    return;
                }
            }
        }
    }

    // اختبار: عدد الألغام يطابق
    [Fact]
    public void Game_MineCountMatches()
    {
        Game game = new Game(12, 25, 42);
        int actualMineCount = game.Board.MineCount;
        Assert.Equal(25, actualMineCount);
    }
}
