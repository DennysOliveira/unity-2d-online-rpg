using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

[Serializable] public class UnityEventPlayer : UnityEvent<Player> {}

// ||-----------------------||
// ||- Require Components --||
[RequireComponent(typeof(Strength))]
[RequireComponent(typeof(Agility))]
[RequireComponent(typeof(Intelligence))]
[RequireComponent(typeof(Spirit))]
[RequireComponent(typeof(PlayerIndicator))]
[RequireComponent(typeof(Experience))]
// [RequireComponent(typeof(PlayerInventory))]     // TO-DO
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
    public PlayerInventory inventory;

    // ||--------------------||
    // ||- Meta Information -||
    [HideInInspector] public string account = "";
    [HideInInspector] public string className = "";
    [SyncVar] public bool isGameMaster;

    // ||---------------||
    // ||- Text Meshes -||
    [Header("Text Meshes")]
    public TextMeshPro nameOverlay;
    public Color nameOverlayDefaulColor = Color.white;
    public Color nameOverlayOffenderColor = Color.yellow;
    public Color nameOverlayMurdererColor = Color.red;
    public Color nameOverlayPartyColor = new Color(0.34f, 1f, 0.7f);
    public string nameOverlayGameMasterPrefix = "[GM] ";

    // ||-------------------------||
    // ||- LocalPlayer Singleton -||
    public static Player localPlayer;
    

    // ||---------||
    // ||- Speed -||
    public override float speed 
    {
        get 
        {
            // Should return the speed + mount speed if (mounted)
            return base.speed;
        }
    }

    // ||---------------||
    // ||- Interaction -||
    [Header("Interaction")]
    public float interactionRange = 4;
    public bool localPlayerClickThrough = true;
    public KeyCode cancelActionKey = KeyCode.Escape;

    // ||--------
    // ||- Delay for commands that need to avoid DDOS, database usage, etc.
    [SyncVar, HideInInspector] public double nextRiskyActionTime = 0;

    // ||- Cache players to save computation (no need ofr NetworkServer.objects calls all the time)
    // ||-> on server: all online players
    // ||-> on client: all observed players
    public static Dictionary<string, Player> onlinePlayers = new Dictionary<string, Player>();

    private GameObject playerHUD;

    // ||------------
    // ||- Timings to prevent combat logging
    public double allowedLogoutTime => lastCombatTime + ((MyNetworkManager)NetworkManager.singleton).combatLogoutDelay;
    public double remainingLogoutTime => NetworkTime.time < allowedLogoutTime ? (allowedLogoutTime - NetworkTime.time) : 0;

    public override void OnStartLocalPlayer()
    {
        // |-- Set singleton
        localPlayer = this;
    }

    protected override void Start()
    {
        // Do nothing if not spawned (character preview on selection)
        if(!isServer && !isClient) return;

        
        base.Start();
        onlinePlayers[name] = this;
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
        
        // ||- Animation -||
        // ||- Animate on client only.
        if (isClient) 
        {
            // Future Animations
            foreach(Animator anim in GetComponentsInChildren<Animator>())
            {
                // anim.SetBool("MOVING", movement.isMoving());
                // anim.SetBool("DEAD", state == "DEAD");
                // anim.SetBool(Animate Skills)
            }
        }

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
        // ||- Remove player from onlinePlayers first. That's a must.
        if(onlinePlayers.TryGetValue(name, out Player entry) && entry == this)
            onlinePlayers.Remove(name);

        // ||- Do nothing if not spawned ( for character selection previews )
        if(!isServer && !isClient) return;

        // ||- Clear localPlayer
        if(isLocalPlayer)
            localPlayer = null;
    }

    //||-|- Some brain events require Cmd's that can't be in ScriptableObject
    [Command]
    public void CmdRespawn() { respawnRequested = true; }
    internal bool respawnRequested;

    [Command]
    public void CmdCancelAction() { cancelActionRequested = true; }
    internal bool cancelActionRequested;
    //-|-||


    [Server]
    public void OnDamageDealtTo(Entity victim) {}

    [Server]
    public void OnKilledEnemy(Entity victim) {}

    // Aggro
    [ServerCallback]
    public override void OnAggro(Entity entity)
    {
        // Call base function
        base.OnAggro(entity);

        // Forward information to pet if it's supposed to defend this entity
        // if pet.ActivePet != null pet.ActivePet.OnAggro(entity)
    }

    // ||-|- Movement: checks if movement is currently allowed
    public bool IsMovementAllowed()
    {
        // check if player is casting 
        // if is, check if cast allows movement

        // check if player is typing
        // if it is, do not move

        return (state == "IDLE" || state == "MOVING");
    }



    // ||-|-
    [Server]
    public override void OnDeath()
    {
        // take care of entity stuff
        base.OnDeath();

        // reset movement and navigation
        // movement.Reset();
    }

    // Item Cooldowns
    public float GetItemCooldown(string cooldownCategory)
    {
        // find for category, return 1

        // found none
        return 0;
    }

    // ||-|- CanAttack check override with Entity type checks
    public override bool CanAttack(Entity entity)
    {
        return base.CanAttack(entity) && (entity is Player || entity is Player);
    }

    // ||-|- Selection Handling
    [Command]
    public void CmdSetTarget(NetworkIdentity ni)
    {
        // validate
        if (ni != null)
        {
            target = ni.GetComponent<Entity>();
        }
    }
}
