using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform parent;
    GameObject sibling;
    Transform sibTransform = null;

    [Header("Camera Controller Config")]
    public float smoothSpeed = 0.125f;
    


    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;

        
    }

    void LateUpdate()
    {
        if(sibTransform == null)
        {
            sibTransform = parent.Find("Networked Player (Local)");
            Debug.Log("Mylog: " + sibling);
            
            if(sibTransform != null)
            {
                sibling = sibTransform.gameObject;
                gameObject.GetComponent<Camera>().enabled = true;
                gameObject.GetComponent<AudioListener>().enabled = true;
                transform.position = new Vector3(transform.position.x, transform.position.y, -1);
            }
        }
        else if (sibling)
        {
            Vector3 nextPos = new Vector3(sibling.transform.position.x,
                                          sibling.transform.position.y, -15);
            Vector3 smPos = Vector3.Lerp(transform.position, nextPos, smoothSpeed * Time.deltaTime);                                          
            transform.position = smPos;
        }
    }
}
