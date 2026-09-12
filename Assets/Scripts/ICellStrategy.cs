using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public interface ICellStrategy
{
    public bool SupportsDrag { get; }
    void OnLeftClick(GridCellData cell);
    void OnRightClick(GridCellData cell);
    void OnMobileTap(GridCellData cell);
    void RefreshVisual(CellUI ui,GridCellData cell);
    void ApplyDrag(GridCellData target, GridCellData source);
}
