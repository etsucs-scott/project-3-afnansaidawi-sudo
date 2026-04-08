using System;

namespace Minesweeper.Core;

/// <summary>
/// A single cell in the game board
/// </summary>
public class Cell
{
    // Does the cell contain a mine
    public bool IsMine { get; private set; }

    // Is the cell revealed or hidden
    public bool IsRevealed { get; private set; }

    // Is the cell flagged
    public bool IsFlagged { get; private set; }

    // Number of adjacent mines
    public int AdjacentMines { get; private set; }

    public Cell()
    {
        IsMine = false;
        IsRevealed = false;
        IsFlagged = false;
        AdjacentMines = 0;
    }

    // Reveal the cell
    public void Reveal()
    {
        IsRevealed = true;
    }

    // Put or remove a flag from the cell
    public void ToggleFlag()
    {
        IsFlagged = !IsFlagged;
    }

    // Place a mine in this cell
    public void PlaceMine()
    {
        IsMine = true;
    }

    // Set the number of adjacent mines
    public void SetAdjacentMines(int count)
    {
        AdjacentMines = count;
    }

    // Reset the cell
    public void Reset()
    {
        IsMine = false;
        IsRevealed = false;
        IsFlagged = false;
        AdjacentMines = 0;
    }
}
