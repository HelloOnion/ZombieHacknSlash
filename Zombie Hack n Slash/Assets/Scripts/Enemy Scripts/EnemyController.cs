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
    public float attackDamage = 10f;
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
    public PlayerController target;

    [Header("Visible Range")]
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public float meshResolution;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        currentState = EnemyState.Idle;
    }


    void Update()
    {
        float changeStateRNG = Random.Range(0f, 1000f);
        if (currentState == EnemyState.Idle || currentState == EnemyState.Wander)
            RNGState(changeStateRNG);

        //FOV GUI
        DrawFOV();

        switch (currentState)
        {
            case EnemyState.Idle:
                {
                    IdleState();
                    //Idele Animation
                    animator.SetBool("isWandering", false);
                    animator.SetBool("isChasing", false);
                    animator.SetBool("isAttacking", false);
                    break;
                }
            case EnemyState.Wander:
                {
                    WanderState();
                    //walk animation
                    animator.SetBool("isWandering", true);
                    animator.SetBool("isChasing", false);
                    animator.SetBool("isAttacking", false);
                    break;
                }
            case EnemyState.Chase:
                {
                    ChaseState();
                    //chase animation
                    animator.SetBool("isWandering", false);
                    animator.SetBool("isChasing", true);
                    animator.SetBool("isAttacking", false);
                    break;
                }
            case EnemyState.Attack:
                {
                    AttackState();
                    //attack animation
                    animator.SetBool("isWandering", false);
                    animator.SetBool("isChasing", false);
                    animator.SetBool("isAttacking", true);
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
                }
                else //obstacle blocked or out of ranged
                {
                    //Search for enemy
                    currentState = EnemyState.Wander;
                }
            }
        }
    }
    //FOV Editor GUI
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

    private void DrawFOV()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        for(int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle/2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);   
            viewPoints.Add(newViewCast.point);
        }
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        // vertices[0] = Vector3.zero;
        // for(int i = 0; i < vertexCount - 1; i++)
        // {
        //     vertices[i+1] = viewPoints[i];

        //     triangles[i * 3] = 0;
        //     triangles[i * 3 + 1] = i + 1;
        //     triangles[i * 3 + 2] = i + 2;
        // }
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;
        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
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
       // CheckPlayerInRange();
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    private void WanderState()
    {
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

        //CheckPlayerInRange();
        StartCoroutine("FindTargetsWithDelay", 0.2f);
    }

    private void ChaseState()
    {
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

}
