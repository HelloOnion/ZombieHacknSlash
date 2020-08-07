using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Text")]
    public Image dialogueImage;
    public Text nameText;
    public Text dialogueText;
    [Header("Add Dialogue")]
    public Queue<string> sentences;

    private bool isGameClear;

    void Start()
    {
        sentences = new Queue<string>();
        LeanTween.scale(dialogueImage.gameObject, new Vector3(0,0,0), 0f);
    }

    public void StartDialogue(Dialogue dialogue)//ダイアログを始まる
    {
        isGameClear = dialogue.gameClear;

        LeanTween.scale(dialogueImage.gameObject, new Vector3(1,1,1), 0.4f).setIgnoreTimeScale(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        Debug.Log("Dialogue Started");

        nameText.text = dialogue.name;
        sentences.Clear();

        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()//次の文をディスプレーする
    {
        if(sentences.Count == 0)//sentences に文をなくなったらダイアログを終了
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)//文を1字ずつ書く
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null; //return every 1 frame
        }
    }

    private void EndDialogue()//ダイアログを終了する
    {
        if(isGameClear)
        {
            //クリア画面へ進む
            FindObjectOfType<GameManager>().GameClear();
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }

        LeanTween.scale(dialogueImage.gameObject, new Vector3(0,0,0), 0.4f).setIgnoreTimeScale(true);
        Debug.Log("Dialogue Ended");
    }
}
