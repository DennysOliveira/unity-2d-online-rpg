using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

public class UIConfirmation : MonoBehaviour
{
    public static UIConfirmation singleton;
    public GameObject panel;
    public Text messageText;
    public Button confirmButton;

    public UIConfirmation()
    {
        // assign singleton only once (to work with DontDestroyOnLoad when using Zones/Switching Scenes)
        if (singleton == null) singleton = this;
    }

    public void Show(string message, UnityAction onConfirm)
    {
        messageText.text = message;
        confirmButton.onClick.SetListener(onConfirm);
        panel.SetActive(true);
    }


}
