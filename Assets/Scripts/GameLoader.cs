using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    [SerializeField] public string m_scene;
    public bool resetGame = false;

    public static Action OnResetGame;

    // Update is called once per frame
    void OnEnable()
    {
        ResetGame();
        StartCoroutine(LoadYourAsyncScene(m_scene));
    }

    public static IEnumerator LoadYourAsyncScene(string scene)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void ResetGame()
    {
        if (!resetGame) return;

        OnResetGame?.Invoke();
    }
}
