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
            MoveToPoint(new Vector3(-10, 0, baseZ));
        }
        if (currentDirection == "right") {
            MoveToPoint(new Vector3(10, 0, baseZ));
        }
        
        

        //transform.position = Vector3.Lerp(curPos, movement * speed, lerpSmoothness * Time.deltaTime);
    }

   void MoveToPoint(Vector3 target)
   {
       movement = new Vector3(target.x + 1, target.y, target.z);
       transform.position += movement * speed * Time.deltaTime;
   }
}
