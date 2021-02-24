using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalIlluminationController : MonoBehaviour
{
    Transform   playerTransform = null;
    GameObject  playerObj;

    [Header("GI Config")]
    public float playerOffsetX = 0;
    public float playerOffsetY = -10;
    public float zOffset;


    void Update()
    {
        if (playerTransform == null)
        {
            playerTransform = transform.parent.Find("Networked Player (Local)");

            if(playerTransform != null)
            {
                playerObj = playerTransform.gameObject;
            }
        }

    }

    void LateUpdate()
    {
        if (playerObj)
        {
            // Create a new position at player's location
            Vector3 nextPos = new Vector3(playerObj.transform.position.x + playerOffsetX,
                                          playerObj.transform.position.y + playerOffsetY, 1 + zOffset);
            
            // Transform self to the new Position
            transform.position = nextPos;
        }
    }

    
    
}
