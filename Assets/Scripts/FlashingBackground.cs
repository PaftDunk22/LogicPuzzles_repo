using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlashingBackgroundCharacters : MonoBehaviour
{
    [Header("Font Settings")]
    public Font customFont;
    public int fontSize = 40;
    public Color characterColor = Color.white;

    [Header("Spawn Settings")]
    public RectTransform spawnArea;
    public float spawnInterval = 1f;
    public int charactersPerWave = 5;
    public float lifeTime = 3f;
    public float moveDistance = 50f;

    private string[] characterPool = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "*", "/", "+", "-", "¬", "∼", "∧", "∨", "⊻", "→", "⇒", "↔", "⇔", "⊤", "⊥", "∀", "∃", "∄", "∈", "∉", "⊆", "⊂", "⊇", "∪", "∩", "∅", "≠", "≡", "≜", "∴", "∵", "⊢", "⊨", "□", "◇" };

    void Start()
    {
        StartCoroutine(SpawnCharacters());
    }

    IEnumerator SpawnCharacters()
    {
        while (true)
        {
            for (int i = 0; i < charactersPerWave; i++)
            {
                SpawnCharacter();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnCharacter()
    {
        string randomChar = characterPool[Random.Range(0, characterPool.Length)];

        GameObject charObj = new GameObject("BGChar");
        charObj.transform.SetParent(spawnArea, false);

        Text text = charObj.AddComponent<Text>();
        text.text = randomChar;
        text.font = customFont;
        text.fontSize = fontSize;
        text.color = characterColor;
        text.alignment = TextAnchor.MiddleCenter;

        RectTransform rt = charObj.GetComponent<RectTransform>();
        rt.localScale = Vector3.one;
        rt.sizeDelta = new Vector2(fontSize * 2, fontSize * 2);
        rt.anchoredPosition = new Vector2(Random.Range(-spawnArea.rect.width / 2, spawnArea.rect.width / 2),Random.Range(-spawnArea.rect.height / 2, spawnArea.rect.height / 2));

        StartCoroutine(MoveFadeDestroy(text, rt, charObj));
    }

    IEnumerator MoveFadeDestroy(Text text, RectTransform rt, GameObject obj)
    {
        Vector2 startPos = rt.anchoredPosition;
        Vector2 endPos = startPos + Random.insideUnitCircle.normalized * moveDistance;

        float t = 0f;
        while (t < lifeTime)
        {
            t += Time.deltaTime;
            float progress = t / lifeTime;

            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, progress);

            Color c = text.color;
            c.a = Mathf.Lerp(1f, 0f, progress);
            text.color = c;

            yield return null;
        }

        Destroy(obj);
    }
}