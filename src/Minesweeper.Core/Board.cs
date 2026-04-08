using System;

namespace Minesweeper.Core;

/// <summary>
/// Main game board - contains cells and mines
/// </summary>
public class Board
{
    // Size of the board (8 or 12 or 16)
    public int Size { get; private set; }

    // Total number of mines
    public int MineCount { get; private set; }

    // The number used to generate mines randomly
    public int Seed { get; private set; }

    // Array containing all cells
    private readonly Cell[,] cells;

    public Board(int size, int mineCount, int seed)
    {
        Size = size;
        MineCount = mineCount;
        Seed = seed;

        // Create an empty array of cells
        cells = new Cell[size, size];
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                cells[row, col] = new Cell();
            }
        }

        // Place mines and calculate numbers
        PlaceMines();
        CalculateAdjacentMines();
    }

    // Place mines randomly using the seed
    private void PlaceMines()
    {
        Random random = new Random(Seed);
        int minesPlaced = 0;

        while (minesPlaced < MineCount)
        {
            int row = random.Next(Size);
            int col = random.Next(Size);

            // Don't place a mine if there is already one
            if (!cells[row, col].IsMine)
            {
                cells[row, col].PlaceMine();
                minesPlaced++;
            }
        }
    }

    // Calculate number of mines adjacent to each cell
    private void CalculateAdjacentMines()
    {
        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                // Don't count for cells that have mines
                if (cells[row, col].IsMine)
                    continue;

                int count = CountAdjacentMines(row, col);
                cells[row, col].SetAdjacentMines(count);
            }
        }
    }

    // Count mines around a specific cell
    private int CountAdjacentMines(int row, int col)
    {
        int count = 0;

        // Check the 8 surrounding cells
        for (int rOffset = -1; rOffset <= 1; rOffset++)
        {
            for (int cOffset = -1; cOffset <= 1; cOffset++)
            {
                // Skip the current cell
                if (rOffset == 0 && cOffset == 0)
                    continue;

                int newRow = row + rOffset;
                int newCol = col + cOffset;

                // Check boundaries
                if (IsValidPosition(newRow, newCol))
                {
                    if (cells[newRow, newCol].IsMine)
                        count++;
                }
            }
        }

        return count;
    }

    // Reveal a cell - if mine = we lose
    public bool Reveal(int row, int col)
    {
        if (!IsValidPosition(row, col))
            return true;

        Cell cell = cells[row, col];

        // Don't reveal if flagged
        if (cell.IsFlagged)
            return true;

        if (cell.IsRevealed)
            return true;

        cell.Reveal();

        // If mine = we lose
        if (cell.IsMine)
            return false;

        // If there are no adjacent mines = reveal surrounding
        if (cell.AdjacentMines == 0)
        {
            RevealAdjacent(row, col);
        }

        return true;
    }

    // Reveal cells around if no mines
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

                        // Continue if this cell has no mines
                        if (adjacentCell.AdjacentMines == 0)
                        {
                            RevealAdjacent(newRow, newCol);
                        }
                    }
                }
            }
        }
    }

    // Put or remove a flag from a cell
    public void ToggleFlag(int row, int col)
    {
        if (!IsValidPosition(row, col))
            return;

        Cell cell = cells[row, col];

        // Don't flag revealed cells
        if (cell.IsRevealed)
            return;

        cell.ToggleFlag();
    }

    // Did we win? (all non-mine cells are revealed)
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

    // Check if position is valid (on the board)
    public bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < Size && col >= 0 && col < Size;
    }

    // Get a specific cell
    public Cell? GetCell(int row, int col)
    {
        if (!IsValidPosition(row, col))
            return null;

        return cells[row, col];
    }

    // Reveal all mines (on loss)
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
