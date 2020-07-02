using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitCheck : MonoBehaviour
{
    public PlayerController player;
    private void OnTriggerEnter(Collider hit)
    {
        if(hit.tag == "Enemy")
        {
            Debug.Log(hit.name);
            hit.GetComponent<EnemyController>().TakeDamage(player.attackDamage);
        }
    }
}
