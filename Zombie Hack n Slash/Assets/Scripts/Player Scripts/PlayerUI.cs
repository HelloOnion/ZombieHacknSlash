using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Text healthText;
    public Image hpBar;
    
    [SerializeField]
    private PlayerController player;

    void Update()
    {
        hpBar.fillAmount = player.GetCurrentHealth() / 100;
        healthText.text = player.GetCurrentHealth() + "/100";
    }
}
