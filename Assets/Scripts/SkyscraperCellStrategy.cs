using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkyscraperCellStrategy : ICellStrategy
{
    private int size;
    public bool SupportsDrag => false;

    public SkyscraperCellStrategy(int size)
    {
        this.size = size;
    }

    public void OnLeftClick(GridCellData cell)
    {
        cell.value++;

        if (cell.value > size)
            cell.value = 0;
    }

    public void OnRightClick(GridCellData cell)
    {
        cell.value--;

        if (cell.value < 0)
            cell.value = size;

    }

    public void OnMobileTap(GridCellData cell)
    {
        OnLeftClick(cell);
    }

    public void RefreshVisual(CellUI ui, GridCellData data)
    {
        TMP_Text clueText = ui.clueText;
        clueText.text = data.value.ToString();

        if (data.value == 0) clueText.text = "";

        if (ui.isInvalid)
        {
            clueText.color = Color.red;
        }
        else
        {
            clueText.color = Color.black;
        }
        return;
    }

    public void ApplyDrag(GridCellData target, GridCellData source)
    {
        // No drag in Skyscraper
    }
}
