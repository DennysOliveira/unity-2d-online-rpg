using UnityEngine;
using UnityEngine.UI;

public class UIPopup : MonoBehaviour
{
    public static UIPopup singleton;
    public GameObject panel;
    public Text messageText;

    public UIPopup()
    {
        if (singleton == null)
            singleton = this;
    }

    public void Show(string message)
    {
        if(panel.activeSelf) 
            messageText.text += ";\n" + message;
        else 
            messageText.text = message;
        panel.SetActive(true);
    }
}