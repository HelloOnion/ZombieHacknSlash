using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool isDead = false;
    public int maxHealth = 100;
    private int currentHealth;

    public Animator animator;
    
    void Start()
    {
        currentHealth = maxHealth;
    }

   
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        //play hurt animation
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        Debug.Log("Enemy Dead!");

        isDead = true;
        //die animation
        animator.SetBool("isDead", true);

        GetComponent<Collider>().enabled = false;
        this.enabled = false;
    }
}
