using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{

    public GameObject exitPanel = null;

    private void Awake()
    {
        if(SceneManager.GetActiveScene().name == "MainMenu") 
        {
            exitPanel = GameObject.Find("ExitPanel");
            exitPanel.SetActive(false);
        }
    }
    public void LoadNextScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex + 1);

    }

    public void LoadPreviousScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex - 1);
    }

    public void LoadMosaicScene()
    {
        SceneManager.LoadScene("MosaicSelection");
    }

    public void LoadNonogramScene()
    {
        SceneManager.LoadScene("NonogramSelection");
    }

    public void LoadSkyscraperScene()
    {
        SceneManager.LoadScene("SkyscraperSelection");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnExitPressed()
    {
        exitPanel.SetActive(true);
    }

    public void OnConfirmExit()
    {
        exitPanel.SetActive(false);
        Exit();
    }

    public void OnCancelExit()
    {
        exitPanel.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
