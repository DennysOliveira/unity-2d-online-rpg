using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRendererSorter : MonoBehaviour
{
    [SerializeField]
    private int sortingOrderBase = 0;
    [SerializeField]    
    private bool runOnlyOnce = false;
    
    private Renderer myRenderer;

    private void Awake()
    {
        myRenderer = gameObject.GetComponent<Renderer>();
    }

    private void LateUpdate()
    {
        myRenderer.sortingOrder = (int)(sortingOrderBase - transform.position.y);
        if(runOnlyOnce)
        {
            Destroy(this);
        }
    }
}
