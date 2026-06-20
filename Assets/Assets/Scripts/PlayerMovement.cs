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

    private string lastDirection = "Up";
    private Vector3 lookDirection = Vector3.forward; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        float cameraYRotation = Camera.main.transform.eulerAngles.y;
        Quaternion cameraRotation = Quaternion.Euler(0f, cameraYRotation, 0f);

        Vector3 rawInput = new Vector3(moveX, 0f, moveZ).normalized;
        moveInput = cameraRotation * rawInput;

        if (moveInput.magnitude > 0)
        {
            lookDirection = moveInput.normalized;
        }

        UpdateFacingDirection(moveX, moveZ);
        HandleAnimations();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, moveInput.z * moveSpeed);
    }

    public Vector3 GetLookDirection()
    {
        return lookDirection;
    }

    void UpdateFacingDirection(float x, float z)
    {
        if (moveInput.magnitude > 0)
        {
            if (Mathf.Abs(x) >= Mathf.Abs(z))
            {
                if (x > 0) lastDirection = "Right";
                else if (x < 0) lastDirection = "Left";
            }
            else
            {
                if (z > 0) lastDirection = "Up";
                else if (z < 0) lastDirection = "Down";
            }
        }
    }

    void HandleAnimations()
    {
        if (moveInput.magnitude > 0)
        {
            if (lastDirection == "Right") { anim.Play("Player_Walk_Right"); spriteRenderer.flipX = false; }
            else if (lastDirection == "Left") anim.Play("Player_Walk_Left");
            else if (lastDirection == "Up") anim.Play("Player_Walk_Up");
            else if (lastDirection == "Down") anim.Play("Player_Walk_Down");
        }
        else
        {
            if (lastDirection == "Right") { anim.Play("Player_Idle_Right"); spriteRenderer.flipX = false; }
            else if (lastDirection == "Left") anim.Play("Player_Idle_Left");
            else if (lastDirection == "Up") anim.Play("Player_Idle_Up");
            else if (lastDirection == "Down") anim.Play("Player_Idle_Down");
        }
    }
}