using System;
using System.Collections.Generic;
using Minesweeper.Core;

class Program
{
    static void Main()
    {
        MainMenu();
    }

    // Main menu
    static void MainMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║       MINESWEEPER CONSOLE GAME      ║");
            Console.WriteLine("╚════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("Select Difficulty:");
            Console.WriteLine("1) Easy   (8x8, 10 mines)");
            Console.WriteLine("2) Medium (12x12, 25 mines)");
            Console.WriteLine("3) Hard   (16x16, 40 mines)");
            Console.WriteLine("4) View High Scores");
            Console.WriteLine("5) Exit");
            Console.WriteLine();
            Console.Write("Enter your choice (1-5): ");

            string choice = Console.ReadLine() ?? "";

            // Handle the user's menu choice
            switch (choice)
            {
                case "1":
                    PlayGame(8, 10);
                    break;
                case "2":
                    PlayGame(12, 25);
                    break;
                case "3":
                    PlayGame(16, 40);
                    break;
                case "4":
                    ViewHighScores();
                    break;
                case "5":
                    // Exit the game
                    Console.WriteLine("Thanks for playing!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    // Start a new game with a specific difficulty
    static void PlayGame(int size, int mineCount)
    {
        Console.Clear();
        Console.WriteLine($"═══════════════════════════════════════════");
        Console.WriteLine($"     MINESWEEPER {size}x{size} - {mineCount} MINES");
        Console.WriteLine($"═══════════════════════════════════════════");
        Console.WriteLine();

        // Get the seed from the user
        int seed = GetSeed();
        Console.WriteLine($"Using seed: {seed}");
        Console.WriteLine();

        // Create a new game with the selected parameters
        Game game = new Game(size, mineCount, seed);

        // Run the game loop
        GameLoop(game);
    }

    // Ask the user for a seed number
    static int GetSeed()
    {
        // Prompt the user for a seed
        Console.Write("Enter seed (leave blank for random): ");
        string seedInput = Console.ReadLine() ?? "";

        // If the user didn't enter anything, use the current time's hash
        if (string.IsNullOrWhiteSpace(seedInput))
        {
            return DateTime.Now.GetHashCode();
        }

        // Try to parse the seed as an integer
        if (int.TryParse(seedInput, out int seed))
        {
            return seed;
        }

        // If parsing failed, use the string's hash code
        return seedInput.GetHashCode();
    }

    // Main game loop
    static void GameLoop(Game game)
    {
        // Continue looping until the game is over
        while (!game.IsGameOver)
        {
            // Clear the screen and show the current board state
            Console.Clear();
            DisplayBoard(game);

            // Show game statistics
            Console.WriteLine();
            Console.WriteLine(game.GetStats());
            Console.WriteLine();

            // Get the next move from the player
            GetPlayerMove(game);

            // Add a small delay so the player can see what happened
            System.Threading.Thread.Sleep(300);
        }

        // Game is over - show the result
        Console.Clear();
        DisplayBoard(game);
        Console.WriteLine();

        if (game.IsWon)
        {
            // Player won!
            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║    🎉 CONGRATULATIONS, YOU WON! 🎉  ║");
            Console.WriteLine("╚════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"Time: {game.GetElapsedTime()} seconds");
            Console.WriteLine($"Moves: {game.MoveCount}");

            // Save the high score
            SaveHighScore(game);
        }
        else
        {
            // Player lost!
            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║      💣 GAME OVER - YOU LOST! 💣     ║");
            Console.WriteLine("╚════════════════════════════════════╝");
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to return to menu...");
        Console.ReadKey();
    }

    // Display the current board
    static void DisplayBoard(Game game)
    {
        Board board = game.Board;
        int size = board.Size;

        // Display column headers (0, 1, 2, ... size-1)
        Console.Write("   ");
        for (int col = 0; col < size; col++)
        {
            Console.Write($"{col,2} ");
        }
        Console.WriteLine();

        // Display each row
        for (int row = 0; row < size; row++)
        {
            // Display row number
            Console.Write($"{row,2} ");

            // Display each cell in the row
            for (int col = 0; col < size; col++)
            {
                Cell? cell = board.GetCell(row, col);
                if (cell == null)
                    continue;

                // Display the appropriate symbol based on cell state
                string symbol = GetCellSymbol(cell);
                Console.Write($"{symbol,2} ");
            }

            Console.WriteLine();
        }
    }

    // Get the appropriate symbol for each cell
    static string GetCellSymbol(Cell cell)
    {
        // If the cell is flagged, show 'f'
        if (cell.IsFlagged)
            return "f";

        // If the cell hasn't been revealed yet, show '#'
        if (!cell.IsRevealed)
            return "#";

        // If the cell is a mine, show 'b' (for bomb)
        if (cell.IsMine)
            return "b";

        // If the cell has adjacent mines, show the count
        if (cell.AdjacentMines > 0)
            return cell.AdjacentMines.ToString();

        // If no adjacent mines, show '.' (empty cell)
        return ".";
    }

    // Get player move
    static void GetPlayerMove(Game game)
    {
        while (true)
        {
            Console.Write("Move (r/f row col) or q to quit: ");
            string input = Console.ReadLine() ?? "";

            // Check for quit command
            if (input.Equals("q", StringComparison.OrdinalIgnoreCase))
            {
                game.IsGameOver = true;
                return;
            }

            // Parse the input
            string[] parts = input.Split(' ');

            // Need exactly 3 parts: command, row, col
            if (parts.Length != 3)
            {
                Console.WriteLine("Invalid format. Use: r row col or f row col");
                continue;
            }

            // Parse the command (reveal or flag)
            string command = parts[0].ToLower();
            bool isFlag = command == "f";

            // Check if command is valid
            if (command != "r" && command != "f")
            {
                Console.WriteLine("Command must be 'r' (reveal) or 'f' (flag)");
                continue;
            }

            // Try to parse row and column
            if (!int.TryParse(parts[1], out int row) || !int.TryParse(parts[2], out int col))
            {
                Console.WriteLine("Row and column must be numbers");
                continue;
            }

            // Check if position is valid
            if (!game.Board.IsValidPosition(row, col))
            {
                Console.WriteLine($"Invalid position. Must be 0-{game.Board.Size - 1}");
                continue;
            }

            // Make the move and return
            game.MakeMove(row, col, isFlag);
            return;
        }
    }

    // Save the score if we won
    static void SaveHighScore(Game game)
    {
        // Create a high score entry
        HighScoreManager.HighScore score = new HighScoreManager.HighScore
        {
            Size = game.Board.Size,
            Seconds = game.GetElapsedTime(),
            Moves = game.MoveCount,
            Seed = game.Board.Seed,
            Timestamp = DateTime.Now
        };

        // Save it to the file
        HighScoreManager manager = new HighScoreManager();
        manager.SaveScore(score);

        // Check if it's a top score
        if (manager.IsTopScore(score))
        {
            Console.WriteLine("🏆 NEW HIGH SCORE! 🏆");
        }
    }

    // Display high scores
    static void ViewHighScores()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════╗");
        Console.WriteLine("║          HIGH SCORES TABLE          ║");
        Console.WriteLine("╚════════════════════════════════════╝");
        Console.WriteLine();

        HighScoreManager manager = new HighScoreManager();

        // Display top scores for each difficulty
        DisplayScoresForSize(8, "EASY (8x8)", manager);
        DisplayScoresForSize(12, "MEDIUM (12x12)", manager);
        DisplayScoresForSize(16, "HARD (16x16)", manager);

        Console.WriteLine();
        Console.WriteLine("Press any key to return to menu...");
        Console.ReadKey();
    }

    // Display scores for a specific size
    static void DisplayScoresForSize(int size, string title, HighScoreManager manager)
    {
        Console.WriteLine($"─────────────────────────────────────");
        Console.WriteLine(title);
        Console.WriteLine($"─────────────────────────────────────");

        List<HighScoreManager.HighScore> scores = manager.GetTop5(size);

        if (scores.Count == 0)
        {
            Console.WriteLine("No scores yet. Be the first to win!");
        }
        else
        {
            int rank = 1;
            foreach (var score in scores)
            {
                Console.WriteLine($"{rank}. {score.Seconds}s | {score.Moves} moves | {score.Timestamp:MM/dd HH:mm}");
                rank++;
            }
        }

        Console.WriteLine();
    }
}
