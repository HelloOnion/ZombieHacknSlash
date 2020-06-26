using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{

    public CharacterController controller;
    public Transform cam;

    //Movement
    float Speed = 2f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public float walkSpeed = 2f;
    public float sprintSpeed = 5f;
    bool sprint = false;

    //Gravity
    Vector3 velocity;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;

    public float jumpHeight = 3f;

    //Animation
    public Animator animator;


    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        Movement();
        GravityCheck();
    }

    private void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //Sprint Trigger
        if (Input.GetButtonDown("Sprint") && !sprint)
        {
            Speed = sprintSpeed;
            sprint = true;
        }
        if (Input.GetButtonUp("Sprint") && sprint)
        {
            Speed = walkSpeed;
            sprint = false;
        }

        //Movement & Camera
        if (direction.magnitude >= 0.1)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;



            controller.Move(moveDir.normalized * Speed * Time.deltaTime);
        }

        //Jump
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //Animation
        animator.SetFloat("Magnitude", direction.magnitude);
        animator.SetBool("Sprint", sprint);

    }

    private void GravityCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}

