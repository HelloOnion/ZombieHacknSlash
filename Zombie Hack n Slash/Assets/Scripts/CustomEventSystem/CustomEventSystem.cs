using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomEventSystem : MonoBehaviour
{

    public static CustomEventSystem current; //singleton
    
    private void Awake() 
    {
        current = this;
    }

    public event Action<int> onDialogueTriggerEnter;
    public void DialogueTriggerEnter(int id)
    {
        if(onDialogueTriggerEnter != null)
        {
            onDialogueTriggerEnter(id);
        }
    }

    public event Action<int> onDialogueTriggerExit;
    public void DialogueTriggerExit(int id)
    {
        if(onDialogueTriggerExit != null)
        {
            onDialogueTriggerExit(id);
        }
    }
}
