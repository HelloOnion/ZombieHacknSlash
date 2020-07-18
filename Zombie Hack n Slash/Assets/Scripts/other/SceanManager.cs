using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceanManager : MonoBehaviour
{
    public GameObject loadingScreen;
    public GameObject menuBox;
    public Slider loadingBar;

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadScene(sceneIndex));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator LoadScene(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        menuBox.SetActive(false);
        loadingScreen.SetActive(true);

        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            loadingBar.value = progress;

            yield return null;
        }
    }
}
