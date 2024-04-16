using System.Collections;
using System.Collections.Generic;
using MimicSpace;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    public float sprintSpeed;
    public float moveSpeed;
    public bool isSprinting;
    public Transform orient;
    float horizontal;
    float vertical;

    public float jumpForce;
    public float jumpCooldownTime;
    public float airMultiplier;
    bool canJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundDrag;
    bool isGrounded;
    Vector3 moveDir;
    Rigidbody rb;
    public float height = 1;
    public Vector3 desiredHeight = Vector3.zero;
    public float velocityLerpCoef = 4f;
    Mimic myMimic;

    [SerializeField] GameObject mimicBody;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        ResetCanJump();
        myMimic = FindAnyObjectByType<Mimic>();
    }

    private void Update()
    {
        InputHandler();
        LimitSpeed();

        // Handle drag
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        moveSpeed = isSprinting ? sprintSpeed : speed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        if(Physics.Raycast(mimicBody.transform.position, Vector3.down, 1.1f, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if(isGrounded && rb.velocity.y < 0.1f) // If the character is grounded and the y velocity is less than 0.1
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset the y velocity to 0
        }

        Movement();
    }

    private void InputHandler()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(jumpKey) && canJump == true) // If the jump key is pressed and the character can jump
        {
            canJump = false;
            Debug.Log("Jump");
            Jump(jumpForce);
            Invoke(nameof(ResetCanJump), jumpCooldownTime);
        }
    }

    private void Movement()
    {
        // calculate move direction
        moveDir = orient.forward * vertical + orient.right * horizontal;
        
        // on ground
        if(isGrounded)
        {
            rb.AddForce(10f * moveSpeed * moveDir.normalized, ForceMode.Force);
        }
        // if in air
        else if(!isGrounded)
        {
            rb.AddForce(10f * airMultiplier * moveSpeed * moveDir.normalized, ForceMode.Force);
        }

        // Assigning velocity to the mimic to assure great leg placement
        myMimic.velocity = rb.velocity;
        ManageHeight(); // Manage the height of character

        // Play playerSteps audio
        if (rb.velocity.magnitude > 0.1f && isGrounded)
        {
            FindObjectOfType<AudioManager>().PlayRandomPlayerSteps();
        }
    }

    
    private void LimitSpeed()
    {
        Vector3 vel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if(vel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = vel.normalized*moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    } 

    private void Jump(float jumpForce)
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset the y velocity to 0

        // Add force to jump in oriented direction
        rb.AddForce(orient.up * jumpForce, ForceMode.Impulse);
        // Assigning velocity to the mimic to assure great leg placement
        myMimic.velocity = rb.velocity;
    }

    private void ManageHeight() { // Function to manage the height of the character
        // Cast a ray downwards to detect the ground
        RaycastHit hit;
        float raycastDistance = 100f;
        if (Physics.Raycast(mimicBody.transform.position + Vector3.up * 5f, -Vector3.up, out hit, raycastDistance, groundLayer))
        {
            Debug.DrawRay(mimicBody.transform.position + Vector3.up * 5f, -Vector3.up * hit.distance, Color.yellow);
            //Debug.Log("Hit Ground");
            // If the ground is detected, set the desired height to be the height above the ground
            desiredHeight = new Vector3(transform.position.x, hit.point.y + height, transform.position.z);
        }
        else // If the ground is not detected, assume in air and don't adjust the height
        {
            desiredHeight = transform.position;
        }
        // Lerp the character's position towards the desired height
        transform.position = Vector3.Lerp(transform.position, desiredHeight, velocityLerpCoef * Time.deltaTime);

        

        // Cast a ray downwards to detect the ground
        /*
        RaycastHit hit;
        float raycastDistance = 100f;
        if (Physics.Raycast(mimicBody.transform.position + Vector3.up * 5f, -Vector3.up, out hit, raycastDistance, groundLayer))
        {
            Debug.DrawRay(mimicBody.transform.position + Vector3.up * 5f, -Vector3.up * hit.distance, Color.yellow);
            // If the ground is detected, set the desired height to be the height above the ground
            desiredHeight = new Vector3(transform.position.x, hit.point.y + height, transform.position.z);
        }
        else // If the ground is not detected, assume in air and don't adjust the height
        {
            desiredHeight = transform.position;
        }

        // Calculate the direction and magnitude of the force to apply
        Vector3 forceDirection = (desiredHeight - transform.position).normalized;
        float forceMagnitude = (desiredHeight - transform.position).magnitude * velocityLerpCoef;

        // Apply the force
        GetComponent<Rigidbody>().AddForce(forceDirection * forceMagnitude, ForceMode.VelocityChange);

        */
    }

    private void ResetCanJump(){
        canJump = true;
    }

    public void IncreaseSpeed(float amount)
    {
        speed += amount;
    }

    public void DecreaseSpeed(float amount)
    {
        speed -= amount;
    }

    public void IncreaseJumpForce(float amount)
    {
        Debug.Log("Jump Force Increased");
        jumpForce += amount;
    }

    


}
