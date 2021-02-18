using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Experimental;

[RequireComponent(typeof(NetworkRigidbody2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : NetworkBehaviour
{
    float input_x = 0;
    float input_y = 0;
    bool isWalking = false;
    float speed = 5.0f;

    Rigidbody2D rb2d;

    void Start()
    {
        if(isLocalPlayer)
        {
            isWalking = false;

            // Get and store a reference to the Rigidbody2D component so we can access it.
            rb2d = GetComponent<Rigidbody2D>();
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
        
    }

    void HandleMovement()
    {
        if(isLocalPlayer)
        {
            input_x = Input.GetAxisRaw("Horizontal");
            input_y = Input.GetAxisRaw("Vertical");
            isWalking = (input_x != 0 || input_y != 0);

            Vector2 movement = new Vector2(input_x * 0.4f, input_y * 0.4f);

            //transform.position = transform.position + movement;
            rb2d.MovePosition(rb2d.position + movement * speed * Time.fixedDeltaTime);
            
        }
    }
}