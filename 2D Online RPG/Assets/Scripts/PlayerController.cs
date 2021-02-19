using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Experimental;

[RequireComponent(typeof(NetworkRigidbody2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : NetworkBehaviour
{
    float input_x = 0;
    float input_y = 0;
    public float speed = 3.0f;
    bool isWalking = false;
    
    Rigidbody2D rb2d;

    [Header("Animator")]
    public Animator playerAnimator;

    Camera playerCamera;
    AudioListener audioListener;

    void Start()
    {

        if(isLocalPlayer)
        {
            isWalking = false;

            // Get and store a reference to the Rigidbody2D component so we can access it.
            rb2d = GetComponent<Rigidbody2D>();

            // Setup camera
            playerCamera = GetComponent<Camera>();
            audioListener = GetComponent<AudioListener>();

            // Setup Camera and Audio Listener
            playerCamera = GetComponent<Camera>();
            audioListener = GetComponent<AudioListener>();

            
            
        }
    }

    void FixedUpdate()
    {
        if(isLocalPlayer)
        {
            // Check User Input and Validate isWalking
            input_x = Input.GetAxisRaw("Horizontal");
            input_y = Input.GetAxisRaw("Vertical");
            isWalking = (input_x != 0 || input_y != 0);

            if (isWalking)
                HandleMovement();

            // Update isWalking State
            playerAnimator.SetBool("isWalking", isWalking);
        }
        
    }

    void HandleMovement()
    {
        // Set movement
        Vector2 movement = new Vector2(input_x * 0.4f, input_y * 0.4f);

        // Move 
        rb2d.MovePosition(rb2d.position + movement * speed * Time.fixedDeltaTime);

        // Animate Movement
        playerAnimator.SetFloat("input_x", input_x);
        playerAnimator.SetFloat("input_y", input_y);
    }

    
}