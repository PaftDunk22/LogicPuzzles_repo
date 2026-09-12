using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayZoom : MonoBehaviour
{
    public RectTransform PuzzleLayout;
    public RectTransform ControlPanel;
    public float zoomSpeed = 0.1f;
    public float minZoom = 0.5f;
    public float maxZoom = 3f;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        yield return FitUI();
    }

    void Update()
    {

        float scroll = 0f;

        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 2)
            {
                Touch a = Input.GetTouch(0);
                Touch b = Input.GetTouch(1);

                float prevDistance = Vector2.Distance(
                    a.position - a.deltaPosition,
                    b.position - b.deltaPosition);

                float currentDistance = Vector2.Distance(a.position, b.position);

                scroll = (currentDistance - prevDistance) * 0.01f;
            }
        }
        else
        {
            scroll = Input.mouseScrollDelta.y;
        }

        if (scroll != 0)
        {
            float scale = Mathf.Clamp(
                PuzzleLayout.localScale.x + scroll * zoomSpeed,
                minZoom,
                maxZoom);

            PuzzleLayout.localScale = Vector3.one * scale;
        }
    }

    bool IsOnScreen(RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        Canvas canvas = rect.GetComponentInParent<Canvas>();

        Camera cam = null;

        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;

        foreach (Vector3 corner in corners)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, corner);

            if (screenPos.x < 0 || screenPos.x > Screen.width ||
                screenPos.y < 0 || screenPos.y > Screen.height)
            {
                return false;
            }
        }

        return true;
    }

    IEnumerator FitUI()
    {
        PuzzleLayout.localScale = Vector3.one;

        while (!IsOnScreen(ControlPanel))
        {
            PuzzleLayout.localScale -= Vector3.one * zoomSpeed;

            Canvas.ForceUpdateCanvases();

            yield return null;

        }

        PuzzleLayout.localScale -= Vector3.one * zoomSpeed;
    }
}
