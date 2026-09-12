using System.Collections.Generic;

public static class NonogramParser
{
    public static GridCellData[,] Parse(string puzzle)
    {
        string[] parts = puzzle.Split(':');

        int size = int.Parse(parts[0].Split('x')[0]);

        GridCellData[,] board = new GridCellData[size, size];

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                board[r, c] = new GridCellData
                {
                    state = CellState.Empty,
                    clue = null
                };
            }
        }

        return board;
    }

    public static void ParseRules(string puzzle,out int size,out List<List<int>> rowRules,out List<List<int>> colRules)
    {
        string[] parts = puzzle.Split(':');

        string sizePart = parts[0];
        string dataPart = parts[1];

        size = int.Parse(sizePart.Split('x')[0]);

        string[] segments = dataPart.Split('/');

        colRules = new List<List<int>>();

        rowRules = new List<List<int>>();

        for (int i = 0; i < size; i++)
        {
            colRules.Add(ParseRule(segments[i]));
        }

        for (int i = size; i < size * 2; i++)
        {
            rowRules.Add(ParseRule(segments[i]));
        }
    }

    private static List<int> ParseRule(string segment)
    {
        List<int> rule = new List<int>();

        if (string.IsNullOrEmpty(segment))
        {
            return rule;
        }

        string[] values =
            segment.Split('.');

        foreach (string value in values)
        {
            rule.Add(
                int.Parse(value));
        }

        return rule;
    }
}