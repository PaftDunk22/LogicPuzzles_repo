using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleGameManager : MonoBehaviour
{
    protected Dictionary<(int, int), CellUI> cellUIs = new();

    protected Stack<Move> undoStack = new();

    public GridCellData[,] Board { get; protected set; }

    public string CurrentPuzzleString { get; protected set; }

    [SerializeField] protected GameObject resetPanel;

    protected abstract string LibraryKey { get; }

    protected virtual void Awake()
    {
        GameManagerLocator.Instance = this;

        if (resetPanel != null)
            resetPanel.SetActive(false);
    }

    public virtual void RegisterCellUI(int row, int col, CellUI ui)
    {
        cellUIs[(row, col)] = ui;
    }

    public virtual void RegisterMove(int row, int col, GridCellData previous, GridCellData current)
    {
        undoStack.Push(new Move
        {
            row = row,
            col = col,
            previous = previous.Clone(),
            current = current.Clone()
        });

        ValidateBoard();
        IsSolved();
    }

    public virtual void UndoLastMove()
    {
        if (undoStack.Count == 0)
            return;

        Move move = undoStack.Pop();

        var cell = Board[move.row, move.col];

        cell.state = move.previous.state;
        cell.value = move.previous.value;

        cellUIs[(move.row, move.col)].RefreshVisual();

        ValidateBoard();
    }

    public virtual void SetCurrentPuzzle(string puzzle)
    {
        CurrentPuzzleString = puzzle;
    }

    protected void RefreshUI()
    {
        int rows = Board.GetLength(0);
        int cols = Board.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                cellUIs[(r, c)].RefreshVisual();
            }
        }
    }

    public void CopyToClipboard()
    {
        Clipboard.Copy(CurrentPuzzleString);
        PopupManager.Instance.Show("Puzzle string copied to clipboard");
    }

    public void AddCurrentPuzzleToLibrary()
    {
        if (string.IsNullOrEmpty(CurrentPuzzleString))
            return;

        List<string> puzzles = UserPuzzleLibrary.LoadPuzzles(LibraryKey);

        if (puzzles.Contains(CurrentPuzzleString))
        {
            PopupManager.Instance.Show("Puzzle already in library");
            return;
        }

        if (puzzles.Count >= 10)
        {
            PopupManager.Instance.Show("Library full");
            return;
        }

        puzzles.Add(CurrentPuzzleString);

        UserPuzzleLibrary.SavePuzzles(LibraryKey, puzzles);

        PopupManager.Instance.Show("Added to library");
    }

    public void OnResetPressed()
    {
        resetPanel.SetActive(true);
    }

    public void OnConfirmReset()
    {
        resetPanel.SetActive(false);
        ResetBoard();
    }

    public void OnCancelReset()
    {
        resetPanel.SetActive(false);
    }

    public abstract void ResetBoard();

    public abstract void ValidateBoard();

    public abstract bool IsSolved();

    public abstract void SolvePuzzle();
}