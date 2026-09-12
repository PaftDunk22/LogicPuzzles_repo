using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BinaryCellStrategy : ICellStrategy
{
    private bool IsNonogram;
    public bool SupportsDrag => true;

    public BinaryCellStrategy(bool isNonogram)
    {
        this.IsNonogram = isNonogram;
    }

    public void OnLeftClick(GridCellData cell)
    {
        switch (cell.state)
        {
            case CellState.Empty: cell.state = CellState.Black; break;
            case CellState.Black: cell.state = CellState.Empty; break;
            case CellState.White: cell.state = CellState.Black; break;
        }

    }

    public void OnRightClick(GridCellData cell)
    {
        switch (cell.state)
        {
            case CellState.Empty: cell.state = CellState.White; break;
            case CellState.White: cell.state = CellState.Empty; break;
            case CellState.Black: cell.state = CellState.White; break;
        }
    }

    public void OnMobileTap(GridCellData cell)
    {
        switch (cell.state)
        {
            case CellState.Empty:
                cell.state = CellState.Black;
                break;

            case CellState.Black:
                cell.state = CellState.White;
                break;

            case CellState.White:
                cell.state = CellState.Empty;
                break;
        }
    }

    public void RefreshVisual(CellUI ui, GridCellData cell)
    {
        TMP_Text clueText = ui.clueText;

        switch (cell.state)
        {
            case CellState.Empty:
                ui.background.color = Color.gray;
                break;

            case CellState.Black:
                ui.background.color = Color.black;
                break;

            case CellState.White:
                ui.background.color = Color.white;
                break;
        }

        if (ui.isInvalid)
        {
            if (IsNonogram)
            {
                clueText.text = "X";
                clueText.color = Color.red;
                return;
            }
            clueText.color = Color.red;
            return;
        }

        if (cell.state == CellState.Black)
        {
            clueText.color = Color.white;
        }
        else
        {
            clueText.color = Color.black;
        }

        clueText.text = cell.clue.HasValue ? cell.clue.Value.ToString() : "";
       
    }

    public void ApplyDrag(GridCellData target, GridCellData source)
    {
        target.state = source.state;
    }
}
