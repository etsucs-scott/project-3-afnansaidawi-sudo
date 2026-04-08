using System;

namespace Minesweeper.Core;

/// <summary>
/// Game management and general state
/// </summary>
public class Game
{
    public Board Board { get; private set; }

    // Is the game over
    public bool IsGameOver { get; set; }

    // Did we win
    public bool IsWon { get; set; }

    // How many moves we made
    public int MoveCount { get; private set; }

    // Start time
    public DateTime StartTime { get; private set; }

    public Game(int size, int mineCount, int seed)
    {
        Board = new Board(size, mineCount, seed);

        IsGameOver = false;
        IsWon = false;
        MoveCount = 0;

        StartTime = DateTime.Now;
    }

    // Make one move in the game
    public bool MakeMove(int row, int col, bool isFlag)
    {
        if (IsGameOver)
            return true;

        if (!Board.IsValidPosition(row, col))
            return true;

        if (isFlag)
        {
            Board.ToggleFlag(row, col);
        }
        else
        {
            // Reveal the cell
            if (!Board.Reveal(row, col))
            {
                // We hit a mine
                IsGameOver = true;
                Board.RevealAllMines();
                return false;
            }

            // Check if we won
            if (Board.CheckWin())
            {
                IsGameOver = true;
                IsWon = true;
            }
        }

        MoveCount++;

        return true;
    }

    // Calculate elapsed time
    public int GetElapsedTime()
    {
        TimeSpan elapsed = DateTime.Now - StartTime;
        return (int)elapsed.TotalSeconds;
    }

    // Get game information
    public string GetStats()
    {
        return $"Moves: {MoveCount} | Time: {GetElapsedTime()}s | Mines: {Board.MineCount}";
    }
}
