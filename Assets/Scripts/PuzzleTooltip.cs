using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleTooltip : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private TooltipManager tooltipManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipManager.ShowTooltip(gameObject.name,transform as RectTransform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipManager.HideTooltip();
    }
}