using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 30f;

    [Header("Input")]
    [Tooltip("Drag your InputReader ScriptableObject asset here.")]
    public InputReader inputReader;

    [Header("Camera Reference (Optional Override)")]
    public Transform cameraTransform;

    private Rigidbody      rb;
    private Vector3        moveInput;
    private Animator       anim;
    private SpriteRenderer spriteRenderer;

    private string  lastDirection = "Up";
    private Vector3 lookDirection = Vector3.forward;
    private Vector2 rawInput;

    void Start()
    {
        rb             = GetComponent<Rigidbody>();
        anim           = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (inputReader != null)
            inputReader.OnMoveEvent += HandleMove;
    }

    void OnDestroy()
    {
        if (inputReader != null)
            inputReader.OnMoveEvent -= HandleMove;
    }

    private void HandleMove(Vector2 input)
    {
        rawInput = input;
    }

    void Update()
    {
        if (!PlayerStateManager.IsFree)
        {
            moveInput = Vector3.zero;
            rawInput  = Vector2.zero;
            HandleAnimations();
            return;
        }

        float cameraYRotation = cameraTransform != null ? cameraTransform.eulerAngles.y : 0f;
        Quaternion cameraRotation = Quaternion.Euler(0f, cameraYRotation, 0f);

        Vector3 raw3D = new Vector3(rawInput.x, 0f, rawInput.y).normalized;
        moveInput = cameraRotation * raw3D;

        if (moveInput.magnitude > 0)
            lookDirection = moveInput.normalized;

        UpdateFacingDirection(rawInput.x, rawInput.y);
        HandleAnimations();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, moveInput.z * moveSpeed);
    }

    public Vector3 GetLookDirection() => lookDirection;

    void UpdateFacingDirection(float x, float z)
    {
        if (moveInput.magnitude > 0)
        {
            if (Mathf.Abs(x) >= Mathf.Abs(z))
            {
                if (x > 0)      lastDirection = "Right";
                else if (x < 0) lastDirection = "Left";
            }
            else
            {
                if (z > 0)      lastDirection = "Up";
                else if (z < 0) lastDirection = "Down";
            }
        }
    }

    void HandleAnimations()
    {
        if (moveInput.magnitude > 0)
        {
            if (lastDirection == "Right") { anim.Play("Player_Walk_Right"); spriteRenderer.flipX = false; }
            else if (lastDirection == "Left")  anim.Play("Player_Walk_Left");
            else if (lastDirection == "Up")    anim.Play("Player_Walk_Up");
            else if (lastDirection == "Down")  anim.Play("Player_Walk_Down");
        }
        else
        {
            if (lastDirection == "Right") { anim.Play("Player_Idle_Right"); spriteRenderer.flipX = false; }
            else if (lastDirection == "Left")  anim.Play("Player_Idle_Left");
            else if (lastDirection == "Up")    anim.Play("Player_Idle_Up");
            else if (lastDirection == "Down")  anim.Play("Player_Idle_Down");
        }
    }
}
