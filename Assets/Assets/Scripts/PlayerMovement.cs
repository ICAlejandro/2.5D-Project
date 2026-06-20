using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 30f;

    private Rigidbody rb;
    private Vector3 moveInput;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    // Track the last direction the player moved in ("Right", "Left", "Up", "Down")
    private string lastDirection = "Up";

    void Start()
    {
        // Cache our physics, animation, and rendering components
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Capture keyboard inputs immediately with GetAxisRaw
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // 1. Get the camera's rotational angle, but ONLY its left/right spin (Y-axis)
        // This completely eliminates errors caused by the camera tilting downward!
        float cameraYRotation = Camera.main.transform.eulerAngles.y;
        Quaternion cameraRotation = Quaternion.Euler(0f, cameraYRotation, 0f);

        // 2. Create a clean local input vector based on your keys
        Vector3 rawInput = new Vector3(moveX, 0f, moveZ).normalized;

        // 3. Rotate our input vector to match the camera's compass heading
        moveInput = cameraRotation * rawInput;

        // Pass the raw keyboard inputs directly to the animation handler.
        // Because moveInput is now perfectly aligned with the floor grid, 
        // raw inputs will accurately match what your eyes expect on screen!
        UpdateFacingDirection(moveX, moveZ);

        // Handle Animations based on direction and movement state
        HandleAnimations();
    }

    void FixedUpdate()
    {
        // Move the Rigidbody container safely through physical 3D space
        rb.linearVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, moveInput.z * moveSpeed);
    }

    void UpdateFacingDirection(float x, float z)
    {
        // Only update the direction if the player is actively moving
        if (moveInput.magnitude > 0)
        {
            // Prioritize horizontal movement (Left / Right) over vertical
            if (Mathf.Abs(x) > Mathf.Abs(z))
            {
                if (x > 0) lastDirection = "Right";
                else if (x < 0) lastDirection = "Left";
            }
            // Prioritize vertical movement (Up / Down)
            else
            {
                if (z > 0) lastDirection = "Up";
                else if (z < 0) lastDirection = "Down";
            }
        }
    }

    void HandleAnimations()
    {
        // STATE 1: PLAYER IS WALKING
        if (moveInput.magnitude > 0)
        {
            if (lastDirection == "Right")
            {
                anim.Play("Player_Walk_Right");
                spriteRenderer.flipX = false;
            }
            else if (lastDirection == "Left")
            {
                anim.Play("Player_Walk_Left");
            }
            else if (lastDirection == "Up")
            {
                anim.Play("Player_Walk_Up");
            }
            else if (lastDirection == "Down")
            {
                anim.Play("Player_Walk_Down");
            }
        }
        // STATE 2: PLAYER IS IDLE (Standing Still)
        else
        {
            if (lastDirection == "Right")
            {
                anim.Play("Player_Idle_Right");
                spriteRenderer.flipX = false;
            }
            else if (lastDirection == "Left")
            {
                anim.Play("Player_Idle_Left");
            }
            else if (lastDirection == "Up")
            {
                anim.Play("Player_Idle_Up");
            }
            else if (lastDirection == "Down")
            {
                anim.Play("Player_Idle_Down");
            }
        }
    }
}