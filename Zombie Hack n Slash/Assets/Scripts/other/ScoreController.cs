using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public static ScoreController scoreInstance;
    
    private int currentScore = 0;
    private int zombieKilled = 0;

    [Header("UI")]
    public Text scoreText;
    public GameObject scoreBoard;
    public Text scoreResultText;
    public Text killResultText;

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
        zombieKilled = 0;
        LeanTween.scale(scoreBoard.gameObject, new Vector3(0,0,0), 0f).setIgnoreTimeScale(true);
    }

    void Update()
    {
        scoreText.text = "Score: " + currentScore;
    }

    public void ShowResult()
    {
        //scoreBoard.SetActive(true);
        LeanTween.scale(scoreBoard.gameObject, new Vector3(1, 1, 1), 0.4f).setIgnoreTimeScale(true);
        StopAllCoroutines();
        StartCoroutine(ScoreCount());
    }

    IEnumerator ScoreCount()
    {
        for(int i = 0; i <= currentScore; i++)
        {
            scoreResultText.text = "Score: " + i;
            yield return null;
        }
        StartCoroutine(KillCount());
    }

    IEnumerator KillCount()
    {
        for(int i = 0; i <= zombieKilled; i++)
        {
            killResultText.text = "Kills: " + i;
            yield return null;
        }
    }

    public void AddScore(int addScore){currentScore += addScore;}
    public void AddKillCount(int killCount){zombieKilled += killCount;}
}
