using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNearestEnemy : MonoBehaviour
{
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            playerController.enemyList.Add(other.transform);
            Debug.Log("Enemy Found!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (playerController.enemyList.Contains(other.transform))
                {
                    playerController.enemyList.Remove(other.transform);
                    Debug.Log("Enemy Removed");
                }
        }
    }
}
