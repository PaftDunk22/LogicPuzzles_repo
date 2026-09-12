using System.Collections.Generic;

public static class MosaicParser
{
    public static GridCellData[,] Parse(string puzzle)
    {
        string[] parts = puzzle.Split(':');

        int size = int.Parse(parts[0].Split('x')[0]);

        string data = parts[1];

        List<GridCellData> cells = new List<GridCellData>();

        foreach (char c in data)
        {
            if (char.IsDigit(c))
            {
                cells.Add(new GridCellData
                {
                    clue = c - '0'
                });
            }
            else if (c >= 'a' && c <= 'z')
            {
                int count = c - 'a' + 1;

                for (int i = 0; i < count; i++)
                {
                    cells.Add(new GridCellData
                    {
                        clue = null
                    });
                }
            }
        }

        GridCellData[,] board = new GridCellData[size, size];

        int index = 0;

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                board[r, c] = cells[index++];
            }
        }

        return board;
    }
}