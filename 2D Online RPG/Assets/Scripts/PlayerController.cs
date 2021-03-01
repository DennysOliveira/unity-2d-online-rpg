using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Experimental;

[RequireComponent(typeof(NetworkRigidbody2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Player))]
public class PlayerController : NetworkBehaviour
{
    // Input Control Variables
    float input_x = 0;
    float input_y = 0;
    bool isWalking = false;
    Vector2 movement;

    float old_input_x = 0;
    float old_input_y = 0;

    // Body Variables
    Rigidbody2D rb2d;

    // Entity Variable
    [HideInInspector] public Player player;
    
    // Local Player Vars
    Camera playerCamera;
    AudioListener audioListener;

    // Sprite-related Variables
    [SyncVar] private bool torchEnabled = false;

    [Header("Local Player Object")]
    public GameObject localPlayerObject;

    [Header("Player Animator Controllers")]
    public RuntimeAnimatorController cntVikingBase;
    public RuntimeAnimatorController cntVikingTorch;

    [Header("Animator")]
    public Animator playerAnimator;

    void Start()
    {   
        // Get and store Component references so we can access them:
        rb2d = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        

        localPlayerObject = GameObject.Find("Local Player Object");
        
        if(isLocalPlayer)
        {
            // Set up this object into a non-networked object
            // this.name = "Networked Player (Local)";
            this.transform.parent = localPlayerObject.transform;

            isWalking = false;

            // Setup camera
            playerCamera = GetComponent<Camera>();
            audioListener = GetComponent<AudioListener>();

            // Setup Camera and Audio Listener
            playerCamera = GetComponent<Camera>();
            audioListener = GetComponent<AudioListener>();

        } 
        else
        {
            // this.name = "Networked Player (Clone)";
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

            // If input has changed since the last update, tell the server:
            if(input_x != old_input_x || input_y != old_input_y)
            {    
                CmdSyncMove(input_x, input_y);
                old_input_x = input_x;
                old_input_y = input_y;
            }

            if (isWalking)
            {
                // If isWalking, move the player:
                CmdMove(input_x, input_y);

                // Set input values and hold them set so the Idle animation can use them as is.
                playerAnimator.SetFloat("input_x", input_x);
                playerAnimator.SetFloat("input_y", input_y);
            }

            // Set isWalking if or if not walking - needs Client Authority.
            playerAnimator.SetBool("isWalking", isWalking);
           
            if(Input.GetKeyDown(KeyCode.T))
            {
                CmdToggleTorch();
            }
            
        }
    }

    [Command]
    void CmdSyncMove(float input_x, float input_y)
    {
        // Calculates Player Movement
        movement = new Vector2(input_x, input_y);
    }

    [Command]
    void CmdMove(float input_x, float input_y)
    {
        // Calls the movement on the server with data calculated at server side.
        rb2d.MovePosition(rb2d.position + movement * player.entity.speed * Time.fixedDeltaTime);
        
    }

    [Command]
    void CmdToggleTorch()
    {
        if(torchEnabled)
        {
            // disable torch
            playerAnimator.runtimeAnimatorController = cntVikingBase;

            torchEnabled = false;

        } else
        {
            // enable torch
            playerAnimator.runtimeAnimatorController = cntVikingTorch;
                        
            torchEnabled = true;
        }
        
    }
    
}