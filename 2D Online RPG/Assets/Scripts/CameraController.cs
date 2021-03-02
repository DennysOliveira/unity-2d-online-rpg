using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject player = null;
    public MyNetworkManager manager;
    

    [Header("Movement")]
    public float smoothSpeed = 0.125f;
    public float baseZPosition = -10f;

    [Header("Position Offsets")]
    public float playerOffsetX = 0f;
    public float playerOffsetY = 0f;
    public float playerOffsetZ = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
        if (Player.localPlayer != null && manager.state == NetworkState.World)
        {
            player = Player.localPlayer.gameObject;
        }
    }

    void LateUpdate()
    {   
        if(manager.state == NetworkState.World && player != null)
        {
            // do world thing
            // Vector3 nextPos = new Vector3(player.transform.position.x + playerOffsetX,
            //                               player.transform.position.y + playerOffsetY,
            //                               baseZPosition + playerOffsetZ);

            // transform.position = Vector3.Lerp(transform.position, nextPos, smoothSpeed * Time.deltaTime);                                          
            Vector3 nextPos = new Vector3(player.transform.position.x + playerOffsetX,
                                          player.transform.position.y + playerOffsetY,
                                          baseZPosition + playerOffsetZ);
            
            float pow = 1.0f - Mathf.Pow(0.5f, Time.deltaTime * smoothSpeed);
            transform.position = Vector3.Lerp(transform.position, nextPos, pow);
            
        }
    }
}
