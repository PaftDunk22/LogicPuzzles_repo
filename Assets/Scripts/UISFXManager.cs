using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UISFXManager : MonoBehaviour
{
    public static UISFXManager Instance;
    public AudioSource source;
    public AudioClip click;
    public AudioClip solvedClip;

    float volume = 1f;

    void Start()
    {
        Button[] buttons = FindObjectsOfType<Button>(true);

        foreach (Button b in buttons)
        {
            b.onClick.AddListener(() =>
            {
                UISFXManager.Instance.PlayClick();
            });
        }
    }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        volume = PlayerPrefs.GetFloat("SFX", 1f);
    }

    public void PlayClick()
    {
        source.PlayOneShot(click, volume);
    }

    public void SetVolume(float v)
    {
        volume = v;
        PlayerPrefs.SetFloat("SFX", v);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Button[] buttons = FindObjectsOfType<Button>(true);

        foreach (Button b in buttons)
        {
            b.onClick.RemoveAllListeners();

            b.onClick.AddListener(() =>
            {
                UISFXManager.Instance.PlayClick();
            });
        }
    }

    public void PlaySolved()
    {
        source.PlayOneShot(solvedClip, volume);
    }
}