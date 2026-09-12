using System.Collections.Generic;
using UnityEngine;

public static class NonogramSolver
{
    static int size;
    static List<List<int>> rowRules;
    static List<List<int>> colRules;

    public static int PropagationIterations { get; set; }
    public static int SearchNodes { get; set; }

    public static void Initialize(int n, List<List<int>> rowR, List<List<int>> colR)
    {
        size = n;
        rowRules = rowR;
        colRules = colR;
    }

    public static bool Solve(GridCellData[,] board)
    {
        SearchNodes++;

        while (true)
        {
            PropagationIterations++;

            bool changed = false;

            if (!ApplyPatterns(board, rowRules, isRow: true, ref changed))
                return false;

            if (!ApplyPatterns(board, colRules, isRow: false, ref changed))
                return false;

            if (IsSolved(board))
                return true;

            if (changed)
                continue;

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
        }
    }

    private static bool ApplyPatterns(GridCellData[,] board,List<List<int>> rules,bool isRow, ref bool changed)
    {
        for (int i = 0; i < size; i++)
        {
            var currentLine = ExtractCurrentLine(board, isRow, i);

            bool allEmpty = true;
            foreach (var v in currentLine)
                if (v != CellState.Empty) allEmpty = false;

            int ruleSum = 0;
            foreach (var r in rules[i])
                ruleSum += r;

            if (allEmpty && ruleSum <= size / 2)
                continue;

            var patterns = GeneratePatterns(rules[i]);

            if (!allEmpty)
                patterns = FilterPatterns(patterns, currentLine);

            if (patterns.Count == 0)
            {
                return false;
            }

            var overlap = GuaranteedOverlap(patterns);

            for (int j = 0; j < size; j++)
            {
                if (overlap[j] == CellState.Empty)
                    continue;

                var cell = isRow ? board[i, j] : board[j, i];

                if (cell.state != overlap[j])
                {
                    cell.state = overlap[j];
                    changed = true;
                }
            }
        }

        return true;
    }

    public static List<List<CellState>> GeneratePatterns(List<int> rule)
    {
        var results = new List<List<CellState>> { new List<CellState>() };

        for (int i = 0; i < rule.Count; i++)
        {
            int blockLen = rule[i];
            var newResults = new List<List<CellState>>();

            foreach (var pattern in results)
            {
                var remaining = rule.GetRange(i + 1, rule.Count - (i + 1));

                int minStart = pattern.Count;

                int remainingSum = 0;
                foreach (var r in remaining)
                    remainingSum += r;

                int maxStart = size - (remainingSum + remaining.Count + (blockLen - 1));

                for (int start = minStart; start < maxStart; start++)
                {
                    var newPattern = new List<CellState>(pattern);

                    while (newPattern.Count < start)
                        newPattern.Add(CellState.White);

                    for (int k = 0; k < blockLen; k++)
                        newPattern.Add(CellState.Black);

                    if (i != rule.Count - 1)
                        newPattern.Add(CellState.White);

                    newResults.Add(newPattern);

                }
            }

            results = newResults;
        }

        for (int i = 0; i < results.Count; i++)
        {
            while (results[i].Count < size)
                results[i].Add(CellState.White);
        }

        return results;
    }

    private static List<List<CellState>> FilterPatterns(List<List<CellState>> patterns, List<CellState> current)
    {
        var valid = new List<List<CellState>>();

        foreach (var p in patterns)
        {
            bool clash = false;

            for (int i = 0; i < p.Count; i++)
            {
                if (i >= current.Count) break;

                if (current[i] == CellState.Black && p[i] != CellState.Black)
                    clash = true;

                if (current[i] == CellState.White && p[i] == CellState.Black)
                    clash = true;
            }

            if (!clash)
                valid.Add(p);
        }

        return valid;
    }

    private static List<CellState> GuaranteedOverlap(List<List<CellState>> patterns)
    {
        var result = new List<CellState>();

        if (patterns.Count == 0)
        {
            for (int i = 0; i < size; i++)
                result.Add(CellState.Empty);

            return result;
        }

        for (int i = 0; i < size; i++)
        {
            bool allBlack = true;
            bool allWhite = true;

            foreach (var p in patterns)
            {
                var cell = p[i];

                if (cell != CellState.Black)
                    allBlack = false;

                if (cell != CellState.White)
                    allWhite = false;
            }

            if (allBlack)
                result.Add(CellState.Black);
            else if (allWhite)
                result.Add(CellState.White);
            else
                result.Add(CellState.Empty);
        }

        return result;
    }

    public static bool LineMatches(GridCellData[,] board, List<int> rule, int index, bool isRow)
    {
        List<int> blocks = new List<int>();
        int count = 0;

        for (int i = 0; i < size; i++)
        {
            CellState cell = isRow ? board[index, i].state : board[i, index].state;

            if (cell == CellState.Black)
            {
                count++;
            }
            else
            {
                if (count > 0)
                {
                    blocks.Add(count);
                    count = 0;
                }
            }
        }

        if (count > 0)
            blocks.Add(count);

        if (blocks.Count != rule.Count)
            return false;

        for (int i = 0; i < blocks.Count; i++)
        {
            if (blocks[i] != rule[i])
                return false;
        }

        return true;
    }

    public static bool IsLinePossible(GridCellData[,] board, List<int> rule, int index, bool isRow)
    {
        var patterns = GeneratePatterns(rule);

        List<CellState> currentLine = ExtractCurrentLine(board, isRow, index);

        patterns = FilterPatterns(patterns, currentLine);

        return patterns.Count > 0;
    }

    static List<CellState> ExtractCurrentLine(GridCellData[,] board, bool isRow, int index)
    {
        List<CellState> currentLine = new List<CellState>();

        for (int i = 0; i < size; i++)
        {
            currentLine.Add(isRow ? board[index, i].state : board[i, index].state);
        }

        return currentLine;
    }

    public static bool IsSolved(GridCellData[,] board)
    {
        for (int r = 0; r < size; r++)
        {
            if (!LineMatches(board, rowRules[r], r, true))
                return false;
        }

        for (int c = 0; c < size; c++)
        {
            if (!LineMatches(board, colRules[c], c, false))
                return false;
        }

        return true;
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