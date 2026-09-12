using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("Music", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFX", 1f);
    }

    public void MusicChanged(float v)
    {
        MusicManager.Instance.SetVolume(v);
    }

    public void SFXChanged(float v)
    {
        UISFXManager.Instance.SetVolume(v);
    }
}