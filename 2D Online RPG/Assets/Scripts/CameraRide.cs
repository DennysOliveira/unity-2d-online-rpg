using UnityEngine;
using Mirror;

public class CameraRide : MonoBehaviour
{
    [Header("Configuration")]
    public float speed = 0.1f;
    
    public float baseZ = -10;
    public string currentDirection;

    
    Vector3 curPos;
    Vector3 movement;

    void Start()
    {
        currentDirection = "left";
    }

    void Update()
    {
        // only while not in character selection or world
        //Debug.Log(((MyNetworkManager)NetworkManager.singleton).state);
        if (((MyNetworkManager)NetworkManager.singleton).state != NetworkState.Offline)
            Destroy(this);


        // Get immutable current position
        curPos = new Vector3(transform.position.x, transform.position.y, baseZ);

        if (curPos.x >= 10f) currentDirection = "left";
        if (curPos.x <= -10f) currentDirection = "right";

        if (currentDirection == "left")
        {
            movement = new Vector3(-1, 0, 0);
            transform.position += movement * speed * Time.deltaTime;
            
        }
        if (currentDirection == "right") {
            movement = new Vector3(1, 0, 0);
            transform.position += movement * speed * Time.deltaTime;
        }
    }
}
