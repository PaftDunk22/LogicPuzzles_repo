using System.Collections.Generic;
using System.Linq;

public static class SkyscraperSolver
{
    static int size;

    static List<int> rowLeft;
    static List<int> rowRight;
    static List<int> colTop;
    static List<int> colBottom;

    public static int PropagationIterations { get; set; }
    public static int SearchNodes { get; set; }

    public static void Initialize(int n, List<int> rowL, List<int> rowR, List<int> colT, List<int> colB)
    {
        size = n;
        rowLeft = rowL;
        rowRight = rowR;
        colTop = colT;
        colBottom = colB;
    }

    public static bool Solve(GridCellData[,] board)
    {
        SearchNodes++;

        while (true)
        {
            PropagationIterations++;

            bool possible = ApplyGuaranteesAndGivePatterns(board, out bool changed, out int spot, out bool isRow, out List<List<int>> patterns);

            if (!possible)
                return false;

            if (IsComplete(board))
            {
                return !DuplicateNumbers(board);
            }

            if (changed)
                continue;


            foreach (List<int> pattern in patterns)
            {
                GridCellData[,] backup = CopyBoard(board);


                ApplyToBoard(board, spot, pattern, isRow);


                if (Solve(board))
                    return true;


                CopyInto(backup, board);
            }

            return false;
        }
    }

    static bool ApplyGuaranteesAndGivePatterns(GridCellData[,] board, out bool changed, out int bestSpot, out bool bestIsRow, out List<List<int>> bestPatterns)
    {
        changed = false;
        int minPattern = int.MaxValue;

        bestSpot = -1;
        bestIsRow = true;
        bestPatterns = null;

        for (int i = 0; i < size; i++)
        {
            List<int> line = GetLine(board, i, true);

            if (DuplicateIn(line))
                return false;


            if (IsLineSolved(line))
                continue;


            List<List<int>> patterns = GeneratePattern(new List<int>(), new List<List<int>>(), line, rowLeft[i], rowRight[i]);

            if (patterns.Count == 0)
                return false;

            if (patterns.Count < minPattern)
            {
                minPattern = patterns.Count;
                bestSpot = i;
                bestIsRow = true;
                bestPatterns = patterns;
            }

            List<int> guaranteed = ApplyGuarantee(patterns, line);

            if (!line.SequenceEqual(guaranteed))
                changed = true;

            ApplyToBoard(board, i, guaranteed, true);
        }

        for (int i = 0; i < size; i++)
        {
            List<int> line = GetLine(board, i, false);

            if (DuplicateIn(line))
                return false;

            if (IsLineSolved(line))
                continue;

            List<List<int>> patterns = GeneratePattern(new List<int>(), new List<List<int>>(), line, colTop[i], colBottom[i]);

            if (patterns.Count == 0)
                return false;

            if (patterns.Count < minPattern)
            {
                minPattern = patterns.Count;
                bestSpot = i;
                bestIsRow = false;
                bestPatterns = patterns;
            }

            List<int> guaranteed = ApplyGuarantee(patterns, line);

            if (!line.SequenceEqual(guaranteed))
                changed = true;

            ApplyToBoard(board, i, guaranteed, false);
        }

        return true;
    }

    static List<int> GetLine(GridCellData[,] board, int index, bool isRow)
    {
        List<int> line = new List<int>();

        for (int i = 0; i < size; i++)
        {
            if (isRow)
                line.Add(board[index, i].value);
            else
                line.Add(board[i, index].value);
        }

        return line;
    }

    static void ApplyToBoard(GridCellData[,] board, int index, List<int> pattern, bool isRow)
    {
        for (int i = 0; i < size; i++)
        {
            if (isRow)
                board[index, i].value = pattern[i];
            else
                board[i, index].value = pattern[i];
        }
    }

