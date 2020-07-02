using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public CharacterController controller;
    public Transform cam;

    //Movement
    [Header("Movement")]
    public float turnSmoothTime = 0.1f;
    public float walkSpeed = 2f;
    private float speed = 2f;
    private float turnSmoothVelocity;

    //Jump & Gravity
    [Header("Gravity & Jump")]
    Vector3 velocity;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float jumpHeight = 3f;
    private bool isGrounded;

    //Animation
    public float attackAnimTimer = 1;
    private Animator animator;
    
    //Combat
    public int attackDamage = 40;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public float attackRate = 2f;
    public Collider hitCollider;
    private float nextAttackTime = 0f;
    private bool isAttacking = false;


    //Misc
    private bool isDead = false;
    private float maxHealth = 100f;
    [SerializeField]
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        speed = walkSpeed;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(!isDead)
        {
            if (!isAttacking)
            {
                Movement();
            }
            else
            {
                StartCoroutine(resetMovement());
            }

            GravityCheck();

            if (Time.time >= nextAttackTime)
            {
                if (Input.GetButtonDown("Fire1") && isGrounded)
                {
                    // isAttacking = true;
                    LightAttack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }
                if(Input.GetButtonDown("Fire2") && isGrounded)
                {
                    HeavyAttack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            }
            
            //damage debug test
            if (Input.GetKeyDown(KeyCode.G)) TakeDamage(20);
        }
    }
    
    private void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //Movement & Camera
        if (direction.magnitude >= 0.1)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            //controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }


        //Jump
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        //Dash
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {

            animator.SetTrigger("Dash");
        }

        //Animation
        animator.SetFloat("Magnitude", direction.magnitude);
    }

    private void GravityCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        animator.SetBool("isGrounded", isGrounded);
    }

    IEnumerator resetMovement()
    {
        yield return new WaitForSeconds(attackAnimTimer);
        isAttacking = false;
    }

    void LightAttack()
    {
        //light attack Animation
        animator.SetTrigger("LightAttack");

        CheckHit();
    }

    void HeavyAttack()
    {
        //heavy attack Animation
        animator.SetTrigger("HeavyAttack");
        
        CheckHit();
    }

    void CheckHit()
    {
        //hitCollider.enabled = true;
        //Detect enemies in range of attack
        //Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        //Damage Enemy
        //foreach (Collider enemy in hitEnemies)
        //{
        //    Debug.Log("Enemy Hit!" + enemy.name);
        //    enemy.GetComponent<EnemyController>().TakeDamage(attackDamage);
        //}
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        animator.SetBool("isDead", true);
        this.enabled = false;
    }

    public float GetCurrentHealth() { return currentHealth; }

}

