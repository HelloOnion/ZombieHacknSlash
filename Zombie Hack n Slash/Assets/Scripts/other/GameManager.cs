using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManagerInstance;

    public Text clearText;
    

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

    void Start()
    {
        
    }

    public void GameClear()
    {
        Debug.Log("Game Cleared!");

        Time.timeScale = 0;

    }

}
