using TMPro;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    public GameObject panel;
    public TMP_Text messageText;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(string message, float duration = 1.5f)
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine(message, duration));
    }

    private System.Collections.IEnumerator ShowRoutine(string message, float duration)
    {
        messageText.text = message;
        panel.SetActive(true);

        yield return new WaitForSeconds(duration);

        panel.SetActive(false);
    }
}