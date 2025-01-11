using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        //SceneManager.LoadScene(SceneManager.GetSceneByName("SampleScene").buildIndex);
        SceneManager.LoadScene(1);
    }

    public void PlayTutorial()
    {
        //SceneManager.LoadScene(SceneManager.GetSceneByName("TutorialScene").buildIndex);
        SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}