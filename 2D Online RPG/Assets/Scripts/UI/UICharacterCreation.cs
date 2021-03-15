using UnityEngine.UI;
using UnityEngine;
using Mirror;
using System.Linq;

public partial class UICharacterCreation : MonoBehaviour
{
    public MyNetworkManager manager; // singleton is null until update
    public GameObject panel;
    public InputField nameInput;
    public Dropdown raceDropdown;
    public Button createButton;
    public Button cancelButton;

    void Update()
    {
        if(panel.activeSelf)
        {
            if(manager.state == NetworkState.Lobby)
            {
                Show();

                // Copy available player races to Race Dropdown
                raceDropdown.options = manager.playerRaces.Select(
                    p => new Dropdown.OptionData(p.name)
                ).ToList();

                createButton.interactable = manager.IsAllowedCharacterName(nameInput.text);
                createButton.onClick.SetListener(() => {
                    CharacterCreateMsg message = new CharacterCreateMsg {
                        name = nameInput.text,
                        server = manager.currentServer,
                        classIndex = raceDropdown.value,
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
