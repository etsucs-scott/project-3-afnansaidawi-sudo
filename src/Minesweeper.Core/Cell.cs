using System;

namespace Minesweeper.Core;

/// <summary>
/// خلية واحدة في لوحة اللعبة
/// </summary>
public class Cell
{
    // هل الخلية تحتوي على لغم
    public bool IsMine { get; private set; }

    // هل الخلية مكشوفة أم مختفية
    public bool IsRevealed { get; private set; }

    // هل وضعنا علم على الخلية
    public bool IsFlagged { get; private set; }

    // كم عدد الألغام بجانب هذه الخلية
    public int AdjacentMines { get; private set; }

    public Cell()
    {
        IsMine = false;
        IsRevealed = false;
        IsFlagged = false;
        AdjacentMines = 0;
    }

    // كشف الخلية
    public void Reveal()
    {
        IsRevealed = true;
    }

    // ضع أو أزل علم من الخلية
    public void ToggleFlag()
    {
        IsFlagged = !IsFlagged;
    }

    // ضع لغم في هذه الخلية
    public void PlaceMine()
    {
        IsMine = true;
    }

    // حدد عدد الألغام المجاورة
    public void SetAdjacentMines(int count)
    {
        AdjacentMines = count;
    }

    // أعد تعيين الخلية
    public void Reset()
    {
        IsMine = false;
        IsRevealed = false;
        IsFlagged = false;
        AdjacentMines = 0;
    }
}
