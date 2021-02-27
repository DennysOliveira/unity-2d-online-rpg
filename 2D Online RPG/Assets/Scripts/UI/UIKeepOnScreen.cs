// This component can be attached to movable windows
// so that tehy are only movable within the Screen boundaries.
using UnityEngine;

public class UIKeepOnScreen : MonoBehaviour
{
    
    [Header("Components")]
    public RectTransform rectTransform;

    void Update()
    {
        // get current rectangle
        Rect rect = rectTransform.rect;

        // to world space
        Vector2 minworld = transform.TransformPoint(rect.min);
        Vector2 maxworld = transform.TransformPoint(rect.max);
        Vector2 sizeworld = maxworld - minworld;

         // keep the min position in screen bounds - size
        maxworld = new Vector2(Screen.width, Screen.height) - sizeworld;

        // keep position between (0,0) and maxworld
        float x = Mathf.Clamp(minworld.x, 0, maxworld.x);
        float y = Mathf.Clamp(minworld.y, 0, maxworld.y);

        // set new position to xy(=local) + offset(=world)
        Vector2 offset = (Vector2)transform.position - minworld;
        transform.position = new Vector2(x, y) + offset;
    
    }
    
}
