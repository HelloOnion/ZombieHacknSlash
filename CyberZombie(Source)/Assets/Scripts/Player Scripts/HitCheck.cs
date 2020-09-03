using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCheck : MonoBehaviour
{
    public PlayerController player;
    public EnemyController enemy;

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("Enemy"))
        {
            Debug.Log(hit.name);
            hit.GetComponent<EnemyController>().TakeDamage(player.attackDamage);
        }
        else if(hit.CompareTag("Player"))
        {
            hit.GetComponent<PlayerController>().TakeDamage(Random.Range(enemy.minAttackDmg, enemy.maxAttackDmg));
        }  
    }
}
