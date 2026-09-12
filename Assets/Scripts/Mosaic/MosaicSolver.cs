using System.Collections.Generic;
using UnityEngine;

public static class MosaicSolver
{
    static int size;
    public static int PropagationIterations { get; set; }
    public static int SearchNodes { get; set; }
    public static readonly (int dr, int dc)[] CardinalDir =
        {
            (1, 0),
            (0, 1),
            (-1, 0),
            (0, -1)
        };


    public static void Initialize(int n)
    {
        size = n;
    }

    public static bool Solve(GridCellData[,] board)
    {
        SearchNodes++;

        while (true)
        {
            bool changed = false;

            PropagationIterations++;

            if(!ApplyRules(board,ref changed))
                return false;

            if (changed)
                continue;

            if (IsSolved(board))
                return true;

            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    if (board[r, c].state != CellState.Empty)
                        continue;

                    var backup = CopyBoard(board, null);
                    board[r, c].state = CellState.Black;

                    if (Solve(board))
                        return true;
                    else
                    {
                        CopyBoard(backup, board);
                        board[r, c].state = CellState.White;
                        return Solve(board);
                    }
                }
            }

            return false;
        }
    }

    static bool ApplyRules(GridCellData[,] board, ref bool changed)
    {
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                if (!board[r, c].clue.HasValue)
                    continue;

                if (!FillGuarantee(board,r, c, ref changed))
                    return false;

                if (!DiscoverRule(board, r, c, ref changed))
                    return false;
            }
        }

        return true;
    }

    static bool DiscoverRule(GridCellData[,] board,int r, int c, ref bool changed)
    {
        foreach (var (dr, dc) in CardinalDir)
        {
            int nr = r + dr;
            int nc = c + dc;

            if (!IsInBounds(nr, nc))
                continue;

            if (!board[nr, nc].clue.HasValue)
                continue;

            int difference = board[r, c].clue.Value - board[nr, nc].clue.Value;

            if (difference == 0)
                continue;

            var searchSpace = new List<(int r, int c)>();

            for (int offset = -1; offset <= 1; offset++)
            {
                int searchR;
                int searchC;

                if (dr != 0)
                {
                    searchR = r + 2 * dr;
                    searchC = c + offset;
                }
                else
                {
                    searchR = r + offset;
                    searchC = c + 2 * dc;
                }

                if (IsInBounds(searchR, searchC))
                {
                    searchSpace.Add((searchR, searchC));
                }
            }

            if (searchSpace.Count == 0)
            {
                continue;
            }

            CellState requiredState = difference > 0 ? CellState.White : CellState.Black;

            int required = Mathf.Abs(difference);

            int matching = 0;
            int empty = 0;

            foreach (var (searchR, searchC) in searchSpace)
            {
                CellState state = board[searchR, searchC].state;

                if (state == requiredState)
                {
                    matching++;
                }
                else if (state == CellState.Empty)
                {
                    empty++;
                }
            }

            if (matching >= required)
            {
                continue;
            }

            if (matching + empty < required)
            {
                return false;
            }

            if (matching + empty == required)
            {
                foreach (var (searchR, searchC) in searchSpace)
                {
                    if (board[searchR, searchC].state != CellState.Empty)
                        continue;

                    board[searchR, searchC].state = requiredState;
                    changed = true;
                }
            }
        }

        return true;
    }

    static bool FillGuarantee(GridCellData[,] board, int r, int c, ref bool changed)
    {
        int value = board[r, c].clue.Value;

        CountNeighbors(board, r, c, out int black, out int white, out int total);

        if (black > value || ((total - white) < value))
            return false;

        if (value == black && (black + white) == total)
            return true;

        if (value == total - white)
        {
            changed = true;
            MarkBlack(board, r, c);
        }

        if (value == black)
        {
            changed = true;
            MarkWhite(board, r, c);
        }

        return true;
    }

    public static void CountNeighbors(GridCellData[,] board, int r, int c, out int black, out int white, out int total)
    {
        black = 0;
        white = 0;
        total = 0;

        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                int nr = r + dr;
                int nc = c + dc;

                if (!IsInBounds(nr, nc))
                    continue;

                total++;

                if (board[nr, nc].state == CellState.Black)
                    black++;
                else if (board[nr, nc].state == CellState.White)
                    white++;
            }
        }
    }

    static void MarkBlack(GridCellData[,] board, int r, int c)
    {
        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                int nr = r + dr;
                int nc = c + dc;

                if (!IsInBounds(nr, nc))
                    continue;

                if (board[nr, nc].state == CellState.Empty)
                    board[nr, nc].state = CellState.Black;
            }
        }
    }

    static void MarkWhite(GridCellData[,] board, int r, int c)
    {
        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                int nr = r + dr;
                int nc = c + dc;

                if (!IsInBounds(nr, nc))
                    continue;

                if (board[nr, nc].state == CellState.Empty)
                    board[nr, nc].state = CellState.White;
            }
        }
    }

    public static bool IsSolved(GridCellData[,] board)
    {
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                if (!board[r, c].clue.HasValue)
                    continue;

                int clue = board[r, c].clue.Value;
                MosaicSolver.CountNeighbors(board, r, c, out int black, out int white, out int total);

                if (clue != black)
                {
                    return false;
                }
            }
        }
        return true;
    }

    static bool IsInBounds(int r, int c)
    {
        return r >= 0 && r < size && c >= 0 && c < size;
    }

    static GridCellData[,] CopyBoard(GridCellData[,] src, GridCellData[,] dst = null)
    {
        if (dst == null)
        {
            dst = new GridCellData[size, size];

            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    dst[r, c] = new GridCellData();
        }

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                dst[r, c].clue = src[r, c].clue;
                dst[r, c].state = src[r, c].state;
            }
        }

        return dst;
    }

}

