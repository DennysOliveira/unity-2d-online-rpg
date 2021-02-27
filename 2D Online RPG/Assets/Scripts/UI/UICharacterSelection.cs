// Simple character selection list. The charcter prefabs are known, so we could
// easily show 3D models, stats, etc. too.
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public partial class UICharacterSelection : MonoBehaviour
{
    public UICharacterCreation uiCharacterCreation;
    public UIConfirmation uiConfirmation;
    public MyNetworkManager manager; // singleton is null until update
    public GameObject panel;
    public Button startButton;
    public Button deleteButton;
    public Button createButton;
    public Button quitButton;

    void Update()
    {

        // show while in lobby and not creating character
        if(manager.state == NetworkState.Lobby && !uiCharacterCreation.IsVisible())
        {
            panel.SetActive(true); // 

            // charactersavailableMessage received already?
            if(manager.charactersAvailableMsg.characters != null)
            {
                CharactersAvailableMsg.CharacterPreview[] characters = manager.charactersAvailableMsg.characters;

                // start button: calls AddPLayer which calls OnServerAddPlayer
                // -> button sends a request to the server
                // -> if we press button again while request hasn't finished
                //    then we will get the error:
                //    'ClientScene::AddPlayer: playerControllerId of 0 already in use.'
                //    which will happen sometimes at low-fps or high-latency
                // -> internally ClientScene.AddPlayer adds to localPlayers
                //    immediately, so let's check that first 

                startButton.gameObject.SetActive(manager.selection != -1);
                startButton.onClick.SetListener(() => {
                    // set client "ready"  -> then we will start receiving world messages
                    // from monsters, players, etc.
                    ClientScene.Ready(NetworkClient.connection);

                    // send CharacterSelect message (need to be ready first!)
                    NetworkClient.connection.Send(new CharacterSelectMsg{ index=manager.selection });

                    // clear character selection previews
                    manager.ClearPreviews();

                    // make sure we can't select twice and call AddPlayer twice
                    panel.SetActive(false);
                });

                // delete button
                deleteButton.gameObject.SetActive(manager.selection != -1);
                deleteButton.onClick.SetListener(() => {
                    uiConfirmation.Show(
                        "Are you sure that you want to delete <b>" + characters[manager.selection].name + "</b>?",
                        () => { NetworkClient.Send(new CharacterDeleteMsg{ index=manager.selection }); }
                    );
                });

                // create button
                createButton.interactable = characters.Length < manager.characterLimit;
                createButton.onClick.SetListener(() => {
                    panel.SetActive(false);
                    uiCharacterCreation.Show();
                });

                // quit button
                quitButton.onClick.SetListener(() => { MyNetworkManager.Quit(); });
            }

        }
        else panel.SetActive(false);
    }


}
