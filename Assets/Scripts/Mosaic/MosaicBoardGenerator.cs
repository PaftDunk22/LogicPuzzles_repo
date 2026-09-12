using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MosaicBoardGenerator : MonoBehaviour
{
    public GameObject cellPrefab;
    public Transform boardParent;

    public RectTransform puzzleLayout;

    private GridLayoutGroup grid;

    void Start()
    {
        if (string.IsNullOrEmpty(PuzzleSelectionManager.SelectedPuzzle))
        {
            LoadPuzzle("5x5:4db45aeeb13a");

            return;
        }

        LoadPuzzle(PuzzleSelectionManager.SelectedPuzzle);
    }

    void LoadPuzzle(string puzzle)
    {
        GridCellData[,] board = MosaicParser.Parse(puzzle);

        MosaicGameManager.Instance.SetBoard(board);

        MosaicGameManager.Instance.SetCurrentPuzzle(puzzle);

        int size = board.GetLength(0);

        grid = boardParent.GetComponent<GridLayoutGroup>();

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

        grid.constraintCount = size;

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                GameObject cell = Instantiate(cellPrefab, boardParent);

                CellUI ui = cell.GetComponent<CellUI>();

                ui.SetStrategy(new BinaryCellStrategy(false));
                ui.Setup(board[r, c], r, c);

                MosaicGameManager.Instance.RegisterCellUI(r, c, ui);
            }
        }
    }

}