using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionMenuController : MonoBehaviour
{
    public TMP_Dropdown sizeDropdown;
    public TMP_InputField importField;

    public Transform libraryContainer;
    public GameObject libraryEntryPrefab;

    public GameObject EmptyLibraryText;

    private string currentScene;
    private string sceneName;
    private string libraryKey;

    private Dictionary<int, List<string>> puzzlePool;
    private System.Func<string, GridCellData[,]> validatePuzzle;

    private void Start()
    {
        SetupContext();

        RefreshLibraryUI();
    }


    public void GeneratePuzzle()
    {

        if (sizeDropdown.value == 0)
        {
            PopupManager.Instance.Show("Please select size");
            return;

        }

        int size = int.Parse(sizeDropdown.options[sizeDropdown.value].text);

        List<string> pool = puzzlePool[size];

        if (pool.Count == 0)
        {
            Debug.LogWarning($"No puzzles available for {size}x{size}");
            return;
        }

        string puzzle = pool[Random.Range(0,pool.Count)];

        PuzzleSelectionManager.SelectedPuzzle = puzzle;

        SceneManager.LoadScene(sceneName);
    }

    public void ImportPuzzle()
    {

        string puzzle = importField.text.Trim();

        try
        {
            validatePuzzle(puzzle);
        }
        catch
        {
            PopupManager.Instance.Show("Invalid puzzle string");
            return;
        }

        if (string.IsNullOrEmpty(puzzle))
            return;

        List<string> puzzles = UserPuzzleLibrary.LoadPuzzles(libraryKey);

        if (puzzles.Contains(puzzle))
        {
            PopupManager.Instance.Show("Puzzle already in library");
            return;
        }

        if (puzzles.Count >= 10)
        {
            PopupManager.Instance.Show("Library limited to 10");
            return;
        }
            
        puzzles.Add(puzzle);

        UserPuzzleLibrary.SavePuzzles(libraryKey,puzzles);

        PopupManager.Instance.Show("Puzzle imported");

        RefreshLibraryUI();
    }

    public void RefreshLibraryUI()
    {
        foreach (Transform child in libraryContainer)
        {
            if (child.gameObject == EmptyLibraryText)
                continue;

            Destroy(child.gameObject);
        }


        List<string> puzzles = UserPuzzleLibrary.LoadPuzzles(libraryKey);


        EmptyLibraryText.SetActive(puzzles.Count == 0);

        for (int i = 0; i < puzzles.Count; i++)
        {
            string puzzle = puzzles[i];

            GameObject entry = Instantiate(libraryEntryPrefab, libraryContainer);

            Button[] buttons = entry.GetComponentsInChildren<Button>();

            Button playButton = buttons[0];
            Button deleteButton = buttons[1];

            foreach (Button b in buttons)
            {
                b.onClick.AddListener(UISFXManager.Instance.PlayClick);
            }

            string size = puzzle.Split(':')[0];
            string cs = currentScene;

            playButton
                .GetComponentInChildren<TMP_Text>()
                .text = $"{cs} {i + 1} ({size})";
                
            playButton.onClick.AddListener(() =>
            {
                LoadPuzzle(puzzle);
            });

            deleteButton.onClick.AddListener(() =>
            {
                DeletePuzzle(puzzle);
            });
        }
    }

    public void LoadPuzzle(string puzzle)
    {
        PuzzleSelectionManager.SelectedPuzzle = puzzle;
        SceneManager.LoadScene(sceneName);
    }

    public void DeletePuzzle(string puzzle)
    {
        List<string> puzzles = UserPuzzleLibrary.LoadPuzzles(libraryKey);

        puzzles.Remove(puzzle);

        UserPuzzleLibrary.SavePuzzles(libraryKey, puzzles);

        RefreshLibraryUI();
    }

    private void SetupContext()
    {
        currentScene = SceneManager.GetActiveScene().name;
        currentScene = currentScene.Remove(currentScene.Length - 9);

        sceneName = currentScene + "Gameplay";

        switch (currentScene)
        {
            case "Mosaic":
                puzzlePool = PuzzleDatabase.MosaicPuzzles;
                validatePuzzle = MosaicParser.Parse;
                libraryKey = "MosaicUserPuzzles";
                break;

            case "Nonogram":
                puzzlePool = PuzzleDatabase.NonogramPuzzles;
                validatePuzzle = NonogramParser.Parse;
                libraryKey = "NonogramUserPuzzles";
                break;
             
            
            case "Skyscraper":
                puzzlePool = PuzzleDatabase.SkyscraperPuzzles;
                validatePuzzle = SkyscraperParser.Parse;
                libraryKey = "SkyscraperUserPuzzles";
                break;
        }
    }
}