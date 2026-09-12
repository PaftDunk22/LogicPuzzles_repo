using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public AudioSource source;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        source.volume = PlayerPrefs.GetFloat("Music", 1f);
        source.loop = true;
        source.Play();
    }

    public void SetVolume(float v)
    {
        source.volume = v;
        PlayerPrefs.SetFloat("Music", v);
    }
}