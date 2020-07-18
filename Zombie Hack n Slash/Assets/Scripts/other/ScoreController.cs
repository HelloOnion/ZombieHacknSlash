using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public static ScoreController scoreInstance;
    
    private int currentScore = 0;

    [Header("UI")]
    public Text scoreText;

    private void Awake() 
    {   
        if(scoreInstance != null && scoreInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            scoreInstance = this;
        }
    }

    void Start()
    {
        currentScore = 0;
    }

    void Update()
    {
        scoreText.text = "Score: " + currentScore;
    }

    public void AddScore(int addScore)
    {
        currentScore += addScore;
    }
}
