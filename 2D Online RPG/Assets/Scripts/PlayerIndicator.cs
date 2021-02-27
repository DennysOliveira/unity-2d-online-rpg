using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    [Header("Indicator")]
    public GameObject indicatorPrefab;
    [HideInInspector] public GameObject indicator;

    public void SetViaParent(Transform parent)
    {
        if(!indicator) indicator = Instantiate(indicatorPrefab);
        indicator.transform.SetParent(parent, true);
        indicator.transform.position = parent.position;
    }

    public void SetViaPosition(Vector3 position)
    {
        if(!indicator) indicator = Instantiate(indicatorPrefab);
        indicator.transform.parent = null;
        indicator.transform.position = position;
    }

    // clear indicator if there is one, and if it's not on a target
    public void ClearIfNoParent()
    {
        if (indicator != null && indicator.transform.parent == null)
            Destroy(indicator);
    }

    // clear in any case
    public void Clear()
    {
        if (indicator != null)
            Destroy(indicator);
    }

    void OnDestroy()
    {
        if(indicator != null)
            Destroy(indicator);
    }

}
