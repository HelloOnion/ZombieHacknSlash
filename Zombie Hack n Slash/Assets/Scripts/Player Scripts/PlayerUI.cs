﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Image hpBar;
    
    [SerializeField]
    private PlayerController player;

    void Update()
    {
        hpBar.fillAmount = player.GetCurrentHealth() / 100;
    }
}
