using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Minesweeper.Core;

/// <summary>
/// Managing high scores and files
/// </summary>
public class HighScoreManager
{
    private const string FilePath = "data/highscores.csv";

    /// <summary>
    /// A single high score
    /// </summary>
    public class HighScore
    {
        public int Size { get; set; }
        public int Seconds { get; set; }
        public int Moves { get; set; }
        public int Seed { get; set; }
        public DateTime Timestamp { get; set; }
    }

    // Read all high scores from file
    public List<HighScore> LoadScores()
    {
        List<HighScore> scores = new List<HighScore>();

        try
        {
            if (!File.Exists(FilePath))
            {
                return scores;
            }

            string[] lines = File.ReadAllLines(FilePath);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                try
                {
                    string[] parts = line.Split(',');

                    if (parts.Length != 5)
                        continue;

                    if (int.TryParse(parts[0], out int size) &&
                        int.TryParse(parts[1], out int seconds) &&
                        int.TryParse(parts[2], out int moves) &&
                        int.TryParse(parts[3], out int seed) &&
                        DateTime.TryParse(parts[4], out DateTime timestamp))
                    {
                        scores.Add(new HighScore
                        {
                            Size = size,
                            Seconds = seconds,
                            Moves = moves,
                            Seed = seed,
                            Timestamp = timestamp
                        });
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
            return scores;
        }

        return scores;
    }

    // Save a new high score
    public void SaveScore(HighScore score)
    {
        try
        {
            string directory = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string line = $"{score.Size},{score.Seconds},{score.Moves},{score.Seed},{score.Timestamp:yyyy-MM-dd HH:mm:ss}";

            File.AppendAllText(FilePath, line + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving: {ex.Message}");
        }
    }

    // Get top 5 scores for a specific size
    public List<HighScore> GetTop5(int size)
    {
        List<HighScore> allScores = LoadScores();

        List<HighScore> topScores = allScores
            .Where(s => s.Size == size)
            .OrderBy(s => s.Seconds)
            .ThenBy(s => s.Moves)
            .Take(5)
            .ToList();

        return topScores;
    }

    // Check if this is a new high score
    public bool IsTopScore(HighScore score)
    {
        List<HighScore> topScores = GetTop5(score.Size);

        if (topScores.Count < 5)
            return true;

        HighScore worstTopScore = topScores.Last();

        if (score.Seconds < worstTopScore.Seconds)
            return true;

        if (score.Seconds == worstTopScore.Seconds && score.Moves < worstTopScore.Moves)
            return true;

        return false;
    }
}
