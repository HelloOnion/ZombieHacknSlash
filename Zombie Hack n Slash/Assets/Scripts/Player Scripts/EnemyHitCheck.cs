using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitCheck : MonoBehaviour
{
    public PlayerController player;
    private void OnTriggerEnter(Collider enemy)
    {
        if(enemy.tag == "Enemy")
        {
            Debug.Log("Enemy Hit!" + enemy.name);
            enemy.GetComponent<EnemyController>().TakeDamage(player.attackDamage);
        }
        else
        {
            return;
        }
    }
}
