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

    void Start()
    {
        sentences = new Queue<string>();
        LeanTween.scale(dialogueImage.gameObject, new Vector3(0,0,0), 0f);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        //TODO:pause game and Animate dialogue box in/ show cursor
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

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null; //return every 1 frame
        }
    }

    private void EndDialogue()
    {
        //TODO:Animate dialogue box out and un pause game/ hide cursor
        LeanTween.scale(dialogueImage.gameObject, new Vector3(0,0,0), 0.4f).setIgnoreTimeScale(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        Debug.Log("Dialogue Ended");
    }
}
