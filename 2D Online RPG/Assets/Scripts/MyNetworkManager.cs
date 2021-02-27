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
[Serializable] public class UnityEventCharactersAvailableMsg : UnityEvent<CharactersAvailableMsg> {}
[Serializable] public class UnityEventCharacterCreateMsgPlayer : UnityEvent<CharacterCreateMsg, Player> {}
[Serializable] public class UnityEventStringGameObjectNetworkConnectionCharacterSelectMsg : UnityEvent<string, GameObject, NetworkConnection, CharacterSelectMsg> {}
[Serializable] public class UnityEventCharacterDeleteMsg : UnityEvent<CharacterDeleteMsg> {}

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

    [Header("Events")]
    public UnityEvent onStartClient;
    public UnityEvent onStopClient;
    public UnityEvent onStartServer;
    public UnityEvent onStopServer;
    public UnityEventNetworkConnection onClientConnect;
    public UnityEventNetworkConnection onServerConnect;
    public UnityEventCharactersAvailableMsg onClientCharactersAvailable;
    public UnityEventCharacterCreateMsgPlayer onServerCharacterCreate;
    public UnityEventStringGameObjectNetworkConnectionCharacterSelectMsg onServerCharacterSelect;
    public UnityEventCharacterDeleteMsg onServerCharacterDelete;
    public UnityEventNetworkConnection onClientDisconnect;
    public UnityEventNetworkConnection onServerDisconnect;

    // store characters available message on client so that UI can access it
    [HideInInspector] public CharactersAvailableMsg charactersAvailableMsg;

    // name check
    public bool IsAllowedCharacterName(string characterName)
    {
        return characterName.Length <= characterNameMaxLength &&
               Regex.IsMatch(characterName, @"^[a-zA-Z0-9_]+$");
    }

    // player classes
    public List<Player> FindPlayerClasses()
    {
        List<Player> classes = new List<Player>();
        foreach(GameObject prefab in spawnPrefabs)
        {
            Player player = prefab.GetComponent<Player>();
            if(player != null)
                classes.Add(player);
        }
        return classes;
    }

    public override void Awake()
    {
        base.Awake();

        // cache the list of player classes
        // this should not be changed at runtime
        playerClasses = FindPlayerClasses();
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

    public void ServerSendError(NetworkConnection conn, string error, bool disconnect)
    {
        conn.Send(new ErrorMsg{text=error, causesDisconnect=disconnect});
    }
    
    void OnClientError(NetworkConnection conn, ErrorMsg message)
    {
        Debug.Log("OnClientError: " + message.text);

        // show a popup
        uiPopup.Show(message.text);

        // Disconnects the client immediately if the message causes disconnect.
        if(message.causesDisconnect)
        {
            conn.Disconnect();

            // Also, stops the host if it is a client-hosted server.
            if (NetworkServer.active) StopHost();
        }
    }

    public override void OnStartClient()
    {
        // setup handlers
        NetworkClient.RegisterHandler<ErrorMsg>(OnClientError, false);
        NetworkClient.RegisterHandler<CharactersAvailableMsg>(OnClientCharactersAvailable);
        
        // addon system hooks
        onStartClient.Invoke();
    }

    public override void OnStartServer()
    {
        // Connect to the database
        Database.singleton.Connect();
                
        // Handshake packet handlers
        NetworkServer.RegisterHandler<CharacterCreateMsg>(OnServerCharacterCreate);
        NetworkServer.RegisterHandler<CharacterSelectMsg>(OnServerCharacterSelect);
        NetworkServer.RegisterHandler<CharacterDeleteMsg>(OnServerCharacterDelete);

        // Invoke Saving
        InvokeRepeating(nameof(SavePlayers), saveInterval, saveInterval);

        // addon system hooks
        onStartServer.Invoke();
        
    }

    public override void OnStopClient()
    {
        // addon system hooks
        onStopClient.Invoke();
    }

    public override void OnStopServer()
    {
        // CancelInvoke(nameof(SavePlayers));

        // addon system hooks
        onStopServer.Invoke();
    }

    // handshake: Login
    public bool IsConnecting() => NetworkClient.active && !ClientScene.ready;

    // Called on the client after a successful authentication
    public override void OnClientConnect(NetworkConnection conn)
    {
        // addon system hooks
        onClientConnect.Invoke(conn);

        // Should not call base function, otherwise, client gets ready.
        // base.OnClientConnect(conn);
    }

    // Called on the server if a client connects after a successful authentication
    public override void OnServerConnect(NetworkConnection conn)
    {
        // grab account related to this connection
        string account = lobby[conn];

        // send necessary data to this client to-do
        conn.Send(MakeCharactersAvailableMessage(account));

        // addon system hooks
        onServerConnect.Invoke(conn);
    }

    public override void OnClientSceneChanged(NetworkConnection conn){}

    CharactersAvailableMsg MakeCharactersAvailableMessage(string account)
    {
        // load from the database - avoid Linq
        List<Player> characters = new List<Player>();

        foreach(var characterName in Database.singleton.CharactersForAccount(account))
        {
            GameObject player = Database.singleton.CharacterLoad(characterName, playerClasses, true);
            characters.Add(player.GetComponent<Player>());
        }

        // construct the message
        CharactersAvailableMsg message = new CharactersAvailableMsg();
        message.Load(characters);

        characters.ForEach(player => Destroy(player.gameObject));
        return message;
    }

    // handshake: character selection
    void LoadPreview(GameObject prefab, Transform location, int selectionIndex, CharactersAvailableMsg.CharacterPreview character)
    {
        // instatiate the prefab
        GameObject preview = Instantiate(prefab.gameObject, location.position, location.rotation);
        preview.transform.parent = location;
        Player player= preview.GetComponent<Player>();

        // assign basic preview values like name and equipment
        player.name = character.name;
        player.isGameMaster = character.isGameMaster;
        
        // assign equipment
        // for( int i = 0; i < character.equipment.Length; ++i )
        // {

        // }

        // add selection script : to-do
        preview.AddComponent<SelectableCharacter>();
        preview.GetComponent<SelectableCharacter>().index = selectionIndex;

    }

    public void ClearPreviews()
    {
        selection = -1;
        foreach(Transform location in selectionLocations)
            if (location.childCount > 0)
                Destroy(location.GetChild(0).gameObject);
    }

    void OnClientCharactersAvailable(NetworkConnection connection, CharactersAvailableMsg message)
    {
        charactersAvailableMsg = message;
        Debug.Log("Characters available:" + charactersAvailableMsg.characters.Length);

        // set state
        state = NetworkState.Lobby;

        // clear previews in any case
        ClearPreviews();

        // load previews for 3D character selection
        for (int i = 0; i < charactersAvailableMsg.characters.Length; ++i)
        {
            CharactersAvailableMsg.CharacterPreview character = charactersAvailableMsg.characters[i];

            // find the prefab for that class
            Player prefab = playerClasses.Find(p => p.name == character.className);
            if (prefab != null)
                LoadPreview(prefab.gameObject, selectionLocations[i], i, character);
            else
                Debug.LogWarning("Character Selection: no prefab found for class " + character.className);
        }

        // setup camera
        Camera.main.transform.position = selectionCameraLocation.position;
        Camera.main.transform.rotation = selectionCameraLocation.rotation;

        //addon sys hooks
        onClientCharactersAvailable.Invoke(charactersAvailableMsg);

    }

    // handshake character creation
    public Transform GetStartPositionFor(string className)   
    {
        return GetStartPosition();
        
        
    }

    Player CreateCharacter(GameObject classPrefab, string characterName, string account, bool gameMaster)
    {
        Player player = Instantiate(classPrefab).GetComponent<Player>();
        player.name = characterName;
        player.account = account;
        player.className = classPrefab.name;
        player.transform.position = new Vector3(0, 0, 0);

        // get inventory
        
        // get equipment

        player.entity.curHealth = player.entity.maxHealth;
        player.entity.curMana = player.entity.maxMana;
        player.isGameMaster = gameMaster;

        return player;
    }

    void OnServerCharacterCreate(NetworkConnection conn, CharacterCreateMsg message)
    {
        if(lobby.ContainsKey(conn))
        {
            if (IsAllowedCharacterName(message.name))
            {
                // check for availability
                string account = lobby[conn];
                if(!Database.singleton.CharacterExists(message.name))
                {
                    // are there slots available to create a new char
                    if(Database.singleton.CharactersForAccount(account).Count < characterLimit)
                    {
                        if (0 <= message.classIndex && message.classIndex < playerClasses.Count)
                        {
                            // GM can only be requested by the host.
                            // DO NOT allow regular conns to create GMs.
                            if(message.gameMaster == false || conn == NetworkServer.localConnection)
                            {
                                // create new char based on prefabs
                                Player player = CreateCharacter(playerClasses[message.classIndex].gameObject, message.name, account, message.gameMaster);

                                // addon system hooks

                                onServerCharacterCreate.Invoke(message, player);

                                // save this player
                                Database.singleton.CharacterSave(player, false);
                                Destroy(player.gameObject);

                                // send available chars to list again
                                // causing the client to switch to charSelect scene again
                                conn.Send(MakeCharactersAvailableMessage(account));
                            }
                            else 
                            {
                                ServerSendError(conn, "insufficient permissions", false);
                            }

                        }
                        else
                        {
                            ServerSendError(conn, "character invalid class", false);
                        }
                    }
                    else
                    {
                        ServerSendError(conn, "character limit reached", false);
                    }
                }
                else
                {
                    ServerSendError(conn, "name already exists", false);
                }
            }
            else
            {
                ServerSendError(conn, "character name not allowed", false);
            }
        }
        else
        {
            ServerSendError(conn, "CharacterCreate: not in lobby?", true);
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn){ Debug.LogWarning("Use the CharacterSelectMsg instead"); }

    void OnServerCharacterSelect(NetworkConnection conn, CharacterSelectMsg message)
    {
        Debug.Log("OnServerCharacterSelect");
        // only while in lobby (after handshake, not in-game)
        if(lobby.ContainsKey(conn))
        {
            // read the index and find the n-th character
            string account = lobby[conn];
            List<string> characters = Database.singleton.CharactersForAccount(account);

            // validate Index
            if(0 <= message.index && message.index < characters.Count)
            {
                Debug.Log(account + " selected player " + characters[message.index]);

                // load character data
                GameObject go = Database.singleton.CharacterLoad(characters[message.index], playerClasses, false);

                // add to client
                NetworkServer.AddPlayerForConnection(conn, go);

                // addon system hooks 
                onServerCharacterSelect.Invoke(account, go, conn, message);

                // remove from lobby
                lobby.Remove(conn);
            }
            else
            {
                Debug.Log("invalid character index: " + account + " " + message.index);
                ServerSendError(conn, "invalid character index", false);
            }

        }
        else
        {
            Debug.Log("CharacterSelect: not in lobby" + conn);
            ServerSendError(conn, "CharacterSelect: not in lobby", true);
        }
    }

    void OnServerCharacterDelete(NetworkConnection conn, CharacterDeleteMsg message)
    {
        Debug.Log("OnServerCharacterDelete " + conn);
    

        // only while in lobby (aka after handshake and not ingame)
        if (lobby.ContainsKey(conn))
        {
            string account = lobby[conn];
            List<string> characters = Database.singleton.CharactersForAccount(account);

            // validate index
            if (0 <= message.index && message.index < characters.Count)
            {
                // delete the character
                Debug.Log("delete character: " + characters[message.index]);
                Database.singleton.CharacterDelete(characters[message.index]);

                // addon system hooks
                onServerCharacterDelete.Invoke(message);

                // send the new character list to client
                conn.Send(MakeCharactersAvailableMessage(account));
            }
            else
            {
                Debug.Log("invalid character index: " + account + " " + message.index);
                ServerSendError(conn, "invalid character index", false);
            }
        }
        else
        {
            Debug.Log("CharacterDelete: not in lobby: " + conn);
            ServerSendError(conn, "CharacterDelete: not in lobby", true);
        }
    }

    // PLAYER SAVING
    // Saving all players at once makes sure that there are no item duplicates
    // in case of a server crash etc.

    void SavePlayers()
    {
        Database.singleton.CharacterSaveMany(Player.onlinePlayers.Values);
        if(Player.onlinePlayers.Count > 0)
            Debug.Log("Saved " + Player.onlinePlayers.Count + " player(s).");
    }

    // stop/disconnect
    // called on the server when a client disconnects
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        float delay = 0;
        if(conn.identity != null)
        {
            Player player = conn.identity.GetComponent<Player>();
            delay = (float)player.remainingLogoutTime;
        }

        StartCoroutine(DoServerDisconnect(conn, delay));
    }

    IEnumerator<WaitForSeconds> DoServerDisconnect(NetworkConnection connection, float delay)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log("DoServerDisconnect " + connection);

        // save player
        // (If there is a player to save. Nothing to save if in lobby.)
        if ( connection.identity != null )
        {
            Database.singleton.CharacterSave(connection.identity.GetComponent<Player>(), false);
            Debug.Log("Character " + connection.identity.name + " on Disconnect.");
        }

        // addon system hooks
        onServerDisconnect.Invoke(connection);

        // remove logged in accout after above was done
        lobby.Remove(connection);

        // do base function logic (removes the player for the connection)
        base.OnServerDisconnect(connection);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnClientDisconnect");

        // take the camera out of the local players so it doesn't get destroyed
        // should arrange this according to my cam-controller script
        Camera mainCamera = Camera.main;
        if(mainCamera.transform.parent != null)
            mainCamera.transform.SetParent(null);

        // show a popup so that users know what happened
        uiPopup.Show("Disconnected.");
        
        // call base function to guarantee proper functionality
        base.OnClientDisconnect(conn);

        // set state
        state = NetworkState.Offline;

        // addon system hooks
        onClientDisconnect.Invoke(conn);

    }

    // universal quit
    public static void Quit()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public override void OnValidate()
    {
        base.OnValidate();

        if(!Application.isPlaying && networkAddress != "")
            networkAddress = "Use the Server List below!";

        // need enough character selection locations for character limit
        if (selectionLocations.Length != characterLimit)
        {
            // create new array with proper size
            Transform[] newArray = new Transform[characterLimit];

            // copy old values
            for (int i = 0; i < Mathf.Min(characterLimit, selectionLocations.Length); ++i)
                newArray[i] = selectionLocations[i];

            // use new array
            selectionLocations = newArray;
        }
    }
}
