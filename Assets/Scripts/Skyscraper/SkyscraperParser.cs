using System.Collections.Generic;

public static class SkyscraperParser
{
    public static GridCellData[,] Parse(string puzzle)
    {
        string[] parts = puzzle.Split(':');

        int size = int.Parse(parts[0].Split('x')[0]);

        string predefined = parts.Length > 2 ? parts[2] : "";

        List<int> values = new List<int>();

        foreach (char c in predefined)
        {
            if (c >= 'a' && c <= 'z')
            {
                int count = c - 'a' + 1;

                for (int i = 0; i < count; i++)
                    values.Add(0);
            }
            else if (char.IsDigit(c))
            {
                values.Add(c - '0');
            }
        }

        while (values.Count < size * size)
            values.Add(0);

        GridCellData[,] board = new GridCellData[size, size];

        int index = 0;

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                board[r, c] = new GridCellData
                {
                    clue = null,
                    value = values[index++]
                };
            }
        }
        return board;
    }

    public static void ParseRules(string puzzle,out int size,out List<int> rowLeft,out List<int> rowRight,out List<int> colTop,out List<int> colBottom)
    {
        string[] parts = puzzle.Split(':');

        size =int.Parse(parts[0].Split('x')[0]);

        string[] segments = parts[1].Split('/');

        List<int> colRules = ParseDigits(segments[0]);

        List<int> rowRules = ParseDigits(segments[1]);

        colTop = colRules.GetRange(0, size);
        colBottom = colRules.GetRange(size, size);

        rowLeft = rowRules.GetRange(0, size);
        rowRight = rowRules.GetRange(size, size);
    }

    private static List<int> ParseDigits(string data)
    {
        List<int> result = new List<int>();

        foreach (char c in data)
        {
            result.Add(c - '0');
        }

        return result;
    }
}