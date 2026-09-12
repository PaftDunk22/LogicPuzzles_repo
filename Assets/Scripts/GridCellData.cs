using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellData
{
    public int? clue;
    public CellState state = CellState.Empty;
    public int value;

    public GridCellData Clone()
    {
        return new GridCellData
        {
            clue = clue,
            state = state,
            value = value
        };
    }

}
