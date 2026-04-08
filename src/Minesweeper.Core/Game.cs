using System;

namespace Minesweeper.Core;

/// <summary>
/// إدارة اللعبة والحالة العامة
/// </summary>
public class Game
{
    public Board Board { get; private set; }

    // هل اللعبة انتهت
    public bool IsGameOver { get; set; }

    // هل فزنا
    public bool IsWon { get; set; }

    // كم عدد الحركات اللي عملناها
    public int MoveCount { get; private set; }

    // وقت البدء
    public DateTime StartTime { get; private set; }

    public Game(int size, int mineCount, int seed)
    {
        Board = new Board(size, mineCount, seed);

        IsGameOver = false;
        IsWon = false;
        MoveCount = 0;

        StartTime = DateTime.Now;
    }

    // نقل واحد في اللعبة
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
            // كشف الخلية
            if (!Board.Reveal(row, col))
            {
                // اصطدمنا بلغم
                IsGameOver = true;
                Board.RevealAllMines();
                return false;
            }

            // تفقد إذا فزنا
            if (Board.CheckWin())
            {
                IsGameOver = true;
                IsWon = true;
            }
        }

        MoveCount++;

        return true;
    }

    // احسب الوقت المنقضي
    public int GetElapsedTime()
    {
        TimeSpan elapsed = DateTime.Now - StartTime;
        return (int)elapsed.TotalSeconds;
    }

    // احصل على معلومات اللعبة
    public string GetStats()
    {
        return $"Moves: {MoveCount} | Time: {GetElapsedTime()}s | Mines: {Board.MineCount}";
    }
}
