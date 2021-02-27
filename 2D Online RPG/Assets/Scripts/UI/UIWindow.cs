using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum CloseOption
{
    DoNothing,
    DeactivateWindow,
    DestroyWindow
}

public class UIWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // close option
    public CloseOption onClose = CloseOption.DeactivateWindow;

    // currently dragged UIWindow in case it's needed somewhere else
    public static UIWindow currentlyDragged;

    // cache
    Transform window;

    void Awake()
    {
        // cache the parent window
        window = transform.parent;
    }

    public void HandleDrag(PointerEventData d)
    {
        // send message in case the parent needs to know about it
        window.SendMessage("OnWindowDrag", d, SendMessageOptions.DontRequireReceiver);

        // move the parent
        window.Translate(d.delta);
    }

    public void OnBeginDrag(PointerEventData d)
    {
        currentlyDragged = this;
        HandleDrag(d);
    }

    public void OnDrag(PointerEventData d)
    {
        HandleDrag(d);
    }

    public void OnEndDrag(PointerEventData d)
    {
        HandleDrag(d);
        currentlyDragged = null;
    }

    // OnClose is called by the close button via Inspector Callbacks
    public void OnClose()
    {
        // send message in case it's needed
        // note: it's important to not name it the same as THIS function to avoid
        //       a deadlock
        window.SendMessage("OnWindowClose", SendMessageOptions.DontRequireReceiver);

        // hide window
        if (onClose == CloseOption.DeactivateWindow)
            window.gameObject.SetActive(false);

        // destroy if needed
        if (onClose == CloseOption.DestroyWindow)
            Destroy(window.gameObject);
    }
}


