using System.Collections.Generic;

public static class CellStateMap
{
    public static readonly Dictionary<int, CellState> IntToState = new()
    {
        { 1, CellState.Black },
        { -1, CellState.White },
        { 0, CellState.Empty }
    };

    public static readonly Dictionary<CellState, int> StateToInt = new()
    {
        { CellState.Black, 1 },
        { CellState.White, -1 },
        { CellState.Empty, 0 }
    };
}
