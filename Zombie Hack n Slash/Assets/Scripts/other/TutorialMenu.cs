using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMenu : MonoBehaviour
{
    public GameObject tutorialBox;
    public GameObject comboBox;
    public GameObject menuBox;

    public void ShowTutorial()
    {
        menuBox.SetActive(false);
        tutorialBox.SetActive(true);
    }

    public void ShowCombos()
    {
        tutorialBox.SetActive(false);
        comboBox.SetActive(true);
    }

    public void HideTutorial()
    {
        tutorialBox.SetActive(false);
        menuBox.SetActive(true);
    }

    public void HideCombos()
    {
        comboBox.SetActive(false);
        tutorialBox.SetActive(true);
    }
}
