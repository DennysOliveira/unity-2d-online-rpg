using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;


[Serializable] public class UnityEventPlayer : UnityEvent<Player> {}

//[RequireComponent(typeof(Experience))] // TO-DO
[RequireComponent(typeof(Strength))]
[RequireComponent(typeof(Agility))]
[RequireComponent(typeof(Intelligence))]
[RequireComponent(typeof(Spirit))]
[RequireComponent(typeof(PlayerIndicator))]
[RequireComponent(typeof(PlayerInventory))]     // TO-DO
// [RequireComponent(typeof(PlayerLooting))]       // TO-DO
// [RequireComponent(typeof(PlayerNpcRevive))]     // TO-DO
// [RequireComponent(typeof(PlayerNpcTeleport))]   // TO-DO
// [RequireComponent(typeof(PlayerNpcTrading))]    // TO-DO
// [RequireComponent(typeof(PlayerParty))]         // TO-DO
// [RequireComponent(typeof(PlayerPetControl))]    // TO-DO
// [RequireComponent(typeof(PlayerQuests))]        // TO-DO
// [RequireComponent(typeof(PlayerSkillbar))]      // TO-DO
// [RequireComponent(typeof(PlayerSkills))]        // TO-DO
// [RequireComponent(typeof(PlayerTrading))]       // TO-DO
// [RequireComponent(typeof(NetworkName))]         // TO-DO

public class Player : Entity
{
    [Header("Components")]
    public Strength strength;
    public Intelligence intelligence;
    public Spirit spirit;
    public Agility agility;
    public PlayerIndicator indicator;


    // localPlayer singleton for easier access from UI scripts, etc.
    public static Player localPlayer;

    [Header("Text Meshes")]
    public TextMeshPro nameOverlay;
    public Color nameOverlayDefaulColor = Color.white;
    public Color nameOverlayOffenderColor = Color.yellow;
    public Color nameOverlayMurdererColor = Color.red;
    public Color nameOverlayPartyColor = new Color(0.34f, 1f, 0.7f);
    public string nameOverlayGameMasterPrefix = "[GM] ";


    [SyncVar] public bool isGameMaster;

    [Header("Components")]
    public string account = "";
    public string className = "";
    public PlayerInventory inventory;
     
    // cached players to save computation
    // => on server: all online players
    // => on client: all observed players
    public static Dictionary<string, Player> onlinePlayers = new Dictionary<string, Player>();

    private GameObject playerHUD;

    public double allowedLogoutTime;
    public double remainingLogoutTime = 0f;

    void Start()
    {
        if(!isServer && !isClient) return;

        
        onlinePlayers[name] = this;

    }

    public override void OnStartLocalPlayer()
    {
        // set singleton
        localPlayer = this;
    }

    
    private void Update()
    {
        // healthSlider.value  = entity.curHealth;
        // manaSlider.value    = entity.curMana;
        // staminaSlider.value = entity.curStamina;

        // if(Input.GetKeyDown(KeyCode.Space))
        // {
        //    CmdTakeDamage(10);
        // }
    }

    void LateUpdate()
    {
        // animate here

        // update overlays, in world and preview
        if(!isServerOnly)
        {
            if(nameOverlay != null)
            {
                string prefix = isGameMaster ? nameOverlayGameMasterPrefix : "";
                nameOverlay.text = prefix + name;

                if(localPlayer != null)
                {
                    nameOverlay.color = nameOverlayDefaulColor;
                }
            }
        }
    }

    void OnDestroy()
    {
        if(onlinePlayers.TryGetValue(name, out Player entry) && entry == this)
            onlinePlayers.Remove(name);

        if(!isServer && !isClient) return;

        if(isLocalPlayer)
            localPlayer = null;
    }
}
