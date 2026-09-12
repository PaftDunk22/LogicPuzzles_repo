using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NonogramBoardGenerator : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject cluePrefab;

    public Transform boardParent;
    public Transform topCluesContainer;
    public Transform leftCluesContainer;
    public Transform rightCluesContainer;

    public RectTransform puzzleLayout;

    private GridLayoutGroup grid;

    void Start()
    {
        string puzzle = PuzzleSelectionManager.SelectedPuzzle;

        if (string.IsNullOrEmpty(puzzle))
        {
            //puzzle = "15x15:1.1/1.2/3/3.3.2/3.1.1.3/1.4/2.4.5/1.1.1.1.1.1/1.1.2.1.1.1.1/1.1.3.1.1/1.1.2.1/1.1.2.1/1.3.1/2.2/7/6/3.2/1.3.1/1.2.1.1/1.1.2.1.1/1.1.2.1.1/1.1.1.1.1/1.2.1.1/1.2.1/8.2/3.2.2/1.10/2.1.1/1.2.2/1.1.1";
            //puzzle = "18x18:3.2.3.2.1/1.1.2.1.1.2.1/1.1.1.1.1.1.1/1.1.1.1.1.1.1/1.1.1.1.1.1.1/1.1.1.1.1.1.1/1.1.1.1.1.1.1/1.1.1.1.1.1.1/1.1.1.1.1.1.1/1.1.1.1.1.1.1/1.1.1.1.1.1.1/1.1.1.1.1.1.1/1.1.1.1.1.1.1/1.1.1.1.1.1.1/1.1.1.1.1.1.1/1.1.1.1.1.1.1/1.1.1.1.1.1.1/2.1.3.1.3/1.2.1.1.1.1.1.1/2.1.1.1.1.1.1.2/1/2.1.1.2.1.2.1/1.1.1.2.1.1.1/1/18/1.1/1.1.1.2.1.1.1/1.2.1.2.1.1.2/1/2.1.1.2.1.2.1/1.1.1.2.1.1.1/1/18/1.1/1.2.1.2.1.1.2/1.1.1.2.1.1.1";
            //puzzle = "10x10:2.3/1.2.1/1.1.2/1.1.1.1/1.1.3/1.1.1.1/1.1.2/1.2.1/2.2/1.5/5.1/1.1/1.5.1/2.2/1.1/1.1.1/1.1.1.1/1.3.1/1.1.2/2.1.1.3";
            //puzzle = "15x15:2.1/2.1/2.1/2.1/1.2.4/1.2/2.2.5/2.4.1/2.2.5/2.2/2.2/1.2.2/2.1/2.1/2.1/1.2/1.2.2.1/2.4.2/2.2.2/2.2/2.2/2.2/2.2/3/1/3/5.3/1.1.1/1.1.1.4/1.3.1";
            puzzle = "20x20:1.1.1/1.1.1.1.1.1.1/2.3.1.3.2/1.2.1.2.1/1.1.1.1/3.2.3.2.4/1.1.3.1.1/1.2.3.2.1/2.1.4.1.2/1.4.1/1.2.2.1/1.1.3.1/7/2.2.1/3.1.4.2/1.1.1.6.4/2.2.3.5.1/1.1.1.2.4.2/3.4.6/1.2/1.3/7.1.1.1/3.1.3.2.2/1.1.1.1/1.1.3/2.1.2/3.2.5/2.9/6.2.2.1/4.5.2.2.2/13/2.11/3.2.1.1.3/2.1.2.1.2.1/1.1.1.2.2/1.2.3/3.1.3.5/7/1/1";
        }

        LoadPuzzle(puzzle);
    }

    void LoadPuzzle(string puzzle)
    {
        GridCellData[,] board = NonogramParser.Parse(puzzle);

        NonogramParser.ParseRules(puzzle,out int size,out List<List<int>> rowRules,out List<List<int>> colRules);

        NonogramGameManager.Instance.SetBoard(board, rowRules, colRules);
        NonogramGameManager.Instance.SetCurrentPuzzle(puzzle);

        grid = boardParent.GetComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = size;

        GenerateColumnClues(colRules);
        GenerateRowClues(rowRules);

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                GameObject cell = Instantiate(cellPrefab, boardParent);

                CellUI ui = cell.GetComponent<CellUI>();

                ui.SetStrategy(new BinaryCellStrategy(true));
                ui.Setup(board[r, c], r, c);

                NonogramGameManager.Instance.RegisterCellUI(r, c, ui);
            }
        }
    }

    void GenerateColumnClues(List<List<int>> colRules)
    {
        foreach (var rule in colRules)
        {
            GameObject clue = Instantiate(cluePrefab, topCluesContainer);
            clue.GetComponentInChildren<TMP_Text>().text = string.Join("\n", rule);
        }
    }

    void GenerateRowClues(List<List<int>> rowRules)
    {
        foreach (var rule in rowRules)
        {
            GameObject clue = Instantiate(cluePrefab, leftCluesContainer);
            clue.GetComponentInChildren<TMP_Text>().text = string.Join(" ", rule);
        }
        
        foreach (var rule in rowRules)
        {
            GameObject clue = Instantiate(cluePrefab, rightCluesContainer);
            clue.GetComponentInChildren<TMP_Text>().text = string.Join(" ", " ");
        }
    }

}