using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    GameObject pannelUI;

    void Awake()
    {
        pannelUI = GameObject.Find("Background");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(0);
        pannelUI.SetActive(false);
    }

    public void PlayTutorial()
    {
        SceneManager.LoadScene(1);
        pannelUI.SetActive(false);
    }

    public void BackToMenu()
    {
        pannelUI.SetActive(true);
    }
}


/*
what if new scene for mainmenu like everyone does it
pause screen main menu button controlled in MainMenu? script (on click enable Background) how does it work with pausing
play button scene change to SampleScene (buildindex 0), disables Background
tutorial button scene change to TutorialScene (buildindex 1), disables Background
*/