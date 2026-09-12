using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Stopwatch = System.Diagnostics.Stopwatch;

public class SkyscraperGameManager : PuzzleGameManager
{
    public static SkyscraperGameManager Instance;

    protected override string LibraryKey => "SkyscraperUserPuzzles";

    private List<int> rowL, rowR, colT, colB;
    private int size;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public void SetBoard(GridCellData[,] board,List<int> rL,List<int> rR,List<int> cT,List<int> cB)
    {
        Board = board;
        size = board.GetLength(0);
        rowL = rL;
        rowR = rR;
        colT = cT;
        colB = cB;

        SkyscraperSolver.Initialize(size, rowL, rowR, colT, colB);
    }

    public override void ResetBoard()
    {
        GridCellData[,] freshBoard = SkyscraperParser.Parse(CurrentPuzzleString);

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                Board[r, c].value = freshBoard[r, c].value;

                if(Board[r, c].value != 0) cellUIs[(r, c)].background.color = new Color(0.75f, 0.9f, 1f);
                cellUIs[(r, c)].RefreshVisual();
            }
        }

        undoStack.Clear();
        ValidateBoard();
    }

    public override void SolvePuzzle()
    {
        ResetBoard();
        if (Board == null)
            return;

        SkyscraperSolver.PropagationIterations = 0;
        SkyscraperSolver.SearchNodes = 0;

        Stopwatch stopwatch = Stopwatch.StartNew();

        SkyscraperSolver.Solve(Board);

        stopwatch.Stop();

        double solvingTime = stopwatch.Elapsed.TotalMilliseconds;

        Debug.Log($"Skyscraper string: {this.CurrentPuzzleString} | " +
                  $"Skyscraper {size}x{size} | " +
                  $"Time: {solvingTime:F3} ms | " +
                  $"Propagation Iterations: {SkyscraperSolver.PropagationIterations} | " +
                  $"Search Nodes: {SkyscraperSolver.SearchNodes}");

        RefreshUI();
        ValidateBoard();
        IsSolved();
    }

    public override bool IsSolved()
    {
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                if (Board[r, c].value == 0)
                    return false;
            }
        }

        foreach (var ui in cellUIs.Values)
        {
            if (ui.isInvalid)
                return false;
        }

        PopupManager.Instance.Show("Puzzle Solved!");
        UISFXManager.Instance.PlaySolved();

        return true;
    }

    public override void ValidateBoard()
    {
        foreach (var ui in cellUIs.Values)
        {
            ui.SetInvalid(false);
        }

        for (int r = 0; r < size; r++)
        {
            List<int> row = new List<int>();

            for (int c = 0; c < size; c++)
                row.Add(Board[r, c].value);

            bool rowInvalid = !SkyscraperSolver.IsLinePossible(row,rowL[r],rowR[r]);

            if (rowInvalid)
            {
                for (int c = 0; c < size; c++)
                    cellUIs[(r, c)].SetInvalid(rowInvalid);
            }
        }

        for (int c = 0; c < size; c++)
        {
            List<int> col = new List<int>();

            for (int r = 0; r < size; r++)
                col.Add(Board[r, c].value);

            bool colInvalid = !SkyscraperSolver.IsLinePossible(col, colT[c], colB[c]);

            if (colInvalid)
            {
                for (int r = 0; r < size; r++)
                    cellUIs[(r, c)].SetInvalid(colInvalid);
            }
        }
    }
}