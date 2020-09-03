using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManagerInstance;

    [Header("UI")]
    public Text clearText;
    public Text gameOverText;

    [Header("PauseUI")]
    public GameObject pauseUI;
    private bool isPaused = false;

    private void Awake() 
    {
        LeanTween.scale(pauseUI.gameObject, new Vector3(0, 0, 0), 0f).setIgnoreTimeScale(true);

        if(gameManagerInstance != null && gameManagerInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            gameManagerInstance = this;
        }
    }

    void Update()
    {
        if(!isPaused && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void GameClear()
    {
        Debug.Log("Game Cleared!");
        clearText.gameObject.SetActive(true);
        FindObjectOfType<ScoreController>().ShowResult();
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameOverText.gameObject.SetActive(true);
        FindObjectOfType<ScoreController>().ShowResult();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        LeanTween.scale(pauseUI.gameObject, new Vector3(1, 1, 1), 0.4f).setIgnoreTimeScale(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        LeanTween.scale(pauseUI.gameObject, new Vector3(0, 0, 0), 0.4f).setIgnoreTimeScale(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
    }

    

}