    static GridCellData[,] CopyBoard(GridCellData[,] src)
    {
        GridCellData[,] copy = new GridCellData[size, size];

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                copy[r, c] = new GridCellData
                {
                    value = src[r, c].value
                };
            }
        }

        return copy;
    }

    static void CopyInto(GridCellData[,] src, GridCellData[,] dst)
    {
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                dst[r, c].value = src[r, c].value;
            }
        }
    }

    static int GetCurrentSight(List<int> pattern)
    {
        int currentSight = 1;

        for (int i = 0; i < pattern.Count; i++)
        {
            bool? isSeen = null;

            for (int x = 0; x < i; x++)
            {
                isSeen = true;

                if (pattern[i] < pattern[x])
                {
                    isSeen = false;
                    break;
                }
            }

            if (isSeen == true)
                currentSight++;
        }

        return currentSight;
    }

    static bool IsFittingRule(List<int> pattern, int rule)
    {
        if (rule == 0)
            return true;

        return GetCurrentSight(pattern) == rule;
    }

    static (int min, int max) MinMaxPatternSight(List<int> pattern)
    {
        if (pattern.Count == 0)
            return (1, size);

        int currentSight = GetCurrentSight(pattern);

        return
        (
            currentSight + (pattern.Max() != size ? 1 : 0),
            currentSight + (size - pattern.Max())
        );
    }

    static (int min, int max) ReverseMinMaxPatternSight(List<int> pattern)
    {
        if (pattern.Count == 0)
            return (1, size);

        int currentSight = GetCurrentSight(pattern);

        return
        (
            (size - pattern.Max() == 0) ? 2 : 1,
            1 + size - currentSight + (pattern.Max() != size ? 1 : 0)
        );
    }

    static bool OverlapWithCurrentLine(List<int> pattern, List<int> currentLine)
    {
        for (int i = 0; i < pattern.Count; i++)
        {
            if (currentLine[i] != 0 &&
                pattern[i] != currentLine[i])
                return false;
        }

        return true;
    }

    static List<List<int>> GeneratePattern(List<int> pattern, List<List<int>> patterns, List<int> currentLine, int rule1, int rule2)
    {
        if (pattern.Count == size)
        {
            var reversed = pattern.AsEnumerable().Reverse().ToList();

            if (IsFittingRule(pattern, rule1) && IsFittingRule(reversed, rule2) &&
                OverlapWithCurrentLine(pattern, currentLine))
            {
                patterns.Add(new List<int>(pattern));
            }

            return patterns;
        }

        var mm = MinMaxPatternSight(pattern);

        if (!OverlapWithCurrentLine(pattern, currentLine) || (rule1 != 0 && (rule1 < mm.min || rule1 > mm.max)))
            return patterns;


        var rmm = ReverseMinMaxPatternSight(pattern);

        if (!OverlapWithCurrentLine(pattern, currentLine) || (rule2 != 0 && (rule2 < rmm.min || rule2 > rmm.max)))
            return patterns;


        for (int i = 1; i <= size; i++)
        {
            if (!pattern.Contains(i))
            {
                var next = new List<int>(pattern);
                next.Add(i);

                GeneratePattern(next, patterns, currentLine, rule1, rule2);
            }
        }

        return patterns;
    }

    static List<int> ApplyGuarantee(List<List<int>> patterns, List<int> currentLine)
    {
        List<int> finalPattern = new List<int>(currentLine);

        if (patterns.Count == 0)
            return finalPattern;

        for (int i = 0; i < size; i++)
        {
            var values = patterns.Select(p => p[i]).ToList();

            if (values.All(v => v == values[0]))
                finalPattern[i] = values[0];
        }

        return finalPattern;
    }

    static bool DuplicateIn(List<int> pattern)
    {
        int[] status = new int[size + 1];

        for (int i = 0; i < size; i++)
        {
            status[pattern[i]]++;

            if (pattern[i] != 0 && status[pattern[i]] > 1)
                return true;
        }

        return false;
    }

    static bool DuplicateNumbers(GridCellData[,] board)
    {
        for (int i = 0; i < size; i++)
        {
            if (DuplicateIn(GetLine(board, i, true)))
                return true;

            if (DuplicateIn(GetLine(board, i, false)))
                return true;
        }

        return false;
    }

    static bool IsLineSolved(List<int> line)
    {
        return line.All(v => v != 0);
    }

    static bool IsComplete(GridCellData[,] board)
    {
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                if (board[r, c].value == 0)
                    return false;
            }
        }

        return true;
    }

    public static bool IsLinePossible(List<int> line, int clueA, int clueB)
    {

        if (DuplicateIn(line))
            return false;

        var patterns = GeneratePattern(new List<int>(), new List<List<int>>(), line, clueA, clueB);

        return patterns.Count > 0;
    }
}