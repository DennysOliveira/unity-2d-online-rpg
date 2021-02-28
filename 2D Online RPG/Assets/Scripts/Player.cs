using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.Events;


[Serializable] public class UnityEventPlayer : UnityEvent<Player> {}

public class Player : NetworkBehaviour
{
    public bool isGameMaster;

    [Header("Components")]
    public string account = "";
    public string className = "";
    public PlayerInventory inventory;

    [Header("Entity")]
    public Entity entity;

    [Header("Player UI")]
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider staminaSlider;
    public Slider expSlider;
    
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

        playerHUD = GameObject.Find("Canvas").gameObject;
        
        if(isLocalPlayer)
        {
            // Grab slider references // GameObject.FindWithTag("HealthSlider").GetComponent<Slider>();
            healthSlider    = GameObject.Find("Canvas/PlayerUI/PlayerHUD/HealthSlider").GetComponent<Slider>();
            manaSlider      = GameObject.Find("Canvas/PlayerUI/PlayerHUD/ManaSlider").GetComponent<Slider>();
            staminaSlider   = GameObject.Find("Canvas/PlayerUI/PlayerHUD/StaminaSlider").GetComponent<Slider>();
            expSlider       = GameObject.Find("Canvas/PlayerUI/PlayerHUD/ExperienceSlider").GetComponent<Slider>();

            // Sets up entity values according to the Database
            CmdSetEntityValues();

            // Set slider values according to Entity values
            healthSlider.value      = entity.curHealth;
            healthSlider.maxValue   = entity.maxHealth;

            manaSlider.value        = entity.curMana;
            manaSlider.maxValue     = entity.maxMana;

            staminaSlider.value     = entity.curStamina;
            staminaSlider.maxValue  = entity.maxStamina;

            expSlider.value = 0;
        }
        

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

    [Command]
    void CmdSetEntityValues()
    {
        // TO-DO: Get values from the database
        // int dbHealth  = GetValue("health", entity.name);
        // int dbMana    = GetValue("mana", entity.name);
        // int dbStamina = GetValue("stamina", entity.name);

        // Set HP/MP/SP values accordingly
        entity.curHealth  = entity.maxHealth;
        entity.curMana    = entity.maxMana;
        entity.curStamina = entity.maxStamina;
    }

    // Commands to get values from the database through the Server
    // In the future, should get current value from the database when the player logs in.
    [Command]
    void CmdGetHealth()
    {
        entity.curHealth = entity.maxHealth;
    }

    [Command]
    void CmdGetMana()
    {
        entity.curHealth = entity.maxHealth;
    }

    [Command]
    void CmdGetStamina()
    {
        entity.curStamina = entity.maxStamina;
    }

    [Command]
    void CmdGetExp()
    {
        Debug.Log("Getting EXP Values from the Database...");
    }

    [Command]
    void CmdTakeDamage(int dmg)
    {
        entity.curHealth -= dmg;
    }

    
}
