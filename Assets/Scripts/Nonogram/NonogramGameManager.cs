using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Stopwatch = System.Diagnostics.Stopwatch;

public class NonogramGameManager : PuzzleGameManager
{
    public static NonogramGameManager Instance;

    protected override string LibraryKey => "NonogramUserPuzzles";

    private List<List<int>> rowRules;
    private List<List<int>> colRules;
    private int size;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public void SetBoard(GridCellData[,] board,List<List<int>> rows,List<List<int>> cols)
    {
        Board = board;
        size = board.GetLength(0);
        rowRules = rows;
        colRules = cols;

        NonogramSolver.Initialize(size,rowRules,colRules);
    }

    public override void ResetBoard()
    {
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                Board[r, c].state = CellState.Empty;
                cellUIs[(r, c)].RefreshVisual();
            }
        }

        undoStack.Clear();
        ValidateBoard();
    }

    public override void ValidateBoard()
    {
        foreach (var ui in cellUIs.Values)
            ui.SetInvalid(false);

        for (int r = 0; r < size; r++)
        {
            if (!NonogramSolver.IsLinePossible(Board, rowRules[r], r, true))
            {
                for (int c = 0; c < size; c++)
                    cellUIs[(r, c)].SetInvalid(true);
            }
        }

        for (int c = 0; c < size; c++)
        {
            if (!NonogramSolver.IsLinePossible(Board, colRules[c], c, false))
            {
                for (int r = 0; r < size; r++)
                    cellUIs[(r, c)].SetInvalid(true);
            }
        }
    }

    public override void SolvePuzzle()
    {
        ResetBoard();

        if (Board == null)
            return;

        NonogramSolver.PropagationIterations = 0;
        NonogramSolver.SearchNodes = 0;

        Stopwatch stopwatch = Stopwatch.StartNew();

        NonogramSolver.Solve(Board);

        stopwatch.Stop();

        double solvingTime = stopwatch.Elapsed.TotalMilliseconds;

        Debug.Log($"Nonogram string: {this.CurrentPuzzleString} | " +
                  $"Nonogram {size}x{size} | " +
                  $"Time: {solvingTime:F3} ms | " +
                  $"Propagation Iterations: {NonogramSolver.PropagationIterations} | " +
                  $"Search Nodes: {NonogramSolver.SearchNodes}");

        RefreshUI();
        ValidateBoard();
        IsSolved();
    }

    public override bool IsSolved()
    {
        bool solved = NonogramSolver.IsSolved(Board);

        if (solved)
        {
            PopupManager.Instance.Show("Puzzle Solved!");
            UISFXManager.Instance.PlaySolved();
        }

        return solved;
    }

}