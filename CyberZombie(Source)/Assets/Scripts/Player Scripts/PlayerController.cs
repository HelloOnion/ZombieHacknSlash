using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    [Header("Controller")]
    public CharacterController controller;
    public Transform cam;

    //Movement
    [Header("Movement")]
    [Range(0, 1)]
    public float turnSmoothTime = 0.1f;
    public float walkSpeed = 2f;
    private float speed = 2f;
    private float turnSmoothVelocity;

    //Jump & Gravity
    [Header("Gravity")]
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float jumpHeight = 3f;
    private Vector3 velocity;
    private bool isGrounded;

    //Animation
    [Header("Animator")]
    public float attackAnimTimer = 1;
    private Animator animator;
    
    //Combat
    [Header("Combat")]
    public int attackDamage = 40;
    public float attackRate = 2f;
    public Collider hitCollider;
    public float skillRadius;
    private int minLightDmg = 5;
    private int maxLightDmg = 20;
    private int minHeavyDmg = 20;
    private int maxHeavyDmg = 50;
    private float nextAttackTime = 0f;
    private bool isAttacking = false;


    //Misc
    private bool isDead = false;
    private float maxHealth = 100f;
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

            CheckAttacking();

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
           // if (Input.GetKeyDown(KeyCode.G)) TakeDamage(20);
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

    private void CheckAttacking()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }
    }

    void LightAttack()
    {
        attackDamage = Random.Range(minLightDmg, maxLightDmg);
        //light attack Animation
        animator.SetTrigger("LightAttack");
    }

    void HeavyAttack()
    {
        attackDamage = Random.Range(minHeavyDmg, maxHeavyDmg);
        //heavy attack Animation
        animator.SetTrigger("HeavyAttack");
    }

    // void AOEAttack()
    // {
    //     //instantiate effect

    //     Collider[] colliders = Physics.OverlapSphere(transform.position, skillRadius);
    //     foreach(Collider nearbyObj in colliders)
    //     {
    //         Rigidbody rb = nearbyObj.GetComponent<Rigidbody>();
    //         if(rb != null)
    //         {
                
    //         }
    //     }
    // }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        if (currentHealth <= 0) 
        {
            currentHealth = 0;
            Die();
        }

    }

    void Die()
    {
        isDead = true;
        animator.SetBool("isDead", true);
        FindObjectOfType<GameManager>().GameOver();
        GetComponent<Collider>().enabled = false;

       // this.enabled = false;
    }

    public float GetCurrentHealth() { return currentHealth; }

}

