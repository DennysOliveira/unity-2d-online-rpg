using UnityEngine.UI;
using UnityEngine;
using Mirror;
using System.Linq;

public partial class UICharacterCreation : MonoBehaviour
{
    public MyNetworkManager manager; // singleton is null until update
    public GameObject panel;
    public InputField nameInput;
    public Button createButton;
    public Button cancelButton;

    void Update()
    {
        if(panel.activeSelf)
        {
            if(manager.state == NetworkState.Lobby)
            {
                Show();

                createButton.interactable = manager.IsAllowedCharacterName(nameInput.text);
                createButton.onClick.SetListener(() => {
                    CharacterCreateMsg message = new CharacterCreateMsg {
                        name = nameInput.text,
                        classIndex = 0,
                        gameMaster = false
                    };
                    NetworkClient.Send(message);
                    Hide();
                });

                // cancel
                cancelButton.onClick.SetListener(() => {
                    nameInput.text = "";
                    Hide();
                });
            }
            else Hide();
        }
    }

    public void Hide() { panel.SetActive(false); }
    public void Show() { panel.SetActive(true); }
    public bool IsVisible() { return panel.activeSelf; }
}
