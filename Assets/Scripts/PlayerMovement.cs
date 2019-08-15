using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 1. 4-Directional isometric movement
/// 2. Maintain orientation absent of input
/// Moves a Rigidbody isometrically based on rotation
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {
    public float baseSpeed = 80;
    public float walkSpeedModifier = 0.1f;
    public float runSpeedModifier = 0.12f;

    public float jumpForce = 10f;
    public bool isGrounded = true;

    public LayerMask hitLayer;

    public int delayMS = 10;
    private float currentDelay = 0;

    Transform cam;
    [SerializeField]
    Transform actionPoint;
    Rigidbody rb;

    WorldMap world;

    private float lastMoveX;
    private float lastVelY;

    void Start() {
        cam = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        world = FindObjectOfType<WorldMap>();
    }

    void Update() {
        CalculateGrounded();
        HandleInput();
    }


    /// <summary>
    /// Handles execution of all input handlers
    /// </summary>
    void HandleInput() {
        HandleAction();
        HandleHoriztonalMovement();
        HandleJump();
    }

    /// <summary>
    ///  Handles action inputs:
    ///  Pressing C will make player try to break blocks
    /// </summary>
    void HandleAction() {
        RaycastHit hit;
        Vector3 direction = Input.GetKey(KeyCode.LeftShift) ? Vector3.down : transform.forward;
        bool hitSuccess = Physics.Raycast(actionPoint.position, direction, out hit, 0.5f, hitLayer);

        currentDelay += Time.deltaTime * 100;
        // if (Input.GetKey(KeyCode.C) && hitSuccess && currentDelay >= delayMS) {
        //     currentDelay = 0;
        //     TerrainModifier.ReplaceBlockAt(hit, Block.BlockType.Air);
        // }
        // if (Input.GetMouseButton(0)) {
        //     world.terrainModifier.ReplaceBlockCursor(Block.BlockType.Air);
        // }

        // if (Input.GetMouseButton(1)) {
        //     world.terrainModifier.AddBlockCursor(Block.BlockType.Grass_Solid);
        // }
    }


    /// <summary>
    /// Handles jump input logic
    /// </summary>
    void HandleJump() {
        // Get jumping input
        Vector3 jumpInput = CaptureVerticalInput();

        // Apply jump force if not jumping
        if (jumpInput.sqrMagnitude != 0 && isGrounded) {
            ApplyImpulse(jumpInput * jumpForce);
        }
    }



    /// <summary>
    /// Handles movement in the XZ plane
    /// </summary>
    void HandleHoriztonalMovement() {
        Vector2 horizontalInput = CaptureHorizontalInput();
        // If the horizontal input is 0, no Move/Rotate update needed
        if (horizontalInput.sqrMagnitude != 0) {
            float angle = CalculateDirection(horizontalInput);
            Rotate(angle);
            Move();
        }
    }


    /// <summary>
    /// Determines if player is on the ground or not;
    /// whether or not it is moving in Y axis. 
    /// </summary>
    void CalculateGrounded() {

        if (rb.velocity.y != 0) {
            isGrounded = false;
        } else {
            isGrounded = true;
        }
    }

    /// <summary>
    /// Returns horizontal input based on keys pressed:
    /// Horizontal: A and D, or arrow-left and arrow-right
    /// Vertical: W and S, or arrow-up and arrow-down
    /// </summary>
    Vector2 CaptureHorizontalInput() {
        return new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
    }

    Vector3 CaptureVerticalInput() {
        return new Vector3(
            0,
            Input.GetAxisRaw("Jump"),
            0
        );
    }

    /// <summary>
    /// Returns angle between x-axis and input vector
    /// plus camera's current rotation
    /// </summary>
    float CalculateDirection(Vector2 input) {
        // Get angle between x-axis and input vector
        float angle = Mathf.Atan2(input.x, input.y);
        // Convert to degrees
        angle = Mathf.Rad2Deg * angle;
        // Add current y angle of camera to that
        angle += cam.eulerAngles.y;

        return angle;
    }

    /// <summary>
    /// Rotates player toward angle
    /// </summary>
    void Rotate(float angle) {
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
        rb.MoveRotation(targetRotation);
    }

    /// <summary>
    /// Moves player along forward axis based on current rotation
    /// </summary>
    void Move() {
        float speed = baseSpeed * walkSpeedModifier;
        rb.MovePosition(transform.position + (transform.forward * speed * Time.deltaTime));
    }

    void ApplyImpulse(Vector3 impulse) {
        rb.AddForce(impulse, ForceMode.Impulse);
    }

}
