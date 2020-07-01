using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool isDead = false;
    public int maxHealth = 100;
    public float speed = 1f;
    public float changeStateChance = 50f;
    public float damage = 10f;
    private Animator animator;
    private int currentHealth;

    [SerializeField] private LayerMask ignoreLayerMask;

    private float attackRange = 1f;
    private float rayDistance = 5f;
    private float stoppingDistance = 5f;

    private Vector3 destination;
    private Quaternion desiredRotation;
    private Vector3 direction;
    private EnemyState currentState;
    public PlayerController target;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        currentState = EnemyState.Idle;
    }


    void Update()
    {
        float changeStateRNG = Random.Range(0f, 1000f);
        if(currentState == EnemyState.Idle || currentState == EnemyState.Wander)
            RNGState(changeStateRNG);

        switch (currentState)
        {
            case EnemyState.Idle:
                {
                    PlayIdle();
                    //Idele Animation
                    animator.SetBool("isWandering", false);
                    animator.SetBool("isChasing", false);
                    animator.SetBool("isAttacking", false);
                    break;
                }
            case EnemyState.Wander:
                {
                    PlayWander();
                    //walk animation
                    animator.SetBool("isWandering", true);
                    animator.SetBool("isChasing", false);
                    animator.SetBool("isAttacking", false);
                    break;
                }
            case EnemyState.Chase:
                {
                    PlayChase();
                    //chase animation
                    animator.SetBool("isWandering", false);
                    animator.SetBool("isChasing", true);
                    animator.SetBool("isAttacking", false);
                    break;
                }
            case EnemyState.Attack:
                {
                    PlayAttack();
                    //attack animation
                    animator.SetBool("isWandering", false);
                    animator.SetBool("isChasing", false);
                    animator.SetBool("isAttacking", true);
                    break;
                }
        }
    }

    private void CheckPlayerInRange()
    {
        var targetToAggro = CheckForAggro();
        if (targetToAggro != null)
        {
            target = targetToAggro.GetComponent<PlayerController>();
            currentState = EnemyState.Chase;
        }
    }

    private void RNGState(float RNG)
    {
        

        if (RNG <= changeStateChance)
        {
            currentState = EnemyState.Wander;
        }
        else
        {
            currentState = EnemyState.Idle;
        }
    }
    private void PlayIdle()
    {
        CheckPlayerInRange();
    }

    private void PlayWander()
    {
        if (NeedsDestination())
        {
            GetDestination();
        }

        transform.rotation = desiredRotation;

        //transform.Translate(Vector3.forward * Time.deltaTime * speed);

        var rayColor = IsPathBlocked() ? Color.red : Color.green;
        Debug.DrawRay(transform.position, direction * rayDistance, rayColor);

        while (IsPathBlocked())
        {
            Debug.Log("Path Blocked");
            GetDestination();
        }

        CheckPlayerInRange();
    }

    private void PlayChase()
    {
        if(target == null)
        {
            currentState = EnemyState.Wander;
            return;
        }

        transform.LookAt(target.transform);

        if (Vector3.Distance(transform.position, target.transform.position) < attackRange)
        {
            currentState = EnemyState.Attack;
        }
    }

    private void PlayAttack()
    {
        if (target != null)
        {
            target.TakeDamage(damage);
        }


    }

    private bool IsPathBlocked()
    {
        Ray ray = new Ray(transform.position, direction);
        var hitSomething = Physics.RaycastAll(ray, rayDistance, ignoreLayerMask);
        return hitSomething.Any();
    }

    private bool NeedsDestination()
    {
        if (destination == Vector3.zero) return true;

        var curDistance = Vector3.Distance(transform.position, destination);
        if (curDistance <= stoppingDistance) return true;

        return false; 
    }

    private void GetDestination()
    {
        Vector3 testPosition = (transform.position + (transform.forward * 4f)) +
                               new Vector3(UnityEngine.Random.Range(-4.5f, 4.5f), 0f,
                                   UnityEngine.Random.Range(-4.5f, 4.5f));

        destination = new Vector3(testPosition.x, 1f, testPosition.z);

        direction = Vector3.Normalize(destination - transform.position);
        direction = new Vector3(direction.x, 0f, direction.z);
        desiredRotation = Quaternion.LookRotation(direction);
    }

    Quaternion startingAngle = Quaternion.AngleAxis(-60, Vector3.up);
    Quaternion stepAngle = Quaternion.AngleAxis(5, Vector3.up);

    private Transform CheckForAggro()
    {
        float aggroRadius = 5f;

        RaycastHit hit;
        var angle = transform.rotation * startingAngle;
        var direction = angle * Vector3.forward;
        var pos = transform.position;
        for (var i = 0; i < 24; i++)
        {
            if (Physics.Raycast(pos, direction, out hit, Mathf.Infinity))
            {
                var targetCheck = hit.collider.GetComponent<PlayerController>();
                if (targetCheck != null)
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.red);
                    Debug.Log(hit.transform.name);
                    return targetCheck.transform;
                }
                else
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(pos, direction * aggroRadius, Color.white);
            }
            direction = stepAngle * direction;
        }

        return null;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        //play hurt animation
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        Debug.Log("Enemy Dead!");

        isDead = true;
        //die animation
        animator.SetBool("isDead", true);

        GetComponent<Collider>().enabled = false;
        this.enabled = false;
    }

    public enum EnemyState
    {
        Idle,
        Wander,
        Chase,
        Attack
    }
}
