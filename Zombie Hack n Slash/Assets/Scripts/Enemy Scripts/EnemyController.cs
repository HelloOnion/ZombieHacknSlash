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

    public int minAttackDmg = 5;
    public int maxAttackDmg = 10;
    public int attackDamage;

    private Animator animator;
    private int currentHealth;

    private float attackRange = 1f;
    private float rayDistance = 5f;
    private float stoppingDistance = 5f;

    private Vector3 destination;
    private Quaternion desiredRotation;
    private Vector3 direction;
    private EnemyState currentState;

    [HideInInspector]
    public bool targetInRange = false;
    private PlayerController target;

    [Header("Visible Range")]
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    
    [Header("Visual FX")]
    public GameObject bloodFXPrefab;

    public GameObject miniMapUI;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        currentState = EnemyState.Idle;

        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    void Update()
    {
        float changeStateRNG = Random.Range(0f, 1000f);
        if (currentState == EnemyState.Idle || currentState == EnemyState.Wander)
            RNGState(changeStateRNG);

        switch (currentState)
        {
            case EnemyState.Idle:
                {
                    IdleState();
                    break;
                }
            case EnemyState.Wander:
                {
                    WanderState();
                    break;
                }
            case EnemyState.Chase:
                {
                    ChaseState();
                    break;
                }
            case EnemyState.Attack:
                {
                    AttackState();
                    break;
                }
        }
    }
    
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    private void FindVisibleTargets()
    {
        target = null;
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform targetTransform = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (targetTransform.position - transform.position).normalized;
            
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, targetTransform.position);

                //if raycast finds player without obstacle blocking
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask)) 
                {
                    //set target
                    target = targetTransform.GetComponent<PlayerController>();
                    //change state to chase
                    currentState = EnemyState.Chase;
                    targetInRange = true;
                }
                else //obstacle blocked
                {
                    //Search for enemy
                    currentState = EnemyState.Wander;
                    targetInRange = false;
                }
            }
        }
    }
    //FOV Editor GUI
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

//AI States
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

    private void IdleState()
    {
        //Idele Animation
        animator.SetBool("isWandering", false);
        animator.SetBool("isChasing", false);
        animator.SetBool("isAttacking", false);
    }

    private void WanderState()
    {
        //walk animation
        animator.SetBool("isWandering", true);
        animator.SetBool("isChasing", false);
        animator.SetBool("isAttacking", false);
        
        if (NeedsDestination())
        {
            GetDestination();
        }

        transform.rotation = desiredRotation;

        var rayColor = IsPathBlocked() ? Color.red : Color.green;
        Debug.DrawRay(transform.position, direction * rayDistance, rayColor);

        while (IsPathBlocked())
        {
            Debug.Log("Path Blocked");
            GetDestination();
        }
    }

    private void ChaseState()
    {
        //chase animation
        animator.SetBool("isWandering", false);
        animator.SetBool("isChasing", true);
        animator.SetBool("isAttacking", false);

        if(target == null)
        {
            currentState = EnemyState.Idle;
            return;
        }

        transform.LookAt(target.transform);

        if (Vector3.Distance(transform.position, target.transform.position) < attackRange)
        {
            currentState = EnemyState.Attack;
        }
    }

    private void AttackState()
    {
        attackDamage = Random.Range(minAttackDmg, maxAttackDmg);
        //attack animation
        animator.SetBool("isWandering", false);
        animator.SetBool("isChasing", false);
        animator.SetBool("isAttacking", true);
    }

    private bool IsPathBlocked()
    {
        Ray ray = new Ray(transform.position, direction);
        var hitSomething = Physics.RaycastAll(ray, rayDistance, obstacleMask);
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

    public enum EnemyState
    {
        Idle,
        Wander,
        Chase,
        Attack
    }

    public Vector3 GetTargetPosition()
    {
        if(target != null)
            return target.GetComponent<Transform>().position;

        return Vector3.zero;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Vector3 bloodPos = new Vector3(transform.position.x, 1.3f, transform.position.z);
        //Instantiate(bloodFXPrefab, bloodPos, Quaternion.Euler(0,transform.rotation.y - 180,0), transform);
        Instantiate(bloodFXPrefab, bloodPos, Quaternion.FromToRotation(Vector3.up, -transform.forward));
        
        //play hurt animation
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        Debug.Log("Enemy Dead!");

        //add score and kill count
        FindObjectOfType<ScoreController>().AddScore(100);
        FindObjectOfType<ScoreController>().AddKillCount(1);

        isDead = true;
        //die animation
        animator.SetBool("isDead", true);
        miniMapUI.SetActive(false);
        GetComponent<Collider>().enabled = false;
        this.enabled = false;
    }
}
