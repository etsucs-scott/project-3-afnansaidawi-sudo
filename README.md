# Minesweeper Console Game - C#

A beginner-friendly Minesweeper game written in C# following professional software architecture practices.

## 📁 Project Structure

```
Minesweeper/
├── Minesweeper.sln
├── README.md
├── data/
│   └── highscores.csv
└── src/
    ├── Minesweeper.Core/
    │   ├── Cell.cs           # Individual cell/tile logic
    │   ├── Board.cs          # Game board and mechanics
    │   ├── Game.cs           # Game state management
    │   ├── HighScoreManager.cs
    │   └── Minesweeper.Core.csproj
    ├── Minesweeper.Console/
    │   ├── Program.cs        # UI and main entry point
    │   └── Minesweeper.Console.csproj
    └── Minesweeper.Tests/
        ├── MinesweeperTests.cs  # 14+ xUnit test cases
        └── Minesweeper.Tests.csproj
```

## 🎮 Game Features

### Three Difficulty Levels
- **Easy**: 8×8 board with 10 mines
- **Medium**: 12×12 board with 25 mines  
- **Hard**: 16×16 board with 40 mines

### Game Mechanics
- **Deterministic Seeding**: Use the same seed to play identical boards
- **Cascade Reveal**: Automatically reveal empty areas when clicking a cell with 0 adjacent mines
- **Flagging**: Mark suspected mines to avoid accidental clicks
- **High Score Tracking**: CSV-based high score storage with top 5 per difficulty
- **Statistics**: Track time elapsed and number of moves

### Display Symbols
- `#` - Hidden cell
- `f` - Flagged cell
- `.` - Revealed empty cell (0 adjacent mines)
- `1-8` - Number of adjacent mines
- `b` - Hit mine (when game over)

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Windows, macOS, or Linux

### Building the Project

```bash
# Navigate to the project root
cd Minesweeper

# Restore packages
dotnet restore

# Build the solution
dotnet build

# Run the console app
dotnet run --project src/Minesweeper.Console
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

## 📖 How to Play

### Main Menu
```
Select Difficulty:
1) Easy   (8x8, 10 mines)
2) Medium (12x12, 25 mines)
3) Hard   (16x16, 40 mines)
4) View High Scores
5) Exit
```

### Game Commands
During gameplay, enter commands in the format:
- `r 5 3` - Reveal cell at row 5, column 3
- `f 2 4` - Flag/unflag cell at row 2, column 4
- `q` - Quit to menu

### Example Game Session
```
═══════════════════════════════════════════
     MINESWEEPER 8x8 - 10 MINES
═══════════════════════════════════════════

Using seed: 12345

   0  1  2  3  4  5  6  7
 0 #  #  #  #  #  #  #  #
 1 #  #  #  #  #  #  #  #
 2 #  #  #  #  #  #  #  #
 3 #  #  #  #  #  #  #  #
 4 #  #  #  #  #  #  #  #
 5 #  #  #  #  #  #  #  #
 6 #  #  #  #  #  #  #  #
 7 #  #  #  #  #  #  #  #

Moves: 0 | Time: 0s | Mines: 10

Move (r/f row col) or q to quit: r 3 3
```

## 🏗️ Code Architecture

### Core Classes

#### `Cell`
Represents a single tile on the board.
- **Properties**: `IsMine`, `IsRevealed`, `IsFlagged`, `AdjacentMines`
- **Methods**: `Reveal()`, `ToggleFlag()`, `PlaceMine()`, etc.

#### `Board`
Manages the game grid and mechanics.
- **Constructor**: Takes size, mine count, and seed
- **Key Methods**:
  - `PlaceMines()` - Deterministic mine placement
  - `CalculateAdjacentMines()` - Count adjacent mines
  - `Reveal(row, col)` - Reveal with cascade
  - `ToggleFlag(row, col)` - Flag selection
  - `CheckWin()` - Win detection

#### `Game`
Manages overall game state.
- **Properties**: `Board`, `IsGameOver`, `IsWon`, `MoveCount`, `StartTime`
- **Key Methods**:
  - `MakeMove(row, col, isFlag)` - Process moves
  - `GetElapsedTime()` - Calculate game time

#### `HighScoreManager`
Handles persistent high score storage.
- **CSV Format**: `size,seconds,moves,seed,timestamp`
- **Key Methods**:
  - `LoadScores()` - Read from file
  - `SaveScore(score)` - Write to file
  - `GetTop5(size)` - Get best scores for difficulty
  - `IsTopScore(score)` - Check if new top score

### UI Layer

#### `Program`
Console interface with menus and board display.
- **Main Methods**:
  - `MainMenu()` - Difficulty selection
  - `GameLoop(game)` - Main game loop
  - `DisplayBoard(game)` - Render game state
  - `ViewHighScores()` - Show leaderboards

## 📊 Testing

The project includes **14+ xUnit test cases** covering:

✅ Board creation with different sizes  
✅ Deterministic mine placement with seeds  
✅ Adjacent mine counting accuracy  
✅ Cascade reveal functionality  
✅ Flag prevents reveal  
✅ Win condition detection  
✅ Loss condition on mine hit  
✅ Invalid move handling  
✅ Flag toggling  
✅ Move counting  
✅ Elapsed time tracking  

Run tests with:
```bash
dotnet test
```

## 💾 High Score Storage

High scores are stored in `data/highscores.csv` with this format:
```csv
size,seconds,moves,seed,timestamp
8,45,120,12345,2024-04-08 15:30:25
12,120,250,67890,2024-04-08 16:45:10
```

The file is created automatically on first win if it doesn't exist.

## 🎓 Learning Outcomes

This project demonstrates:
- ✅ Object-oriented design with separate concerns
- ✅ 2D array manipulation
- ✅ Recursion (cascade reveal)
- ✅ File I/O with error handling
- ✅ CSV parsing and formatting
- ✅ Unit testing with xUnit
- ✅ Random number generation with seeds
- ✅ Console UI and user input handling
- ✅ Clean code practices (comments, naming, methods under 30 lines)
- ✅ XML documentation comments

## 📝 Code Style

The codebase follows these principles for beginners:
- **Maximum method length**: 30 lines
- **Every class has XML documentation** (`/// <summary>`)
- **Every non-obvious code block has inline comments**
- **Clear variable names** (no abbreviations)
- **Try-catch for file operations**
- **No advanced patterns** - straightforward and readable

## 🐛 Troubleshooting

### "File not found" for high scores
This is normal - the CSV file is created automatically on your first win.

### Seeds don't work consistently
Make sure you're using the same seed number. Seeds are integers; string seeds are converted via `.GetHashCode()`.

### Board won't build
Ensure you have .NET 8.0 or later installed:
```bash
dotnet --version
```

## 📚 Further Learning

To enhance this project:
- Add difficulty presets (custom board/mine counts)
- Implement timer with GUI
- Add sound effects
- Create a graphical UI (WinForms or WPF)
- Add game statistics and win rate tracking
- Implement settings file for customization
- Add multiplayer scoring

---

**Happy Mining! 💣⛏️**
- Implementing the required functionality

---

## Getting Started (CLI)

You may use **Visual Studio**, **VS Code**, or the **terminal**.

### Create a solution
```bash
dotnet new sln -n ProjectName
```

### Create a project (example: console app)
```bash
dotnet new console -n ProjectName.App
```

### Add the project to the solution
```bash
dotnet sln add ProjectName.App
```

### Build and run
```bash
dotnet build
dotnet run --project ProjectName.App
```

## Notes
- Commit early and commit often.
- Your repository history is part of your submission.
- Update this README with build/run instructions specific to your project.
