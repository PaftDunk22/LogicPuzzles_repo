using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Stopwatch = System.Diagnostics.Stopwatch;

public class MosaicGameManager : PuzzleGameManager
{
    public static MosaicGameManager Instance;

    protected override string LibraryKey => "MosaicUserPuzzles";

    private int size;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public void SetBoard(GridCellData[,] board)
    {
        Board = board;
        size = board.GetLength(0);

        MosaicSolver.Initialize(size);
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

    public override bool IsSolved()
    {
        if(!MosaicSolver.IsSolved(Board))
            return false;

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
            ui.SetInvalid(false);

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                if (!Board[r, c].clue.HasValue)
                    continue;

                int clue = Board[r, c].clue.Value;
                MosaicSolver.CountNeighbors(Board, r, c, out int black, out int white, out int total);

                if (black > clue || ((total - white) < clue))
                {
                    cellUIs[(r, c)].SetInvalid(true);
                }
            }
        }
    }

    public override void SolvePuzzle()
    {
        ResetBoard();

        if (Board == null)
            return;

        MosaicSolver.PropagationIterations = 0;
        MosaicSolver.SearchNodes = 0;

        Stopwatch stopwatch = Stopwatch.StartNew();

        MosaicSolver.Solve(Board);

        stopwatch.Stop();

        double solvingTime = stopwatch.Elapsed.TotalMilliseconds;

        Debug.Log($"Mosaic string: {this.CurrentPuzzleString} | " +
                  $"Mosaic {size}x{size} | " +
                  $"Time: {solvingTime:F3} ms | " +
                  $"Propagation Iterations: {MosaicSolver.PropagationIterations} | " +
                  $"Search Nodes: {MosaicSolver.SearchNodes}");

        RefreshUI();

        ValidateBoard();
        IsSolved();
    }
}