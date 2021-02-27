using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    public UIPopup uiPopup;
    public MyNetworkManager manager; // singleton = null in Start/Awake
    public NetworkAuthenticatorMMO auth;
    public GameObject panel;

    public Text statusText;
    public InputField accountInput;
    public InputField passwordInput;
    public Dropdown serverDropdown;
    public Button loginButton;
    public Button registerButton;
    [TextArea(1, 30)] public string registerMessage = "First time? Just log in and we will\ncreate an account automatically.";
    public Button hostButton;
    public Button dedicatedButton;
    public Button cancelButton;
    public Button quitButton;



    // Start is called before the first frame update
    void Start()
    {
        // load last server by name in case order changes some day.
        if (PlayerPrefs.HasKey("LastServer"))
        {
            string last = PlayerPrefs.GetString("LastServer", "");
            serverDropdown.value = manager.serverList.FindIndex(s => s.name == last);
        }
    }

    void OnDestroy()
    {
        PlayerPrefs.SetString("LastServer", serverDropdown.captionText.text);
    }

    // Update is called once per frame
    void Update()
    {
        // only show while offline
        // and while in handshake since we don't want to show nothing while
        // trying to login and waiting for the server's response
        if(manager.state == NetworkState.Offline || manager.state == NetworkState.Handshake)
        {
            panel.SetActive(true);

            // status
            if(manager.IsConnecting())
                statusText.text = "Connecting...";
            else if (manager.state == NetworkState.Handshake)
                statusText.text = "Handshake...";
            else
                statusText.text = "";

            // buttons.interactable while network is not active
            registerButton.interactable = !manager.isNetworkActive;
            
            registerButton.onClick.SetListener(() => uiPopup.Show(registerMessage));

            loginButton.interactable = !manager.isNetworkActive; // && auth.IsAwllowedAccountName(accountInput.text);
            loginButton.onClick.SetListener(() => manager.StartClient());

            hostButton.interactable = Application.platform != RuntimePlatform.WebGLPlayer && !manager.isNetworkActive; // && auth.IsAllowedAccountName(accountInput.text)
            hostButton.onClick.SetListener(() => manager.StartHost());

            cancelButton.gameObject.SetActive(manager.IsConnecting());
            cancelButton.onClick.SetListener(() => manager.StopClient());

            //dedicatedButton.interactable = Application.platform != RuntimePlatform.WebGLPlayer && !manager.isNetworkActive;
            //dedicatedButton.onClick.SetListener(() => manager.StartServer());

            quitButton.onClick.SetListener(() => MyNetworkManager.Quit());

            // inputs
            auth.loginAccount = accountInput.text;
            auth.loginPassword = passwordInput.text;

            // copy servers to dropdown; copy selected one to networkmanager ip/port
            serverDropdown.interactable = !manager.isNetworkActive;
            serverDropdown.options = manager.serverList.Select(
                sv => new Dropdown.OptionData(sv.name)
            ).ToList();
            manager.networkAddress = manager.serverList[serverDropdown.value].ip;
        }
        else panel.SetActive(false);
    }
}
