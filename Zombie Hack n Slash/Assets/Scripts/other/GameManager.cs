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

    private void Awake() 
    {
        if(gameManagerInstance != null && gameManagerInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            gameManagerInstance = this;
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

}
