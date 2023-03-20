using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover3 : MonoBehaviour
{
    public enum State
    {
        Idle,
        Walking,
        Jumping,
        Falling,
        Dying
    }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    private Rigidbody rb;

    public State currentState;
    private Vector3 movement;
    private bool isGrounded = true;
    private float distToGround = 0.1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;

        currentState = State.Idle;
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;

            case State.Walking:
                Walk();
                break;

            case State.Jumping:
                Jump();
                break;

            case State.Falling:
                Fall();
                break;

            case State.Dying:
                Die();
                break;
        }
    }

    private void Idle()
    {
        // Get input from user
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Move character
        Move(horizontalInput, verticalInput);

        // Change state to walking if the character is moving
        if (horizontalInput != 0f || verticalInput != 0f)
        {
            currentState = State.Walking;
        }

        // Jump if user presses spacebar and character is on the ground
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            currentState = State.Jumping;
        }
    }

    private void Walk()
    {
        // Get input from user
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Move character
        Move(horizontalInput, verticalInput);

        // Change state to idle if the character stops moving
        if (horizontalInput == 0f && verticalInput == 0f)
        {
            currentState = State.Idle;
        }

        // Jump if user presses spacebar and character is on the ground
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            currentState = State.Jumping;
        }
    }

    private void Jump()
    {
        // Add upward force to make character jump
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;

        // Change state to falling
        currentState = State.Falling;
    }

    //With this implementation, character is able to move while falling
    private void Fall()
    {
        // Get input from user
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        // Move character
        Move(horizontalInput, verticalInput);
        // Check if character has landed on the ground
        if (isGrounded)
        {
            currentState = State.Idle;
        }
    }

    private void Move(float horizontal, float vertical)
    {
        // Calculate movement direction
        movement = new Vector3(horizontal, 0f, vertical).normalized;

        // Move character
        rb.MovePosition(transform.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void Die()
    {
        gameObject.GetComponent<PlayerHealth>().Die();
    }


    private void OnCollisionEnter(Collision collision)
    {
        // Check if character has collided with the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            // Change state to idle or walking, depending on whether the user is moving
            if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
            {
                currentState = State.Walking;
            }
            else
            {
                currentState = State.Idle;
            }
        }
    }
    //Important to first set the deathzone as "Trigger"
    private void OnTriggerEnter(Collider other)
    {
        //If the player falls and touches the Death zone, it dies
        if (other.tag=="Death")
        {

            
            currentState = State.Dying;
            print("sono morto");
            
        }
    }
}
