using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic character movement controller that takes user input
/// to move an object.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Character controller attached to the player.
    /// </summary>
    private CharacterController characterController;

    /// <summary>
    /// Speed of movement.
    /// </summary>
    public float speed = 6.0f;
    /// <summary>
    /// Jump speed of the character.
    /// </summary>
    public float jumpSpeed = 8.0f;
    /// <summary>
    /// Strength of gravity for the character (m/s^2)
    /// </summary>
    public float gravity = 20.0f;

    /// <summary>
    /// Current direction that the character is facing
    /// </summary>
    private Transform playerDirection;

    /// <summary>
    /// Vector of character movement
    /// </summary>
    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        // Setup the character controller
        characterController = GetComponent<CharacterController>();
        // Get the object for the character's current orientation
        playerDirection = gameObject.transform;
    }

    void Update()
    {
        if (characterController.isGrounded)
        {
            // Parse player movement from the input axes
            float strafeMovement = Input.GetAxis("Horizontal");
            float forwardMovement = Input.GetAxis("Vertical");
            
            // Rotate movement by the current player orientation
            moveDirection = this.transform.rotation * new Vector3(strafeMovement, 0.0f, forwardMovement);

            // Modify movement by the player speed
            moveDirection *= speed;

            // Check if the player is jumping
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
