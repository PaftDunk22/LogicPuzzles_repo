using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkyscraperBoardGenerator : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject cluePrefab;

    public Transform boardParent;
    public Transform topCluesContainer;
    public Transform bottomCluesContainer;
    public Transform leftCluesContainer;
    public Transform rightCluesContainer;

    void Start()
    {
        string puzzle = PuzzleSelectionManager.SelectedPuzzle;

        if (string.IsNullOrEmpty(puzzle))
        {
            //puzzle = "4x4:33212132/42121242:p";
            puzzle = "6x6:422125233421/422313332221:a3d2i6s";
        }

        LoadPuzzle(puzzle);
    }

    void LoadPuzzle(string puzzle)
    {
        GridCellData[,] board = SkyscraperParser.Parse(puzzle);

        SkyscraperParser.ParseRules(puzzle,out int size,out List<int> rowLeft,out List<int> rowRight,out List<int> colTop,out List<int> colBottom);

        SkyscraperGameManager.Instance.SetBoard(board,rowLeft,rowRight,colTop,colBottom);

        SkyscraperGameManager.Instance.SetCurrentPuzzle(puzzle);

        GridLayoutGroup grid = boardParent.GetComponent<GridLayoutGroup>();

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

        grid.constraintCount = size;

        GenerateClues(colTop, topCluesContainer);
        GenerateClues(colBottom, bottomCluesContainer);

        GenerateClues(rowLeft, leftCluesContainer);
        GenerateClues(rowRight, rightCluesContainer);

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                GameObject cell = Instantiate(cellPrefab, boardParent);

                CellUI ui = cell.GetComponent<CellUI>();

                ui.SetStrategy(new SkyscraperCellStrategy(size));
                ui.Setup(board[r, c], r, c);

                if (board[r, c].value != 0)
                    ui.background.color = new Color(0.75f, 0.9f, 1f);
                else
                    ui.background.color = Color.gray;

                SkyscraperGameManager.Instance.RegisterCellUI(r, c, ui);
            }
        }
    }

    void GenerateClues(List<int> clues,Transform container)
    {
        foreach (int clue in clues)
        {
            GameObject obj = Instantiate(cluePrefab, container);

            obj.GetComponentInChildren<TMP_Text>().text = clue == 0 ? "" : clue.ToString();
        }
    }

    public int returnSize()
    {
        GridLayoutGroup grid = boardParent.GetComponent<GridLayoutGroup>();

        return grid.constraintCount;

    }
}