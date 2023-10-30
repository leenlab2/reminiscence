using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public string mainMenu;
    public static bool GameIsPaused = false;

    public void TogglePauseMenu()
    {
        if (!GameIsPaused)
        {
            OpenPauseMenu();
        }
        else
        {
            ClosePauseMenu();
        }
    }
    public void GoMainMenu()
    {

        SceneManager.LoadScene(mainMenu);
    }

    public void ClosePauseMenu()
    {
        transform.FindChild("PauseMenuUI").gameObject.SetActive(false);
        GameIsPaused = false;
    }
    public void OpenPauseMenu()
    {
        transform.FindChild("PauseMenuUI").gameObject.SetActive(true);
        GameIsPaused = true;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting");
    }
}
