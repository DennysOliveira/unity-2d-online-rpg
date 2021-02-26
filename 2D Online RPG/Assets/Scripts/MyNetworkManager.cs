//
// Custom NetworkManager
//
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Network States
public enum NetworkState {Offline, Handshake, Lobby, World}

// Unity Events

[RequireComponent(typeof(Database))]
[DisallowMultipleComponent]
public class MyNetworkManager : NetworkManager
{
    // current NwManager state on client
    public NetworkState state = NetworkState.Offline;

    // Connection -> Account
    // (for lobby -> Creating/Selecting characters)
    public Dictionary<NetworkConnection, string> lobby = new Dictionary<NetworkConnection, string>();

    

    // UI
    [Header("UI")]
    public UIPopup uiPopup;

    AudioSource audioSource;

    [Serializable]
    public class ServerInfo
    {
        public string name;
        public string ip;
    }
    public List<ServerInfo> serverList = new List<ServerInfo>()
    {
        new ServerInfo{name="Local", ip="localhost"}
    };

    [Header("Logout")]
    [Tooltip("Delay after combat for players to be able to log out.")]
    public float combatLogoutDelay = 5;

    [Header("Character Selection")]
    public int selection = -1;
    public Transform[] selectionLocations;
    public Transform selectionCameraLocation;
    [HideInInspector] public List<Player> playerClasses = new List<Player>();

    [Header("Database")]
    public int characterLimit = 4;
    public int characterNameMaxLength = 16;
    public float saveInterval = 60f; // in seconds

    public bool IsAllowedCharacterName(string characterName)
    {
        return characterName.Length <= characterNameMaxLength &&
               Regex.IsMatch(characterName, @"^[a-zA-Z0-9_]+$");
    }


    public override void Start()
    {
        base.Start();
        audioSource = GetComponentInChildren<AudioSource>();
        audioSource.Play();

    }

    void Update()
    {
        if(ClientScene.localPlayer != null)
            state = NetworkState.World;
    }

    // public void ServerSendError(uint id, NetworkConnection conn, string error, bool disconnect)
    // {
    //     ErrorMsg errm = new ErrorMsg({ text=error, causesDisconnect=disconnect})
    //     conn.Send<ErrorMsg>(errm);
    // }
    
    void OnClientError(NetworkConnection conn, ErrorMsg message)
    {
        print("OnClientError: " + message.text);

        // show a popup
        uiPopup.Show(message.text);

        if(message.causesDisconnect)
        {
            // Disconnects the client immediately if the message causes disconnect.
            conn.Disconnect();

            // Also, stops the host if it is a client-hosted server.
            if (NetworkServer.active) StopHost();
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        //NetworkClient.RegisterHandler<ErrorMsg>(OnClientError, false); // allowed without authentication.
    }

    public override void OnStartServer()
    {
        // Connect to the database
        Database.singleton.Connect();
        //Database.singleton.TestSaveChar("somchar", "someacc");
        
        // Handshake packet handlers
        NetworkServer.RegisterHandler<CharacterCreateMsg>(OnServerCharacterCreate);
        NetworkServer.RegisterHandler<CharacterSelectMsg>(OnServerCharacterSelect);
        NetworkServer.RegisterHandler<CharacterDeleteMsg>(OnServerCharacterDelete);

        // Invoke players saving
        // InvokeRepeating(nameof(SavePlayers), saveInterval, saveInterval);

        // Addon System Hooks
        // InvokeMany( , , "OnStartServer_");

        base.OnStartServer();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        print("OnStopServer");
        
        // CancelInvoke(nameof(SavePlayers));

        // Addon System Hooks
        // InvokeMany( , , "OnStopServer_");
    }

    // handshake: Login
    public bool IsConnecting() => NetworkClient.active && !ClientScene.ready;

    // Called on the client after a successful authentication
    public override void OnClientConnect(NetworkConnection conn)
    {
        audioSource.Stop();

        Utils.InvokeMany(typeof(MyNetworkManager), this, "OnClientConnect_", conn);

        // Should not call base function after above methods are created /\
        base.OnClientConnect(conn);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        // grab account related to this connection
        string account = lobby[conn];

        // send necessary data to this conn to-do
        //conn.Send( -- );

        // Addon System hooks
        Utils.InvokeMany(typeof(MyNetworkManager), this, "OnServerConnect_", conn);
        
        // Should not call base function after above methods are created /\
        base.OnServerConnect(conn);
    }

    public override void OnClientSceneChanged(NetworkConnection conn){}

    // CharactersAvailableMsg MakeCharactersAvailableMessage(string account)
    // {
    //     // load from database
    //     // (avoid Linq for performance/gc. Characters are loaded frequently)
    //     List<Player> character = new List<Player>();
    //     foreach(string characterName in D)

    // }
}
