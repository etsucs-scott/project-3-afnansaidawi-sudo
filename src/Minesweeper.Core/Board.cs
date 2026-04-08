using System;

namespace Minesweeper.Core;

/// <summary>
/// لوحة اللعبة الرئيسية - تحتوي على الخلايا والألغام
/// </summary>
public class Board
{
    // حجم اللوحة (8 أو 12 أو 16)
    public int Size { get; private set; }

    // عدد الألغام الكلي
    public int MineCount { get; private set; }

    // الرقم المستخدم لتوليد الألغام عشوائياً
    public int Seed { get; private set; }

    // جدول يحتوي على جميع الخلايا
    private readonly Cell[,] cells;

    public Board(int size, int mineCount, int seed)
    {
        Size = size;
        MineCount = mineCount;
        Seed = seed;

        // إنشاء جدول فارغ من الخلايا
        cells = new Cell[size, size];
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                cells[row, col] = new Cell();
            }
        }

        // وضع الألغام وحساب الأرقام
        PlaceMines();
        CalculateAdjacentMines();
    }

    // وضع الألغام بشكل عشوائي باستخدام البذرة
    private void PlaceMines()
    {
        Random random = new Random(Seed);
        int minesPlaced = 0;

        while (minesPlaced < MineCount)
        {
            int row = random.Next(Size);
            int col = random.Next(Size);

            // لا نضع لغم إذا كان فيه لغم بالفعل
            if (!cells[row, col].IsMine)
            {
                cells[row, col].PlaceMine();
                minesPlaced++;
            }
        }
    }

    // حساب كم لغم بجانب كل خلية
    private void CalculateAdjacentMines()
    {
        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                // لا نحسب للخلايا اللي فيها ألغام
                if (cells[row, col].IsMine)
                    continue;

                int count = CountAdjacentMines(row, col);
                cells[row, col].SetAdjacentMines(count);
            }
        }
    }

    // عد الألغام حول خلية معينة
    private int CountAdjacentMines(int row, int col)
    {
        int count = 0;

        // تفقد الـ 8 خلايا حول الخلية
        for (int rOffset = -1; rOffset <= 1; rOffset++)
        {
            for (int cOffset = -1; cOffset <= 1; cOffset++)
            {
                // تخطي الخلية الحالية
                if (rOffset == 0 && cOffset == 0)
                    continue;

                int newRow = row + rOffset;
                int newCol = col + cOffset;

                // تفقد الحدود
                if (IsValidPosition(newRow, newCol))
                {
                    if (cells[newRow, newCol].IsMine)
                        count++;
                }
            }
        }

        return count;
    }

    // كشف خلية - إذا لغم = خسرنا
    public bool Reveal(int row, int col)
    {
        if (!IsValidPosition(row, col))
            return true;

        Cell cell = cells[row, col];

        // لا نكشف لو كانت مُعلّمة
        if (cell.IsFlagged)
            return true;

        if (cell.IsRevealed)
            return true;

        cell.Reveal();

        // إذا لغم = خسرنا
        if (cell.IsMine)
            return false;

        // إذا لا توجد ألغام بجانبها = كشف ما حول
        if (cell.AdjacentMines == 0)
        {
            RevealAdjacent(row, col);
        }

        return true;
    }

    // كشف الخلايا حول إذا ما في ألغام
    private void RevealAdjacent(int row, int col)
    {
        for (int rOffset = -1; rOffset <= 1; rOffset++)
        {
            for (int cOffset = -1; cOffset <= 1; cOffset++)
            {
                if (rOffset == 0 && cOffset == 0)
                    continue;

                int newRow = row + rOffset;
                int newCol = col + cOffset;

                if (IsValidPosition(newRow, newCol))
                {
                    Cell adjacentCell = cells[newRow, newCol];
                    
                    if (!adjacentCell.IsRevealed && !adjacentCell.IsFlagged)
                    {
                        adjacentCell.Reveal();

                        // استمر إذا هذي الخلية ما فيها ألغام
                        if (adjacentCell.AdjacentMines == 0)
                        {
                            RevealAdjacent(newRow, newCol);
                        }
                    }
                }
            }
        }
    }

    // ضع أو أزل علم من خلية
    public void ToggleFlag(int row, int col)
    {
        if (!IsValidPosition(row, col))
            return;

        Cell cell = cells[row, col];

        // لا نعلم الخلايا المكشوفة
        if (cell.IsRevealed)
            return;

        cell.ToggleFlag();
    }

    // هل فزنا؟ (كل الخلايا اللي ما فيها ألغام مكشوفة)
    public bool CheckWin()
    {
        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                Cell cell = cells[row, col];

                if (!cell.IsMine && !cell.IsRevealed)
                    return false;
            }
        }

        return true;
    }

    // شيك إذا الموضع شرعي (على اللوحة)
    public bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < Size && col >= 0 && col < Size;
    }

    // احصل على خلية معينة
    public Cell? GetCell(int row, int col)
    {
        if (!IsValidPosition(row, col))
            return null;

        return cells[row, col];
    }

    // كشف كل الألغام (عند الخسارة)
    public void RevealAllMines()
    {
        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                if (cells[row, col].IsMine)
                {
                    cells[row, col].Reveal();
                }
            }
        }
    }
}
