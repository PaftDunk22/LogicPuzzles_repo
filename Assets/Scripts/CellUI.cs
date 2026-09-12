using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CellUI : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler
{
    private int row;
    private int col;

    public bool isInvalid { get; private set; }

    public TMP_Text clueText;
    public Image background;

    private GridCellData data;

    private static bool isDragging;
    private static GridCellData dragData;

    private ICellStrategy strategy;

    public void Setup(GridCellData cellData, int r, int c)
    {
        data = cellData;
        row = r;
        col = c;

        RefreshVisual();
    }

    public void SetStrategy(ICellStrategy strat)
    {
        strategy = strat;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }
    }

    void HandleLeftClick()
    {
        strategy.OnLeftClick(data);
    }

    void HandleRightClick()
    {
        strategy.OnRightClick(data);
    }

    public void RefreshVisual()
    {
        strategy.RefreshVisual(this,data);
    }

    public void SetInvalid(bool invalid)
    {
        isInvalid = invalid;
        RefreshVisual();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;

        GridCellData previous = data.Clone();

        if (Application.isMobilePlatform)
        {
            strategy.OnMobileTap(data);
        }
        else
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                strategy.OnLeftClick(data);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                strategy.OnRightClick(data);
            }
        }

        dragData = data.Clone();

        if (!previous.Equals(data))
        {
            GameManagerLocator.Instance.RegisterMove(row, col, previous, data.Clone());
        }

        RefreshVisual();
        GameManagerLocator.Instance.ValidateBoard();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDragging || !strategy.SupportsDrag)
            return;

        GridCellData previous = data.Clone();

        strategy.ApplyDrag(data, dragData);

        if (previous.Equals(data))
            return;

        GameManagerLocator.Instance.RegisterMove(row, col, previous, data.Clone());

        RefreshVisual();
        GameManagerLocator.Instance.ValidateBoard();
    }

    void HandleMobileTap()
    {
        strategy.OnMobileTap(data);
    }
}