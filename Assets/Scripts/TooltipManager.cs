using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text tooltipText;

    private Dictionary<string, string> rules = new()
    {
        { "Mosaic", "Color every square either black or white. Each number indicates how many black squares are in the 3×3 square surrounding the number – including the clue square itself" },
        { "Nonogram", "Fill in the grid with a pattern of black and white squares, so that the numbers in each row and column match the lengths of consecutive runs of black squares" },
        { "Skyscraper", "Fill in the grid with towers whose heights range from 1 to the grid size, so that every possible height appears exactly once in each row and column, and so that each clue around the edge counts the number of towers that are visible when looking into the grid from that direction. (Taller towers hide shorter ones behind them)" }
    };

    private void Start()
    {
        panel.SetActive(false);
    }

    public void ShowTooltip(string buttonName, RectTransform buttonRect)
    {
        if (rules.TryGetValue(buttonName, out string rule))
        {
            tooltipText.text = rule;
        }
        else
        {
            tooltipText.text = "No rules found.";
            return;
        }
        
        panel.SetActive(true);
    }

    public void HideTooltip()
    {
        panel.SetActive(false);
    }
}